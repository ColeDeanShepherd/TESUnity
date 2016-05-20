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

namespace TESUnity
{
	public class TESUnity : MonoBehaviour
	{
		const string MorrowindDataPath = "C:/Program Files (x86)/Steam/steamapps/common/Morrowind/Data Files";
		public static TESUnity instance;

		public Sprite UIBackgroundImg;
		public Sprite UICheckmarkImg;
		public Sprite UIDropdownArrowImg;
		public Sprite UIInputFieldBackgroundImg;
		public Sprite UIKnobImg;
		public Sprite UIMaskImg;
		public Sprite UISpriteImg;
		
		// UI Prefabs
		public GameObject sliderPrefab;
		public GameObject togglePrefab;

		public Material defaultMaterial;
		public Material cutoutMaterial;
		public Material fadeMaterial;

		public GameObject waterPrefab;

		private GameObject canvasObject;
		private GameObject waterObj;

		private MorrowindDataReader MWDataReader;
		private MorrowindEngine MWEngine;

		private Texture2D testImg;
		private GameObject testObj;
		private string testObjPath;

		private Dictionary<Vector2i, GameObject> cellObjects = new Dictionary<Vector2i, GameObject>();
		private int cellRadius = 1;

		private ESM.CELLRecord currentCell;
		private bool isInteriorCell;

		private void Awake()
		{
			instance = this;
		}
		private void Start()
		{
			canvasObject = GUIUtils.CreateCanvas();
			GUIUtils.CreateEventSystem();

			/*var cameraObj = GameObjectUtils.CreateMainCamera();

			GUIUtils.CreateToggle(canvasObject);
			var UIObj = GameObject.Instantiate(togglePrefab);
			UIObj.transform.SetParent(canvasObject.transform, false);*/

			waterObj = Instantiate(waterPrefab);

			var cameraObj = GameObjectUtils.CreateMainCamera();
			var flyingCameraComponent = cameraObj.AddComponent<FlyingCameraComponent>();

			MWDataReader = new MorrowindDataReader(MorrowindDataPath);
			MWEngine = new MorrowindEngine(MWDataReader);

			//CreatePlayer(Vector3.up * 50, Quaternion.identity);

			//MWDataReader.InstantiateInteriorCell("Helan Ancestral Tomb");
			//MWDataReader.InstantiateInteriorCell("Suran, Ibarnadad Assirnarari: Apothecary");

			//GUIUtils.CreateScrollView(canvasObject);
			//var UIObj = GameObject.Instantiate(dropdownPrefab);
			//UIObj.transform.SetParent(canvasObject.transform, false);

			//CreateBSABrowser();
		}
		private Vector2i GetCellIndices(Vector3 point)
		{
			return new Vector2i(Mathf.FloorToInt(point.x / Convert.exteriorCellSideLengthInMeters), Mathf.FloorToInt(point.z / Convert.exteriorCellSideLengthInMeters));
		}
		private void UpdateExteriorCells()
		{
			var cameraCellIndices = GetCellIndices(Camera.main.transform.position);
			var minCellX = cameraCellIndices.x - cellRadius;
			var maxCellX = cameraCellIndices.x + cellRadius;
			var minCellY = cameraCellIndices.y - cellRadius;
			var maxCellY = cameraCellIndices.y + cellRadius;

			// Destroy out of range cells.
			var outOfRangeCellIndices = new List<Vector2i>();

			foreach(var KVPair in cellObjects)
			{
				if((KVPair.Key.x < minCellX) || (KVPair.Key.x > maxCellX) || (KVPair.Key.y < minCellY) || (KVPair.Key.y > maxCellY))
				{
					outOfRangeCellIndices.Add(KVPair.Key);
				}
			}

			foreach(var cellIndices in outOfRangeCellIndices)
			{
				DestroyExteriorCell(cellIndices);
			}

			// Create new cells.
			for(int x = minCellX; x <= maxCellX; x++)
			{
				for(int y = minCellY; y <= maxCellY; y++)
				{
					var cellIndices = new Vector2i(x, y);

					if(!cellObjects.ContainsKey(cellIndices))
					{
						CreateExteriorCell(cellIndices);
					}
				}
			}
		}
		private GameObject CreateExteriorCell(Vector2i indices)
		{
			var cellObj = MWEngine.InstantiateExteriorCell(indices.x, indices.y);
			cellObjects[indices] = cellObj;

			return cellObj;
		}
		private void DestroyExteriorCell(Vector2i indices)
		{
			GameObject cellObj;

			if(cellObjects.TryGetValue(indices, out cellObj))
			{
				cellObjects.Remove(indices);
				Destroy(cellObj);
			}
			else
			{
				Debug.LogError("Tried to destroy a cell that isn't created.");
			}
		}
		private GameObject CreateInteriorCell(string cellName)
		{
			var cellObj = MWEngine.InstantiateInteriorCell(cellName);
			cellObjects[Vector2i.zero] = cellObj;

			return cellObj;
		}
		private void DestroyAllCells()
		{
			foreach(var keyValuePair in cellObjects)
			{
				Destroy(keyValuePair.Value);
			}

			cellObjects.Clear();
		}
		private void OnDestroy()
		{
			MWDataReader.Close();
		}

		private void Update()
		{
			if(!isInteriorCell)
			{
				UpdateExteriorCells();

				if(Input.GetKeyDown(KeyCode.P))
				{
					Debug.Log(GetCellIndices(Camera.main.transform.position));
				}
			}

			if(Input.GetKeyDown(KeyCode.G))
			{
				TryOpenDoor();
			}
		}
		private void OnGUI()
		{
			if(testImg != null)
			{
				GUI.DrawTexture(new Rect(10, 10, testImg.width, testImg.height), testImg);
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

		private void TryOpenDoor()
		{
			RaycastHit hitInfo;
			var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

			if(Physics.Raycast(ray, out hitInfo, 2))
			{
				// Find the door object.
				GameObject doorObj = GameObjectUtils.FindObjectWithTagUpHeirarchy(hitInfo.collider.gameObject, "Door");

				if(doorObj != null)
				{
					var doorComponent = doorObj.GetComponent<DoorComponent>();

					if(doorComponent.leadsToAnotherCell)
					{
						DestroyAllCells();

						ESM.CELLRecord CELL;

						if((doorComponent.doorExitName != null) && (doorComponent.doorExitName != ""))
						{
							CELL = MWDataReader.FindInteriorCellRecord(doorComponent.doorExitName);
							cellObjects[Vector2i.zero] = MWEngine.InstantiateCell(CELL);

							if(CELL.WHGT != null)
							{
								waterObj.transform.position = new Vector3(0, CELL.WHGT.value, 0);
								waterObj.SetActive(true);
							}
							else
							{
								waterObj.SetActive(false);
							}
						}
						else
						{
							var cellIndices = GetCellIndices(doorComponent.doorExitPos);
							CELL = MWDataReader.FindExteriorCellRecord(cellIndices.x, cellIndices.y);

							waterObj.transform.position = Vector3.zero;
							waterObj.SetActive(true);
						}

						Camera.main.transform.position = doorComponent.doorExitPos;
						Camera.main.transform.rotation = doorComponent.doorExitOrientation;

						isInteriorCell = CELL.isInterior;
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