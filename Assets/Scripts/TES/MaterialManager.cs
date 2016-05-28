using System;
using System.Collections.Generic;
using UnityEngine;

namespace TESUnity
{
	public enum MWMaterialType
	{
		Opaque,
		Translucent,
		Cutout
	}

	public struct MWMaterialProperties
	{
		public MWMaterialType type;
		public string mainTextureFileName;
		public float alphaCutoff;
	}

	/// <summary>
	/// Manages loading and instantiation of Morrowind materials.
	/// </summary>
	public class MaterialManager
	{
		public MaterialManager(TextureManager textureManager)
		{
			this.textureManager = textureManager;
		}
		public Material CreateMaterial(MWMaterialProperties materialProps)
		{
			Material material;

			if(!cachedMaterials.TryGetValue(materialProps, out material))
			{
				material = ForceCreateMaterial(materialProps);
				cachedMaterials[materialProps] = material;
			}

			return material;
		}

		private TextureManager textureManager;
		private Dictionary<MWMaterialProperties, Material> cachedMaterials = new Dictionary<MWMaterialProperties, Material>();

		private Material ForceCreateMaterial(MWMaterialProperties materialProps)
		{
			Material material;

			switch(materialProps.type)
			{
				case MWMaterialType.Opaque:
					material = new Material(TESUnity.instance.defaultMaterial);
					break;
				case MWMaterialType.Translucent:
					material = new Material(TESUnity.instance.fadeMaterial);
					break;
				case MWMaterialType.Cutout:
					material = new Material(TESUnity.instance.cutoutMaterial);
					material.SetFloat("alphaCutoff", materialProps.alphaCutoff);
					break;
				default:
					throw new NotImplementedException("Unsupported MWMaterialType: " + materialProps.type.ToString());
			}

			if(materialProps.mainTextureFileName != null)
			{
				material.mainTexture = textureManager.LoadTexture(materialProps.mainTextureFileName);
			}

			return material;
		}
	}
}