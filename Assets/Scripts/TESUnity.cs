using System.IO;
using UnityEngine;
using UnityEngine.UI;

// TODO: Figure out and use C# documentation standards.

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

	private GameObject canvasObject;
	private BSAFile MWArchiveFile;

	private Texture2D testImg;
	private GameObject testObj;

	private void Awake()
	{
		instance = this;
	}
	private void Start()
	{
		canvasObject = GUIUtils.CreateCanvas();
		GUIUtils.CreateEventSystem();

		//GUIUtils.CreateScrollView(canvasObject);
		//var UIObj = GameObject.Instantiate(dropdownPrefab);
		//UIObj.transform.SetParent(canvasObject.transform, false);

		//CreateBSABrowser();

		//var parserGenerator = new NIFParserGenerator();
		//parserGenerator.GenerateParser("Assets/Misc/nif.xml", "Assets/Scripts/AutoNIFReader.cs");

		var NIFFile = new NIF.NiFile();
		NIFFile.Deserialize(new BinaryReader(new FileStream("C:/Users/Cole/Desktop/c_m_robe_common_02h.1st.nif", FileMode.Open, FileAccess.Read)));
		NIF.NiUtils.InstantiateNIFFile(NIFFile);
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
		MWArchiveFile = new BSAFile(MorrowindDataPath + "/Morrowind.bsa");

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
			var filePath = MWArchiveFile.fileMetadatas[i].name;

			if(Path.GetExtension(filePath) == ".nif")
			{
				int iCopy = i;
				float y = y0 - drawI * (height + yMarginBottom);

				var button = GUIUtils.CreateTextButton(filePath, scrollViewContent);
				button.GetComponent<Button>().onClick.AddListener(() =>
				{
					byte[] fileData = MWArchiveFile.LoadFileData(MWArchiveFile.fileMetadatas[iCopy]);
					File.WriteAllBytes("C:/Users/Cole/Desktop/" + Path.GetFileName(filePath), fileData);

					var NIFFile = new NIF.NiFile();
					NIFFile.Deserialize(new BinaryReader(new MemoryStream(fileData)));
					
					if(testObj != null)
					{
						Destroy(testObj);
						testObj = null;
					}

					testObj = NIF.NiUtils.InstantiateNIFFile(NIFFile);

					//testImg = TextureUtils.LoadDDSTexture(new MemoryStream(fileData));
				});

				drawI++;
			}
		}
	}
	private void ReadESMFile()
	{
		var MWMasterFile = new ESMFile(MorrowindDataPath + "/Morrowind.esm");

		while(!MWMasterFile.isAtEOF)
		{
			MWMasterFile.SkipRecord();

			int i = 3;
		}
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