using UnityEngine;

public static class ColorUtils
{
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