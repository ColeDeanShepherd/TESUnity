﻿using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using MP3Sharp;

// TODO: Handle long audio files.

public static class AudioUtils
{
	public static AudioClip CreateAudioClip(string name, PCMAudioBuffer audioBuffer)
	{
		var sampleData = audioBuffer.ToFloatArray();

		var audioClip = AudioClip.Create(name, sampleData.Length, audioBuffer.channelCount, audioBuffer.samplingRate, false);
		audioClip.SetData(sampleData, 0);

		return audioClip;
	}

	public static int SampleFramesToBytes(int sampleFrameCount, int channelCount, int bitDepth)
	{
		return sampleFrameCount * channelCount * (bitDepth / 8);
	}
	public static int BytesToSampleFrames(int byteCount, int channelCount, int bitDepth)
	{
		return byteCount / (bitDepth / 8) / channelCount;
	}

	public static void Play2DAudioClip(AudioClip audioClip)
	{
		GameObject gameObject = new GameObject("tmp2DAudioClip");

		var audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = audioClip;

		gameObject.AddComponent<OneShotAudioSourceComponent>();
	}
	public static void Play2DAudioClip(PCMAudioBuffer audioBuffer)
	{
		var audioClip = CreateAudioClip("tmp2DAudioClip", audioBuffer);

		Play2DAudioClip(audioClip);
	}
	public static void Play2DAudioStream(MP3StreamReader audioStream)
	{
		GameObject gameObject = new GameObject("tmp2DAudioStream");

		var audioSource = gameObject.AddComponent<AudioSource>();
		//audioSource.clip = CreateStreamingAudioClip("tmpAudioClip", audioStream);
		//audioSource.loop = true;

		var audioStreamComponent = gameObject.AddComponent<OneShotAudioStreamComponent>();
		audioStreamComponent.audioStream = audioStream;
	}

	// TODO: throw better exception
	public static PCMAudioBuffer ReadAudioFile(string filePath)
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
	public static PCMAudioBuffer ReadWAV(string filePath)
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
			var samplingRate = reader.ReadUInt32(); // # of samples per second (not including all channels).
			var byteRate = reader.ReadUInt32(); // # of bytes per second (including all channels).
			var blockAlign = reader.ReadUInt16(); // # of bytes for one sample (including all channels).
			var bitsPerSample = reader.ReadUInt16(); // # of bits per sample (not including all channels).

			if(subchunk1Size == 18)
			{
				// Read any extra values.
				var subchunk1ExtraSize = reader.ReadUInt16();
				reader.ReadBytes(subchunk1ExtraSize);
			}

			var subchunk2ID = new string(reader.ReadChars(4)); // "data"
			var subchunk2Size = reader.ReadUInt32(); // Size of rest of subchunk.
			byte[] audioData = reader.ReadBytes((int)subchunk2Size);

