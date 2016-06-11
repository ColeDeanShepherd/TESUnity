using System;
using System.IO;
using UnityEngine;

namespace DDS
{
	public enum DDSFlags
	{
		Caps = 0x1,
		Height = 0x2,
		Width = 0x4,
		Pitch = 0x8,
		PixelFormat = 0x1000,
		MipmapCount = 0x20000,
		LinearSize = 0x80000,
		Depth = 0x800000
	}
	public enum DDSPixelFormatFlags
	{
		AlphaPixels = 0x1,
		Alpha = 0x2,
		FourCC = 0x4,
		RGB = 0x40,
		YUV = 0x200,
		Luminance = 0x20000
	}
	public enum DDSCaps
	{
		Complex = 0x8,
		Mipmap = 0x400000,
		Texture = 0x1000
	}
	public enum DDSCaps2
	{
		Cubemap = 0x200,
		CubemapPositiveX = 0x400,
		CubemapNegativeX = 0x800,
		CubemapPositiveY = 0x1000,
		CubemapNegativeY = 0x2000,
		CubemapPositiveZ = 0x4000,
		CubemapNegativeZ = 0x8000,
		Volume = 0x200000
	}

	public struct DDSHeader
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

		public void Deserialize(UnityBinaryReader reader)
		{
			dwSize = reader.ReadLEUInt32();
			if(dwSize != 124)
			{
				throw new FileFormatException("Invalid DDS file header size: " + dwSize.ToString() + '.');
			}

			dwFlags = reader.ReadLEUInt32();
			if(!Utils.ContainsBitFlags(dwFlags, (uint)DDSFlags.Height, (uint)DDSFlags.Width))
			{
				throw new FileFormatException("Invalid DDS file flags: " + dwFlags.ToString() + '.');
			}

			dwHeight = reader.ReadLEUInt32();
			dwWidth = reader.ReadLEUInt32();
			dwPitchOrLinearSize = reader.ReadLEUInt32();
			dwDepth = reader.ReadLEUInt32();
			dwMipMapCount = reader.ReadLEUInt32();

			dwReserved1 = new uint[11];
			for(int i = 0; i < dwReserved1.Length; i++)
			{
				dwReserved1[i] = reader.ReadLEUInt32();
			}

			pixelFormat = new DDSPixelFormat();
			pixelFormat.Deserialize(reader);

			dwCaps = reader.ReadLEUInt32();
			if(!Utils.ContainsBitFlags(dwCaps, (uint)DDSCaps.Texture))
			{
				throw new FileFormatException("Invalid DDS file caps: " + dwCaps.ToString() + '.');
			}

			dwCaps2 = reader.ReadLEUInt32();
			dwCaps3 = reader.ReadLEUInt32();
			dwCaps4 = reader.ReadLEUInt32();
			dwReserved2 = reader.ReadLEUInt32();
		}
	}
	public struct DDSPixelFormat
	{
		public uint size;
		public uint flags;
		public byte[] fourCC;
		public uint RGBBitCount;
		public uint RBitMask;
		public uint GBitMask;
		public uint BBitMask;
		public uint ABitMask;

		public void Deserialize(UnityBinaryReader reader)
		{
			size = reader.ReadLEUInt32();
			if(size != 32)
			{
				throw new FileFormatException("Invalid DDS file pixel format size: " + size.ToString() + '.');
			}

			flags = reader.ReadLEUInt32();
			fourCC = reader.ReadBytes(4);
			RGBBitCount = reader.ReadLEUInt32();
			RBitMask = reader.ReadLEUInt32();
			GBitMask = reader.ReadLEUInt32();
			BBitMask = reader.ReadLEUInt32();
			ABitMask = reader.ReadLEUInt32();
		}
	}

	public static class DDSReader
	{
		/// <summary>
		/// Loads a DDS texture from a file.
		/// </summary>
		public static Texture2DInfo LoadDDSTexture(string filePath, bool flipVertically = false)
		{
			return LoadDDSTexture(File.Open(filePath, FileMode.Open, FileAccess.Read), flipVertically);
		}

		/// <summary>
		/// Loads a DDS texture from an input stream.
		/// </summary>
		public static Texture2DInfo LoadDDSTexture(Stream inputStream, bool flipVertically = false)
		{
			using(var reader = new UnityBinaryReader(inputStream))
			{
				// Check the magic string.
				var magicString = reader.ReadBytes(4);
				if(!StringUtils.Equals(magicString, "DDS "))
				{
					throw new FileFormatException("Invalid DDS file magic string: \"" + System.Text.Encoding.ASCII.GetString(magicString) + "\".");
				}

				// Deserialize the DDS file header.
				var header = new DDSHeader();
				header.Deserialize(reader);

				// Figure out the texture format and load the texture data.
				bool hasMipmaps;
				uint DDSMipmapLevelCount;
				TextureFormat textureFormat;
				int bytesPerPixel;
				byte[] textureData;
				ExtractDDSTextureFormatAndData(header, reader, out hasMipmaps, out DDSMipmapLevelCount, out textureFormat, out bytesPerPixel, out textureData);

				// Post-process the texture to generate missing mipmaps and possibly flip it vertically.
				PostProcessDDSTexture((int)header.dwWidth, (int)header.dwHeight, bytesPerPixel, hasMipmaps, (int)DDSMipmapLevelCount, textureData, flipVertically);

				return new Texture2DInfo((int)header.dwWidth, (int)header.dwHeight, textureFormat, hasMipmaps, textureData);
			}
		}

		/// <summary>
		/// Decodes a DXT1-compressed 4x4 block of texels using a prebuilt 4-color color table.
		/// </summary>
		/// <remarks>See https://msdn.microsoft.com/en-us/library/windows/desktop/bb694531(v=vs.85).aspx#BC1 </remarks>
		private static Color32[] DecodeDXT1TexelBlock(UnityBinaryReader reader, Color[] colorTable)
		{
			Debug.Assert(colorTable.Length == 4);

			// Read pixel color indices.
			var colorIndices = new uint[16];

			var colorIndexBytes = new byte[4];
			reader.Read(colorIndexBytes, 0, colorIndexBytes.Length);

			const uint bitsPerColorIndex = 2;

			for(uint rowIndex = 0; rowIndex < 4; rowIndex++)
			{
				var rowBaseColorIndexIndex = 4 * rowIndex;
				var rowBaseBitOffset = 8 * rowIndex;

				for(uint columnIndex = 0; columnIndex < 4; columnIndex++)
				{
					// Color indices are arranged from right to left.
					var bitOffset = rowBaseBitOffset + (bitsPerColorIndex * (3 - columnIndex));

					colorIndices[rowBaseColorIndexIndex + columnIndex] = (uint)Utils.GetBits(bitOffset, bitsPerColorIndex, colorIndexBytes);
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

		/// <summary>
		/// Builds a 4-color color table for a DXT1-compressed 4x4 block of texels and then decodes the texels.
		/// </summary>
		/// <remarks>See https://msdn.microsoft.com/en-us/library/windows/desktop/bb694531(v=vs.85).aspx#BC1 </remarks>
		private static Color32[] DecodeDXT1TexelBlock(UnityBinaryReader reader, bool containsAlpha)
		{
			// Create the color table.
			var colorTable = new Color[4];
			colorTable[0] = ColorUtils.R5G6B5ToColor(reader.ReadLEUInt16());
			colorTable[1] = ColorUtils.R5G6B5ToColor(reader.ReadLEUInt16());

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

		/// <summary>
		/// Decodes a DXT3-compressed 4x4 block of texels.
		/// </summary>
		/// <remarks>See https://msdn.microsoft.com/en-us/library/windows/desktop/bb694531(v=vs.85).aspx#BC2 </remarks>
		private static Color32[] DecodeDXT3TexelBlock(UnityBinaryReader reader)
		{
			// Read compressed pixel alphas.
			var compressedAlphas = new byte[16];

			for(int rowIndex = 0; rowIndex < 4; rowIndex++)
			{
				var compressedAlphaRow = reader.ReadLEUInt16();

				for(int columnIndex = 0; columnIndex < 4; columnIndex++)
				{
					// Each compressed alpha is 4 bits.
					compressedAlphas[(4 * rowIndex) + columnIndex] = (byte)((compressedAlphaRow >> (columnIndex * 4)) & 0xF);
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
			colorTable[0] = ColorUtils.R5G6B5ToColor(reader.ReadLEUInt16());
			colorTable[1] = ColorUtils.R5G6B5ToColor(reader.ReadLEUInt16());
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

		/// <summary>
		/// Decodes a DXT5-compressed 4x4 block of texels.
		/// </summary>
		/// <remarks>See https://msdn.microsoft.com/en-us/library/windows/desktop/bb694531(v=vs.85).aspx#BC3 </remarks>
		private static Color32[] DecodeDXT5TexelBlock(UnityBinaryReader reader)
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

			var alphaIndexBytesRow0 = new byte[3];
			reader.Read(alphaIndexBytesRow0, 0, alphaIndexBytesRow0.Length);
			Array.Reverse(alphaIndexBytesRow0); // Take care of little-endianness.

			var alphaIndexBytesRow1 = new byte[3];
			reader.Read(alphaIndexBytesRow1, 0, alphaIndexBytesRow1.Length);
			Array.Reverse(alphaIndexBytesRow1); // Take care of little-endianness.

			const uint bitsPerAlphaIndex = 3;

			alphaIndices[0] = (uint)Utils.GetBits(21, bitsPerAlphaIndex, alphaIndexBytesRow0);
			alphaIndices[1] = (uint)Utils.GetBits(18, bitsPerAlphaIndex, alphaIndexBytesRow0);
			alphaIndices[2] = (uint)Utils.GetBits(15, bitsPerAlphaIndex, alphaIndexBytesRow0);
			alphaIndices[3] = (uint)Utils.GetBits(12, bitsPerAlphaIndex, alphaIndexBytesRow0);
			alphaIndices[4] = (uint)Utils.GetBits(9, bitsPerAlphaIndex, alphaIndexBytesRow0);
			alphaIndices[5] = (uint)Utils.GetBits(6, bitsPerAlphaIndex, alphaIndexBytesRow0);
			alphaIndices[6] = (uint)Utils.GetBits(3, bitsPerAlphaIndex, alphaIndexBytesRow0);
			alphaIndices[7] = (uint)Utils.GetBits(0, bitsPerAlphaIndex, alphaIndexBytesRow0);
			alphaIndices[8] = (uint)Utils.GetBits(21, bitsPerAlphaIndex, alphaIndexBytesRow1);
			alphaIndices[9] = (uint)Utils.GetBits(18, bitsPerAlphaIndex, alphaIndexBytesRow1);
			alphaIndices[10] = (uint)Utils.GetBits(15, bitsPerAlphaIndex, alphaIndexBytesRow1);
			alphaIndices[11] = (uint)Utils.GetBits(12, bitsPerAlphaIndex, alphaIndexBytesRow1);
			alphaIndices[12] = (uint)Utils.GetBits(9, bitsPerAlphaIndex, alphaIndexBytesRow1);
			alphaIndices[13] = (uint)Utils.GetBits(6, bitsPerAlphaIndex, alphaIndexBytesRow1);
			alphaIndices[14] = (uint)Utils.GetBits(3, bitsPerAlphaIndex, alphaIndexBytesRow1);
			alphaIndices[15] = (uint)Utils.GetBits(0, bitsPerAlphaIndex, alphaIndexBytesRow1);

			// Create the color table.
			var colorTable = new Color[4];
			colorTable[0] = ColorUtils.R5G6B5ToColor(reader.ReadLEUInt16());
			colorTable[1] = ColorUtils.R5G6B5ToColor(reader.ReadLEUInt16());
			colorTable[2] = Color.Lerp(colorTable[0], colorTable[1], 1.0f / 3);
			colorTable[3] = Color.Lerp(colorTable[0], colorTable[1], 2.0f / 3);

			// Calculate pixel colors.
			var colors = DecodeDXT1TexelBlock(reader, colorTable);

			for(int i = 0; i < 16; i++)
			{
				colors[i].a = (byte)Mathf.Round(alphaTable[alphaIndices[i]]);
			}

			return colors;
		}

		/// <summary>
		/// Copies a decoded texel block to a texture's data buffer. Takes into account DDS mipmap padding.
		/// </summary>
		/// <param name="decodedTexels">The decoded DDS texels.</param>
		/// <param name="argb">The texture's data buffer.</param>
		/// <param name="baseARGBIndex">The desired offset into the texture's data buffer. Used for mipmaps.</param>
		/// <param name="baseRowIndex">The base row index in the texture where decoded texels are copied.</param>
		/// <param name="baseColumnIndex">The base column index in the texture where decoded texels are copied.</param>
		/// <param name="textureWidth">The width of the texture.</param>
		/// <param name="textureHeight">The height of the texture.</param>
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

		/// <summary>
		/// Decodes DXT data to ARGB.
		/// </summary>
		private static byte[] DecodeDXTToARGB(int DXTVersion, byte[] compressedData, uint width, uint height, DDSPixelFormat pixelFormat, uint mipmapCount)
		{
			bool alphaFlag = Utils.ContainsBitFlags(pixelFormat.flags, (uint)DDSPixelFormatFlags.AlphaPixels);
			bool containsAlpha = alphaFlag || ((pixelFormat.RGBBitCount == 32) && (pixelFormat.ABitMask != 0));

			var reader = new UnityBinaryReader(new MemoryStream(compressedData));
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
						Color32[] colors = null;

						// Doing a switch instead of using a delegate for speed.
						switch(DXTVersion)
						{
							case 1:
								colors = DecodeDXT1TexelBlock(reader, containsAlpha);
								break;
							case 3:
								colors = DecodeDXT3TexelBlock(reader);
								break;
							case 5:
								colors = DecodeDXT5TexelBlock(reader);
								break;
							default:
								throw new NotImplementedException("Tried decoding a DDS file using an unsupported DXT format: DXT" + DXTVersion.ToString());
						}

						CopyDecodedTexelBlock(colors, argb, baseARGBIndex, rowIndex, columnIndex, mipMapWidth, mipMapHeight);
					}
				}

				baseARGBIndex += (mipMapWidth * mipMapHeight * 4);

				mipMapWidth /= 2;
				mipMapHeight /= 2;
			}

			return argb;
		}

		private static byte[] DecodeDXT1ToARGB(byte[] compressedData, uint width, uint height, DDSPixelFormat pixelFormat, uint mipmapCount)
		{
			return DecodeDXTToARGB(1, compressedData, width, height, pixelFormat, mipmapCount);
		}
		private static byte[] DecodeDXT3ToARGB(byte[] compressedData, uint width, uint height, DDSPixelFormat pixelFormat, uint mipmapCount)
		{
			return DecodeDXTToARGB(3, compressedData, width, height, pixelFormat, mipmapCount);
		}
		private static byte[] DecodeDXT5ToARGB(byte[] compressedData, uint width, uint height, DDSPixelFormat pixelFormat, uint mipmapCount)
		{
			return DecodeDXTToARGB(5, compressedData, width, height, pixelFormat, mipmapCount);
		}

		/// <summary>
		/// Extracts a DDS file's texture format and pixel data.
		/// </summary>
		private static void ExtractDDSTextureFormatAndData(DDSHeader header, UnityBinaryReader reader, out bool hasMipmaps, out uint DDSMipmapLevelCount, out TextureFormat textureFormat, out int bytesPerPixel, out byte[] textureData)
		{
			hasMipmaps = Utils.ContainsBitFlags(header.dwCaps, (uint)DDSCaps.Mipmap);

			// Non-mipmapped textures still have one mipmap level: the texture itself.
			DDSMipmapLevelCount = hasMipmaps ? header.dwMipMapCount : 1;

			// If the DDS file contains uncompressed data.
			if(Utils.ContainsBitFlags(header.pixelFormat.flags, (uint)DDSPixelFormatFlags.RGB))
			{
				// some permutation of RGB
				if(!Utils.ContainsBitFlags(header.pixelFormat.flags, (uint)DDSPixelFormatFlags.AlphaPixels))
				{
					throw new NotImplementedException("Unsupported DDS file pixel format.");
				}
				// some permutation of RGBA
				else
				{
					// There should be 32 bits per pixel.
					if(header.pixelFormat.RGBBitCount != 32)
					{
						throw new FileFormatException("Invalid DDS file pixel format.");
					}

					// BGRA32
					if((header.pixelFormat.BBitMask == 0x000000FF) && (header.pixelFormat.GBitMask == 0x0000FF00) && (header.pixelFormat.RBitMask == 0x00FF0000) && (header.pixelFormat.ABitMask == 0xFF000000))
					{
						textureFormat = TextureFormat.BGRA32;
						bytesPerPixel = 4;
					}
					// ARGB32
					else if((header.pixelFormat.ABitMask == 0x000000FF) && (header.pixelFormat.RBitMask == 0x0000FF00) && (header.pixelFormat.GBitMask == 0x00FF0000) && (header.pixelFormat.BBitMask == 0xFF000000))
					{
						textureFormat = TextureFormat.ARGB32;
						bytesPerPixel = 4;
					}
					else
					{
						throw new NotImplementedException("Unsupported DDS file pixel format.");
					}

					if(!hasMipmaps)
					{
						textureData = new byte[header.dwPitchOrLinearSize * header.dwHeight];
					}
					else
					{
						// Create a data buffer to hold all mipmap levels down to 1x1.
						textureData = new byte[TextureUtils.CalculateMipMappedTextureDataSize((int)header.dwWidth, (int)header.dwHeight, bytesPerPixel)];
					}
					
					reader.ReadRestOfBytes(textureData, 0);
				}
			}
			else if(StringUtils.Equals(header.pixelFormat.fourCC, "DXT1"))
			{
				textureFormat = TextureFormat.ARGB32;
				bytesPerPixel = 4;

				var compressedTextureData = reader.ReadRestOfBytes();
				textureData = DecodeDXT1ToARGB(compressedTextureData, header.dwWidth, header.dwHeight, header.pixelFormat, DDSMipmapLevelCount);
			}
			else if(StringUtils.Equals(header.pixelFormat.fourCC, "DXT3"))
			{
				textureFormat = TextureFormat.ARGB32;
				bytesPerPixel = 4;

				var compressedTextureData = reader.ReadRestOfBytes();
				textureData = DecodeDXT3ToARGB(compressedTextureData, header.dwWidth, header.dwHeight, header.pixelFormat, DDSMipmapLevelCount);
			}
			else if(StringUtils.Equals(header.pixelFormat.fourCC, "DXT5"))
			{
				textureFormat = TextureFormat.ARGB32;
				bytesPerPixel = 4;

				var compressedTextureData = reader.ReadRestOfBytes();
				textureData = DecodeDXT5ToARGB(compressedTextureData, header.dwWidth, header.dwHeight, header.pixelFormat, DDSMipmapLevelCount);
			}
			else
			{
				throw new NotImplementedException("Unsupported DDS file pixel format.");
			}
		}

		/// <summary>
		/// Generates missing mipmap levels for a DDS texture and optionally flips it.
		/// </summary>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		/// <param name="bytesPerPixel">The number of bytes per pixel.</param>
		/// <param name="hasMipmaps">Does the DDS texture have mipmaps?</param>
		/// <param name="DDSMipmapLevelCount">The number of mipmap levels in the DDS file. 1 if the DDS file doesn't have mipmaps.</param>
		/// <param name="data">The texture's data.</param>
		/// <param name="flipVertically">Should the texture be flipped vertically?</param>
		private static void PostProcessDDSTexture(int width, int height, int bytesPerPixel, bool hasMipmaps, int DDSMipmapLevelCount, byte[] data, bool flipVertically)
		{
			Debug.Assert((width > 0) && (height > 0) && (bytesPerPixel > 0) && (DDSMipmapLevelCount > 0) && (data != null));

			// Flip mip-maps if necessary and generate missing mip-map levels.
			int mipMapLevelWidth = width;
			int mipMapLevelHeight = height;
			int mipMapLevelIndex = 0;
			int mipMapLevelDataOffset = 0;

			// While we haven't processed all of the mipmap levels we should process.
			while((mipMapLevelWidth > 1) || (mipMapLevelHeight > 1))
			{
				var mipMapDataSize = (mipMapLevelWidth * mipMapLevelHeight * bytesPerPixel);

				// If the DDS file contains the current mipmap level, flip it vertically if necessary.
				if(flipVertically && (mipMapLevelIndex < DDSMipmapLevelCount))
				{
					Utils.Flip2DSubArrayVertically(data, mipMapLevelDataOffset, mipMapLevelHeight, mipMapLevelWidth * bytesPerPixel);
				}

				// Break after optionally flipping the first mipmap level if the DDS texture doesn't have mipmaps.
				if(!hasMipmaps)
				{
					break;
				}

				// Generate the next mipmap level's data if the DDS file doesn't contain it.
				if((mipMapLevelIndex + 1) >= DDSMipmapLevelCount)
				{
					TextureUtils.Downscale4Component32BitPixelsX2(data, mipMapLevelDataOffset, mipMapLevelHeight, mipMapLevelWidth, data, mipMapLevelDataOffset + mipMapDataSize);
				}

				// Switch to the next mipmap level.
				mipMapLevelIndex++;

				mipMapLevelWidth = (mipMapLevelWidth > 1) ? (mipMapLevelWidth / 2) : mipMapLevelWidth;
				mipMapLevelHeight = (mipMapLevelHeight > 1) ? (mipMapLevelHeight / 2) : mipMapLevelHeight;

				mipMapLevelDataOffset += mipMapDataSize;
			}
		}
	}
}