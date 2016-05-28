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
		public Texture2D LoadTexture(string textureName)
		{
			Texture2D texture;

			if(!cachedTextures.TryGetValue(textureName, out texture))
			{
				texture = dataReader.LoadTexture(textureName);

				cachedTextures[textureName] = texture;
			}

			return texture;
		}

		private MorrowindDataReader dataReader;
		private Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();
	}
}