using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: fix rotation errors
// TODO: redesign destructors/idisposable
// TODO: handle untextured activators
// TODO: add walking mode
// TODO: add music playing
// TODO: add more creation functions for GUI elements
// TODO: refactor water level handling
// TODO: use MW lights
// TODO: fix doors with no colliders (Hlaalu Council Manor in Balmora)

namespace TESUnity
{
	public class TESUnity : MonoBehaviour
	{
		const string MorrowindDataPath = "C:/Program Files (x86)/Steam/steamapps/common/Morrowind/Data Files";
		public static TESUnity instance;

		#region Inspector-set Members
		public Sprite UIBackgroundImg;
		public Sprite UICheckmarkImg;
		public Sprite UIDropdownArrowImg;
		public Sprite UIInputFieldBackgroundImg;
		public Sprite UIKnobImg;
		public Sprite UIMaskImg;
		public Sprite UISpriteImg;

		public Material defaultMaterial;
		public Material cutoutMaterial;
		public Material fadeMaterial;

		public GameObject waterPrefab;
		#endregion

		private GameObject canvasObject;

		private MorrowindDataReader MWDataReader;
		private MorrowindEngine MWEngine;
		
		private GameObject testObj;
		private string testObjPath;

		private void Awake()
		{
			instance = this;
		}
		private void Start()
		{
			canvasObject = GUIUtils.CreateCanvas();
			GUIUtils.CreateEventSystem();

			MWDataReader = new MorrowindDataReader(MorrowindDataPath);
			MWEngine = new MorrowindEngine(MWDataReader);

			ExtractFileFromMorrowind("meshes\\x\\ex_dae_ruin_01.nif");
			MWEngine.SpawnPlayerOutside(Vector3.up * 50);

			//CreateBSABrowser();
		}
		private void OnDestroy()
		{
			MWDataReader.Close();
		}

		private void Update()
		{
			MWEngine.Update();

			if(Input.GetKeyDown(KeyCode.G))
			{
				MWEngine.TryOpenDoor();
			}

			if(Input.GetKeyDown(KeyCode.P))
			{
				if(!MWEngine.currentCell.isInterior)
				{
					Debug.Log(MWEngine.currentCell.gridCoords);
				}
				else
				{
					Debug.Log(MWEngine.currentCell.NAME.value);
				}
			}
		}

		private void CreateBSABrowser()
		{
			var MWArchiveFile = MWDataReader.MorrowindBSAFile;

			var scrollView = GUIUtils.CreateScrollView(canvasObject);
			scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(480, 400);

			var scrollViewContent = GUIUtils.GetScrollViewContent(scrollView);
			scrollViewContent.AddComponent<VerticalLayoutGroup>();
			var scrollViewContentTransform = scrollViewContent.GetComponent<RectTransform>();
			scrollViewContentTransform.sizeDelta = new Vector2(scrollViewContentTransform.sizeDelta.x, 128000);

			float x = 0;
			float y0 = 0;
			float width = 400;
			float height = 20;
			float yMarginBottom = 0;
			int drawI = 0;

			for(int i = 0; i < MWArchiveFile.fileMetadatas.Length; i++)
			{
				var filePath = MWArchiveFile.fileMetadatas[i].path;

				if(Path.GetExtension(filePath) == ".nif")
				{
					int iCopy = i;
					float y = y0 - drawI * (height + yMarginBottom);

					var button = GUIUtils.CreateTextButton(filePath, scrollViewContent);
					button.GetComponent<Button>().onClick.AddListener(() =>
					{
						if(testObj != null)
						{
							Destroy(testObj);
							testObj = null;
						}

						testObj = MWEngine.InstantiateNIF(filePath);
						testObjPath = filePath;

						//testImg = TextureUtils.LoadDDSTexture(new MemoryStream(fileData));
					});

					drawI++;
				}
			}
		}
		private void WriteBSAFilePaths()
		{
			using(var writer = new StreamWriter(new FileStream("C:/Users/Cole/Desktop/MorrowindBSA.txt", FileMode.OpenOrCreate, FileAccess.Write)))
			{
				foreach(var fileMetadata in MWDataReader.MorrowindBSAFile.fileMetadatas)
				{
					writer.WriteLine(fileMetadata.path);
				}
			}

			using(var writer = new StreamWriter(new FileStream("C:/Users/Cole/Desktop/BloodmoonBSA.txt", FileMode.OpenOrCreate, FileAccess.Write)))
			{
				foreach(var fileMetadata in MWDataReader.BloodmoonBSAFile.fileMetadatas)
				{
					writer.WriteLine(fileMetadata.path);
				}
			}

			using(var writer = new StreamWriter(new FileStream("C:/Users/Cole/Desktop/TribunalBSA.txt", FileMode.OpenOrCreate, FileAccess.Write)))
			{
				foreach(var fileMetadata in MWDataReader.TribunalBSAFile.fileMetadatas)
				{
					writer.WriteLine(fileMetadata.path);
				}
			}
		}
		private void ExtractFileFromMorrowind(string filePath)
		{
			File.WriteAllBytes("C:/Users/Cole/Desktop/" + Path.GetFileName(filePath), MWDataReader.MorrowindBSAFile.LoadFileData(filePath));
		}
		private void TestAllCells()
		{
			using(StreamWriter writer = new StreamWriter("C:/Users/Cole/Desktop/cellresults.txt"))
			{
				foreach(var record in MWDataReader.MorrowindESMFile.GetRecordsOfType<ESM.CELLRecord>())
				{
					var CELL = (ESM.CELLRecord)record;

					try
					{
						var cellObj = MWEngine.InstantiateCell(CELL);
						DestroyImmediate(cellObj);

						writer.Write("Pass: ");
					}
					catch
					{
						writer.Write("Fail: ");
					}

					if(!CELL.isInterior)
					{
						writer.WriteLine(CELL.gridCoords.ToString());
					}
					else
					{
						writer.WriteLine(CELL.NAME.value);
					}
				}
			}
		}

		private GameObject CreatePlayer(Vector3 position, Quaternion orientation)
		{
			GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			player.AddComponent<Rigidbody>();

			player.transform.position = position;
			player.transform.rotation = orientation;

			return player;
		}

		/*
		//string testSoundFilePath = MorrowindDataPath + "/Sound/Fx/BMWind.wav";
		string testSoundFilePath = MorrowindDataPath + "/Music/Explore/mx_explore_1.mp3";
		//string testSoundFilePath = MorrowindDataPath + "/Sound/Vo/a/f/Atk_AF001.mp3";

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.B))
			{
				if(File.Exists(testSoundFilePath))
				{
					PCMAudioBuffer audioBuffer = AudioUtils.ReadAudioFile(testSoundFilePath);
					AudioUtils.Play2DAudioClip(audioBuffer);
				}
			}

			if(Input.GetKeyDown(KeyCode.S))
			{
				if(File.Exists(testSoundFilePath))
				{
					MP3StreamReader audioStream = new MP3StreamReader(testSoundFilePath);
					AudioUtils.Play2DAudioStream(audioStream);
				}
			}
		}*/
	}
}