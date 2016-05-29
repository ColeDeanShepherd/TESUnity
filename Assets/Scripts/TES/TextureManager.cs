using System;
using System.Collections.Generic;
using UnityEngine;

namespace TESUnity
{
	/// <summary>
	/// Manages loading and instantiation of Morrowind textures.
	/// </summary>
	public class TextureManager
	{
		public TextureManager(MorrowindDataReader dataReader)
		{
			this.dataReader = dataReader;
		}
		public Texture2D LoadTexture(string texturePath)
		{
			Texture2D texture;

			if(!cachedTextures.TryGetValue(texturePath, out texture))
			{
				texture = dataReader.LoadTexture(texturePath);

				cachedTextures[texturePath] = texture;
			}

			return texture;
		}

		private MorrowindDataReader dataReader;
		private Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();
	}
}