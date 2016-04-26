using System.IO;
using UnityEngine;

// TODO: Figure out and use C# documentation standards.

public class TestComponent : MonoBehaviour
{
	const string MorrowindDataPath = "C:/Program Files (x86)/Steam/steamapps/common/Morrowind/Data Files";

	private Texture2D loadedTexture;

	private void Start()
	{
		/*var MWArchiveFile = new BSAFile(MorrowindDataPath + "/Morrowind.bsa");

		var fileMetadata = MWArchiveFile.fileMetadatas[1];
		byte[] fileData = MWArchiveFile.LoadFileData(fileMetadata);

		File.WriteAllBytes("C:/Users/Cole/Desktop/" + Path.GetFileName(fileMetadata.name), fileData);*/

		loadedTexture = TextureUtils.LoadDDSTexture("C:/Users/Cole/Desktop/menu_rightbuttonup_top.dds");
		File.WriteAllBytes("C:/Users/Cole/Desktop/menu_rightbuttonup_top.png", loadedTexture.EncodeToPNG());

		int i = 3;
		/*var MWMasterFile = new ESMFile(MorrowindDataPath + "/Morrowind.esm");

		while(!MWMasterFile.isAtEOF)
		{
			MWMasterFile.SkipRecord();

			int i = 3;
		}*/
	}
	private void OnGUI()
	{
		if(loadedTexture != null)
		{
			GUI.DrawTexture(new Rect(10, 10, loadedTexture.width, loadedTexture.height), loadedTexture);
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