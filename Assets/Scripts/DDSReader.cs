using System;
using System.IO;
using UnityEngine;

public static class DDSReader
{
	public static Texture2D LoadDDSTexture(string filePath, bool flipVertically = false)
	{
		return LoadDDSTexture(File.Open(filePath, FileMode.Open, FileAccess.Read), flipVertically);
	}
	public static Texture2D LoadDDSTexture(Stream inputStream, bool flipVertically = false)
	{
		using(BinaryReader reader = new BinaryReader(inputStream))
		{
			var magicString = reader.ReadBytes(4); // "DDS "
			if(!StringUtils.Equals(magicString, "DDS "))
			{
				throw new FileFormatException("Invalid DDS file magic string: \"" + System.Text.Encoding.ASCII.GetString(magicString) + "\".");
			}

			var header = new DDSHeader();
			header.Deserialize(reader);

			// Figure out the texture format and load the texture data.
			bool hasMipMaps = Utils.ContainsBitFlags(header.dwCaps, (uint)DDSCaps.MIPMAP);
			uint DDSMipMapCount = hasMipMaps ? header.dwMipMapCount : 1;

			TextureFormat textureFormat;
			int bytesPerPixel;
			byte[] textureData;

			// Set textureFormat, bytesPerPixel, and textureData.
			if(Utils.ContainsBitFlags(header.pixelFormat.flags, (uint)DDSPixelFormatFlags.RGB)) // If the DDS file contains uncompressed data.
			{
				if(!Utils.ContainsBitFlags(header.pixelFormat.flags, (uint)DDSPixelFormatFlags.ALPHAPIXELS)) // RGB
				{
					throw new NotImplementedException("Unsupported DDS file pixel format.");
				}
				else // RGBA
				{
					if(header.pixelFormat.RGBBitCount != 32)
					{
						throw new FileFormatException("Invalid DDS file pixel format.");
					}

					if((header.pixelFormat.BBitMask == 0x000000FF) && (header.pixelFormat.GBitMask == 0x0000FF00) && (header.pixelFormat.RBitMask == 0x00FF0000) && (header.pixelFormat.ABitMask == 0xFF000000))
					{
						textureFormat = TextureFormat.BGRA32;
						bytesPerPixel = 4;
					}
					else if((header.pixelFormat.ABitMask == 0x000000FF) && (header.pixelFormat.RBitMask == 0x0000FF00) && (header.pixelFormat.GBitMask == 0x00FF0000) && (header.pixelFormat.BBitMask == 0xFF000000))
					{
						textureFormat = TextureFormat.ARGB32;
						bytesPerPixel = 4;
					}
					else
					{
						throw new NotImplementedException("Unsupported DDS file pixel format.");
					}

					if(!hasMipMaps)
					{
						textureData = new byte[header.dwPitchOrLinearSize * header.dwHeight];
					}
					else // if(hasMipMaps)
					{
						textureData = new byte[TextureUtils.CalculateMipMappedTextureDataSize((int)header.dwWidth, (int)header.dwHeight, bytesPerPixel)];
					}
					
					BinaryReaderExtensions.ReadRestOfBytes(reader, textureData, 0);
				}
			}
			else if(StringUtils.Equals(header.pixelFormat.fourCC, "DXT1"))
			{
				//textureFormat = TextureFormat.DXT1;
				//textureData = reader.ReadBytes((int)dwPitchOrLinearSize);

				textureFormat = TextureFormat.ARGB32;
				bytesPerPixel = 4;

				var compressedTextureData = BinaryReaderExtensions.ReadRestOfBytes(reader);
				textureData = DecodeDXT1ToARGB(compressedTextureData, header.dwWidth, header.dwHeight, header.pixelFormat, DDSMipMapCount);
			}
			else if(StringUtils.Equals(header.pixelFormat.fourCC, "DXT3"))
			{
				textureFormat = TextureFormat.ARGB32;
				bytesPerPixel = 4;

				var compressedTextureData = BinaryReaderExtensions.ReadRestOfBytes(reader);
				textureData = DecodeDXT3ToARGB(compressedTextureData, header.dwWidth, header.dwHeight, DDSMipMapCount);
			}
			else if(StringUtils.Equals(header.pixelFormat.fourCC, "DXT5"))
			{
				//textureFormat = TextureFormat.DXT5;
				//textureData = reader.ReadBytes((int)dwPitchOrLinearSize);

				textureFormat = TextureFormat.ARGB32;
				bytesPerPixel = 4;

				var compressedTextureData = BinaryReaderExtensions.ReadRestOfBytes(reader);
				textureData = DecodeDXT5ToARGB(compressedTextureData, header.dwWidth, header.dwHeight, DDSMipMapCount);
			}
			else
			{
				throw new NotImplementedException("Unsupported DDS file pixel format.");
			}

			// Flip mip-maps if necessary and generate missing mip-map levels.
			if(hasMipMaps)
			{
				int mipMapIndex = 0;
				int currentWidth = (int)header.dwWidth;
				int currentHeight = (int)header.dwHeight;
				int mipMapStartIndex = 0;

				while(true)
				{
					var mipMapDataSize = (currentWidth * currentHeight * bytesPerPixel);

					// Flip the current mip-map if necessary.
					if(flipVertically && (mipMapIndex < DDSMipMapCount))
					{
						Utils.Flip2DSubArrayVertically(textureData, mipMapStartIndex, currentHeight, currentWidth * bytesPerPixel);
					}
					
					if((currentWidth == 1) && (currentHeight == 1))
					{
						break;
					}
					else if((mipMapIndex + 1) >= DDSMipMapCount) // Generate the next mip-map if necessary.
					{
						TextureUtils.Downscale4Component32BitPixelsX2(textureData, mipMapStartIndex, currentHeight, currentWidth, textureData, mipMapStartIndex + mipMapDataSize);
					}

					mipMapIndex++;

					currentWidth = (currentWidth > 1) ? (currentWidth / 2) : currentWidth;
					currentHeight = (currentHeight > 1) ? (currentHeight / 2) : currentHeight;

					mipMapStartIndex += mipMapDataSize;
				}
			}

			var texture = new Texture2D((int)header.dwWidth, (int)header.dwHeight, textureFormat, hasMipMaps);
			texture.LoadRawTextureData(textureData);
			texture.Apply();

			return texture;
		}
	}

