using System;
using System.IO;
using UnityEngine;

public static class TextureUtils
{
	private enum DDSFlags
	{
		DDSD_CAPS = 0x1,
		DDSD_HEIGHT = 0x2,
		DDSD_WIDTH = 0x4,
		DDSD_PITCH = 0x8,
		DDSD_PIXELFORMAT = 0x1000,
		DDSD_MIPMAPCOUNT = 0x20000,
		DDSD_LINEARSIZE = 0x80000,
		DDSD_DEPTH = 0x800000
	}

	private enum DDSPixelFormatFlags
	{
		DDPF_ALPHAPIXELS = 0x1,
		DDPF_ALPHA = 0x2,
		DDPF_FOURCC = 0x4,
		DDPF_RGB = 0x40,
		DDPF_YUV = 0x200,
		DDPF_LUMINANCE = 0x20000
	}

	private enum DDSCaps
	{
		DDSCAPS_COMPLEX = 0x8,
		DDSCAPS_MIPMAP = 0x400000,
		DDSCAPS_TEXTURE = 0x1000
	}

	private enum DDSCaps2
	{
		DDSCAPS2_CUBEMAP = 0x200,
		DDSCAPS2_CUBEMAP_POSITIVEX = 0x400,
		DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800,
		DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000,
		DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000,
		DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000,
		DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000,
		DDSCAPS2_VOLUME = 0x200000
	}

	// TODO: improve error handling
	public static Texture2D LoadDDSTexture(string filePath)
	{
		return LoadDDSTexture(File.Open(filePath, FileMode.Open, FileAccess.Read));
	}
	public static Texture2D LoadDDSTexture(Stream inputStream)
	{
		using(BinaryReader reader = new BinaryReader(inputStream))
		{
			var magicString = reader.ReadBytes(4); // "DDS "

			var dwSize = reader.ReadUInt32();
			Debug.Assert(dwSize == 124);

			var dwFlags = reader.ReadUInt32();
			Debug.Assert(Utils.ContainsBitFlags(dwFlags, (uint)DDSFlags.DDSD_CAPS, (uint)DDSFlags.DDSD_HEIGHT, (uint)DDSFlags.DDSD_WIDTH, (uint)DDSFlags.DDSD_PIXELFORMAT));

			var dwHeight = reader.ReadUInt32();
			var dwWidth = reader.ReadUInt32();
			var dwPitchOrLinearSize = reader.ReadUInt32();
			var dwDepth = reader.ReadUInt32();
			var dwMipMapCount = reader.ReadUInt32();

			var dwReserved1 = new uint[11];

			for(int i = 0; i < dwReserved1.Length; i++)
			{
				dwReserved1[i] = reader.ReadUInt32();
			}

			var dwPixelFormatSize = reader.ReadUInt32();
			Debug.Assert(dwPixelFormatSize == 32);

			var dwPixelFormatFlags = reader.ReadUInt32();
			var dwPixelFormatFourCC = reader.ReadBytes(4);
			var dwPixelFormatRGBBitCount = reader.ReadUInt32();
			var dwPixelFormatRBitMask = reader.ReadUInt32();
			var dwPixelFormatGBitMask = reader.ReadUInt32();
			var dwPixelFormatBBitMask = reader.ReadUInt32();
			var dwPixelFormatABitMask = reader.ReadUInt32();

			var dwCaps = reader.ReadUInt32();
			Debug.Assert(Utils.ContainsBitFlags(dwCaps, (uint)DDSCaps.DDSCAPS_TEXTURE));

			var dwCaps2 = reader.ReadUInt32();
			var dwCaps3 = reader.ReadUInt32();
			var dwCaps4 = reader.ReadUInt32();
			var dwReserved2 = reader.ReadUInt32();

			TextureFormat textureFormat = TextureFormat.ARGB32;
			byte[] textureData;

			if(Utils.ContainsBitFlags(dwPixelFormatFlags, (uint)DDSPixelFormatFlags.DDPF_RGB, (uint)DDSPixelFormatFlags.DDPF_ALPHAPIXELS))
			{
				// uncompressed RGBA

				Debug.Assert(dwPixelFormatRGBBitCount == 32);

				if((dwPixelFormatBBitMask == 0x000000FF) && (dwPixelFormatGBitMask == 0x0000FF00) && (dwPixelFormatRBitMask == 0x00FF0000) && (dwPixelFormatABitMask == 0xFF000000))
				{
					textureFormat = TextureFormat.BGRA32;
				}
				else if((dwPixelFormatABitMask == 0x000000FF) && (dwPixelFormatRBitMask == 0x0000FF00) && (dwPixelFormatGBitMask == 0x00FF0000) && (dwPixelFormatBBitMask == 0xFF000000))
				{
					textureFormat = TextureFormat.ARGB32;
				}
				else
				{
					throw new NotImplementedException();
				}
				

				uint dataSize = dwPitchOrLinearSize * dwHeight;
				textureData = reader.ReadBytes((int)dataSize);
				Utils.Flip2DArrayVertically(ref textureData, dwHeight, dwPitchOrLinearSize);
			}
			else if(StringUtils.Equals(dwPixelFormatFourCC, "DXT1"))
			{
				textureFormat = TextureFormat.DXT1;

				textureData = reader.ReadBytes((int)dwPitchOrLinearSize);
			}
			else if(StringUtils.Equals(dwPixelFormatFourCC, "DXT5"))
			{
				textureFormat = TextureFormat.DXT5;

				textureData = reader.ReadBytes((int)dwPitchOrLinearSize);
			}
			else
			{
				throw new NotImplementedException();
			}

			var texture = new Texture2D((int)dwWidth, (int)dwHeight, textureFormat, false);
			texture.LoadRawTextureData(textureData);
			texture.Apply();

			return texture;
		}
	}
}