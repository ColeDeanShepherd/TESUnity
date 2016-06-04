using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TESUnity
{
	public class TESUnity : MonoBehaviour
	{
		public static TESUnity instance;

		#region Inspector-set Members
		public bool music = false;
		public bool sunShadows = false;
		public RenderingPath renderPath = RenderingPath.Forward;
		public bool exteriorCellLights;

		public Sprite UIBackgroundImg;
		public Sprite UICheckmarkImg;
		public Sprite UIDropdownArrowImg;
		public Sprite UIInputFieldBackgroundImg;
		public Sprite UIKnobImg;
		public Sprite UIMaskImg;
		public Sprite UISpriteImg;

		public GameObject waterPrefab;
		#endregion

		public string MWDataPath;

		private LocalSettingsObject settingsFile;
		public bool FoundSettingsFile { get { return settingsFile != null; } }
		private MorrowindDataReader MWDataReader;
		private MorrowindEngine MWEngine;
		private MusicPlayer musicPlayer;
		
		private GameObject testObj;
		private string testObjPath;

		private void Awake()
		{
			instance = this;
		}

		public void TryFindSettings()
		{
			var foundSettings = Resources.LoadAll<LocalSettingsObject>("");
			if ( foundSettings.Length > 0 )
				settingsFile = foundSettings[ 0 ]; // search for and load the first found Settings file from a Resources folder
		}

		private void Start()
		{
			if ( settingsFile != null )
			{
				MWDataPath = settingsFile.engine.dataFilesPath;
				music = settingsFile.audio.enableMusic;
				renderPath = settingsFile.graphics.preferredRenderMode;
				exteriorCellLights = settingsFile.graphics.exteriorCellLights;
			}

			MWDataReader = new MorrowindDataReader(MWDataPath);
			MWEngine = new MorrowindEngine(MWDataReader , sunShadows );

			if ( music )
			{// Start the music.
				musicPlayer = new MusicPlayer();

				foreach ( var songFilePath in Directory.GetFiles( MWDataPath + "/Music/Explore" ) )
				{
					if ( !songFilePath.Contains( "Morrowind Title" ) )
					{
						musicPlayer.AddSong( songFilePath );
					}
				}

				musicPlayer.Play();
			}

			// Spawn the player.
			MWEngine.SpawnPlayerInside("Imperial Prison Ship", new Vector3(0.8f, -0.25f, -1.4f));
		}
		private void OnDestroy()
		{
			if(MWDataReader != null)
			{
				MWDataReader.Close();
				MWDataReader = null;
			}
		}

		private void Update()
		{
			MWEngine.Update();
			if ( music ) musicPlayer.Update();

			if(Input.GetKeyDown(KeyCode.P))
			{
				if(MWEngine.currentCell == null || !MWEngine.currentCell.isInterior)
				{
					Debug.Log(MWEngine.GetExteriorCellIndices(Camera.main.transform.position));
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

			var scrollView = GUIUtils.CreateScrollView(MWEngine.canvasObj);
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

						testObj = MWEngine.theNIFManager.InstantiateNIF(filePath);
						testObjPath = filePath;
					});

					drawI++;
				}
			}
		}
		private void WriteBSAFilePaths(string parentDirectoryPath)
		{
			using(var writer = new StreamWriter(new FileStream(parentDirectoryPath + "/MorrowindBSA.txt", FileMode.OpenOrCreate, FileAccess.Write)))
			{
				foreach(var fileMetadata in MWDataReader.MorrowindBSAFile.fileMetadatas)
				{
					writer.WriteLine(fileMetadata.path);
				}
			}

			using(var writer = new StreamWriter(new FileStream(parentDirectoryPath + "/BloodmoonBSA.txt", FileMode.OpenOrCreate, FileAccess.Write)))
			{
				foreach(var fileMetadata in MWDataReader.BloodmoonBSAFile.fileMetadatas)
				{
					writer.WriteLine(fileMetadata.path);
				}
			}

			using(var writer = new StreamWriter(new FileStream(parentDirectoryPath + "/TribunalBSA.txt", FileMode.OpenOrCreate, FileAccess.Write)))
			{
				foreach(var fileMetadata in MWDataReader.TribunalBSAFile.fileMetadatas)
				{
					writer.WriteLine(fileMetadata.path);
				}
			}
		}
		private void ExtractFileFromMorrowind(string filePathInBSA, string parentDirectoryPath)
		{
			File.WriteAllBytes(parentDirectoryPath + '/' + Path.GetFileName(filePathInBSA), MWDataReader.MorrowindBSAFile.LoadFileData(filePathInBSA));
		}
		private void TestAllCells(string resultsFilePath)
		{
			using(StreamWriter writer = new StreamWriter(resultsFilePath))
			{
				foreach(var record in MWDataReader.MorrowindESMFile.GetRecordsOfType<ESM.CELLRecord>())
				{
					var CELL = (ESM.CELLRecord)record;

					try
					{
						var cellInfo = MWEngine.InstantiateCell(CELL);
						MWEngine.temporalLoadBalancer.WaitForTask(cellInfo.creationCoroutine);
						DestroyImmediate(cellInfo.gameObject);

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
	}
}