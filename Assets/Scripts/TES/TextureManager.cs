using System.Collections.Generic;
using UnityEngine;

namespace TESUnity
{
	/// <summary>
	/// Manages loading and instantiation of Morrowind textures. Thread safe.
	/// </summary>
	public class TextureManager
	{
        private static TextureManager instance = null;

        public static TextureManager Instance
        {
            get { return instance; }
        }

		public TextureManager(MorrowindDataReader dataReader)
		{
            instance = this;
			this.dataReader = dataReader;
		}

        public static Texture2D FlipTexture(Texture2D orig)
        {
            var flippedTexture = new Texture2D(orig.width, orig.height);
            var width = orig.width;
            var height = orig.height;

            for(var i = 0; i<width; i++)
                for(var j = 0; j<height; j++) 
                    flippedTexture.SetPixel(i, height - j - 1, orig.GetPixel(i, j));

            flippedTexture.Apply();
            return flippedTexture;
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