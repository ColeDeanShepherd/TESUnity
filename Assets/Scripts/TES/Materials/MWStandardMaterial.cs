using UnityEngine;
using ur = UnityEngine.Rendering;

namespace TESUnity
{
    public class MWStandardMaterial : MWBaseMaterial
    {
        public MWStandardMaterial(TextureManager textureManager) : base(textureManager) { }

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

                if (mp.textures.mainFilePath != null && material.HasProperty("_Albedo"))
                    material.SetTexture("_Albedo", m_textureManager.LoadTexture(mp.textures.mainFilePath));

                if (mp.textures.detailFilePath != null && material.HasProperty("_DetailMask"))
                    material.SetTexture("_DetailMask", m_textureManager.LoadTexture(mp.textures.detailFilePath));

                if (mp.textures.darkFilePath != null && material.HasProperty("_Occlusion"))
                    material.SetTexture("_Occlusion", m_textureManager.LoadTexture(mp.textures.darkFilePath));

                if (mp.textures.glowFilePath != null && material.HasProperty("_Emission"))
                    material.SetTexture("_Emission", m_textureManager.LoadTexture(mp.textures.glowFilePath));

                if (mp.textures.bumpFilePath != null && material.HasProperty("_NormalMap"))
                    material.SetTexture("_NormalMap", m_textureManager.LoadTexture(mp.textures.bumpFilePath));

                if (material.HasProperty("_Metallic"))
                    material.SetFloat("_Metallic", 0f);

                if (material.HasProperty("_Smoothness"))
                    material.SetFloat("_Smoothness", 0f);

                m_existingMaterials[mp] = material;
            }
            return material;
        }

        public override Material BuildMaterial()
        {
            return new Material(Shader.Find("Standard"));
        }

        public override Material BuildMaterialBlended(ur.BlendMode sourceBlendMode, ur.BlendMode destinationBlendMode)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.SetInt("_SrcBlend", (int)sourceBlendMode);
            material.SetInt("_DstBlend", (int)destinationBlendMode);
            material.SetFloat("_Mode", 1);
            return material;
        }

        public override Material BuildMaterialTested(float cutoff = 0.5f)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.SetFloat("_Mode", 1);
            material.SetFloat("_Cutout", cutoff);
            return material;
        }
    }
}