			return new PCMAudioBuffer((int)numChannels, (int)bitsPerSample, (int)samplingRate, audioData);
		}
	}

	// TODO: Handle exceptions
	public static PCMAudioBuffer ReadMP3(string filePath)
	{
		using(MP3StreamReader audioStream = new MP3StreamReader(filePath))
		{
			var audioData = new List<byte>(2 * (int)audioStream.compressedStreamLengthInBytes); // Allocate enough space for a 50% compression ratio.

			int streamBufferSizeInSampleFrames = 16384;
			var streamBuffer = new byte[SampleFramesToBytes(streamBufferSizeInSampleFrames, audioStream.channelCount, audioStream.bitDepth)];

			do
			{
				int sampleFramesRead = audioStream.ReadSampleFrames(streamBuffer, 0, streamBufferSizeInSampleFrames);

				if(sampleFramesRead > 0)
				{
					int bytesRead = SampleFramesToBytes(sampleFramesRead, audioStream.channelCount, audioStream.bitDepth);

					audioData.AddRange(new ArrayRange<byte>(streamBuffer, 0, bytesRead));
				}
			} while(!audioStream.isDoneStreaming);

			return new PCMAudioBuffer(audioStream.channelCount, audioStream.bitDepth, audioStream.samplingRate, audioData.ToArray());
		}
	}

	// Returns the number of samples that were read from the stream.
	public static int FillUnityStreamBuffer(float[] unityBuffer, PCMAudioBuffer intermediateBuffer, MP3StreamReader audioStream)
	{
		if(audioStream.isDoneStreaming)
		{
			// Fill the Unity sample buffer with zeros.
			Array.Clear(unityBuffer, 0, unityBuffer.Length);

			return 0;
		}

		int totalSampleFramesToRead = unityBuffer.Length / audioStream.channelCount;
		int sampleFramesRead = 0;

		while(sampleFramesRead < totalSampleFramesToRead)
		{
			// Read some sample frames.
			int sampleFramesLeftToRead = totalSampleFramesToRead - sampleFramesRead;
			int sampleFramesReturned = audioStream.ReadSampleFrames(intermediateBuffer.data, 0, Math.Min(sampleFramesLeftToRead, intermediateBuffer.sampleFrameCount));

			if(sampleFramesReturned > 0)
			{
				// Convert the read samples to floats copy them to the output buffer.
				intermediateBuffer.ToFloatArray(unityBuffer, sampleFramesRead, sampleFramesReturned);

				sampleFramesRead += sampleFramesReturned;
			}
			else
			{
				// Fill the rest of the Unity sample buffer with zeros.
				int samplesRead = sampleFramesRead * audioStream.channelCount;
				Array.Clear(unityBuffer, samplesRead, unityBuffer.Length - samplesRead);

				break;
			}
		}

		return sampleFramesRead * audioStream.channelCount;
	}

	// Quick hack until Unity bugs are fixed.
	public static void ResampleHack(float[] srcSamples, float[] dstSamples)
	{
		var srcSampleFrameCount = srcSamples.Length / 2;
		var dstSampleFrameCount = dstSamples.Length / 2;

		var lastSrcSampleFrameIndex = srcSampleFrameCount - 1;
		var lastDstSampleFrameIndex = dstSampleFrameCount - 1;

		for(int channelIndex = 0; channelIndex < 2; channelIndex++)
		{
			for(int dstSampleFrameIndex = 0; dstSampleFrameIndex <= lastDstSampleFrameIndex; dstSampleFrameIndex++)
			{
				int dstSmpI = channelIndex + (2 * dstSampleFrameIndex);

				float sample;

				if(dstSampleFrameIndex == 0)
				{
					sample = srcSamples[channelIndex];
				}
				else if(dstSampleFrameIndex == lastDstSampleFrameIndex)
				{
					sample = srcSamples[channelIndex + (2 * lastSrcSampleFrameIndex)];
				}
				else
				{
					float iPercent = (float)dstSampleFrameIndex / lastDstSampleFrameIndex;

					var srcSampleFrameIF = iPercent * lastSrcSampleFrameIndex;
					int LSrcSampleFrameI = (int)Math.Floor(srcSampleFrameIF);
					int RSrcSampleFrameI = LSrcSampleFrameI + 1;
					float t = srcSampleFrameIF - LSrcSampleFrameI;

					int srcSmp0I = channelIndex + (2 * LSrcSampleFrameI);
					int srcSmp1I = channelIndex + (2 * RSrcSampleFrameI);
					
					sample = Mathf.Lerp(srcSamples[srcSmp0I], srcSamples[srcSmp1I], t);
				}

				dstSamples[dstSmpI] = sample;
			}
		}
	}

	public static void LowPassHack(float[] samples)
	{
		var sampleFrameCount = samples.Length / 2;

		for(int channelI = 0; channelI < 2; channelI++)
		{
			for(int sampleFrameI = 0; sampleFrameI < sampleFrameCount - 1; sampleFrameI++)
			{
				int sample0I = channelI + (2 * sampleFrameI);
				int sample1I = sample0I + 2;

				samples[sample0I] = (samples[sample0I] + samples[sample1I]) / 2;
			}
		}
	}

	private static AudioClip CreateStreamingAudioClip(string name, MP3StreamReader audioStream)
	{
		PCMAudioBuffer streamBuffer = new PCMAudioBuffer(audioStream.channelCount, audioStream.bitDepth, audioStream.samplingRate, 8192);
		int bufferAudioClipSampleFrameCount = audioStream.samplingRate;

		return AudioClip.Create(name, bufferAudioClipSampleFrameCount, audioStream.channelCount, audioStream.samplingRate, true, delegate (float[] samples)
		{
			int samplesReturned = FillUnityStreamBuffer(samples, streamBuffer, audioStream);

			if(audioStream.isOpen && audioStream.isDoneStreaming)
			{
				audioStream.Close();
			}
		});
	}
}

// pulse-code modulation (uncompressed samples) audio buffer
public struct PCMAudioBuffer
{
	public int channelCount;
	public int bitDepth; // bits per sample
	public int samplingRate; // sample frames per second
	public byte[] data; // sample data (channels are interleaved)

	public int bytesPerSample
	{
		get
		{
			return bitDepth / 8;
		}
	}
	public int bytesPerSampleFrame
	{
		get
		{
			return channelCount * bytesPerSample;
		}
	}
	public int sampleFrameCount
	{
		get
		{
			return sampleCount / channelCount;
		}
	}
	public int sampleCount
	{
		get
		{
			return data.Length / bytesPerSample;
		}
	}

