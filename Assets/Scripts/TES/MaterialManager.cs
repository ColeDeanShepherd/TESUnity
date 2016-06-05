using System;
using System.Collections.Generic;
using UnityEngine;

using ur = UnityEngine.Rendering;

namespace TESUnity
{
	public enum MatTestMode { Always, Less, LEqual, Equal, GEqual, Greater, NotEqual, Never }

	public struct MWMaterialTextures
	{
		public string mainFilePath;
		public string darkFilePath;
		public string detailFilePath;
		public string glossFilePath;
		public string glowFilePath;
		public string bumpFilePath;
	}

	public struct MWMaterialProps
	{
		public MWMaterialTextures textures;
		public bool alphaBlended;
		public ur.BlendMode srcBlendMode;
		public ur.BlendMode dstBlendMode;
		public bool alphaTest;
		public float alphaCutoff;
		public bool zWrite;
	}

	/// <summary>
	/// Manages loading and instantiation of Morrowind materials. Not thread safe.
	/// </summary>
	public class MaterialManager
	{
		public TextureManager textureManager;
		private Dictionary<MWMaterialProps , Material> existingMaterials = new Dictionary<MWMaterialProps , Material>();
		public MaterialManager ( TextureManager textureManager ){ this.textureManager = textureManager; }

		public Material BuildMaterialFromProperties ( MWMaterialProps mp )
		{
			Material m;
			//check if the material is already cached
			if ( !existingMaterials.TryGetValue( mp , out m ) )
			{
				//otherwise create a new material and cache it
				if ( mp.alphaBlended )
					m = BuildMaterialBlended( mp.srcBlendMode , mp.dstBlendMode );
				else if ( mp.alphaTest )
					m = BuildMaterialTested( mp.alphaCutoff );
				else
					m = BuildMaterial();

				if ( mp.textures.mainFilePath != null && m.HasProperty( "_MainTex" ) ) m.SetTexture( "_MainTex" , textureManager.LoadTexture( mp.textures.mainFilePath ) );
				if ( mp.textures.detailFilePath != null && m.HasProperty( "_DetailTex" ) ) m.SetTexture( "_DetailTex" , textureManager.LoadTexture( mp.textures.detailFilePath ) );
				if ( mp.textures.darkFilePath != null && m.HasProperty( "_DarkTex" ) ) m.SetTexture( "_DarkTex" , textureManager.LoadTexture( mp.textures.darkFilePath ) );
				if ( mp.textures.glossFilePath != null && m.HasProperty( "_GlossTex" ) ) m.SetTexture( "_GlossTex" , textureManager.LoadTexture( mp.textures.glossFilePath ) );
				if ( mp.textures.glowFilePath != null && m.HasProperty( "_Glowtex" ) )
				{
					m.SetTexture( "_Glowtex" , textureManager.LoadTexture( mp.textures.glowFilePath ) );
					Debug.Log( "Glowing Mat Created" );
				}
				if ( mp.textures.bumpFilePath != null && m.HasProperty( "_BumpTex" ) ) m.SetTexture( "_BumpTex" , textureManager.LoadTexture( mp.textures.bumpFilePath ) );

				if ( m.HasProperty( "_Metallic" ) ) m.SetFloat( "_Metallic" , 0f );
				if ( m.HasProperty( "_Glossiness" ) ) m.SetFloat( "_Glossiness" , 0f );
				existingMaterials[ mp ] = m;
			}
			return m;
		}

		private Material BuildMaterial ()
		{
			return new Material( Shader.Find( "TES Unity/Standard" ) );
		}

		private Material BuildMaterialBlended ( ur.BlendMode sourceBlendMode , ur.BlendMode destinationBlendMode )
		{
			Material m = new Material( Shader.Find( "TES Unity/Alpha Blended" ) );
			m.SetInt( "_SrcBlend" , ( int )sourceBlendMode );
			m.SetInt( "_DstBlend" , ( int )destinationBlendMode );
			return m;
		}

		private Material BuildMaterialTested ( float cutoff = 0.5f )
		{
			Material m = new Material( Shader.Find( "TES Unity/Alpha Tested" ) );
			m.SetFloat( "_Cutoff" , cutoff );
			return m;
		}
	}
}