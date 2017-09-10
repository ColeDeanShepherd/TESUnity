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
                var textureInfo = dataReader.LoadTexture(texturePath);

                texture = (textureInfo != null) ? textureInfo.ToTexture2D() : new Texture2D(1, 1);
                if(flipVertically) { TextureUtils.FlipTexture2DVertically(texture); }

                cachedTextures[texturePath] = texture;
            }
            
			return texture;
		}

        private MorrowindDataReader dataReader;
		private Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();
	}
}