	public PCMAudioBuffer(int channelCount, int bitDepth, int samplingRate, int sampleFrameCount)
	{
		this.channelCount = channelCount;
		this.bitDepth = bitDepth;
		this.samplingRate = samplingRate;
		data = null; // Finish assigning values to all members so that properties can be used.

		data = new byte[sampleFrameCount * bytesPerSampleFrame];
	}

	// TODO: verify data size
	public PCMAudioBuffer(int channelCount, int bitDepth, int samplingRate, byte[] data)
	{
		this.channelCount = channelCount;
		this.bitDepth = bitDepth;
		this.samplingRate = samplingRate;
		this.data = data;
	}

	public float[] ToFloatArray()
	{
		float[] floatArray = new float[sampleCount];
		ToFloatArray(floatArray, 0, sampleFrameCount);

		return floatArray;
	}

	// TODO: assert numSampleFrames valid
	public void ToFloatArray(float[] floatArray, int offsetInSampleFrames, int numSampleFrames)
	{
		int offsetInSamples = offsetInSampleFrames * channelCount;
		int numSamples = numSampleFrames * channelCount;

		switch(bitDepth)
		{
			case 8:
				for(int i = 0; i < numSamples; i++)
				{
					floatArray[offsetInSamples + i] = (float)(unchecked((sbyte)data[i])) / sbyte.MaxValue;
				}

				break;
			case 16:
				for(int i = 0; i < numSamples; i++)
				{
					floatArray[offsetInSamples + i] = (float)BitConverter.ToInt16(data, 2 * i) / short.MaxValue;
				}

				break;
			case 32:
				for(int i = 0; i < numSamples; i++)
				{
					floatArray[offsetInSamples + i] = BitConverter.ToSingle(data, 4 * i);
				}

				break;
			case 64:
				for(int i = 0; i < numSamples; i++)
				{
					floatArray[offsetInSamples + i] = (float)BitConverter.ToDouble(data, 8 * i);
				}

				break;
			default:
				throw new NotImplementedException("Tried to convert a PCMAudioBuffer with an unsupported bit depth (" + bitDepth.ToString() + ") to a float array.");
		}
	}
}

// TODO: protect public attributes
// TODO: Handle exceptions
// TODO: Change MP3 libraries to properly handle mono/stereo.
public class MP3StreamReader : IDisposable
{
	public int channelCount = 2;
	public int bitDepth = 16;
	public int samplingRate;
	public long compressedStreamLengthInBytes;
	public int streamedSampleFrameCount = 0;
	public bool isDoneStreaming
	{
		get
		{
			return !isOpen || audioStream.IsEOF;
		}
	}
	public bool isOpen
	{
		get
		{
			return audioStream != null;
		}
	}

	public MP3StreamReader(string filePath)
	{
		audioStream = new MP3Stream(filePath);
		samplingRate = audioStream.Frequency;
		compressedStreamLengthInBytes = audioStream.Length;
	}

	public void Close()
	{
		Debug.Assert(isOpen);

		audioStream.Close();
		audioStream = null;
	}
	public void Dispose()
	{
		if(isOpen)
		{
			Close();
		}
	}

	// Returns how many sample frames were actually read.
	public int ReadSampleFrames(byte[] buffer, int offsetInSampleFrames, int sampleFrameCount)
	{
		Debug.Assert(isOpen);

		int offsetInBytes = AudioUtils.SampleFramesToBytes(offsetInSampleFrames, channelCount, bitDepth);
		int requestedByteCount = AudioUtils.SampleFramesToBytes(sampleFrameCount, channelCount, bitDepth);

		int bytesRead = 0;
		int bytesReturned;

		do
		{
			bytesReturned = audioStream.Read(buffer, offsetInBytes + bytesRead, requestedByteCount - bytesRead);
			bytesRead += bytesReturned;
		} while(bytesReturned > 0);

		Debug.Assert((bytesRead % AudioUtils.SampleFramesToBytes(1, channelCount, bitDepth)) == 0);

		// Stereoize audio and fix MP3Sharp's strange behavior.
		if(audioStream.ChannelCount == 1)
		{
			int iEnd = offsetInBytes + bytesRead;

			for(int i = offsetInBytes; i < iEnd; i += 4)
			{
				buffer[i + 2] = buffer[i];
				buffer[i + 3] = buffer[i + 1];
			}
		}

		int sampleFramesRead = AudioUtils.BytesToSampleFrames(bytesRead, channelCount, bitDepth);
		streamedSampleFrameCount += sampleFramesRead;

		return sampleFramesRead;
	}

	private MP3Stream audioStream;
}