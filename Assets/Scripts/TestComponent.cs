using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MP3Sharp;

// TODO: improve error handling
public struct ArrayRange<T> : IEnumerable<T>
{
	public struct Enumerator : IEnumerator<T>
	{
		public ArrayRange<T> arrayRange;
		public int currentIndex;

		public Enumerator(ArrayRange<T> arrayRange)
		{
			this.arrayRange = arrayRange;
			currentIndex = arrayRange.offset - 1;
		}

		public T Current
		{
			get
			{
				return arrayRange.array[currentIndex];
			}
		}
		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		public void Dispose()
		{
			arrayRange = new ArrayRange<T>();
			currentIndex = -1;
		}
		public bool MoveNext()
		{
			currentIndex++;

			return currentIndex < (arrayRange.offset + arrayRange.count);
		}
		public void Reset()
		{
			currentIndex = arrayRange.offset - 1;
		}
	}

	public T[] array
	{
		get
		{
			return _array;
		}
	}
	public int offset
	{
		get
		{
			return _offset;
		}
	}
	public int count
	{
		get
		{
			return _count;
		}
	}

	public ArrayRange(T[] array, int offset, int count)
	{
		_array = array;
		_offset = offset;
		_count = count;
	}
	public IEnumerator<T> GetEnumerator()
	{
		return new Enumerator(this);
	}

	private T[] _array;
	private int _offset;
	private int _count;

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

// Pulse-code modulation (uncompressed samples) audio buffer
public struct PCMAudioBuffer
{
	public int sampleRate; // samples per second
	public int bitDepth; // bits per sample (not taking into account multiple channels)
	public int channelCount;
	public byte[] data; // sample data (channels are interleaved)

	public int sampleCount
	{
		get
		{
			return (data.Length / channelCount) / (bitDepth / 8);
		}
	}

	public PCMAudioBuffer(int sampleRate, int bitDepth, int channelCount, byte[] data)
	{
		this.sampleRate = sampleRate;
		this.bitDepth = bitDepth;
		this.channelCount = channelCount;
		this.data = data;
	}
}

public class TestComponent : MonoBehaviour
{
	private void Start()
	{
		var MorrowindDataPath = "C:/Program Files (x86)/Steam/steamapps/common/Morrowind/Data Files";

		//var testSoundFilePath = MorrowindDataPath + "/Sound/Fx/BMWind.wav";
		//var testSoundFilePath = MorrowindDataPath + "/Music/Explore/mx_explore_1.mp3";
		var testSoundFilePath = MorrowindDataPath + "/Sound/Vo/a/f/Atk_AF001.mp3";

		if(File.Exists(testSoundFilePath))
		{
			PCMAudioBuffer audioBuffer = ReadAudioFile(testSoundFilePath);
			AudioClip audioClip = CreateAudioClip(Path.GetFileNameWithoutExtension(testSoundFilePath), audioBuffer);
			Play2DAudioClip(audioClip);
		}
	}

	// TODO: throw better exception type
	private PCMAudioBuffer ReadAudioFile(string filePath)
	{
		string fileExtension = Path.GetExtension(filePath).ToLower();

		switch(fileExtension)
		{
			case ".wav":
				return ReadWAV(filePath);
			case ".mp3":
				return ReadMP3(filePath);
			default:
				throw new NotImplementedException();
		}
	}

	// TODO: Handle exceptions
	// TODO: Endianness?
	private PCMAudioBuffer ReadWAV(string filePath)
	{
		using(BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
		{
			var chunkID = new string(reader.ReadChars(4)); // "RIFF".
			var chunkSize = reader.ReadUInt32(); // Size of the rest of the chunk after this number.
			var format = new string(reader.ReadChars(4)); // "WAVE".

			var subchunk1ID = new string(reader.ReadChars(4)); // "fmt "
			var subchunk1Size = reader.ReadUInt32(); // Size of rest of subchunk.
			var audioFormat = reader.ReadUInt16(); // 1 = PCM
			var numChannels = reader.ReadUInt16();
			var sampleRate = reader.ReadUInt32(); // # of samples per second (not including all channels).
			var byteRate = reader.ReadUInt32(); // # of bytes per second (including all channels).
			var blockAlign = reader.ReadUInt16(); // # of bytes for one sample (including all channels).
			var bitsPerSample = reader.ReadUInt16(); // # of bits per sample (not including all channels).

			var subchunk2ID = new string(reader.ReadChars(4)); // "data"
			var subchunk2Size = reader.ReadUInt32(); // Size of rest of subchunk.
			byte[] audioData = reader.ReadBytes((int)subchunk2Size);

			return new PCMAudioBuffer((int)sampleRate, (int)bitsPerSample, (int)numChannels, audioData);
		}
	}

	// TODO: Handle exceptions
	// TODO: Change MP3 libraries to properly handle mono/stereo.
	private PCMAudioBuffer ReadMP3(string filePath)
	{
		using(MP3Stream audioStream = new MP3Stream(filePath))
		{
			var audioData = new List<byte>(2 * (int)audioStream.Length); // 16-bit samples
			byte[] readBuffer = new byte[16384];
			int bytesReturned;

			while(true)
			{
				bytesReturned = audioStream.Read(readBuffer, 0, readBuffer.Length);

				if(bytesReturned > 0)
				{
					audioData.AddRange(new ArrayRange<byte>(readBuffer, 0, bytesReturned));
				}
				else
				{
					break;
				}
			}

			// Stereoize audio.
			if(audioStream.ChannelCount == 1)
			{
				for(int i = 0; i < audioData.Count; i += 4)
				{
					audioData[i + 2] = audioData[i];
					audioData[i + 3] = audioData[i + 1];
				}
			}

			return new PCMAudioBuffer(audioStream.Frequency, 16, 2, audioData.ToArray());
		}
	}

	private AudioClip CreateAudioClip(string name, PCMAudioBuffer audioBuffer)
	{
		var sampleData = new float[audioBuffer.channelCount * audioBuffer.sampleCount];

		switch(audioBuffer.bitDepth)
		{
			case 8:
				for(int i = 0; i < sampleData.Length; i++)
				{
					sampleData[i] = audioBuffer.data[i] / sbyte.MaxValue;
				}

				break;
			case 16:
				for(int i = 0; i < sampleData.Length; i++)
				{
					sampleData[i] = (float)BitConverter.ToInt16(audioBuffer.data, 2 * i) / short.MaxValue;
				}

				break;
			case 32:
				for(int i = 0; i < sampleData.Length; i++)
				{
					sampleData[i] = BitConverter.ToSingle(audioBuffer.data, 4 * i);
				}

				break;
			case 64:
				for(int i = 0; i < sampleData.Length; i++)
				{
					sampleData[i] = (float)BitConverter.ToDouble(audioBuffer.data, 8 * i);
				}

				break;
			default:
				throw new NotImplementedException("Tried to create an audio clip from a PCMAudioBuffer with an unsupported bit depth (" + audioBuffer.bitDepth.ToString() + ").");
		}

		var audioClip = AudioClip.Create(name, sampleData.Length, audioBuffer.channelCount, audioBuffer.sampleRate, false);
		audioClip.SetData(sampleData, 0);

		return audioClip;
	}

	private void Play2DAudioClip(AudioClip audioClip)
	{
		GameObject gameObject = new GameObject("tmpAudioSrc");

		var audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.PlayOneShot(audioClip);

		gameObject.AddComponent<OneShotAudioComponent>();
	}
}