	private enum DDSFlags
	{
		CAPS = 0x1,
		HEIGHT = 0x2,
		WIDTH = 0x4,
		PITCH = 0x8,
		PIXELFORMAT = 0x1000,
		MIPMAPCOUNT = 0x20000,
		LINEARSIZE = 0x80000,
		DEPTH = 0x800000
	}
	private enum DDSPixelFormatFlags
	{
		ALPHAPIXELS = 0x1,
		ALPHA = 0x2,
		FOURCC = 0x4,
		RGB = 0x40,
		YUV = 0x200,
		LUMINANCE = 0x20000
	}
	private enum DDSCaps
	{
		COMPLEX = 0x8,
		MIPMAP = 0x400000,
		TEXTURE = 0x1000
	}
	private enum DDSCaps2
	{
		CUBEMAP = 0x200,
		CUBEMAP_POSITIVEX = 0x400,
		CUBEMAP_NEGATIVEX = 0x800,
		CUBEMAP_POSITIVEY = 0x1000,
		CUBEMAP_NEGATIVEY = 0x2000,
		CUBEMAP_POSITIVEZ = 0x4000,
		CUBEMAP_NEGATIVEZ = 0x8000,
		VOLUME = 0x200000
	}
	private struct DDSHeader
	{
		public uint dwSize;
		public uint dwFlags;
		public uint dwHeight;
		public uint dwWidth;
		public uint dwPitchOrLinearSize;
		public uint dwDepth;
		public uint dwMipMapCount;
		public uint[] dwReserved1;
		public DDSPixelFormat pixelFormat;
		public uint dwCaps;
		public uint dwCaps2;
		public uint dwCaps3;
		public uint dwCaps4;
		public uint dwReserved2;

