using UnityEngine;
using ur = UnityEngine.Rendering;

namespace TESUnity
{
    /// <summary>
    /// A material that uses the new Standard Shader.
    /// </summary>
    public class MWStandardMaterial : MWBaseMaterial
    {
        private Material _standardMaterial;
        private Material _standardCutoutMaterial;

        public MWStandardMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            _standardMaterial = Resources.Load<Material>("Materials/Standard");
            _standardCutoutMaterial = Resources.Load<Material>("Materials/StandardCutout");
        }

        public override Material BuildMaterialFromProperties(MWMaterialProps mp)
        {
            Material material;

            //check if the material is already cached
            if (!m_existingMaterials.TryGetValue(mp, out material))
            {
                //otherwise create a new material and cache it
                if (mp.alphaBlended)
                    material = BuildMaterialBlended(mp.srcBlendMode, mp.dstBlendMode);
                else if (mp.alphaTest)
                    material = BuildMaterialTested(mp.alphaCutoff);
                else
                    material = BuildMaterial();

                if (mp.textures.mainFilePath != null)
                    material.mainTexture = m_textureManager.LoadTexture(mp.textures.mainFilePath);

                if (mp.textures.glowFilePath != null)
                {
                    material.EnableKeyword("_EMISION");
                    material.SetTexture("_EMISSION", m_textureManager.LoadTexture(mp.textures.glowFilePath));
                }

                if (mp.textures.bumpFilePath != null)
                {
                    material.EnableKeyword("_NORMALMAP");
                    material.SetTexture("_NORMALMAP", m_textureManager.LoadTexture(mp.textures.bumpFilePath));
                }

                if (mp.textures.glossFilePath != null)
                {
                    material.EnableKeyword("_METALLICGLOSSMAP");
                    material.SetTexture("_METALLICGLOSSMAP", m_textureManager.LoadTexture(mp.textures.glossFilePath));
                }

                m_existingMaterials[mp] = material;
            }
            return material;
        }

        public override Material BuildMaterial()
        {
            var material = new Material(Shader.Find("Standard"));
            material.CopyPropertiesFromMaterial(_standardMaterial);
            return material;
        }

        public override Material BuildMaterialBlended(ur.BlendMode sourceBlendMode, ur.BlendMode destinationBlendMode)
        {
            var material = BuildMaterialTested();
            //material.SetInt("_SrcBlend", (int)sourceBlendMode);
            //material.SetInt("_DstBlend", (int)destinationBlendMode);
            return material;
        }

        public override Material BuildMaterialTested(float cutoff = 0.5f)
        {
            var material = new Material(Shader.Find("Standard"));
            material.CopyPropertiesFromMaterial(_standardCutoutMaterial);
            material.SetFloat("_Cutout", cutoff);
            return material;
        }
    }
}