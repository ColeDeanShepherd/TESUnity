using System.IO;
using UnityEngine;

// TODO: Figure out and use C# documentation standards.

public class TestComponent : MonoBehaviour
{
	const string MorrowindDataPath = "C:/Program Files (x86)/Steam/steamapps/common/Morrowind/Data Files";

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
	}
}