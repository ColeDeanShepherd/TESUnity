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

	public GameObject scrollViewPrefab;

	private GameObject canvasObject;
	private BSAFile MWArchiveFile;

	private Texture2D testImg;

	private void Awake()
	{
		instance = this;
	}
	private void Start()
	{
		canvasObject = GUIUtils.CreateCanvas();
		GUIUtils.CreateEventSystem();

		MWArchiveFile = new BSAFile(MorrowindDataPath + "/Morrowind.bsa");

		var scrollView = GUIUtils.CreateScrollView(canvasObject);
		scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(480, 400);

		var scrollViewContent = GUIUtils.GetScrollViewContent(scrollView);
		scrollViewContent.AddComponent<GridLayoutGroup>();
		var contentSizeFitter = scrollViewContent.AddComponent<ContentSizeFitter>();
		contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

		/*var contentTransform = scrollViewContent.GetComponent<RectTransform>();
		//contentTransform.anchorMin = Vector2.zero;
		//contentTransform.anchorMax = Vector2.one;
		//contentTransform.offsetMin = Vector2.zero;
		//contentTransform.offsetMax = Vector2.zero;
		var contentSizeFitter = scrollViewContent.AddComponent<UnityEngine.UI.ContentSizeFitter>();
		contentSizeFitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
		contentSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;*/

		float x = 0;
		float y0 = 0;
		float width = 400;
		float height = 20;
		float yMarginBottom = 0;
		int drawI = 0;

		//for(int i = 0; i < MWArchiveFile.fileMetadatas.Length; i++)
		for(int i = 0; i < 20; i++)
		{
			if(Path.GetExtension(MWArchiveFile.fileMetadatas[i].name) == ".dds")
			{
				float y = y0 - drawI * (height + yMarginBottom);

				var button = GUIUtils.CreateTextButton(MWArchiveFile.fileMetadatas[i].name, scrollViewContent);
				button.GetComponent<Button>().onClick.AddListener(() =>
				{
					Debug.Log(i);
					byte[] fileData = MWArchiveFile.LoadFileData(MWArchiveFile.fileMetadatas[i]);
					File.WriteAllBytes("C:/Users/Cole/Desktop/" + Path.GetFileName(MWArchiveFile.fileMetadatas[i].name), fileData);
					testImg = TextureUtils.LoadDDSTexture(new MemoryStream(fileData));
				});

				drawI++;
			}
		}







		/*var fileMetadata = MWArchiveFile.fileMetadatas[1287];
		byte[] fileData = MWArchiveFile.LoadFileData(fileMetadata);

		File.WriteAllBytes("C:/Users/Cole/Desktop/" + Path.GetFileName(fileMetadata.name), fileData);*/

		//var fileMetadata = MWArchiveFile.fileMetadatas[1287];
		//var fileMetadata = MWArchiveFile.fileMetadatas[132];
		
		//testImg = TextureUtils.LoadDDSTexture("C:/Users/Cole/Desktop/tx_book_08.dds");
		//File.WriteAllBytes("C:/Users/Cole/Desktop/menu_rightbuttonup_top.png", loadedTexture.EncodeToPNG());




		// meshes\i\in_r_l_int_entrance_03.nif

		/*var MWArchiveFile = 

		var fileMetadata = MWArchiveFile.fileMetadatas[1];
		byte[] fileData = MWArchiveFile.LoadFileData(fileMetadata);

		File.WriteAllBytes("C:/Users/Cole/Desktop/" + Path.GetFileName(fileMetadata.name), fileData);*/

		/*

		int i = 3;*/
		/*var MWMasterFile = new ESMFile(MorrowindDataPath + "/Morrowind.esm");

		while(!MWMasterFile.isAtEOF)
		{
			MWMasterFile.SkipRecord();

			int i = 3;
		}*/
	}

	private void OnGUI()
	{
		if(testImg != null)
		{
			GUI.DrawTexture(new Rect(10, 10, testImg.width, testImg.height), testImg);
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