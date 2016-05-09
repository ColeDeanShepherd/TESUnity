using UnityEngine;

public static class TextureUtils
{
	public static void FlipTexture2DVertically(Texture2D texture2D)
	{
		var pixels = texture2D.GetPixels32();

		Utils.Flip2DArrayVertically(ref pixels, texture2D.height, texture2D.width);

		texture2D.SetPixels32(pixels);
		texture2D.Apply();
	}
}