using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: fix rotation errors
// TODO: load mipmaps
// TODO: reenable ACTI
// TODO: redesign destructors/idisposable

public class TESUnity : MonoBehaviour
{
	const string MorrowindDataPath = "C:/Program Files (x86)/Steam/steamapps/common/Morrowind/Data Files";
	public static TESUnity instance;

	public Sprite UIBackgroundImg;
	public Sprite UISpriteImg;
	public Sprite UIMaskImg;

	// UI Prefabs
	public GameObject dropdownPrefab;
	public GameObject inputFieldPrefab;
	public GameObject sliderPrefab;
	public GameObject togglePrefab;

	public Material defaultMaterial;
	public Material cutoutMaterial;
	public Material fadeMaterial;

	private GameObject canvasObject;

	private MorrowindDataReader MWDataReader;
	private Texture2D testImg;
	private GameObject testObj;
	private string testObjPath;

	private Dictionary<Vector2i, GameObject> cellObjects = new Dictionary<Vector2i, GameObject>();
	private int cellRadius = 2;

	private void Awake()
	{
		instance = this;
	}
	private void Start()
	{
		canvasObject = GUIUtils.CreateCanvas();
		GUIUtils.CreateEventSystem();

		MWDataReader = new MorrowindDataReader(MorrowindDataPath);

		ExtractFileFromMorrowind("meshes\\f\\flora_tree_wg_05.nif");

		//GUIUtils.CreateScrollView(canvasObject);
		//var UIObj = GameObject.Instantiate(dropdownPrefab);
		//UIObj.transform.SetParent(canvasObject.transform, false);

		//CreateBSABrowser();

		//var parserGenerator = new NIFParserGenerator();
		//parserGenerator.GenerateParser("Assets/Misc/nif.xml", "Assets/Scripts/AutoNIFReader.cs");
	}
	private Vector2i GetCellIndices(Vector3 point)
	{
		float CELL_LENGTH = 64;

		return new Vector2i(Mathf.FloorToInt(point.x / CELL_LENGTH), Mathf.FloorToInt(point.z / CELL_LENGTH));
	}
	private void UpdateCells()
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
			DestroyCell(cellIndices);
		}

		// Create new cells.
		for(int x = minCellX; x <= maxCellX; x++)
		{
			for(int y = minCellY; y <= maxCellY; y++)
			{
				var cellIndices = new Vector2i(x, y);

				if(!cellObjects.ContainsKey(cellIndices))
				{
					CreateCell(cellIndices);
				}
			}
		}
	}
	private void CreateCell(Vector2i indices)
	{
		cellObjects[indices] = MWDataReader.InstantiateExteriorCell(indices.x, indices.y);
	}
	private void DestroyCell(Vector2i indices)
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
	private void OnDestroy()
	{
		MWDataReader.Close();
	}

	private void Update()
	{
		UpdateCells();

		if(Input.GetKeyDown(KeyCode.K))
		{
			var fileName = Path.GetFileName(testObjPath);
			var fileBytes = MWDataReader.MorrowindBSAFile.LoadFileData(testObjPath);
			File.WriteAllBytes("C:/Users/Cole/Desktop/" + fileName, fileBytes);
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

		/*
		File Formats:
		.nif - model
		.dds - texture
		.kl - animation
		.pso - DirectX Shader
		.vso - DirectX Shader
		*/

		//for(int i = 0; i < MWArchiveFile.fileMetadatas.Length; i++)
		for(int i = 0; i < 1000; i++)
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

					testObj = MWDataReader.InstantiateNIF(filePath);
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