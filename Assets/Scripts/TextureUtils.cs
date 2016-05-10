using UnityEngine;

public static class TextureUtils
{
	public static void FlipTexture2DVertically(Texture2D texture2D)
	{
		var pixels = texture2D.GetPixels32();

		Utils.Flip2DArrayVertically(pixels, texture2D.height, texture2D.width);

		texture2D.SetPixels32(pixels);
		texture2D.Apply();
	}

	public static int CalculateMipMapCount(int baseTextureWidth, int baseTextureHeight)
	{
		Debug.Assert((baseTextureWidth > 0) && (baseTextureHeight > 0));

		int longerLength = Mathf.Max(baseTextureWidth, baseTextureHeight);

		int mipMapCount = 0;
		int currentLongerLength = longerLength;

		while(currentLongerLength > 0)
		{
			mipMapCount++;

			currentLongerLength /= 2;
		}

		return mipMapCount;
	}
	public static int CalculateMipMappedTextureDataSize(int baseTextureWidth, int baseTextureHeight, int bytesPerPixel)
	{
		Debug.Assert((baseTextureWidth > 0) && (baseTextureHeight > 0) && (bytesPerPixel > 0));

		int dataSize = 0;
		int currentWidth = baseTextureWidth;
		int currentHeight = baseTextureHeight;

		while(true)
		{
			dataSize += (currentWidth * currentHeight * bytesPerPixel);

			if((currentWidth == 1) && (currentHeight == 1))
			{
				break;
			}

			currentWidth = (currentWidth > 1) ? (currentWidth / 2) : currentWidth;
			currentHeight = (currentHeight > 1) ? (currentHeight / 2) : currentHeight;
		}

		return dataSize;
	}

	public static void Downscale4Component32BitPixelsX2(byte[] srcBytes, int srcStartIndex, int srcRowCount, int srcColumnCount, byte[] dstBytes, int dstStartIndex)
	{

	}
}