		public void Deserialize(BinaryReader reader)
		{
			dwSize = reader.ReadUInt32();
			if(dwSize != 124)
			{
				throw new FileFormatException("Invalid DDS file header size: " + dwSize.ToString() + '.');
			}

			dwFlags = reader.ReadUInt32();
			if(!Utils.ContainsBitFlags(dwFlags, (uint)DDSFlags.HEIGHT, (uint)DDSFlags.WIDTH))
			{
				throw new FileFormatException("Invalid DDS file flags: " + dwFlags.ToString() + '.');
			}

			dwHeight = reader.ReadUInt32();
			dwWidth = reader.ReadUInt32();
			dwPitchOrLinearSize = reader.ReadUInt32();
			dwDepth = reader.ReadUInt32();
			dwMipMapCount = reader.ReadUInt32();

			dwReserved1 = new uint[11];

			for(int i = 0; i < dwReserved1.Length; i++)
			{
				dwReserved1[i] = reader.ReadUInt32();
			}

			pixelFormat = new DDSPixelFormat();
			pixelFormat.Deserialize(reader);

			dwCaps = reader.ReadUInt32();
			if(!Utils.ContainsBitFlags(dwCaps, (uint)DDSCaps.TEXTURE))
			{
				throw new FileFormatException("Invalid DDS file caps: " + dwCaps.ToString() + '.');
			}

			dwCaps2 = reader.ReadUInt32();
			dwCaps3 = reader.ReadUInt32();
			dwCaps4 = reader.ReadUInt32();
			dwReserved2 = reader.ReadUInt32();
		}
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

		public void Deserialize(BinaryReader reader)
		{
			size = reader.ReadUInt32();
			if(size != 32)
			{
				throw new FileFormatException("Invalid DDS file pixel format size: " + size.ToString() + '.');
			}

			flags = reader.ReadUInt32();
			fourCC = reader.ReadBytes(4);
			RGBBitCount = reader.ReadUInt32();
			RBitMask = reader.ReadUInt32();
			GBitMask = reader.ReadUInt32();
			BBitMask = reader.ReadUInt32();
			ABitMask = reader.ReadUInt32();
		}
	}

	// Assumes the color table has already been built.
	private static Color32[] DecodeDXT1TexelBlock(BinaryReader reader, Color[] colorTable)
	{
		Debug.Assert(colorTable.Length == 4);

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

		return colors;
	}

	private static Color32[] DecodeDXT1TexelBlock(BinaryReader reader, bool containsAlpha)
	{
		// Create the color table.
		var colorTable = new Color[4];
		colorTable[0] = ColorUtils.R5G6B5ToColor(reader.ReadUInt16());
		colorTable[1] = ColorUtils.R5G6B5ToColor(reader.ReadUInt16());

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

		// Calculate pixel colors.
		return DecodeDXT1TexelBlock(reader, colorTable);
	}
	private static Color32[] DecodeDXT3TexelBlock(BinaryReader reader)
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
		colorTable[0] = ColorUtils.R5G6B5ToColor(reader.ReadUInt16());
		colorTable[1] = ColorUtils.R5G6B5ToColor(reader.ReadUInt16());
		colorTable[2] = Color.Lerp(colorTable[0], colorTable[1], 1.0f / 3);
		colorTable[3] = Color.Lerp(colorTable[0], colorTable[1], 2.0f / 3);

		// Calculate pixel colors.
		var colors = DecodeDXT1TexelBlock(reader, colorTable);

		for(int i = 0; i < 16; i++)
		{
			colors[i].a = alphas[i];
		}

