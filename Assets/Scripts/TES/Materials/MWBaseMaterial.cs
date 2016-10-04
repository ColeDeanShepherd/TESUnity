using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TESUnity
{
    /// <summary>
    /// An abstract class to describe a material.
    /// </summary>
    public abstract class MWBaseMaterial
    {
        protected Dictionary<MWMaterialProps, Material> m_existingMaterials;
        protected TextureManager m_textureManager;

        public MWBaseMaterial(TextureManager textureManager)
        {
            m_textureManager = textureManager;
            m_existingMaterials = new Dictionary<MWMaterialProps, Material>();
        }

        public abstract Material BuildMaterialFromProperties(MWMaterialProps mp);
        public abstract Material BuildMaterial();
        public abstract Material BuildMaterialBlended(BlendMode sourceBlendMode, BlendMode destinationBlendMode);
        public abstract Material BuildMaterialTested(float cutoff);
    }
}
