using System;
using System.IO;
using UnityEngine;

public static class TextureUtils
{
	public static void FlipTexture2D(Texture2D texture2D)
	{
		var pixels = texture2D.GetPixels32();

		Utils.Flip2DArrayVertically<Color32>(ref pixels, (uint)texture2D.height, (uint)texture2D.width);

		texture2D.SetPixels32(pixels);
		texture2D.Apply();
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

			var pixelFormat = new DDSPixelFormat();
			pixelFormat.size = reader.ReadUInt32();
			Debug.Assert(pixelFormat.size == 32);

			pixelFormat.flags = reader.ReadUInt32();
			pixelFormat.fourCC = reader.ReadBytes(4);
			pixelFormat.RGBBitCount = reader.ReadUInt32();
			pixelFormat.RBitMask = reader.ReadUInt32();
			pixelFormat.GBitMask = reader.ReadUInt32();
			pixelFormat.BBitMask = reader.ReadUInt32();
			pixelFormat.ABitMask = reader.ReadUInt32();

			var dwCaps = reader.ReadUInt32();
			Debug.Assert(Utils.ContainsBitFlags(dwCaps, (uint)DDSCaps.DDSCAPS_TEXTURE));

			var dwCaps2 = reader.ReadUInt32();
			var dwCaps3 = reader.ReadUInt32();
			var dwCaps4 = reader.ReadUInt32();
			var dwReserved2 = reader.ReadUInt32();

			TextureFormat textureFormat = TextureFormat.ARGB32;
			byte[] textureData;

			if(Utils.ContainsBitFlags(pixelFormat.flags, (uint)DDSPixelFormatFlags.DDPF_RGB, (uint)DDSPixelFormatFlags.DDPF_ALPHAPIXELS))
			{
				// uncompressed RGBA

				Debug.Assert(pixelFormat.RGBBitCount == 32);

				if((pixelFormat.BBitMask == 0x000000FF) && (pixelFormat.GBitMask == 0x0000FF00) && (pixelFormat.RBitMask == 0x00FF0000) && (pixelFormat.ABitMask == 0xFF000000))
				{
					textureFormat = TextureFormat.BGRA32;
				}
				else if((pixelFormat.ABitMask == 0x000000FF) && (pixelFormat.RBitMask == 0x0000FF00) && (pixelFormat.GBitMask == 0x00FF0000) && (pixelFormat.BBitMask == 0xFF000000))
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
			else if(StringUtils.Equals(pixelFormat.fourCC, "DXT1"))
			{
				//textureFormat = TextureFormat.DXT1;
				//textureData = reader.ReadBytes((int)dwPitchOrLinearSize);

				textureFormat = TextureFormat.ARGB32;
				textureData = DecodeDXT1ToARGB(dwWidth, dwHeight, pixelFormat, reader.ReadBytes((int)dwPitchOrLinearSize));
				Utils.Flip2DArrayVertically<byte>(ref textureData, dwHeight, 4 * dwWidth);
			}
			else if(StringUtils.Equals(pixelFormat.fourCC, "DXT3"))
			{
				textureFormat = TextureFormat.ARGB32;
				textureData = DecodeDXT3ToARGB(dwWidth, dwHeight, reader.ReadBytes((int)dwPitchOrLinearSize));
				Utils.Flip2DArrayVertically<byte>(ref textureData, dwHeight, 4 * dwWidth);
			}
			else if(StringUtils.Equals(pixelFormat.fourCC, "DXT5"))
			{
				//textureFormat = TextureFormat.DXT5;
				//textureData = reader.ReadBytes((int)dwPitchOrLinearSize);

				textureFormat = TextureFormat.ARGB32;
				textureData = DecodeDXT5ToARGB(dwWidth, dwHeight, reader.ReadBytes((int)dwPitchOrLinearSize));
				Utils.Flip2DArrayVertically<byte>(ref textureData, dwHeight, 4 * dwWidth);
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

	public static Color R5G6B5ToColor(ushort R5G6B5)
	{
		// compressed components
		var rC = ((R5G6B5 >> 11) & 31);
		var gC = ((R5G6B5 >> 5) & 63);
		var bC = (R5G6B5 & 31);

		// float components
		var r = (float)rC / 31;
		var g = (float)gC / 63;
		var b = (float)bC / 31;

		return new Color(r, g, b, 1);
	}
	public static Color32 R5G6B5ToColor32(ushort R5G6B5)
	{
		return R5G6B5ToColor(R5G6B5);
	}

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

	private struct DDSPixelFormat
	{
		public uint size;
		public uint flags;
		public byte[] fourCC;
		public uint RGBBitCount;
		public uint RBitMask;
		public uint GBitMask;
		public uint BBitMask;
		public uint ABitMask;
	}

	private static byte[] DecodeDXT1ToARGB(uint width, uint height, DDSPixelFormat pixelFormat, byte[] compressedData)
	{
		var reader = new BinaryReader(new MemoryStream(compressedData));
		var argb = new byte[4 * width * height];
		bool alphaFlag = Utils.ContainsBitFlags(pixelFormat.flags, (uint)DDSPixelFormatFlags.DDPF_ALPHAPIXELS);
		bool containsAlpha = alphaFlag || ((pixelFormat.RGBBitCount == 32) && (pixelFormat.ABitMask != 0));

		for(uint rowI = 0; rowI < height; rowI += 4)
		{
			for(uint colI = 0; colI < width; colI += 4)
			{
				// Create the color table.
				var colorTable = new Color[4];
				colorTable[0] = R5G6B5ToColor(reader.ReadUInt16());
				colorTable[1] = R5G6B5ToColor(reader.ReadUInt16());

				if(!containsAlpha)
				{
					colorTable[2] = Color.Lerp(colorTable[0], colorTable[1], 1.0f / 3);
					colorTable[3] = Color.Lerp(colorTable[0], colorTable[1], 2.0f / 3);
				}
				else
				{
					colorTable[2] = Color.Lerp(colorTable[0], colorTable[1], 1.0f / 2);
					colorTable[3] = new Color(0, 0, 0, 0);
				}

				// Read pixel color indices.
				var colorIndices = new uint[16];
				var colorIndexBytes = reader.ReadBytes(4);

				for(uint i = 0; i < 4; i++) // row
				{
					for(uint j = 0; j < 4; j++) // column
					{
						var bitOffset = (8 * i) + (2 * (3 - j));

						colorIndices[(4 * i) + j] = (uint)Utils.GetBits(bitOffset, 2, colorIndexBytes);
					}
				}

				// Calculate pixel colors.
				var colors = new Color32[16];

				for(int i = 0; i < 16; i++)
				{
					colors[i] = colorTable[colorIndices[i]];
				}

				// Store the pixels.
				var pixel0Index = (rowI * width) + colI;

				for(int i = 0; i < 4; i++) // row
				{
					for(int j = 0; j < 4; j++) // column
					{
						var color = colors[(i * 4) + j];
						var pixelIndexOffset = (i * width) + j;
						var byte0Index = 4 * (pixel0Index + pixelIndexOffset);

						argb[byte0Index] = color.a;
						argb[byte0Index + 1] = color.r;
						argb[byte0Index + 2] = color.g;
						argb[byte0Index + 3] = color.b;
					}
				}
			}
		}

		return argb;
	}
	private static byte[] DecodeDXT3ToARGB(uint width, uint height, byte[] compressedData)
	{
		var reader = new BinaryReader(new MemoryStream(compressedData));
		var argb = new byte[4 * width * height];

		for(uint rowI = 0; rowI < height; rowI += 4)
		{
			for(uint colI = 0; colI < width; colI += 4)
			{
				// Read compressed pixel alphas.
				var compressedAlphas = new byte[16];
				
				for(int i = 0; i < 4; i++) // row
				{
					var compressedAlphaRow = reader.ReadUInt16();

					for(int j = 0; j < 4; j++) // column
					{
						compressedAlphas[(4 * i) + j] = (byte)((compressedAlphaRow >> (j * 4)) & 0xF);
					}
				}

				// Calculate pixel alphas.
				var alphas = new byte[16];

				for(int i = 0; i < 16; i++)
				{
					var alphaPercent = (float)compressedAlphas[i] / 15;
					alphas[i] = (byte)Mathf.RoundToInt(alphaPercent * 255);
				}

				// Create the color table.
				var colorTable = new Color[4];
				colorTable[0] = R5G6B5ToColor(reader.ReadUInt16());
				colorTable[1] = R5G6B5ToColor(reader.ReadUInt16());
				colorTable[2] = Color.Lerp(colorTable[0], colorTable[1], 1.0f / 3);
				colorTable[3] = Color.Lerp(colorTable[0], colorTable[1], 2.0f / 3);

				// Read pixel color indices.
				var colorIndices = new uint[16];
				var colorIndexBytes = reader.ReadBytes(4);

				for(uint i = 0; i < 4; i++) // row
				{
					for(uint j = 0; j < 4; j++) // column
					{
						var bitOffset = (8 * i) + (2 * (3 - j));

						colorIndices[(4 * i) + j] = (uint)Utils.GetBits(bitOffset, 2, colorIndexBytes);
					}
				}

				// Calculate pixel colors.
				var colors = new Color32[16];

				for(int i = 0; i < 16; i++)
				{
					colors[i] = colorTable[colorIndices[i]];
					colors[i].a = alphas[i];
				}

				// Store the pixels.
				var pixel0Index = (rowI * width) + colI;

				for(int i = 0; i < 4; i++) // row
				{
					for(int j = 0; j < 4; j++) // column
					{
						var color = colors[(i * 4) + j];
						var pixelIndexOffset = (i * width) + j;
						var byte0Index = 4 * (pixel0Index + pixelIndexOffset);

						argb[byte0Index] = color.a;
						argb[byte0Index + 1] = color.r;
						argb[byte0Index + 2] = color.g;
						argb[byte0Index + 3] = color.b;
					}
				}
			}
		}

		return argb;
	}
	private static byte[] DecodeDXT5ToARGB(uint width, uint height, byte[] compressedData)
	{
		var reader = new BinaryReader(new MemoryStream(compressedData));
		var argb = new byte[4 * width * height];

		for(uint rowI = 0; rowI < height; rowI += 4)
		{
			for(uint colI = 0; colI < width; colI += 4)
			{
				// Create the alpha table.
				var alphaTable = new float[8];
				alphaTable[0] = reader.ReadByte();
				alphaTable[1] = reader.ReadByte();

				if(alphaTable[0] > alphaTable[1])
				{
					for(int i = 0; i < 6; i++)
					{
						alphaTable[2 + i] = Mathf.Lerp(alphaTable[0], alphaTable[1], (float)(1 + i) / 7);
					}
				}
				else
				{
					for(int i = 0; i < 4; i++)
					{
						alphaTable[2 + i] = Mathf.Lerp(alphaTable[0], alphaTable[1], (float)(1 + i) / 5);
					}

					alphaTable[6] = 0;
					alphaTable[7] = 255;
				}

				// Read pixel alpha indices.
				var alphaIndices = new uint[16];

				var alphaIndexBytesRow0 = reader.ReadBytes(3);
				Array.Reverse(alphaIndexBytesRow0); // Take care of little-endianness.

				var alphaIndexBytesRow1 = reader.ReadBytes(3);
				Array.Reverse(alphaIndexBytesRow1); // Take care of little-endianness.

				alphaIndices[0] = (uint)Utils.GetBits(21, 3, alphaIndexBytesRow0); // a
				alphaIndices[1] = (uint)Utils.GetBits(18, 3, alphaIndexBytesRow0); // b
				alphaIndices[2] = (uint)Utils.GetBits(15, 3, alphaIndexBytesRow0); // c
				alphaIndices[3] = (uint)Utils.GetBits(12, 3, alphaIndexBytesRow0); // d
				alphaIndices[4] = (uint)Utils.GetBits(9, 3, alphaIndexBytesRow0); // e
				alphaIndices[5] = (uint)Utils.GetBits(6, 3, alphaIndexBytesRow0); // f
				alphaIndices[6] = (uint)Utils.GetBits(3, 3, alphaIndexBytesRow0); // g
				alphaIndices[7] = (uint)Utils.GetBits(0, 3, alphaIndexBytesRow0); // h
				alphaIndices[8] = (uint)Utils.GetBits(21, 3, alphaIndexBytesRow1); // i
				alphaIndices[9] = (uint)Utils.GetBits(18, 3, alphaIndexBytesRow1); // j
				alphaIndices[10] = (uint)Utils.GetBits(15, 3, alphaIndexBytesRow1); // k
				alphaIndices[11] = (uint)Utils.GetBits(12, 3, alphaIndexBytesRow1); // l
				alphaIndices[12] = (uint)Utils.GetBits(9, 3, alphaIndexBytesRow1); // m
				alphaIndices[13] = (uint)Utils.GetBits(6, 3, alphaIndexBytesRow1); // n
				alphaIndices[14] = (uint)Utils.GetBits(3, 3, alphaIndexBytesRow1); // o
				alphaIndices[15] = (uint)Utils.GetBits(0, 3, alphaIndexBytesRow1); // p

				// Create the color table.
				var colorTable = new Color[4];
				colorTable[0] = R5G6B5ToColor(reader.ReadUInt16());
				colorTable[1] = R5G6B5ToColor(reader.ReadUInt16());
				colorTable[2] = Color.Lerp(colorTable[0], colorTable[1], 1.0f / 3);
				colorTable[3] = Color.Lerp(colorTable[0], colorTable[1], 2.0f / 3);

				// Read pixel color indices.
				var colorIndices = new uint[16];
				var colorIndexBytes = reader.ReadBytes(4);

				for(uint i = 0; i < 4; i++) // row
				{
					for(uint j = 0; j < 4; j++) // column
					{
						var bitOffset = (8 * i) + (2 * (3 - j));

						colorIndices[(4 * i) + j] = (uint)Utils.GetBits(bitOffset, 2, colorIndexBytes); // a
					}
				}

				// Calculate pixel colors.
				var colors = new Color32[16];

				for(int i = 0; i < 16; i++)
				{
					colors[i] = colorTable[colorIndices[i]];
					colors[i].a = (byte)Mathf.Round(alphaTable[alphaIndices[i]]);
				}

				// Store the pixels.
				var pixel0Index = (rowI * width) + colI;

				for(int i = 0; i < 4; i++) // row
				{
					for(int j = 0; j < 4; j++) // column
					{
						var color = colors[(i * 4) + j];
						var pixelIndexOffset = (i * width) + j;
						var byte0Index = 4 * (pixel0Index + pixelIndexOffset);

						argb[byte0Index] = color.a;
						argb[byte0Index + 1] = color.r;
						argb[byte0Index + 2] = color.g;
						argb[byte0Index + 3] = color.b;
					}
				}
			}
		}

		return argb;
	}
}