		return colors;
	}
	private static Color32[] DecodeDXT5TexelBlock(BinaryReader reader)
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
		colorTable[0] = ColorUtils.R5G6B5ToColor(reader.ReadUInt16());
		colorTable[1] = ColorUtils.R5G6B5ToColor(reader.ReadUInt16());
		colorTable[2] = Color.Lerp(colorTable[0], colorTable[1], 1.0f / 3);
		colorTable[3] = Color.Lerp(colorTable[0], colorTable[1], 2.0f / 3);

		// Read pixel color indices.
		DecodeDXT1TexelBlock(reader, colorTable);

		// Calculate pixel colors.
		var colors = new Color32[16];

		for(int i = 0; i < 16; i++)
		{
			colors[i].a = (byte)Mathf.Round(alphaTable[alphaIndices[i]]);
		}

		return colors;
	}

	private static void CopyDecodedTexelBlock(Color32[] decodedTexels, byte[] argb, int baseARGBIndex, int baseRowIndex, int baseColumnIndex, int textureWidth, int textureHeight)
	{
		for(int i = 0; i < 4; i++) // row
		{
			for(int j = 0; j < 4; j++) // column
			{
				var rowIndex = baseRowIndex + i;
				var columnIndex = baseColumnIndex + j;

				// Don't copy padding on mipmaps.
				if((rowIndex < textureHeight) && (columnIndex < textureWidth))
				{
					var decodedTexelIndex = (4 * i) + j;
					var color = decodedTexels[decodedTexelIndex];

					var ARGBPixelOffset = (textureWidth * rowIndex) + columnIndex;
					var basePixelARGBIndex = baseARGBIndex + (4 * ARGBPixelOffset);

					argb[basePixelARGBIndex] = color.a;
					argb[basePixelARGBIndex + 1] = color.r;
					argb[basePixelARGBIndex + 2] = color.g;
					argb[basePixelARGBIndex + 3] = color.b;
				}
			}
		}
	}
	private static byte[] DecodeDXT1ToARGB(byte[] compressedData, uint width, uint height, DDSPixelFormat pixelFormat, uint mipmapCount)
	{
		bool alphaFlag = Utils.ContainsBitFlags(pixelFormat.flags, (uint)DDSPixelFormatFlags.ALPHAPIXELS);
		bool containsAlpha = alphaFlag || ((pixelFormat.RGBBitCount == 32) && (pixelFormat.ABitMask != 0));

		var reader = new BinaryReader(new MemoryStream(compressedData));
		var argb = new byte[TextureUtils.CalculateMipMappedTextureDataSize((int)width, (int)height, 4)];

		int mipMapWidth = (int)width;
		int mipMapHeight = (int)height;
		int baseARGBIndex = 0;

		for(int mipMapIndex = 0; mipMapIndex < mipmapCount; mipMapIndex++)
		{
			for(int rowIndex = 0; rowIndex < mipMapHeight; rowIndex += 4)
			{
				for(int columnIndex = 0; columnIndex < mipMapWidth; columnIndex += 4)
				{
					var colors = DecodeDXT1TexelBlock(reader, containsAlpha);
					CopyDecodedTexelBlock(colors, argb, baseARGBIndex, rowIndex, columnIndex, mipMapWidth, mipMapHeight);
				}
			}

			baseARGBIndex += (mipMapWidth * mipMapHeight * 4);

			mipMapWidth /= 2;
			mipMapHeight /= 2;
		}

		return argb;
	}
	private static byte[] DecodeDXT3ToARGB(byte[] compressedData, uint width, uint height, uint mipmapCount)
	{
		var reader = new BinaryReader(new MemoryStream(compressedData));
		var argb = new byte[TextureUtils.CalculateMipMappedTextureDataSize((int)width, (int)height, 4)];

		int mipMapWidth = (int)width;
		int mipMapHeight = (int)height;
		int baseARGBIndex = 0;

		for(int mipMapIndex = 0; mipMapIndex < mipmapCount; mipMapIndex++)
		{
			for(int rowIndex = 0; rowIndex < mipMapHeight; rowIndex += 4)
			{
				for(int columnIndex = 0; columnIndex < mipMapWidth; columnIndex += 4)
				{
					var colors = DecodeDXT3TexelBlock(reader);
					CopyDecodedTexelBlock(colors, argb, baseARGBIndex, rowIndex, columnIndex, mipMapWidth, mipMapHeight);
				}
			}

			baseARGBIndex += (mipMapWidth * mipMapHeight * 4);

			mipMapWidth /= 2;
			mipMapHeight /= 2;
		}

		return argb;
	}
	private static byte[] DecodeDXT5ToARGB(byte[] compressedData, uint width, uint height, uint mipmapCount)
	{
		var reader = new BinaryReader(new MemoryStream(compressedData));
		var argb = new byte[TextureUtils.CalculateMipMappedTextureDataSize((int)width, (int)height, 4)];

		int mipMapWidth = (int)width;
		int mipMapHeight = (int)height;
		int baseARGBIndex = 0;

		for(int mipMapIndex = 0; mipMapIndex < mipmapCount; mipMapIndex++)
		{
			for(int rowIndex = 0; rowIndex < mipMapHeight; rowIndex += 4)
			{
				for(int columnIndex = 0; columnIndex < mipMapWidth; columnIndex += 4)
				{
					var colors = DecodeDXT5TexelBlock(reader);
					CopyDecodedTexelBlock(colors, argb, baseARGBIndex, rowIndex, columnIndex, mipMapWidth, mipMapHeight);
				}
			}

			baseARGBIndex += (mipMapWidth * mipMapHeight * 4);

			mipMapWidth /= 2;
			mipMapHeight /= 2;
		}

		return argb;
	}
}