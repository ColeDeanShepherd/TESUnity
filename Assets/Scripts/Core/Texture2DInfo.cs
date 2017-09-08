using UnityEngine;

/// <summary>
/// Stores information about a 2D texture.
/// </summary>
public class Texture2DInfo
{
	public int width, height;
	public TextureFormat format;
	public bool hasMipmaps;
	public byte[] rawData;

	public Texture2DInfo(int width, int height, TextureFormat format, bool hasMipmaps, byte[] rawData)
	{
		this.width = width;
		this.height = height;
		this.format = format;
		this.hasMipmaps = hasMipmaps;
		this.rawData = rawData;
	}

	/// <summary>
	/// Creates a Unity Texture2D from this Texture2DInfo.
	/// </summary>
	public Texture2D ToTexture2D()
	{
		var texture = new Texture2D(width, height, format, hasMipmaps);

		if(rawData != null)
		{
			texture.LoadRawTextureData(rawData);
			texture.Apply();
		}
		
		return texture;
	}
}