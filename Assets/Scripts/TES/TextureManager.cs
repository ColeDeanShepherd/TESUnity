using System.Collections.Generic;
using System.Threading.Tasks;
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

        /// <summary>
        /// Loads a texture.
        /// </summary>
        /// <param name="texturePath">The texture's path</param>
        /// <param name="flipVertically">Indicates if the texture must be vertically flipped. Default is False.</param>
        /// <returns></returns>
        public Texture2D LoadTexture(string texturePath, bool flipVertically = false)
        {
            Texture2D texture;

            if (!cachedTextures.TryGetValue(texturePath, out texture))
            {
                // Load & cache the texture.
                var textureInfo = LoadTextureInfo(texturePath);

                texture = (textureInfo != null) ? textureInfo.ToTexture2D() : new Texture2D(1, 1);
                if(flipVertically) { TextureUtils.FlipTexture2DVertically(texture); }

                cachedTextures[texturePath] = texture;
            }
            
			return texture;
		}
        public void PreloadTextureFileAsync(string texturePath)
        {
            // If the texture has already been created we don't have to load the file again.
            if(cachedTextures.ContainsKey(texturePath)) { return; }

            Task<Texture2DInfo> textureFileLoadingTask;

            // Start loading the texture file asynchronously if we haven't already started.
            if(!textureFilePreloadTasks.TryGetValue(texturePath, out textureFileLoadingTask))
            {
                textureFileLoadingTask = dataReader.LoadTextureAsync(texturePath);
                textureFilePreloadTasks[texturePath] = textureFileLoadingTask;
            }
        }

        private MorrowindDataReader dataReader;
        private Dictionary<string, Task<Texture2DInfo>> textureFilePreloadTasks = new Dictionary<string, Task<Texture2DInfo>>();
		private Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();

        private Texture2DInfo LoadTextureInfo(string texturePath)
        {
            Debug.Assert(!cachedTextures.ContainsKey(texturePath));

            PreloadTextureFileAsync(texturePath);
            var textureInfo = textureFilePreloadTasks[texturePath].Result;
            textureFilePreloadTasks.Remove(texturePath);

            return textureInfo;
        }
	}
}