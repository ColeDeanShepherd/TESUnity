using UnityEngine;

public static class ColorUtils
{
	public static Color B8G8R8ToColor(uint B8G8R8)
	{
		return B8G8R8ToColor32(B8G8R8);
	}
	public static Color32 B8G8R8ToColor32(uint B8G8R8)
	{
		var B8 = (byte)((B8G8R8 >> 16) & 0xFF);
		var G8 = (byte)((B8G8R8 >> 8) & 0xFF);
		var R8 = (byte)(B8G8R8 & 0xFF);

		return new Color32(R8, G8, B8, 255);
	}

	public static Color R5G6B5ToColor(ushort R5G6B5)
	{
		var R5 = ((R5G6B5 >> 11) & 31);
		var G6 = ((R5G6B5 >> 5) & 63);
		var B5 = (R5G6B5 & 31);

		return new Color((float)R5 / 31, (float)G6 / 63, (float)B5 / 31, 1);
	}
	public static Color32 R5G6B5ToColor32(ushort R5G6B5)
	{
		return R5G6B5ToColor(R5G6B5);
	}
}