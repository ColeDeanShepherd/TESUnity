using System.Collections.Generic;
using UnityEngine;

namespace TESUnity
{
	/// <summary>
	/// Manages loading and instantiation of Morrowind textures. Thread safe.
	/// </summary>
	public class TextureManager
	{
		public TextureManager(MorrowindDataReader dataReader)
		{
			this.dataReader = dataReader;
		}

		/// <summary>
		/// Loads a texture and caches it. Thread safe.
		/// </summary>
		public void PreLoadTexture(string texturePath)
		{
			// If the texture is already loaded, return.
			lock(dictionariesLock)
			{
				if(cachedTextureInfos.ContainsKey(texturePath) || cachedTextures.ContainsKey(texturePath))
				{
					return;
				}
			}

			// The texture hasn't been loaded yet, so load it.
			var textureInfo = dataReader.LoadTexture(texturePath);

			lock(dictionariesLock)
			{
				cachedTextureInfos[texturePath] = textureInfo;
			}
		}

		/// <summary>
		/// Loads a texture. Can only be called from the main thread.
		/// </summary>
		public Texture2D LoadTexture(string texturePath)
		{
			// Try to get the cached Texture2D.
			Texture2D texture;

			lock(dictionariesLock)
			{
				cachedTextures.TryGetValue(texturePath, out texture);
			}

			// If there is no cached Texture2D.
			if(texture == null)
			{
				// Load the Texture2DInfo.
				Texture2DInfo textureInfo = LoadTextureInfoAndRemoveFromCache(texturePath);

				texture = textureInfo.ToTexture2D();

				lock(dictionariesLock)
				{
					cachedTextures[texturePath] = texture;
				}
			}

			return texture;
		}

		private MorrowindDataReader dataReader;

		private object dictionariesLock = new object();
		private Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();
		private Dictionary<string, Texture2DInfo> cachedTextureInfos = new Dictionary<string, Texture2DInfo>();

		private Texture2DInfo LoadTextureInfoAndRemoveFromCache(string texturePath)
		{
			// Try to get the cached Texture2DInfo.
			Texture2DInfo textureInfo;

			lock(dictionariesLock)
			{
				cachedTextureInfos.TryGetValue(texturePath, out textureInfo);
			}

			// If there is no cached Texture2DInfo.
			if(textureInfo == null)
			{
				textureInfo = dataReader.LoadTexture(texturePath);
			}
			else
			{
				lock(dictionariesLock)
				{
					cachedTextureInfos.Remove(texturePath);
				}
			}

			return textureInfo;
		}
	}
}