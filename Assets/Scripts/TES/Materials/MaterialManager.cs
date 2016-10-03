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
        private MWBaseMaterial _materialInterface;
        private TextureManager _textureManager;

        public TextureManager TextureManager
        {
            get { return _textureManager; }
        }

        public MaterialManager(TextureManager textureManager)
        {
            _textureManager = textureManager;

            switch(TESUnity.instance.materialType)
            {
                case TESUnity.MWMaterialType.Default:
                    _materialInterface = new MWDefaultMaterial(textureManager);
                    break;
                case TESUnity.MWMaterialType.Standard:
                    _materialInterface = new MWStandardMaterial(textureManager);
                    break;
                case TESUnity.MWMaterialType.Unlit:
                    _materialInterface = new MWUnliteMaterial(textureManager);
                    break;
                default:
                    _materialInterface = new MWBumpedDiffuseMaterial(textureManager);
                    break;
            }
        }

        public Material BuildMaterialFromProperties(MWMaterialProps mp)
        {
            return _materialInterface.BuildMaterialFromProperties(mp);
        }

        private Material BuildMaterial()
        {
            return _materialInterface.BuildMaterial();
        }

        private Material BuildMaterialBlended(ur.BlendMode sourceBlendMode, ur.BlendMode destinationBlendMode)
        {
            return _materialInterface.BuildMaterialBlended(sourceBlendMode, destinationBlendMode);
        }

        private Material BuildMaterialTested(float cutoff = 0.5f)
        {
            return _materialInterface.BuildMaterialTested(cutoff);
        }
    }
}