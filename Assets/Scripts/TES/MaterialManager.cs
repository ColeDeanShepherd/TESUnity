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

				if ( mp.textures.mainFilePath != null )
				{
					m.mainTexture = textureManager.LoadTexture( mp.textures.mainFilePath );
				}
				m.SetFloat( "_Metallic" , 0f );
				m.SetFloat( "_Glossiness" , 0f );
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
			Material m = new Material( Shader.Find( "TES Unity/Standard Blended" ) );
			m.SetInt( "_SrcBlend" , ( int )sourceBlendMode );
			m.SetInt( "_DstBlend" , ( int )destinationBlendMode );
			return m;
		}

		private Material BuildMaterialTested ( float cutoff = 0.5f )
		{
			Material m = new Material( Shader.Find( "TES Unity/Standard Blended" ) );
			m.SetFloat( "_Cutoff" , cutoff );
			return m;
		}
	}
}