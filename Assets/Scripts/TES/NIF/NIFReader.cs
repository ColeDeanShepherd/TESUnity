using System;
using System.IO;
using UnityEngine;

namespace TESUnity
{
	namespace NIF
	{
		public class NiReaderUtils
		{
			public static Ptr<T> ReadPtr<T>(UnityBinaryReader reader)
			{
				var ptr = new Ptr<T>();
				ptr.Deserialize(reader);

				return ptr;
			}
			public static Ref<T> ReadRef<T>(UnityBinaryReader reader)
			{
				var readRef = new Ref<T>();
				readRef.Deserialize(reader);

				return readRef;
			}
			public static Ref<T>[] ReadLengthPrefixedRefs32<T>(UnityBinaryReader reader)
			{
				var refs = new Ref<T>[reader.ReadLEUInt32()];

				for(int i = 0; i < refs.Length; i++)
				{
					refs[i] = ReadRef<T>(reader);
				}

				return refs;
			}
			public static ushort ReadFlags(UnityBinaryReader reader)
			{
				return reader.ReadLEUInt16();
			}
			public static T Read<T>(UnityBinaryReader reader)
			{
				if(typeof(T) == typeof(float))
				{
					return (T)((object)reader.ReadLESingle());
				}
				else if(typeof(T) == typeof(byte))
				{
					return (T)((object)reader.ReadByte());
				}
				else if(typeof(T) == typeof(string))
				{
					return (T)((object)reader.ReadLELength32PrefixedASCIIString());
				}
				else if(typeof(T) == typeof(Vector3))
				{
					return (T)((object)reader.ReadLEVector3());
				}
				else if(typeof(T) == typeof(Quaternion))
				{
					return (T)((object)reader.ReadLEQuaternionWFirst());
				}
				else if(typeof(T) == typeof(Color4))
				{
					var color = new Color4();
					color.Deserialize(reader);

					return (T)((object)color);
				}
				else
				{
					throw new NotImplementedException("Tried to read an unsupported type.");
				}
			}
			public static NiObject ReadNiObject(UnityBinaryReader reader)
			{
				var nodeTypeBytes = reader.ReadLELength32PrefixedBytes();

				if(StringUtils.Equals(nodeTypeBytes, "NiNode"))
				{
					var node = new NiNode();
					node.Deserialize(reader);

					return node;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiTriShape"))
				{
					var triShape = new NiTriShape();
					triShape.Deserialize(reader);

					return triShape;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiTexturingProperty"))
				{
					var prop = new NiTexturingProperty();
					prop.Deserialize(reader);

					return prop;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiSourceTexture"))
				{
					var srcTexture = new NiSourceTexture();
					srcTexture.Deserialize(reader);

					return srcTexture;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiMaterialProperty"))
				{
					var prop = new NiMaterialProperty();
					prop.Deserialize(reader);

					return prop;
				}
                else if(StringUtils.Equals(nodeTypeBytes, "NiMaterialColorController"))
                {
                    var controller = new NiMaterialColorController();
                    controller.Deserialize(reader);

                    return controller;
                }
				else if(StringUtils.Equals(nodeTypeBytes, "NiTriShapeData"))
				{
					var data = new NiTriShapeData();
					data.Deserialize(reader);

					return data;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "RootCollisionNode"))
				{
					var node = new RootCollisionNode();
					node.Deserialize(reader);

					return node;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiStringExtraData"))
				{
					var data = new NiStringExtraData();
					data.Deserialize(reader);

					return data;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiSkinInstance"))
				{
					var instance = new NiSkinInstance();
					instance.Deserialize(reader);

					return instance;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiSkinData"))
				{
					var data = new NiSkinData();
					data.Deserialize(reader);

					return data;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiAlphaProperty"))
				{
					var prop = new NiAlphaProperty();
					prop.Deserialize(reader);

					return prop;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiZBufferProperty"))
				{
					var prop = new NiZBufferProperty();
					prop.Deserialize(reader);

					return prop;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiVertexColorProperty"))
				{
					var prop = new NiVertexColorProperty();
					prop.Deserialize(reader);

					return prop;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiBSAnimationNode"))
				{
					var node = new NiBSAnimationNode();
					node.Deserialize(reader);

					return node;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiBSParticleNode"))
				{
					var node = new NiBSParticleNode();
					node.Deserialize(reader);

					return node;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiParticles"))
				{
					var node = new NiParticles();
					node.Deserialize(reader);

					return node;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiParticlesData"))
				{
					var data = new NiParticlesData();
					data.Deserialize(reader);

					return data;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiRotatingParticles"))
				{
					var node = new NiRotatingParticles();
					node.Deserialize(reader);

					return node;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiRotatingParticlesData"))
				{
					var data = new NiRotatingParticlesData();
					data.Deserialize(reader);

					return data;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiAutoNormalParticles"))
				{
					var node = new NiAutoNormalParticles();
					node.Deserialize(reader);

					return node;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiAutoNormalParticlesData"))
				{
					var data = new NiAutoNormalParticlesData();
					data.Deserialize(reader);

					return data;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiUVController"))
				{
					var controller = new NiUVController();
					controller.Deserialize(reader);

					return controller;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiUVData"))
				{
					var data = new NiUVData();
					data.Deserialize(reader);

					return data;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiTextureEffect"))
				{
					var effect = new NiTextureEffect();
					effect.Deserialize(reader);

					return effect;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiTextKeyExtraData"))
				{
					var data = new NiTextKeyExtraData();
					data.Deserialize(reader);

					return data;
				}
                else if(StringUtils.Equals(nodeTypeBytes, "NiVertWeightsExtraData"))
                {
                    var data = new NiVertWeightsExtraData();
                    data.Deserialize(reader);

                    return data;
                }
				else if(StringUtils.Equals(nodeTypeBytes, "NiParticleSystemController"))
				{
					var controller = new NiParticleSystemController();
					controller.Deserialize(reader);

					return controller;
				}
                else if(StringUtils.Equals(nodeTypeBytes, "NiBSPArrayController"))
                {
                    var controller = new NiBSPArrayController();
                    controller.Deserialize(reader);

                    return controller;
                }
				else if(StringUtils.Equals(nodeTypeBytes, "NiGravity"))
				{
					var obj = new NiGravity();
					obj.Deserialize(reader);

					return obj;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiParticleBomb"))
				{
					var modifier = new NiParticleBomb();
					modifier.Deserialize(reader);

					return modifier;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiParticleColorModifier"))
				{
					var modifier = new NiParticleColorModifier();
					modifier.Deserialize(reader);

					return modifier;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiParticleGrowFade"))
				{
					var modifier = new NiParticleGrowFade();
					modifier.Deserialize(reader);

					return modifier;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiParticleMeshModifier"))
				{
					var modifier = new NiParticleMeshModifier();
					modifier.Deserialize(reader);

					return modifier;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiParticleRotation"))
				{
					var modifier = new NiParticleRotation();
					modifier.Deserialize(reader);

					return modifier;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiKeyframeController"))
				{
					var controller = new NiKeyframeController();
					controller.Deserialize(reader);

					return controller;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiKeyframeData"))
				{
					var data = new NiKeyframeData();
					data.Deserialize(reader);

					return data;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiColorData"))
				{
					var data = new NiColorData();
					data.Deserialize(reader);

					return data;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiGeomMorpherController"))
				{
					var controller = new NiGeomMorpherController();
					controller.Deserialize(reader);

					return controller;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiMorphData"))
				{
					var data = new NiMorphData();
					data.Deserialize(reader);

					return data;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "AvoidNode"))
				{
					var node = new AvoidNode();
					node.Deserialize(reader);

					return node;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiVisController"))
				{
					var controller = new NiVisController();
					controller.Deserialize(reader);

					return controller;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiVisData"))
				{
					var data = new NiVisData();
					data.Deserialize(reader);

					return data;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiAlphaController"))
				{
					var controller = new NiAlphaController();
					controller.Deserialize(reader);

					return controller;
				}
				else if(StringUtils.Equals(nodeTypeBytes, "NiFloatData"))
				{
					var data = new NiFloatData();
					data.Deserialize(reader);

					return data;
				}
                else if(StringUtils.Equals(nodeTypeBytes, "NiPosData"))
                {
                    var data = new NiPosData();
                    data.Deserialize(reader);

                    return data;
                }
                else if(StringUtils.Equals(nodeTypeBytes, "NiBillboardNode"))
                {
                    var data = new NiBillboardNode();
                    data.Deserialize(reader);

                    return data;
                }
                else if(StringUtils.Equals(nodeTypeBytes, "NiShadeProperty"))
                {
                    var property = new NiShadeProperty();
                    property.Deserialize(reader);

                    return property;
                }
				else

                {
					Debug.Log("Tried to read an unsupported NiObject type (" + System.Text.Encoding.ASCII.GetString(nodeTypeBytes) + ").");
                    return null;
				}
			}
			public static Matrix4x4 Read3x3RotationMatrix(UnityBinaryReader reader)
			{
				return reader.ReadLERowMajorMatrix3x3();
			}
		}

		public class NiFile
		{
			public NiFile(string name)
			{
				this.name = name;
			}
			public string name;
			public NiHeader header;
			public NiObject[] blocks;
			public NiFooter footer;

			public void Deserialize(UnityBinaryReader reader)
			{
				header = new NiHeader();
				header.Deserialize(reader);

				blocks = new NiObject[header.numBlocks];
				for(int i = 0; i < header.numBlocks; i++)
				{
					blocks[i] = NiReaderUtils.ReadNiObject(reader);
				}

				footer = new NiFooter();
				footer.Deserialize(reader);
			}
		}

        public class NiHeader
        {
            public byte[] str; // 40 bytes (including \n)
            public uint version;
            public uint numBlocks;

            public void Deserialize(UnityBinaryReader reader)
            {
                str = reader.ReadBytes(40);
                version = reader.ReadLEUInt32();
                numBlocks = reader.ReadLEUInt32();
            }
        }
        public class NiFooter
        {
            public uint numRoots;
            public int[] roots;

            public void Deserialize(UnityBinaryReader reader)
            {
                numRoots = reader.ReadLEUInt32();

                roots = new int[numRoots];
                for(int i = 0; i < numRoots; i++)
                {
                    roots[i] = reader.ReadLEInt32();
                }
            }
        }

        // Refers to an object before the current one in the hierarchy.
        public struct Ptr<T>
        {
            public int value;
            public bool isNull { get { return value < 0; } }

            public void Deserialize(UnityBinaryReader reader)
            {
                value = reader.ReadLEInt32();
            }
        }

        // Refers to an object after the current one in the hierarchy.
        public struct Ref<T>
        {
            public int value;
            public bool isNull { get { return value < 0; } }

            public void Deserialize(UnityBinaryReader reader)
            {
                value = reader.ReadLEInt32();
            }
        }

        #region Enums
        // An unsigned 32-bit integer, describing how transparency is handled in a texture.
        public enum AlphaFormat : uint
        {
            ALPHA_NONE = 0, // No alpha blending; the texture is fully opaque.
            ALPHA_BINARY = 1, // Texture is either fully transparent or fully opaque.  There are no partially transparent areas.
            ALPHA_SMOOTH = 2, // Full range of alpha values can be used from fully transparent to fully opaque including all partially transparent values in between.
            ALPHA_DEFAULT = 3 // Use default setting.
        }

        // An unsigned 32-bit integer, describing the apply mode of a texture.
        public enum ApplyMode : uint
        {
            APPLY_REPLACE = 0, // Replaces existing color
            APPLY_DECAL = 1, // For placing images on the object like stickers.
            APPLY_MODULATE = 2, // Modulates existing color. (Default)
            APPLY_HILIGHT = 3, // PS2 Only.  Function Unknown.
            APPLY_HILIGHT2 = 4 // Parallax Flag in some Oblivion meshes.
        }

        // The type of texture.
        public enum TexType : uint
        {
            BASE_MAP = 0, // The basic texture used by most meshes.
            DARK_MAP = 1, // Used to darken the model with false lighting.
            DETAIL_MAP = 2, // Combined with base map for added detail.  Usually tiled over the mesh many times for close-up view.
            GLOSS_MAP = 3, // Allows the specularity (glossyness) of an object to differ across its surface.
            GLOW_MAP = 4, // Creates a glowing effect.  Basically an incandescence map.
            BUMP_MAP = 5, // Used to make the object appear to have more detail than it really does.
            NORMAL_MAP = 6, // Used to make the object appear to have more detail than it really does.
            UNKNOWN2_MAP = 7, // Unknown map.
            DECAL_0_MAP = 8, // For placing images on the object like stickers.
            DECAL_1_MAP = 9, // For placing images on the object like stickers.
            DECAL_2_MAP = 10, // For placing images on the object like stickers.
            DECAL_3_MAP = 11 // For placing images on the object like stickers.
        }

        // The type of animation interpolation (blending) that will be used on the associated key frames.
        public enum KeyType : uint
        {
            LINEAR_KEY = 1, // Use linear interpolation.
            QUADRATIC_KEY = 2, // Use quadratic interpolation.  Forward and back tangents will be stored.
            TBC_KEY = 3, // Use Tension Bias Continuity interpolation.  Tension, bias, and continuity will be stored.
            XYZ_ROTATION_KEY = 4, // For use only with rotation data.  Separate X, Y, and Z keys will be stored instead of using quaternions.
            CONST_KEY = 5 // Step function. Used for visibility keys in NiBoolData.
        }

        // An unsigned 32-bit integer, describing how vertex colors influence lighting.
        public enum LightMode : uint
        {
            LIGHT_MODE_EMISSIVE = 0, // Emissive.
            LIGHT_MODE_EMI_AMB_DIF = 1 // Emissive + Ambient + Diffuse. (Default)
        }

        // A material, used by havok shape objects in Oblivion.
        public enum OblivionHavokMaterial : uint
        {
            OB_HAV_MAT_STONE = 0, // Stone
            OB_HAV_MAT_CLOTH = 1, // Cloth
            OB_HAV_MAT_DIRT = 2, // Dirt
            OB_HAV_MAT_GLASS = 3, // Glass
            OB_HAV_MAT_GRASS = 4, // Grass
            OB_HAV_MAT_METAL = 5, // Metal
            OB_HAV_MAT_ORGANIC = 6, // Organic
            OB_HAV_MAT_SKIN = 7, // Skin
            OB_HAV_MAT_WATER = 8, // Water
            OB_HAV_MAT_WOOD = 9, // Wood
            OB_HAV_MAT_HEAVY_STONE = 10, // Heavy Stone
            OB_HAV_MAT_HEAVY_METAL = 11, // Heavy Metal
            OB_HAV_MAT_HEAVY_WOOD = 12, // Heavy Wood
            OB_HAV_MAT_CHAIN = 13, // Chain
            OB_HAV_MAT_SNOW = 14, // Snow
            OB_HAV_MAT_STONE_STAIRS = 15, // Stone Stairs
            OB_HAV_MAT_CLOTH_STAIRS = 16, // Cloth Stairs
            OB_HAV_MAT_DIRT_STAIRS = 17, // Dirt Stairs
            OB_HAV_MAT_GLASS_STAIRS = 18, // Glass Stairs
            OB_HAV_MAT_GRASS_STAIRS = 19, // Grass Stairs
            OB_HAV_MAT_METAL_STAIRS = 20, // Metal Stairs
            OB_HAV_MAT_ORGANIC_STAIRS = 21, // Organic Stairs
            OB_HAV_MAT_SKIN_STAIRS = 22, // Skin Stairs
            OB_HAV_MAT_WATER_STAIRS = 23, // Water Stairs
            OB_HAV_MAT_WOOD_STAIRS = 24, // Wood Stairs
            OB_HAV_MAT_HEAVY_STONE_STAIRS = 25, // Heavy Stone Stairs
            OB_HAV_MAT_HEAVY_METAL_STAIRS = 26, // Heavy Metal Stairs
            OB_HAV_MAT_HEAVY_WOOD_STAIRS = 27, // Heavy Wood Stairs
            OB_HAV_MAT_CHAIN_STAIRS = 28, // Chain Stairs
            OB_HAV_MAT_SNOW_STAIRS = 29, // Snow Stairs
            OB_HAV_MAT_ELEVATOR = 30, // Elevator
            OB_HAV_MAT_RUBBER = 31 // Rubber
        }

        // A material, used by havok shape objects in Fallout 3.        Bit 5: flag for PLATFORM (for values 32-63 substract 32 to know material number)        Bit 6: flag for STAIRS  (for values 64-95 substract 64 to know material number)        Bit 5+6: flag for STAIRS+PLATFORM  (for values 96-127 substract 96 to know material number)
        public enum Fallout3HavokMaterial : uint
        {
            FO_HAV_MAT_STONE = 0, // Stone
            FO_HAV_MAT_CLOTH = 1, // Cloth
            FO_HAV_MAT_DIRT = 2, // Dirt
            FO_HAV_MAT_GLASS = 3, // Glass
            FO_HAV_MAT_GRASS = 4, // Grass
            FO_HAV_MAT_METAL = 5, // Metal
            FO_HAV_MAT_ORGANIC = 6, // Organic
            FO_HAV_MAT_SKIN = 7, // Skin
            FO_HAV_MAT_WATER = 8, // Water
            FO_HAV_MAT_WOOD = 9, // Wood
            FO_HAV_MAT_HEAVY_STONE = 10, // Heavy Stone
            FO_HAV_MAT_HEAVY_METAL = 11, // Heavy Metal
            FO_HAV_MAT_HEAVY_WOOD = 12, // Heavy Wood
            FO_HAV_MAT_CHAIN = 13, // Chain
            FO_HAV_MAT_BOTTLECAP = 14, // Bottlecap
            FO_HAV_MAT_ELEVATOR = 15, // Elevator
            FO_HAV_MAT_HOLLOW_METAL = 16, // Hollow Metal
            FO_HAV_MAT_SHEET_METAL = 17, // Sheet Metal
            FO_HAV_MAT_SAND = 18, // Sand
            FO_HAV_MAT_BROKEN_CONCRETE = 19, // Broken Concrete
            FO_HAV_MAT_VEHICLE_BODY = 20, // Vehicle Body
            FO_HAV_MAT_VEHICLE_PART_SOLID = 21, // Vehicle Part Solid
            FO_HAV_MAT_VEHICLE_PART_HOLLOW = 22, // Vehicle Part Hollow
            FO_HAV_MAT_BARREL = 23, // Barrel
            FO_HAV_MAT_BOTTLE = 24, // Bottle
            FO_HAV_MAT_SODA_CAN = 25, // Soda Can
            FO_HAV_MAT_PISTOL = 26, // Pistol
            FO_HAV_MAT_RIFLE = 27, // Rifle
            FO_HAV_MAT_SHOPPING_CART = 28, // Shopping Cart
            FO_HAV_MAT_LUNCHBOX = 29, // Lunchbox
            FO_HAV_MAT_BABY_RATTLE = 30, // Baby Rattle
            FO_HAV_MAT_RUBBER_BALL = 31, // Rubber Ball
            FO_HAV_MAT_STONE_PLATFORM = 32, // Stone
            FO_HAV_MAT_CLOTH_PLATFORM = 33, // Cloth
            FO_HAV_MAT_DIRT_PLATFORM = 34, // Dirt
            FO_HAV_MAT_GLASS_PLATFORM = 35, // Glass
            FO_HAV_MAT_GRASS_PLATFORM = 36, // Grass
            FO_HAV_MAT_METAL_PLATFORM = 37, // Metal
            FO_HAV_MAT_ORGANIC_PLATFORM = 38, // Organic
            FO_HAV_MAT_SKIN_PLATFORM = 39, // Skin
            FO_HAV_MAT_WATER_PLATFORM = 40, // Water
            FO_HAV_MAT_WOOD_PLATFORM = 41, // Wood
            FO_HAV_MAT_HEAVY_STONE_PLATFORM = 42, // Heavy Stone
            FO_HAV_MAT_HEAVY_METAL_PLATFORM = 43, // Heavy Metal
            FO_HAV_MAT_HEAVY_WOOD_PLATFORM = 44, // Heavy Wood
            FO_HAV_MAT_CHAIN_PLATFORM = 45, // Chain
            FO_HAV_MAT_BOTTLECAP_PLATFORM = 46, // Bottlecap
            FO_HAV_MAT_ELEVATOR_PLATFORM = 47, // Elevator
            FO_HAV_MAT_HOLLOW_METAL_PLATFORM = 48, // Hollow Metal
            FO_HAV_MAT_SHEET_METAL_PLATFORM = 49, // Sheet Metal
            FO_HAV_MAT_SAND_PLATFORM = 50, // Sand
            FO_HAV_MAT_BROKEN_CONCRETE_PLATFORM = 51, // Broken Concrete
            FO_HAV_MAT_VEHICLE_BODY_PLATFORM = 52, // Vehicle Body
            FO_HAV_MAT_VEHICLE_PART_SOLID_PLATFORM = 53, // Vehicle Part Solid
            FO_HAV_MAT_VEHICLE_PART_HOLLOW_PLATFORM = 54, // Vehicle Part Hollow
            FO_HAV_MAT_BARREL_PLATFORM = 55, // Barrel
            FO_HAV_MAT_BOTTLE_PLATFORM = 56, // Bottle
            FO_HAV_MAT_SODA_CAN_PLATFORM = 57, // Soda Can
            FO_HAV_MAT_PISTOL_PLATFORM = 58, // Pistol
            FO_HAV_MAT_RIFLE_PLATFORM = 59, // Rifle
            FO_HAV_MAT_SHOPPING_CART_PLATFORM = 60, // Shopping Cart
            FO_HAV_MAT_LUNCHBOX_PLATFORM = 61, // Lunchbox
            FO_HAV_MAT_BABY_RATTLE_PLATFORM = 62, // Baby Rattle
            FO_HAV_MAT_RUBBER_BALL_PLATFORM = 63, // Rubber Ball
            FO_HAV_MAT_STONE_STAIRS = 64, // Stone
            FO_HAV_MAT_CLOTH_STAIRS = 65, // Cloth
            FO_HAV_MAT_DIRT_STAIRS = 66, // Dirt
            FO_HAV_MAT_GLASS_STAIRS = 67, // Glass
            FO_HAV_MAT_GRASS_STAIRS = 68, // Grass
            FO_HAV_MAT_METAL_STAIRS = 69, // Metal
            FO_HAV_MAT_ORGANIC_STAIRS = 70, // Organic
            FO_HAV_MAT_SKIN_STAIRS = 71, // Skin
            FO_HAV_MAT_WATER_STAIRS = 72, // Water
            FO_HAV_MAT_WOOD_STAIRS = 73, // Wood
            FO_HAV_MAT_HEAVY_STONE_STAIRS = 74, // Heavy Stone
            FO_HAV_MAT_HEAVY_METAL_STAIRS = 75, // Heavy Metal
            FO_HAV_MAT_HEAVY_WOOD_STAIRS = 76, // Heavy Wood
            FO_HAV_MAT_CHAIN_STAIRS = 77, // Chain
            FO_HAV_MAT_BOTTLECAP_STAIRS = 78, // Bottlecap
            FO_HAV_MAT_ELEVATOR_STAIRS = 79, // Elevator
            FO_HAV_MAT_HOLLOW_METAL_STAIRS = 80, // Hollow Metal
            FO_HAV_MAT_SHEET_METAL_STAIRS = 81, // Sheet Metal
            FO_HAV_MAT_SAND_STAIRS = 82, // Sand
            FO_HAV_MAT_BROKEN_CONCRETE_STAIRS = 83, // Broken Concrete
            FO_HAV_MAT_VEHICLE_BODY_STAIRS = 84, // Vehicle Body
            FO_HAV_MAT_VEHICLE_PART_SOLID_STAIRS = 85, // Vehicle Part Solid
            FO_HAV_MAT_VEHICLE_PART_HOLLOW_STAIRS = 86, // Vehicle Part Hollow
            FO_HAV_MAT_BARREL_STAIRS = 87, // Barrel
            FO_HAV_MAT_BOTTLE_STAIRS = 88, // Bottle
            FO_HAV_MAT_SODA_CAN_STAIRS = 89, // Soda Can
            FO_HAV_MAT_PISTOL_STAIRS = 90, // Pistol
            FO_HAV_MAT_RIFLE_STAIRS = 91, // Rifle
            FO_HAV_MAT_SHOPPING_CART_STAIRS = 92, // Shopping Cart
            FO_HAV_MAT_LUNCHBOX_STAIRS = 93, // Lunchbox
            FO_HAV_MAT_BABY_RATTLE_STAIRS = 94, // Baby Rattle
            FO_HAV_MAT_RUBBER_BALL_STAIRS = 95, // Rubber Ball
            FO_HAV_MAT_STONE_STAIRS_PLATFORM = 96, // Stone
            FO_HAV_MAT_CLOTH_STAIRS_PLATFORM = 97, // Cloth
            FO_HAV_MAT_DIRT_STAIRS_PLATFORM = 98, // Dirt
            FO_HAV_MAT_GLASS_STAIRS_PLATFORM = 99, // Glass
            FO_HAV_MAT_GRASS_STAIRS_PLATFORM = 100, // Grass
            FO_HAV_MAT_METAL_STAIRS_PLATFORM = 101, // Metal
            FO_HAV_MAT_ORGANIC_STAIRS_PLATFORM = 102, // Organic
            FO_HAV_MAT_SKIN_STAIRS_PLATFORM = 103, // Skin
            FO_HAV_MAT_WATER_STAIRS_PLATFORM = 104, // Water
            FO_HAV_MAT_WOOD_STAIRS_PLATFORM = 105, // Wood
            FO_HAV_MAT_HEAVY_STONE_STAIRS_PLATFORM = 106, // Heavy Stone
            FO_HAV_MAT_HEAVY_METAL_STAIRS_PLATFORM = 107, // Heavy Metal
            FO_HAV_MAT_HEAVY_WOOD_STAIRS_PLATFORM = 108, // Heavy Wood
            FO_HAV_MAT_CHAIN_STAIRS_PLATFORM = 109, // Chain
            FO_HAV_MAT_BOTTLECAP_STAIRS_PLATFORM = 110, // Bottlecap
            FO_HAV_MAT_ELEVATOR_STAIRS_PLATFORM = 111, // Elevator
            FO_HAV_MAT_HOLLOW_METAL_STAIRS_PLATFORM = 112, // Hollow Metal
            FO_HAV_MAT_SHEET_METAL_STAIRS_PLATFORM = 113, // Sheet Metal
            FO_HAV_MAT_SAND_STAIRS_PLATFORM = 114, // Sand
            FO_HAV_MAT_BROKEN_CONCRETE_STAIRS_PLATFORM = 115, // Broken Concrete
            FO_HAV_MAT_VEHICLE_BODY_STAIRS_PLATFORM = 116, // Vehicle Body
            FO_HAV_MAT_VEHICLE_PART_SOLID_STAIRS_PLATFORM = 117, // Vehicle Part Solid
            FO_HAV_MAT_VEHICLE_PART_HOLLOW_STAIRS_PLATFORM = 118, // Vehicle Part Hollow
            FO_HAV_MAT_BARREL_STAIRS_PLATFORM = 119, // Barrel
            FO_HAV_MAT_BOTTLE_STAIRS_PLATFORM = 120, // Bottle
            FO_HAV_MAT_SODA_CAN_STAIRS_PLATFORM = 121, // Soda Can
            FO_HAV_MAT_PISTOL_STAIRS_PLATFORM = 122, // Pistol
            FO_HAV_MAT_RIFLE_STAIRS_PLATFORM = 123, // Rifle
            FO_HAV_MAT_SHOPPING_CART_STAIRS_PLATFORM = 124, // Shopping Cart
            FO_HAV_MAT_LUNCHBOX_STAIRS_PLATFORM = 125, // Lunchbox
            FO_HAV_MAT_BABY_RATTLE_STAIRS_PLATFORM = 126, // Baby Rattle
            FO_HAV_MAT_RUBBER_BALL_STAIRS_PLATFORM = 127 // Rubber Ball
        }

        // A material, used by havok shape objects in Skyrim.
        public enum SkyrimHavokMaterial : uint
        {
            SKY_HAV_MAT_BROKEN_STONE = 131151687, // Broken Stone
            SKY_HAV_MAT_LIGHT_WOOD = 365420259, // Light Wood
            SKY_HAV_MAT_SNOW = 398949039, // Snow
            SKY_HAV_MAT_GRAVEL = 428587608, // Gravel
            SKY_HAV_MAT_MATERIAL_CHAIN_METAL = 438912228, // Material Chain Metal
            SKY_HAV_MAT_BOTTLE = 493553910, // Bottle
            SKY_HAV_MAT_WOOD = 500811281, // Wood
            SKY_HAV_MAT_SKIN = 591247106, // Skin
            SKY_HAV_MAT_UNKNOWN_617099282 = 617099282, // Unknown in Creation Kit v1.9.32.0. Found in Dawnguard DLC in meshes\dlc01\clutter\dlc01deerskin.nif.
            SKY_HAV_MAT_BARREL = 732141076, // Barrel
            SKY_HAV_MAT_MATERIAL_CERAMIC_MEDIUM = 781661019, // Material Ceramic Medium
            SKY_HAV_MAT_MATERIAL_BASKET = 790784366, // Material Basket
            SKY_HAV_MAT_ICE = 873356572, // Ice
            SKY_HAV_MAT_STAIRS_STONE = 899511101, // Stairs Stone
            SKY_HAV_MAT_WATER = 1024582599, // Water
            SKY_HAV_MAT_UNKNOWN_1028101969 = 1028101969, // Unknown in Creation Kit v1.6.89.0. Found in actors\draugr\character assets\skeletons.nif.
            SKY_HAV_MAT_MATERIAL_BLADE_1HAND = 1060167844, // Material Blade 1 Hand
            SKY_HAV_MAT_MATERIAL_BOOK = 1264672850, // Material Book
            SKY_HAV_MAT_MATERIAL_CARPET = 1286705471, // Material Carpet
            SKY_HAV_MAT_SOLID_METAL = 1288358971, // Solid Metal
            SKY_HAV_MAT_MATERIAL_AXE_1HAND = 1305674443, // Material Axe 1Hand
            SKY_HAV_MAT_UNKNOWN_1440721808 = 1440721808, // Unknown in Creation Kit v1.6.89.0. Found in armor\draugr\draugrbootsfemale_go.nif or armor\amuletsandrings\amuletgnd.nif.
            SKY_HAV_MAT_STAIRS_WOOD = 1461712277, // Stairs Wood
            SKY_HAV_MAT_MUD = 1486385281, // Mud
            SKY_HAV_MAT_MATERIAL_BOULDER_SMALL = 1550912982, // Material Boulder Small
            SKY_HAV_MAT_STAIRS_SNOW = 1560365355, // Stairs Snow
            SKY_HAV_MAT_HEAVY_STONE = 1570821952, // Heavy Stone
            SKY_HAV_MAT_UNKNOWN_1574477864 = 1574477864, // Unknown in Creation Kit v1.6.89.0. Found in actors\dragon\character assets\skeleton.nif.
            SKY_HAV_MAT_UNKNOWN_1591009235 = 1591009235, // Unknown in Creation Kit v1.6.89.0. Found in trap objects or clutter\displaycases\displaycaselgangled01.nif or actors\deer\character assets\skeleton.nif.
            SKY_HAV_MAT_MATERIAL_BOWS_STAVES = 1607128641, // Material Bows Staves
            SKY_HAV_MAT_MATERIAL_WOOD_AS_STAIRS = 1803571212, // Material Wood As Stairs
            SKY_HAV_MAT_GRASS = 1848600814, // Grass
            SKY_HAV_MAT_MATERIAL_BOULDER_LARGE = 1885326971, // Material Boulder Large
            SKY_HAV_MAT_MATERIAL_STONE_AS_STAIRS = 1886078335, // Material Stone As Stairs
            SKY_HAV_MAT_MATERIAL_BLADE_2HAND = 2022742644, // Material Blade 2Hand
            SKY_HAV_MAT_MATERIAL_BOTTLE_SMALL = 2025794648, // Material Bottle Small
            SKY_HAV_MAT_SAND = 2168343821, // Sand
            SKY_HAV_MAT_HEAVY_METAL = 2229413539, // Heavy Metal
            SKY_HAV_MAT_UNKNOWN_2290050264 = 2290050264, // Unknown in Creation Kit v1.9.32.0. Found in Dawnguard DLC in meshes\dlc01\clutter\dlc01sabrecatpelt.nif.
            SKY_HAV_MAT_DRAGON = 2518321175, // Dragon
            SKY_HAV_MAT_MATERIAL_BLADE_1HAND_SMALL = 2617944780, // Material Blade 1Hand Small
            SKY_HAV_MAT_MATERIAL_SKIN_SMALL = 2632367422, // Material Skin Small
            SKY_HAV_MAT_STAIRS_BROKEN_STONE = 2892392795, // Stairs Broken Stone
            SKY_HAV_MAT_MATERIAL_SKIN_LARGE = 2965929619, // Material Skin Large
            SKY_HAV_MAT_ORGANIC = 2974920155, // Organic
            SKY_HAV_MAT_MATERIAL_BONE = 3049421844, // Material Bone
            SKY_HAV_MAT_HEAVY_WOOD = 3070783559, // Heavy Wood
            SKY_HAV_MAT_MATERIAL_CHAIN = 3074114406, // Material Chain
            SKY_HAV_MAT_DIRT = 3106094762, // Dirt
            SKY_HAV_MAT_MATERIAL_ARMOR_LIGHT = 3424720541, // Material Armor Light
            SKY_HAV_MAT_MATERIAL_SHIELD_LIGHT = 3448167928, // Material Shield Light
            SKY_HAV_MAT_MATERIAL_COIN = 3589100606, // Material Coin
            SKY_HAV_MAT_MATERIAL_SHIELD_HEAVY = 3702389584, // Material Shield Heavy
            SKY_HAV_MAT_MATERIAL_ARMOR_HEAVY = 3708432437, // Material Armor Heavy
            SKY_HAV_MAT_MATERIAL_ARROW = 3725505938, // Material Arrow
            SKY_HAV_MAT_GLASS = 3739830338, // Glass
            SKY_HAV_MAT_STONE = 3741512247, // Stone
            SKY_HAV_MAT_CLOTH = 3839073443, // Cloth
            SKY_HAV_MAT_MATERIAL_BLUNT_2HAND = 3969592277, // Material Blunt 2Hand
            SKY_HAV_MAT_UNKNOWN_4239621792 = 4239621792, // Unknown in Creation Kit v1.9.32.0. Found in Dawnguard DLC in meshes\dlc01\prototype\dlc1protoswingingbridge.nif.
            SKY_HAV_MAT_MATERIAL_BOULDER_MEDIUM = 4283869410 // Material Boulder Medium
        }

        // Sets mesh color in Oblivion Construction Set.  Anything higher than 57 is also null.
        public enum OblivionLayer : byte
        {
            OL_UNIDENTIFIED = 0, // Unidentified (white)
            OL_STATIC = 1, // Static (red)
            OL_ANIM_STATIC = 2, // AnimStatic (magenta)
            OL_TRANSPARENT = 3, // Transparent (light pink)
            OL_CLUTTER = 4, // Clutter (light blue)
            OL_WEAPON = 5, // Weapon (orange)
            OL_PROJECTILE = 6, // Projectile (light orange)
            OL_SPELL = 7, // Spell (cyan)
            OL_BIPED = 8, // Biped (green) Seems to apply to all creatures/NPCs
            OL_TREES = 9, // Trees (light brown)
            OL_PROPS = 10, // Props (magenta)
            OL_WATER = 11, // Water (cyan)
            OL_TRIGGER = 12, // Trigger (light grey)
            OL_TERRAIN = 13, // Terrain (light yellow)
            OL_TRAP = 14, // Trap (light grey)
            OL_NONCOLLIDABLE = 15, // NonCollidable (white)
            OL_CLOUD_TRAP = 16, // CloudTrap (greenish grey)
            OL_GROUND = 17, // Ground (none)
            OL_PORTAL = 18, // Portal (green)
            OL_STAIRS = 19, // Stairs (white)
            OL_CHAR_CONTROLLER = 20, // CharController (yellow)
            OL_AVOID_BOX = 21, // AvoidBox (dark yellow)
            OL_UNKNOWN1 = 22, // ? (white)
            OL_UNKNOWN2 = 23, // ? (white)
            OL_CAMERA_PICK = 24, // CameraPick (white)
            OL_ITEM_PICK = 25, // ItemPick (white)
            OL_LINE_OF_SIGHT = 26, // LineOfSight (white)
            OL_PATH_PICK = 27, // PathPick (white)
            OL_CUSTOM_PICK_1 = 28, // CustomPick1 (white)
            OL_CUSTOM_PICK_2 = 29, // CustomPick2 (white)
            OL_SPELL_EXPLOSION = 30, // SpellExplosion (white)
            OL_DROPPING_PICK = 31, // DroppingPick (white)
            OL_OTHER = 32, // Other (white)
            OL_HEAD = 33, // Head
            OL_BODY = 34, // Body
            OL_SPINE1 = 35, // Spine1
            OL_SPINE2 = 36, // Spine2
            OL_L_UPPER_ARM = 37, // LUpperArm
            OL_L_FOREARM = 38, // LForeArm
            OL_L_HAND = 39, // LHand
            OL_L_THIGH = 40, // LThigh
            OL_L_CALF = 41, // LCalf
            OL_L_FOOT = 42, // LFoot
            OL_R_UPPER_ARM = 43, // RUpperArm
            OL_R_FOREARM = 44, // RForeArm
            OL_R_HAND = 45, // RHand
            OL_R_THIGH = 46, // RThigh
            OL_R_CALF = 47, // RCalf
            OL_R_FOOT = 48, // RFoot
            OL_TAIL = 49, // Tail
            OL_SIDE_WEAPON = 50, // SideWeapon
            OL_SHIELD = 51, // Shield
            OL_QUIVER = 52, // Quiver
            OL_BACK_WEAPON = 53, // BackWeapon
            OL_BACK_WEAPON2 = 54, // BackWeapon (?)
            OL_PONYTAIL = 55, // PonyTail
            OL_WING = 56, // Wing
            OL_NULL = 57 // Null
        }

        // Sets mesh color in Fallout 3 GECK. Anything higher than 72 is also null.
        public enum Fallout3Layer : byte
        {
            FOL_UNIDENTIFIED = 0, // Unidentified (white)
            FOL_STATIC = 1, // Static (red)
            FOL_ANIM_STATIC = 2, // AnimStatic (magenta)
            FOL_TRANSPARENT = 3, // Transparent (light pink)
            FOL_CLUTTER = 4, // Clutter (light blue)
            FOL_WEAPON = 5, // Weapon (orange)
            FOL_PROJECTILE = 6, // Projectile (light orange)
            FOL_SPELL = 7, // Spell (cyan)
            FOL_BIPED = 8, // Biped (green) Seems to apply to all creatures/NPCs
            FOL_TREES = 9, // Trees (light brown)
            FOL_PROPS = 10, // Props (magenta)
            FOL_WATER = 11, // Water (cyan)
            FOL_TRIGGER = 12, // Trigger (light grey)
            FOL_TERRAIN = 13, // Terrain (light yellow)
            FOL_TRAP = 14, // Trap (light grey)
            FOL_NONCOLLIDABLE = 15, // NonCollidable (white)
            FOL_CLOUD_TRAP = 16, // CloudTrap (greenish grey)
            FOL_GROUND = 17, // Ground (none)
            FOL_PORTAL = 18, // Portal (green)
            FOL_DEBRIS_SMALL = 19, // DebrisSmall (white)
            FOL_DEBRIS_LARGE = 20, // DebrisLarge (white)
            FOL_ACOUSTIC_SPACE = 21, // AcousticSpace (white)
            FOL_ACTORZONE = 22, // Actorzone (white)
            FOL_PROJECTILEZONE = 23, // Projectilezone (white)
            FOL_GASTRAP = 24, // GasTrap (yellowish green)
            FOL_SHELLCASING = 25, // ShellCasing (white)
            FOL_TRANSPARENT_SMALL = 26, // TransparentSmall (white)
            FOL_INVISIBLE_WALL = 27, // InvisibleWall (white)
            FOL_TRANSPARENT_SMALL_ANIM = 28, // TransparentSmallAnim (white)
            FOL_DEADBIP = 29, // Dead Biped (green)
            FOL_CHARCONTROLLER = 30, // CharController (yellow)
            FOL_AVOIDBOX = 31, // Avoidbox (orange)
            FOL_COLLISIONBOX = 32, // Collisionbox (white)
            FOL_CAMERASPHERE = 33, // Camerasphere (white)
            FOL_DOORDETECTION = 34, // Doordetection (white)
            FOL_CAMERAPICK = 35, // Camerapick (white)
            FOL_ITEMPICK = 36, // Itempick (white)
            FOL_LINEOFSIGHT = 37, // LineOfSight (white)
            FOL_PATHPICK = 38, // Pathpick (white)
            FOL_CUSTOMPICK1 = 39, // Custompick1 (white)
            FOL_CUSTOMPICK2 = 40, // Custompick2 (white)
            FOL_SPELLEXPLOSION = 41, // SpellExplosion (white)
            FOL_DROPPINGPICK = 42, // Droppingpick (white)
            FOL_NULL = 43 // Null (white)
        }

        // Physical purpose of collision object? The setting affects object's havok behavior in game. Anything higher than 47 is also null.
        public enum SkyrimLayer : byte
        {
            SKYL_UNIDENTIFIED = 0, // Unidentified
            SKYL_STATIC = 1, // Static
            SKYL_ANIMSTATIC = 2, // Anim Static
            SKYL_TRANSPARENT = 3, // Transparent
            SKYL_CLUTTER = 4, // Clutter. Object with this layer will float on water surface.
            SKYL_WEAPON = 5, // Weapon
            SKYL_PROJECTILE = 6, // Projectile
            SKYL_SPELL = 7, // Spell
            SKYL_BIPED = 8, // Biped. Seems to apply to all creatures/NPCs
            SKYL_TREES = 9, // Trees
            SKYL_PROPS = 10, // Props
            SKYL_WATER = 11, // Water
            SKYL_TRIGGER = 12, // Trigger
            SKYL_TERRAIN = 13, // Terrain
            SKYL_TRAP = 14, // Trap
            SKYL_NONCOLLIDABLE = 15, // NonCollidable
            SKYL_CLOUD_TRAP = 16, // CloudTrap
            SKYL_GROUND = 17, // Ground. It seems that produces no sound when collide.
            SKYL_PORTAL = 18, // Portal
            SKYL_DEBRIS_SMALL = 19, // Debris Small
            SKYL_DEBRIS_LARGE = 20, // Debris Large
            SKYL_ACOUSTIC_SPACE = 21, // Acoustic Space
            SKYL_ACTORZONE = 22, // Actor Zone
            SKYL_PROJECTILEZONE = 23, // Projectile Zone
            SKYL_GASTRAP = 24, // Gas Trap
            SKYL_SHELLCASING = 25, // Shell Casing
            SKYL_TRANSPARENT_SMALL = 26, // Transparent Small
            SKYL_INVISIBLE_WALL = 27, // Invisible Wall
            SKYL_TRANSPARENT_SMALL_ANIM = 28, // Transparent Small Anim
            SKYL_WARD = 29, // Ward
            SKYL_CHARCONTROLLER = 30, // Char Controller
            SKYL_STAIRHELPER = 31, // Stair Helper
            SKYL_DEADBIP = 32, // Dead Bip
            SKYL_BIPED_NO_CC = 33, // Biped No CC
            SKYL_AVOIDBOX = 34, // Avoid Box
            SKYL_COLLISIONBOX = 35, // Collision Box
            SKYL_CAMERASHPERE = 36, // Camera Sphere
            SKYL_DOORDETECTION = 37, // Door Detection
            SKYL_CONEPROJECTILE = 38, // Cone Projectile
            SKYL_CAMERAPICK = 39, // Camera Pick
            SKYL_ITEMPICK = 40, // Item Pick
            SKYL_LINEOFSIGHT = 41, // Line of Sight
            SKYL_PATHPICK = 42, // Path Pick
            SKYL_CUSTOMPICK1 = 43, // Custom Pick 1
            SKYL_CUSTOMPICK2 = 44, // Custom Pick 2
            SKYL_SPELLEXPLOSION = 45, // Spell Explosion
            SKYL_DROPPINGPICK = 46, // Dropping Pick
            SKYL_NULL = 47 // Null
        }

        // A byte describing if MOPP Data is organized into chunks (PS3) or not (PC)
        public enum MoppDataBuildType : byte
        {
            BUILT_WITH_CHUNK_SUBDIVISION = 0, // Organized in chunks for PS3.
            BUILT_WITHOUT_CHUNK_SUBDIVISION = 1, // Not organized in chunks for PC. (Default)
            BUILD_NOT_SET = 2 // Build type not set yet.
        }

        // An unsigned 32-bit integer, describing how mipmaps are handled in a texture.
        public enum MipMapFormat : uint
        {
            MIP_FMT_NO = 0, // Texture does not use mip maps.
            MIP_FMT_YES = 1, // Texture uses mip maps.
            MIP_FMT_DEFAULT = 2 // Use default setting.
        }

        // Specifies the pixel format used by the NiPixelData object to store a texture.
        public enum PixelFormat : uint
        {
            PX_FMT_RGB8 = 0, // 24-bit color: uses 8 bit to store each red, blue, and green component.
            PX_FMT_RGBA8 = 1, // 32-bit color with alpha: uses 8 bits to store each red, blue, green, and alpha component.
            PX_FMT_PAL8 = 2, // 8-bit palette index: uses 8 bits to store an index into the palette stored in a NiPalette object.
            PX_FMT_DXT1 = 4, // DXT1 compressed texture.
            PX_FMT_DXT5 = 5, // DXT5 compressed texture.
            PX_FMT_DXT5_ALT = 6 // DXT5 compressed texture. It is not clear what the difference is with PX_FMT_DXT5.
        }

        // An unsigned 32-bit integer, describing the color depth of a texture.
        public enum PixelLayout : uint
        {
            PIX_LAY_PALETTISED = 0, // Texture is in 8-bit paletized format.
            PIX_LAY_HIGH_COLOR_16 = 1, // Texture is in 16-bit high color format.
            PIX_LAY_TRUE_COLOR_32 = 2, // Texture is in 32-bit true color format.
            PIX_LAY_COMPRESSED = 3, // Texture is compressed.
            PIX_LAY_BUMPMAP = 4, // Texture is a grayscale bump map.
            PIX_LAY_PALETTISED_4 = 5, // Texture is in 4-bit paletized format.
            PIX_LAY_DEFAULT = 6 // Use default setting.
        }

        // Specifies the availiable texture clamp modes.  That is, the behavior of pixels outside the range of the texture.
        public enum TexClampMode : uint
        {
            CLAMP_S_CLAMP_T = 0, // Clamp in both directions.
            CLAMP_S_WRAP_T = 1, // Clamp in the S(U) direction but wrap in the T(V) direction.
            WRAP_S_CLAMP_T = 2, // Wrap in the S(U) direction but clamp in the T(V) direction.
            WRAP_S_WRAP_T = 3 // Wrap in both directions.
        }

        // Specifies the availiable texture filter modes.  That is, the way pixels within a texture are blended together when textures are displayed on the screen at a size other than their original dimentions.
        public enum TexFilterMode : uint
        {
            FILTER_NEAREST = 0, // Simply uses the nearest pixel.  Very grainy.
            FILTER_BILERP = 1, // Uses bilinear filtering.
            FILTER_TRILERP = 2, // Uses trilinear filtering.
            FILTER_NEAREST_MIPNEAREST = 3, // Uses the nearest pixel from the mipmap that is closest to the display size.
            FILTER_NEAREST_MIPLERP = 4, // Blends the two mipmaps closest to the display size linearly, and then uses the nearest pixel from the result.
            FILTER_BILERP_MIPNEAREST = 5 // Uses the closest mipmap to the display size and then uses bilinear filtering on the pixels.
        }

        // An unsigned 32-bit integer, which describes how to apply vertex colors.
        public enum VertMode : uint
        {
            VERT_MODE_SRC_IGNORE = 0, // Source Ignore.
            VERT_MODE_SRC_EMISSIVE = 1, // Source Emissive.
            VERT_MODE_SRC_AMB_DIF = 2 // Source Ambient/Diffuse. (Default)
        }

        // The animation cyle behavior.
        public enum CycleType : uint
        {
            CYCLE_LOOP = 0, // Loop
            CYCLE_REVERSE = 1, // Reverse
            CYCLE_CLAMP = 2 // Clamp
        }

        // The force field's type.
        public enum FieldType : uint
        {
            FIELD_WIND = 0, // Wind (fixed direction)
            FIELD_POINT = 1 // Point (fixed origin)
        }

        // Determines the way the billboard will react to the camera.        Billboard mode is stored in lowest 3 bits although Oblivion vanilla nifs uses values higher than 7.
        public enum BillboardMode : ushort
        {
            ALWAYS_FACE_CAMERA = 0, // The billboard will always face the camera.
            ROTATE_ABOUT_UP = 1, // The billboard will only rotate around the up axis.
            RIGID_FACE_CAMERA = 2, // Rigid Face Camera.
            ALWAYS_FACE_CENTER = 3, // Always Face Center.
            RIGID_FACE_CENTER = 4, // Rigid Face Center.
            BSROTATE_ABOUT_UP = 5, // The billboard will only rotate around its local Z axis (it always stays in its local X-Y plane).
            ROTATE_ABOUT_UP2 = 9 // The billboard will only rotate around the up axis (same as ROTATE_ABOUT_UP?).
        }

        // This enum contains the options for doing stencil buffer tests.
        public enum StencilCompareMode : uint
        {
            TEST_NEVER = 0, // Test will allways return false. Nothing is drawn at all.
            TEST_LESS = 1, // The test will only succeed if the pixel is nearer than the previous pixel.
            TEST_EQUAL = 2, // Test will only succeed if the z value of the pixel to be drawn is equal to the value of the previous drawn pixel.
            TEST_LESS_EQUAL = 3, // Test will succeed if the z value of the pixel to be drawn is smaller than or equal to the value in the Stencil Buffer.
            TEST_GREATER = 4, // Opposite of TEST_LESS.
            TEST_NOT_EQUAL = 5, // Test will succeed if the z value of the pixel to be drawn is NOT equal to the value of the previously drawn pixel.
            TEST_GREATER_EQUAL = 6, // Opposite of TEST_LESS_EQUAL.
            TEST_ALWAYS = 7 // Test will allways succeed. The Stencil Buffer value is ignored.
        }

        // This enum contains the options for doing z buffer tests.
        public enum ZCompareMode : uint
        {
            ZCOMP_ALWAYS = 0, // Test will allways succeed. The Z Buffer value is ignored.
            ZCOMP_LESS = 1, // The test will only succeed if the pixel is nearer than the previous pixel.
            ZCOMP_EQUAL = 2, // Test will only succeed if the z value of the pixel to be drawn is equal to the value of the previous drawn pixel.
            ZCOMP_LESS_EQUAL = 3, // Test will succeed if the z value of the pixel to be drawn is smaller than or equal to the value in the Z Buffer.
            ZCOMP_GREATER = 4, // Opposite of TEST_LESS.
            ZCOMP_NOT_EQUAL = 5, // Test will succeed if the z value of the pixel to be drawn is NOT equal to the value of the previously drawn pixel.
            ZCOMP_GREATER_EQUAL = 6, // Opposite of TEST_LESS_EQUAL.
            ZCOMP_NEVER = 7 // Test will allways return false. Nothing is drawn at all.
        }

        // This enum defines the various actions used in conjunction with the stencil buffer.        For a detailed description of the individual options please refer to the OpenGL docs.
        public enum StencilAction : uint
        {
            ACTION_KEEP = 0,
            ACTION_ZERO = 1,
            ACTION_REPLACE = 2,
            ACTION_INCREMENT = 3,
            ACTION_DECREMENT = 4,
            ACTION_INVERT = 5
        }

        // This enum lists the different face culling options.
        public enum FaceDrawMode : uint
        {
            DRAW_CCW_OR_BOTH = 0, // use application defaults?
            DRAW_CCW = 1, // Draw counter clock wise faces, cull clock wise faces. This is the default for most (all?) Nif Games so far.
            DRAW_CW = 2, // Draw clock wise faces, cull counter clock wise faces. This will flip all the faces.
            DRAW_BOTH = 3 // Draw double sided faces.
        }

        // The motion system. 4 (Box) is used for everything movable. 7 (Keyframed) is used on statics and animated stuff.
        public enum MotionSystem : byte
        {
            MO_SYS_INVALID = 0, // Invalid
            MO_SYS_DYNAMIC = 1, // A fully-simulated, movable rigid body. At construction time the engine checks the input inertia and selects MO_SYS_SPHERE_INERTIA or MO_SYS_BOX_INERTIA as appropriate.
            MO_SYS_SPHERE = 2, // Simulation is performed using a sphere inertia tensor.
            MO_SYS_SPHERE_INERTIA = 3, // This is the same as MO_SYS_SPHERE_INERTIA, except that simulation of the rigid body is "softened".
            MO_SYS_BOX = 4, // Simulation is performed using a box inertia tensor.
            MO_SYS_BOX_STABILIZED = 5, // This is the same as MO_SYS_BOX_INERTIA, except that simulation of the rigid body is "softened".
            MO_SYS_KEYFRAMED = 6, // Simulation is not performed as a normal rigid body. The keyframed rigid body has an infinite mass when viewed by the rest of the system. (used for creatures)
            MO_SYS_FIXED = 7, // This motion type is used for the static elements of a game scene, e.g. the landscape. Faster than MO_SYS_KEYFRAMED at velocity 0. (used for weapons)
            MO_SYS_THIN_BOX = 8, // A box inertia motion which is optimized for thin boxes and has less stability problems
            MO_SYS_CHARACTER = 9 // A specialized motion used for character controllers
        }

        public enum DeactivatorType : byte
        {
            DEACTIVATOR_INVALID = 0, // Invalid
            DEACTIVATOR_NEVER = 1, // This will force the rigid body to never deactivate.
            DEACTIVATOR_SPATIAL = 2 // Tells Havok to use a spatial deactivation scheme. This makes use of high and low frequencies of positional motion to determine when deactivation should occur.
        }

        // A list of possible solver deactivation settings. This value defines how the        solver deactivates objects. The solver works on a per object basis.        Note: Solver deactivation does not save CPU, but reduces creeping of        movable objects in a pile quite dramatically.
        public enum SolverDeactivation : byte
        {
            SOLVER_DEACTIVATION_INVALID = 0, // Invalid
            SOLVER_DEACTIVATION_OFF = 1, // No solver deactivation
            SOLVER_DEACTIVATION_LOW = 2, // Very conservative deactivation, typically no visible artifacts.
            SOLVER_DEACTIVATION_MEDIUM = 3, // Normal deactivation, no serious visible artifacts in most cases
            SOLVER_DEACTIVATION_HIGH = 4, // Fast deactivation, visible artifacts
            SOLVER_DEACTIVATION_MAX = 5 // Very fast deactivation, visible artifacts
        }

        // The motion type. Determines quality of motion?
        public enum MotionQuality : byte
        {
            MO_QUAL_INVALID = 0, // Automatically assigned to MO_QUAL_FIXED, MO_QUAL_KEYFRAMED or MO_QUAL_DEBRIS
            MO_QUAL_FIXED = 1, // Use this for fixed bodies.
            MO_QUAL_KEYFRAMED = 2, // Use this for moving objects with infinite mass.
            MO_QUAL_DEBRIS = 3, // Use this for all your debris objects
            MO_QUAL_MOVING = 4, // Use this for moving bodies, which should not leave the world.
            MO_QUAL_CRITICAL = 5, // Use this for all objects, which you cannot afford to tunnel through the world at all
            MO_QUAL_BULLET = 6, // Use this for very fast objects
            MO_QUAL_USER = 7, // For user.
            MO_QUAL_CHARACTER = 8, // Use this for rigid body character controllers
            MO_QUAL_KEYFRAMED_REPORT = 9 // Use this for moving objects with infinite mass which should report contact points and Toi-collisions against all other bodies, including other fixed and keyframed bodies.
        }

        // The type of force?  May be more valid values.
        public enum ForceType : uint
        {
            FORCE_PLANAR = 0,
            FORCE_SPHERICAL = 1,
            FORCE_UNKNOWN = 2
        }

        // Determines how a NiTextureTransformController animates the UV coordinates.
        public enum TexTransform : uint
        {
            TT_TRANSLATE_U = 0, // Means this controller moves the U texture cooridnates.
            TT_TRANSLATE_V = 1, // Means this controller moves the V texture cooridnates.
            TT_ROTATE = 2, // Means this controller roates the UV texture cooridnates.
            TT_SCALE_U = 3, // Means this controller scales the U texture cooridnates.
            TT_SCALE_V = 4 // Means this controller scales the V texture cooridnates.
        }

        // Determines decay function.  Used by NiPSysBombModifier.
        public enum DecayType : uint
        {
            DECAY_NONE = 0, // No decay.
            DECAY_LINEAR = 1, // Linear decay.
            DECAY_EXPONENTIAL = 2 // Exponential decay.
        }

        // Determines symetry type used by NiPSysBombModifier.
        public enum SymmetryType : uint
        {
            SPHERICAL_SYMMETRY = 0, // Spherical Symmetry.
            CYLINDRICAL_SYMMETRY = 1, // Cylindrical Symmetry.
            PLANAR_SYMMETRY = 2 // Planar Symmetry.
        }

        // Controls the way the a particle mesh emitter determines the starting speed and direction of the particles that are emitted.
        public enum VelocityType : uint
        {
            VELOCITY_USE_NORMALS = 0, // Uses the normals of the meshes to determine staring velocity.
            VELOCITY_USE_RANDOM = 1, // Starts particles with a random velocity.
            VELOCITY_USE_DIRECTION = 2 // Uses the emission axis to determine initial particle direction?
        }

        // Controls which parts of the mesh that the particles are emitted from.
        public enum EmitFrom : uint
        {
            EMIT_FROM_VERTICES = 0, // Emit particles from the vertices of the mesh.
            EMIT_FROM_FACE_CENTER = 1, // Emit particles from the center of the faces of the mesh.
            EMIT_FROM_EDGE_CENTER = 2, // Emit particles from the center of the edges of the mesh.
            EMIT_FROM_FACE_SURFACE = 3, // Perhaps randomly emit particles from anywhere on the faces of the mesh?
            EMIT_FROM_EDGE_SURFACE = 4 // Perhaps randomly emit particles from anywhere on the edges of the mesh?
        }

        // The type of information that's store in a texture used by a NiTextureEffect.
        public enum EffectType : uint
        {
            EFFECT_PROJECTED_LIGHT = 0, // Apply a projected light texture.
            EFFECT_PROJECTED_SHADOW = 1, // Apply a projected shaddow texture.
            EFFECT_ENVIRONMENT_MAP = 2, // Apply an environment map texture.
            EFFECT_FOG_MAP = 3 // Apply a fog map texture.
        }

        // Determines the way that UV texture coordinates are generated.
        public enum CoordGenType : uint
        {
            CG_WORLD_PARALLEL = 0, // Use plannar mapping.
            CG_WORLD_PERSPECTIVE = 1, // Use perspective mapping.
            CG_SPHERE_MAP = 2, // Use spherical mapping.
            CG_SPECULAR_CUBE_MAP = 3, // Use specular cube mapping.
            CG_DIFFUSE_CUBE_MAP = 4 // Use Diffuse cube mapping.
        }

        public enum EndianType : byte
        {
            ENDIAN_BIG = 0, // The numbers are stored in big endian format, such as those used by PowerPC Mac processors.
            ENDIAN_LITTLE = 1 // The numbers are stored in little endian format, such as those used by Intel and AMD x86 processors.
        }

        // Used by NiPoint3InterpControllers to select which type of color in the controlled object that will be animated.
        public enum TargetColor : ushort
        {
            TC_AMBIENT = 0, // Control the ambient color.
            TC_DIFFUSE = 1, // Control the diffuse color.
            TC_SPECULAR = 2, // Control the specular color.
            TC_SELF_ILLUM = 3 // Control the self illumination color.
        }

        // Used by NiGeometryData to control the volatility of the mesh.  While they appear to be flags they behave as an enum.
        public enum ConsistencyType : ushort
        {
            CT_MUTABLE = 0x0000, // Mutable Mesh
            CT_STATIC = 0x4000, // Static Mesh
            CT_VOLATILE = 0x8000 // Volatile Mesh
        }

        public enum SortingMode : uint
        {
            SORTING_INHERIT = 0, // Inherit
            SORTING_OFF = 1 // Disable
        }

        public enum PropagationMode : uint
        {
            PROPAGATE_ON_SUCCESS = 0, // On Success
            PROPAGATE_ON_FAILURE = 1, // On Failure
            PROPAGATE_ALWAYS = 2, // Always
            PROPAGATE_NEVER = 3 // Never
        }

        public enum CollisionMode : uint
        {
            CM_USE_OBB = 0, // Use Bounding Box
            CM_USE_TRI = 1, // Use Triangles
            CM_USE_ABV = 2, // Use Alternate Bounding Volumes
            CM_NOTEST = 3, // No Test
            CM_USE_NIBOUND = 4 // Use NiBound
        }

        public enum BoundVolumeType : uint
        {
            BASE_BV = 0xffffffff, // Default
            SPHERE_BV = 0, // Sphere
            BOX_BV = 1, // Box
            CAPSULE_BV = 2, // Capsule
            UNION_BV = 4, // Union
            HALFSPACE_BV = 5 // Half Space
        }

        public enum hkResponseType : byte
        {
            RESPONSE_INVALID = 0, // Invalid Response
            RESPONSE_SIMPLE_CONTACT = 1, // Do normal collision resolution
            RESPONSE_REPORTING = 2, // No collision resolution is performed but listeners are called
            RESPONSE_NONE = 3 // Do nothing, ignore all the results.
        }

        // Biped bodypart data used for visibility control of triangles.  Options are Fallout 3, except where marked for Skyrim (uses SBP prefix)        Skyrim BP names are listed only for vanilla names, different creatures have different defnitions for naming.
        public enum BSDismemberBodyPartType : ushort
        {
            BP_TORSO = 0, // Torso
            BP_HEAD = 1, // Head
            BP_HEAD2 = 2, // Head 2
            BP_LEFTARM = 3, // Left Arm
            BP_LEFTARM2 = 4, // Left Arm 2
            BP_RIGHTARM = 5, // Right Arm
            BP_RIGHTARM2 = 6, // Right Arm 2
            BP_LEFTLEG = 7, // Left Leg
            BP_LEFTLEG2 = 8, // Left Leg 2
            BP_LEFTLEG3 = 9, // Left Leg 3
            BP_RIGHTLEG = 10, // Right Leg
            BP_RIGHTLEG2 = 11, // Right Leg 2
            BP_RIGHTLEG3 = 12, // Right Leg 3
            BP_BRAIN = 13, // Brain
            SBP_30_HEAD = 30, // Skyrim, Head(Human), Body(Atronachs,Beasts), Mask(Dragonpriest)
            SBP_31_HAIR = 31, // Skyrim, Hair(human), Far(Dragon), Mask2(Dragonpriest),SkinnedFX(Spriggan)
            SBP_32_BODY = 32, // Skyrim, Main body, extras(Spriggan)
            SBP_33_HANDS = 33, // Skyrim, Hands L/R, BodyToo(Dragonpriest), Legs(Draugr), Arms(Giant)
            SBP_34_FOREARMS = 34, // Skyrim, Forearms L/R, Beard(Draugr)
            SBP_35_AMULET = 35, // Skyrim, Amulet
            SBP_36_RING = 36, // Skyrim, Ring
            SBP_37_FEET = 37, // Skyrim, Feet L/R
            SBP_38_CALVES = 38, // Skyrim, Calves L/R
            SBP_39_SHIELD = 39, // Skyrim, Shield
            SBP_40_TAIL = 40, // Skyrim, Tail(Argonian/Khajiit), Skeleton01(Dragon), FX01(AtronachStorm),FXMist (Dragonpriest), Spit(Chaurus,Spider),SmokeFins(IceWraith)
            SBP_41_LONGHAIR = 41, // Skyrim, Long Hair(Human), Skeleton02(Dragon),FXParticles(Dragonpriest)
            SBP_42_CIRCLET = 42, // Skyrim, Circlet(Human, MouthFireEffect(Dragon)
            SBP_43_EARS = 43, // Skyrim, Ears
            SBP_44_DRAGON_BLOODHEAD_OR_MOD_MOUTH = 44, // Skyrim, Bloodied dragon head, or NPC face/mouth
            SBP_45_DRAGON_BLOODWINGL_OR_MOD_NECK = 45, // Skyrim, Left Bloodied dragon wing, Saddle(Horse), or NPC cape, scarf, shawl, neck-tie, etc.
            SBP_46_DRAGON_BLOODWINGR_OR_MOD_CHEST_PRIMARY = 46, // Skyrim, Right Bloodied dragon wing, or NPC chest primary or outergarment
            SBP_47_DRAGON_BLOODTAIL_OR_MOD_BACK = 47, // Skyrim, Bloodied dragon tail, or NPC backpack/wings/...
            SBP_48_MOD_MISC1 = 48, // Anything that does not fit in the list
            SBP_49_MOD_PELVIS_PRIMARY = 49, // Pelvis primary or outergarment
            SBP_50_DECAPITATEDHEAD = 50, // Skyrim, Decapitated Head
            SBP_51_DECAPITATE = 51, // Skyrim, Decapitate, neck gore
            SBP_52_MOD_PELVIS_SECONDARY = 52, // Pelvis secondary or undergarment
            SBP_53_MOD_LEG_RIGHT = 53, // Leg primary or outergarment or right leg
            SBP_54_MOD_LEG_LEFT = 54, // Leg secondary or undergarment or left leg
            SBP_55_MOD_FACE_JEWELRY = 55, // Face alternate or jewelry
            SBP_56_MOD_CHEST_SECONDARY = 56, // Chest secondary or undergarment
            SBP_57_MOD_SHOULDER = 57, // Shoulder
            SBP_58_MOD_ARM_LEFT = 58, // Arm secondary or undergarment or left arm
            SBP_59_MOD_ARM_RIGHT = 59, // Arm primary or outergarment or right arm
            SBP_60_MOD_MISC2 = 60, // Anything that does not fit in the list
            SBP_61_FX01 = 61, // Skyrim, FX01(Humanoid)
            BP_SECTIONCAP_HEAD = 101, // Section Cap | Head
            BP_SECTIONCAP_HEAD2 = 102, // Section Cap | Head 2
            BP_SECTIONCAP_LEFTARM = 103, // Section Cap | Left Arm
            BP_SECTIONCAP_LEFTARM2 = 104, // Section Cap | Left Arm 2
            BP_SECTIONCAP_RIGHTARM = 105, // Section Cap | Right Arm
            BP_SECTIONCAP_RIGHTARM2 = 106, // Section Cap | Right Arm 2
            BP_SECTIONCAP_LEFTLEG = 107, // Section Cap | Left Leg
            BP_SECTIONCAP_LEFTLEG2 = 108, // Section Cap | Left Leg 2
            BP_SECTIONCAP_LEFTLEG3 = 109, // Section Cap | Left Leg 3
            BP_SECTIONCAP_RIGHTLEG = 110, // Section Cap | Right Leg
            BP_SECTIONCAP_RIGHTLEG2 = 111, // Section Cap | Right Leg 2
            BP_SECTIONCAP_RIGHTLEG3 = 112, // Section Cap | Right Leg 3
            BP_SECTIONCAP_BRAIN = 113, // Section Cap | Brain
            SBP_130_HEAD = 130, // Skyrim, Head slot, use on full-face helmets
            SBP_131_HAIR = 131, // Skyrim, Hair slot 1, use on hoods
            SBP_141_LONGHAIR = 141, // Skyrim, Hair slot 2, use for longer hair
            SBP_142_CIRCLET = 142, // Skyrim, Circlet slot 1, use for circlets
            SBP_143_EARS = 143, // Skyrim, Ear slot
            SBP_150_DECAPITATEDHEAD = 150, // Skyrim, neck gore on head side
            BP_TORSOCAP_HEAD = 201, // Torso Cap | Head
            BP_TORSOCAP_HEAD2 = 202, // Torso Cap | Head 2
            BP_TORSOCAP_LEFTARM = 203, // Torso Cap | Left Arm
            BP_TORSOCAP_LEFTARM2 = 204, // Torso Cap | Left Arm 2
            BP_TORSOCAP_RIGHTARM = 205, // Torso Cap | Right Arm
            BP_TORSOCAP_RIGHTARM2 = 206, // Torso Cap | Right Arm 2
            BP_TORSOCAP_LEFTLEG = 207, // Torso Cap | Left Leg
            BP_TORSOCAP_LEFTLEG2 = 208, // Torso Cap | Left Leg 2
            BP_TORSOCAP_LEFTLEG3 = 209, // Torso Cap | Left Leg 3
            BP_TORSOCAP_RIGHTLEG = 210, // Torso Cap | Right Leg
            BP_TORSOCAP_RIGHTLEG2 = 211, // Torso Cap | Right Leg 2
            BP_TORSOCAP_RIGHTLEG3 = 212, // Torso Cap | Right Leg 3
            BP_TORSOCAP_BRAIN = 213, // Torso Cap | Brain
            SBP_230_HEAD = 230, // Skyrim, Head slot, use for neck on character head
            BP_TORSOSECTION_HEAD = 1000, // Torso Section | Head
            BP_TORSOSECTION_HEAD2 = 2000, // Torso Section | Head 2
            BP_TORSOSECTION_LEFTARM = 3000, // Torso Section | Left Arm
            BP_TORSOSECTION_LEFTARM2 = 4000, // Torso Section | Left Arm 2
            BP_TORSOSECTION_RIGHTARM = 5000, // Torso Section | Right Arm
            BP_TORSOSECTION_RIGHTARM2 = 6000, // Torso Section | Right Arm 2
            BP_TORSOSECTION_LEFTLEG = 7000, // Torso Section | Left Leg
            BP_TORSOSECTION_LEFTLEG2 = 8000, // Torso Section | Left Leg 2
            BP_TORSOSECTION_LEFTLEG3 = 9000, // Torso Section | Left Leg 3
            BP_TORSOSECTION_RIGHTLEG = 10000, // Torso Section | Right Leg
            BP_TORSOSECTION_RIGHTLEG2 = 11000, // Torso Section | Right Leg 2
            BP_TORSOSECTION_RIGHTLEG3 = 12000, // Torso Section | Right Leg 3
            BP_TORSOSECTION_BRAIN = 13000 // Torso Section | Brain
        }

        // Values for configuring the shader type in a BSLightingShaderProperty
        public enum BSLightingShaderPropertyShaderType : uint
        {
            DEFAULT = 0,
            ENVIRONMENT_MAP = 1, // Enables EnvMap Mask(TS6), EnvMap Scale
            GLOW_SHADER = 2, // Enables Glow(TS3)
            HEIGHTMAP = 3, // Enables Height(TS4)
            FACE_TINT = 4, // Enables SubSurface(TS3), Detail(TS4), Tint(TS7)
            SKIN_TINT = 5, // Enables Skin Tint Color
            HAIR_TINT = 6, // Enables Hair Tint Color
            PARALLAX_OCC_MATERIAL = 7, // Enables Height(TS4), Max Passes, Scale.  Unused?
            WORLD_MULTITEXTURE = 8,
            WORLDMAP1 = 9,
            UNKNOWN_10 = 10,
            MULTILAYER_PARALLAX = 11, // Enables EnvMap Mask(TS6), Layer(TS7), Parallax Layer Thickness, Parallax Refraction Scale, Parallax Inner Layer U Scale, Parallax Inner Layer V Scale, EnvMap Scale
            UNKNOWN_12 = 12,
            WORLDMAP2 = 13,
            SPARKLE_SNOW = 14, // Enables SparkleParams
            WORLDMAP3 = 15,
            EYE_ENVMAP = 16, // Enables EnvMap Mask(TS6), Eye EnvMap Scale
            UNKNOWN_17 = 17,
            WORLDMAP4 = 18,
            WORLD_LOD_MULTITEXTURE = 19
        }

        // An unsigned 32-bit integer, describing which float variable in BSEffectShaderProperty to animate.
        public enum EffectShaderControlledVariable : uint
        {
            ESCV_EMISSIVEMULTIPLE = 0, // EmissiveMultiple.
            ESCV_FALLOFF_START_ANGLE = 1, // Falloff Start Angle (degrees).
            ESCV_FALLOFF_STOP_ANGLE = 2, // Falloff Stop Angle (degrees).
            ESCV_FALLOFF_START_OPACITY = 3, // Falloff Start Opacity.
            ESCV_FALLOFF_STOP_OPACITY = 4, // Falloff Stop Opacity.
            ESCV_ALPHA_TRANSPARENCY = 5, // Alpha Transparency (Emissive alpha?).
            ESCV_U_OFFSET = 6, // U Offset.
            ESCV_U_SCALE = 7, // U Scale.
            ESCV_V_OFFSET = 8, // V Offset.
            ESCV_V_SCALE = 9 // V Scale.
        }

        // An unsigned 32-bit integer, describing which color in BSEffectShaderProperty to animate.
        public enum EffectShaderControlledColor : uint
        {
            ECSC_EMISSIVE_COLOR = 0 // Emissive Color.
        }

        // An unsigned 32-bit integer, describing which float variable in BSLightingShaderProperty to animate.
        public enum LightingShaderControlledVariable : uint
        {
            LSCV_REFRACTION_STRENGTH = 0, // The amount of distortion.
            LSCV_ENVIRONMENT_MAP_SCALE = 8, // Environment Map Scale.
            LSCV_GLOSSINESS = 9, // Glossiness.
            LSCV_SPECULAR_STRENGTH = 10, // Specular Strength.
            LSCV_EMISSIVE_MULTIPLE = 11, // Emissive Multiple.
            LSCV_ALPHA = 12, // Alpha.
            LSCV_U_OFFSET = 20, // U Offset.
            LSCV_U_SCALE = 21, // U Scale.
            LSCV_V_OFFSET = 22, // V Offset.
            LSCV_V_SCALE = 23 // V Scale.
        }

        // An unsigned 32-bit integer, describing which color in BSLightingShaderProperty to animate.
        public enum LightingShaderControlledColor : uint
        {
            LSCC_SPECULAR_COLOR = 0, // Specular Color.
            LSCC_EMISSIVE_COLOR = 1 // Emissive Color.
        }

        // The type of constraint.
        public enum hkConstraintType : uint
        {
            BALLANDSOCKET = 0, // A ball and socket constraint.
            HINGE = 1, // A hinge constraint.
            LIMITED_HINGE = 2, // A limited hinge constraint.
            PRISMATIC = 6, // A prismatic constraint.
            RAGDOLL = 7, // A ragdoll constraint.
            STIFFSPRING = 8, // A stiff spring constraint.
            MALLEABLE = 13 // A malleable constraint.
        }

        // Animation type used on this position. This specifies the function of this position.
        public enum AnimationType : ushort
        {
            SIT = 1, // Actor use sit animation.
            SLEEP = 2, // Actor use sleep animation.
            LEAN = 4 // Used for lean animations?
        }

        public enum MotorType : byte
        {
            MOTOR_NONE = 0,
            MOTOR_POSITION = 1,
            MOTOR_VELOCITY = 2,
            MOTOR_SPRING = 3
        }

        // Determines how the raw image data is stored in NiRawImageData.
        public enum ImageType : uint
        {
            RGB = 1, // Colors store red, blue, and green components.
            RGBA = 2 // Colors store red, blue, green, and alpha components.
        }

        public enum BroadPhaseType : byte
        {
            BROAD_PHASE_INVALID = 0,
            BROAD_PHASE_ENTITY = 1,
            BROAD_PHASE_PHANTOM = 2,
            BROAD_PHASE_BORDER = 3
        }

        public enum PathFlags : ushort
        {
            NIPI_CVDATANEEDSUPDATE = 0x1, // CVDataNeedsUpdate
            NIPI_CURVETYPEOPEN = 0x2, // CurveTypeOpen
            NIPI_ALLOWFLIP = 0x4, // AllowFlip
            NIPI_BANK = 0x8, // Bank
            NIPI_CONSTANTVELOCITY = 0x10, // ConstantVelocity
            NIPI_FOLLOW = 0x20, // Follow
            NIPI_FLIP = 0x40 // Flip
        }

        public enum InterpBlendFlags : byte
        {
            MANAGER_CONTROLLED = 1 // MANAGER_CONTROLLED
        }

        // Should be a bitfield for flip toggle.
        public enum LookAtFlags : ushort
        {
            LOOK_X_AXIS = 0, // X-Axis
            LOOK_FLIP = 1, // Flip
            LOOK_Y_AXIS = 2, // Y-Axis
            LOOK_Z_AXIS = 4 // Z-Axis
        }

        public enum ChannelType : uint
        {
            CHNL_RED = 0, // Red
            CHNL_GREEN = 1, // Green
            CHNL_BLUE = 2, // Blue
            CHNL_ALPHA = 3, // Alpha
            CHNL_COMPRESSED = 4, // Compressed
            CHNL_INDEX = 16, // Index
            CHNL_EMPTY = 19 // Empty
        }

        public enum ChannelConvention : uint
        {
            CC_FIXED = 0, // Fixed
            CC_INDEX = 3, // Palettized
            CC_COMPRESSED = 4, // Compressed
            CC_EMPTY = 5 // Empty
        }

        // The type of animation interpolation (blending) that will be used on the associated key frames.
        public enum BSShaderType : uint
        {
            SHADER_TALL_GRASS = 0, // Tall Grass Shader
            SHADER_DEFAULT = 1, // Standard Lighting Shader
            SHADER_SKY = 10, // Sky Shader
            SHADER_SKIN = 14, // Skin Shader
            SHADER_WATER = 17, // Water Shader
            SHADER_LIGHTING30 = 29, // Lighting 3.0 Shader
            SHADER_TILE = 32, // Tiled Shader
            SHADER_NOLIGHTING = 33 // No Lighting Shader
        }

        // Sets what sky function this object fulfills in BSSkyShaderProperty or SkyShaderProperty.
        public enum SkyObjectType : uint
        {
            BSSM_SKY_TEXTURE = 0, // BSSM_Sky_Texture
            BSSM_SKY_SUNGLARE = 1, // BSSM_Sky_Sunglare
            BSSM_SKY = 2, // BSSM_Sky
            BSSM_SKY_CLOUDS = 3, // BSSM_Sky_Clouds
            BSSM_SKY_STARS = 5, // BSSM_Sky_Stars
            BSSM_SKY_MOON_STARS_MASK = 7 // BSSM_Sky_Moon_Stars_Mask
        }

        // Anim note types.
        public enum AnimNoteType : uint
        {
            ANT_INVALID = 0, // ANT_INVALID
            ANT_GRABIK = 1, // ANT_GRABIK
            ANT_LOOKIK = 2 // ANT_LOOKIK
        }

        // Culling modes for multi bound nodes.
        public enum BSCPCullingType : uint
        {
            BSCP_CULL_NORMAL = 0, // Normal
            BSCP_CULL_ALLPASS = 1, // All Pass
            BSCP_CULL_ALLFAIL = 2, // All Fail
            BSCP_CULL_IGNOREMULTIBOUNDS = 3, // Ignore Multi Bounds
            BSCP_CULL_FORCEMULTIBOUNDSNOUPDATE = 4 // Force Multi Bounds No Update
        }

        // Sets how objects are to be cloned.
        public enum CloningBehavior : uint
        {
            CLONING_SHARE = 0, // Share this object pointer with the newly cloned scene.
            CLONING_COPY = 1, // Create an exact duplicate of this object for use with the newly cloned scene.
            CLONING_BLANK_COPY = 2 // Create a copy of this object for use with the newly cloned stream, leaving some of the data to be written later.
        }

        // The data format of components.
        public enum ComponentFormat : uint
        {
            F_UNKNOWN = 0x00000000, // Unknown, or don't care, format.
            F_INT8_1 = 0x00010101,
            F_INT8_2 = 0x00020102,
            F_INT8_3 = 0x00030103,
            F_INT8_4 = 0x00040104,
            F_UINT8_1 = 0x00010105,
            F_UINT8_2 = 0x00020106,
            F_UINT8_3 = 0x00030107,
            F_UINT8_4 = 0x00040108,
            F_NORMINT8_1 = 0x00010109,
            F_NORMINT8_2 = 0x0002010A,
            F_NORMINT8_3 = 0x0003010B,
            F_NORMINT8_4 = 0x0004010C,
            F_NORMUINT8_1 = 0x0001010D,
            F_NORMUINT8_2 = 0x0002010E,
            F_NORMUINT8_3 = 0x0003010F,
            F_NORMUINT8_4 = 0x00040110,
            F_INT16_1 = 0x00010211,
            F_INT16_2 = 0x00020212,
            F_INT16_3 = 0x00030213,
            F_INT16_4 = 0x00040214,
            F_UINT16_1 = 0x00010215,
            F_UINT16_2 = 0x00020216,
            F_UINT16_3 = 0x00030217,
            F_UINT16_4 = 0x00040218,
            F_NORMINT16_1 = 0x00010219,
            F_NORMINT16_2 = 0x0002021A,
            F_NORMINT16_3 = 0x0003021B,
            F_NORMINT16_4 = 0x0004021C,
            F_NORMUINT16_1 = 0x0001021D,
            F_NORMUINT16_2 = 0x0002021E,
            F_NORMUINT16_3 = 0x0003021F,
            F_NORMUINT16_4 = 0x00040220,
            F_INT32_1 = 0x00010421,
            F_INT32_2 = 0x00020422,
            F_INT32_3 = 0x00030423,
            F_INT32_4 = 0x00040424,
            F_UINT32_1 = 0x00010425,
            F_UINT32_2 = 0x00020426,
            F_UINT32_3 = 0x00030427,
            F_UINT32_4 = 0x00040428,
            F_NORMINT32_1 = 0x00010429,
            F_NORMINT32_2 = 0x0002042A,
            F_NORMINT32_3 = 0x0003042B,
            F_NORMINT32_4 = 0x0004042C,
            F_NORMUINT32_1 = 0x0001042D,
            F_NORMUINT32_2 = 0x0002042E,
            F_NORMUINT32_3 = 0x0003042F,
            F_NORMUINT32_4 = 0x00040430,
            F_FLOAT16_1 = 0x00010231,
            F_FLOAT16_2 = 0x00020232,
            F_FLOAT16_3 = 0x00030233,
            F_FLOAT16_4 = 0x00040234,
            F_FLOAT32_1 = 0x00010435,
            F_FLOAT32_2 = 0x00020436,
            F_FLOAT32_3 = 0x00030437,
            F_FLOAT32_4 = 0x00040438,
            F_UINT_10_10_10_L1 = 0x00010439,
            F_NORMINT_10_10_10_L1 = 0x0001043A,
            F_NORMINT_11_11_10 = 0x0001043B,
            F_NORMUINT8_4_BGRA = 0x0004013C,
            F_NORMINT_10_10_10_2 = 0x0001043D,
            F_UINT_10_10_10_2 = 0x0001043E
        }

        // Determines how a data stream is used?
        public enum DataStreamUsage : uint
        {
            USAGE_VERTEX_INDEX = 0,
            USAGE_VERTEX = 1,
            USAGE_SHADER_CONSTANT = 2,
            USAGE_USER = 3
        }

        // Describes the type of primitives stored in a mesh object.
        public enum MeshPrimitiveType : uint
        {
            MESH_PRIMITIVE_TRIANGLES = 0, // Triangle primitive type.
            MESH_PRIMITIVE_TRISTRIPS = 1, // Triangle strip primitive type.
            MESH_PRIMITIVE_LINESTRIPS = 2, // Line strip primitive type.
            MESH_PRIMITIVE_QUADS = 3, // Quadrilateral primitive type.
            MESH_PRIMITIVE_POINTS = 4 // Point primitive type.
        }

        // Specifies the time when an application must syncronize for some reason.
        public enum SyncPoint : ushort
        {
            SYNC_ANY = 0x8000, // Value used when no specific sync point is desired.
            SYNC_UPDATE = 0x8010, // Synchronize when an object is updated.
            SYNC_POST_UPDATE = 0x8020, // Synchronize when an entire scene graph has been updated.
            SYNC_VISIBLE = 0x8030, // Synchronize when an object is determined to be potentially visible.
            SYNC_RENDER = 0x8040, // Synchronize when an object is rendered.
            SYNC_PHYSICS_SIMULATE = 0x8050, // Synchronize when a physics simulation step is about to begin.
            SYNC_PHYSICS_COMPLETED = 0x8060, // Synchronize when a physics simulation step has produced results.
            SYNC_REFLECTIONS = 0x8070 // Syncronize after all data necessary to calculate reflections is ready.
        }
        #endregion

        #region Compounds
        public struct BoundingBox
		{
			public uint unknownInt;
			public Vector3 translation;
			public Matrix4x4 rotation;
			public Vector3 radius;

			public void Deserialize(UnityBinaryReader reader)
			{
				unknownInt = reader.ReadLEUInt32();
				translation = reader.ReadLEVector3();
				rotation = NiReaderUtils.Read3x3RotationMatrix(reader);
				radius = reader.ReadLEVector3();
			}
		}
		
		public struct Color3
		{
			public float r;
			public float g;
			public float b;

			public void Deserialize(UnityBinaryReader reader)
			{
				r = reader.ReadLESingle();
				g = reader.ReadLESingle();
				b = reader.ReadLESingle();
			}
		}
		public struct Color4
		{
			public float r;
			public float g;
			public float b;
			public float a;

			public void Deserialize(UnityBinaryReader reader)
			{
				r = reader.ReadLESingle();
				g = reader.ReadLESingle();
				b = reader.ReadLESingle();
				a = reader.ReadLESingle();
			}
		}

		public struct TexDesc
		{
			public Ref<NiSourceTexture> source;
			public TexClampMode clampMode;
			public TexFilterMode filterMode;
			public uint UVSet;
			public short PS2L;
			public short PS2K;
			public ushort unknown1;

			public void Deserialize(UnityBinaryReader reader)
			{
				source = NiReaderUtils.ReadRef<NiSourceTexture>(reader);
				clampMode = (TexClampMode)reader.ReadLEUInt32();
				filterMode = (TexFilterMode)reader.ReadLEUInt32();
				UVSet = reader.ReadLEUInt32();
				PS2L = reader.ReadLEInt16();
				PS2K = reader.ReadLEInt16();
				unknown1 = reader.ReadLEUInt16();
			}
		}
		public struct TexCoord
		{
			public float u;
			public float v;

			public void Deserialize(UnityBinaryReader reader)
			{
				u = reader.ReadLESingle();
				v = reader.ReadLESingle();
			}
		}

		public struct Triangle
		{
			public ushort v1;
			public ushort v2;
			public ushort v3;

			public void Deserialize(UnityBinaryReader reader)
			{
				v1 = reader.ReadLEUInt16();
				v2 = reader.ReadLEUInt16();
				v3 = reader.ReadLEUInt16();
			}
		}

		public struct MatchGroup
		{
			public ushort numVertices;
			public ushort[] vertexIndices;

			public void Deserialize(UnityBinaryReader reader)
			{
				numVertices = reader.ReadLEUInt16();

				vertexIndices = new ushort[numVertices];
				for(int i = 0; i < vertexIndices.Length; i++)
				{
					vertexIndices[i] = reader.ReadLEUInt16();
				}
			}
		}
		
		public struct TBC
		{
			public float t;
			public float b;
			public float c;

			public void Deserialize(UnityBinaryReader reader)
			{
				t = reader.ReadLESingle();
				b = reader.ReadLESingle();
				c = reader.ReadLESingle();
			}
		}

		public struct Key<T>
		{
			public float time;
			public T value;
			public T forward;
			public T backward;
			public TBC TBC;

			public void Deserialize(UnityBinaryReader reader, KeyType keyType)
			{
				time = reader.ReadLESingle();
				value = NiReaderUtils.Read<T>(reader);

				if(keyType == KeyType.QUADRATIC_KEY)
				{
					forward = NiReaderUtils.Read<T>(reader);
					backward = NiReaderUtils.Read<T>(reader);
				}
				else if(keyType == KeyType.TBC_KEY)
				{
					TBC = new TBC();
					TBC.Deserialize(reader);
				}
			}
		}
		public struct KeyGroup<T>
		{
			public uint numKeys;
			public KeyType interpolation;
			public Key<T>[] keys;

			public void Deserialize(UnityBinaryReader reader)
			{
				numKeys = reader.ReadLEUInt32();

				if(numKeys != 0)
				{
					interpolation = (KeyType)reader.ReadLEUInt32();
				}

				keys = new Key<T>[numKeys];
				for(int i = 0; i < keys.Length; i++)
				{
					keys[i] = new Key<T>();
					keys[i].Deserialize(reader, interpolation);
				}
			}
		}
		public struct QuatKey<T>
		{
			public float time;
			public T value;
			public TBC TBC;

			public void Deserialize(UnityBinaryReader reader, KeyType keyType)
			{
				time = reader.ReadLESingle();

				if(keyType != KeyType.XYZ_ROTATION_KEY)
				{
					value = NiReaderUtils.Read<T>(reader);
				}

				if(keyType == KeyType.TBC_KEY)
				{
					TBC = new TBC();
					TBC.Deserialize(reader);
				}
			}
		}

		public struct SkinData
		{
			public SkinTransform skinTransform;
			public Vector3 boundingSphereOffset;
			public float boundingSphereRadius;
			public ushort numVertices;
			public SkinWeight[] vertexWeights;

			public void Deserialize(UnityBinaryReader reader)
			{
				skinTransform = new SkinTransform();
				skinTransform.Deserialize(reader);

				boundingSphereOffset = reader.ReadLEVector3();
				boundingSphereRadius = reader.ReadLESingle();
				numVertices = reader.ReadLEUInt16();

				vertexWeights = new SkinWeight[numVertices];
				for(int i = 0; i < vertexWeights.Length; i++)
				{
					vertexWeights[i] = new SkinWeight();
					vertexWeights[i].Deserialize(reader);
				}
			}
		}
		public struct SkinWeight
		{
			public ushort index;
			public float weight;

			public void Deserialize(UnityBinaryReader reader)
			{
				index = reader.ReadLEUInt16();
				weight = reader.ReadLESingle();
			}
		}
		public struct SkinTransform
		{
			public Matrix4x4 rotation;
			public Vector3 translation;
			public float scale;

			public void Deserialize(UnityBinaryReader reader)
			{
				rotation = NiReaderUtils.Read3x3RotationMatrix(reader);
				translation = reader.ReadLEVector3();
				scale = reader.ReadLESingle();
			}
		}

		public struct Particle
		{
			public Vector3 velocity;
			public Vector3 unknownVector;
			public float lifetime;
			public float lifespan;
			public float timestamp;
			public ushort unknownShort;
			public ushort vertexID;

			public void Deserialize(UnityBinaryReader reader)
			{
				velocity = reader.ReadLEVector3();
				unknownVector = reader.ReadLEVector3();
				lifetime = reader.ReadLESingle();
				lifespan = reader.ReadLESingle();
				timestamp = reader.ReadLESingle();
				unknownShort = reader.ReadLEUInt16();
				vertexID = reader.ReadLEUInt16();
			}
		}

		public struct Morph
		{
			public uint numKeys;
			public KeyType interpolation;
			public Key<float>[] keys;
			public Vector3[] vectors;

			public void Deserialize(UnityBinaryReader reader, uint numVertices)
			{
				numKeys = reader.ReadLEUInt32();
				interpolation = (KeyType)reader.ReadLEUInt32();

				keys = new Key<float>[numKeys];
				for(int i = 0; i < keys.Length; i++)
				{
					keys[i] = new Key<float>();
					keys[i].Deserialize(reader, interpolation);
				}

				vectors = new Vector3[numVertices];
				for(int i = 0; i < vectors.Length; i++)
				{
					vectors[i] = reader.ReadLEVector3();
				}
			}
		}
        #endregion

        #region NiObjects
        /// <summary>
        /// These are the main units of data that NIF files are arranged in.
        /// </summary>
		public abstract class NiObject
		{
			public virtual void Deserialize(UnityBinaryReader reader) {}
		}

        /// <summary>
        /// An object that can be controlled by a controller.
        /// </summary>
		public abstract class NiObjectNET : NiObject
		{
			public string name;
			public Ref<NiExtraData> extraData;
			public Ref<NiTimeController> controller;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				name = reader.ReadLELength32PrefixedASCIIString();
				extraData = NiReaderUtils.ReadRef<NiExtraData>(reader);
				controller = NiReaderUtils.ReadRef<NiTimeController>(reader);
			}
		}
		public abstract class NiAVObject : NiObjectNET
		{
			public enum Flags
			{
				Hidden = 0x1
			}

			public ushort flags;
			public Vector3 translation;
			public Matrix4x4 rotation;
			public float scale;
			public Vector3 velocity;
			//public uint numProperties;
			public Ref<NiProperty>[] properties;
			public bool hasBoundingBox;
			public BoundingBox boundingBox;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				flags = NiReaderUtils.ReadFlags(reader);
				translation = reader.ReadLEVector3();
				rotation = NiReaderUtils.Read3x3RotationMatrix(reader);
				scale = reader.ReadLESingle();
				velocity = reader.ReadLEVector3();
				properties = NiReaderUtils.ReadLengthPrefixedRefs32<NiProperty>(reader);
				hasBoundingBox = reader.ReadLEBool32();

				if(hasBoundingBox)
				{
					boundingBox = new BoundingBox();
					boundingBox.Deserialize(reader);
				}
			}
		}

		// Nodes
		public class NiNode : NiAVObject
		{
			//public uint numChildren;
			public Ref<NiAVObject>[] children;
			//public uint numEffects;
			public Ref<NiDynamicEffect>[] effects;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				children = NiReaderUtils.ReadLengthPrefixedRefs32<NiAVObject>(reader);
				effects = NiReaderUtils.ReadLengthPrefixedRefs32<NiDynamicEffect>(reader);
			}
		}
		public class RootCollisionNode : NiNode { }
		public class NiBSAnimationNode : NiNode { }
		public class NiBSParticleNode : NiNode { }
        public class NiBillboardNode : NiNode { }
        public class AvoidNode : NiNode { }

		// Geometry
		public abstract class NiGeometry : NiAVObject
		{
			public Ref<NiGeometryData> data;
			public Ref<NiSkinInstance> skinInstance;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				data = NiReaderUtils.ReadRef<NiGeometryData>(reader);
				skinInstance = NiReaderUtils.ReadRef<NiSkinInstance>(reader);
			}
		}
		public abstract class NiGeometryData : NiObject
		{
			public ushort numVertices;
			public bool hasVertices;
			public Vector3[] vertices;
			public bool hasNormals;
			public Vector3[] normals;
			public Vector3 center;
			public float radius;
			public bool hasVertexColors;
			public Color4[] vertexColors;
			public ushort numUVSets;
			public bool hasUV;
			public TexCoord[,] UVSets;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				numVertices = reader.ReadLEUInt16();
				hasVertices = reader.ReadLEBool32();

				if(hasVertices)
				{
					vertices = new Vector3[numVertices];
					for(int i = 0; i < vertices.Length; i++)
					{
						vertices[i] = reader.ReadLEVector3();
					}
				}

				hasNormals = reader.ReadLEBool32();

				if(hasNormals)
				{
					normals = new Vector3[numVertices];
					for(int i = 0; i < normals.Length; i++)
					{
						normals[i] = reader.ReadLEVector3();
					}
				}

				center = reader.ReadLEVector3();
				radius = reader.ReadLESingle();
				hasVertexColors = reader.ReadLEBool32();

				if(hasVertexColors)
				{
					vertexColors = new Color4[numVertices];
					for(int i = 0; i < vertexColors.Length; i++)
					{
						vertexColors[i] = new Color4();
						vertexColors[i].Deserialize(reader);
					}
				}

				numUVSets = reader.ReadLEUInt16();
				hasUV = reader.ReadLEBool32();

				if(hasUV)
				{
					UVSets = new TexCoord[numUVSets, numVertices];

					for(int i = 0; i < numUVSets; i++)
					{
						for(int j = 0; j < numVertices; j++)
						{
							UVSets[i, j] = new TexCoord();
							UVSets[i, j].Deserialize(reader);
						}
					}
				}
			}
		}
		public abstract class NiTriBasedGeom : NiGeometry
		{
			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);
			}
		}
		public abstract class NiTriBasedGeomData : NiGeometryData
		{
			public ushort numTriangles;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				numTriangles = reader.ReadLEUInt16();
			}
		}
		public class NiTriShape : NiTriBasedGeom
		{
			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);
			}
		}
		public class NiTriShapeData : NiTriBasedGeomData
		{
			public uint numTrianglePoints;
			public Triangle[] triangles;
			public ushort numMatchGroups;
			public MatchGroup[] matchGroups;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				numTrianglePoints = reader.ReadLEUInt32();

				triangles = new Triangle[numTriangles];
				for(int i = 0; i < triangles.Length; i++)
				{
					triangles[i] = new Triangle();
					triangles[i].Deserialize(reader);
				}

				numMatchGroups = reader.ReadLEUInt16();

				matchGroups = new MatchGroup[numMatchGroups];
				for(int i = 0; i < matchGroups.Length; i++)
				{
					matchGroups[i] = new MatchGroup();
					matchGroups[i].Deserialize(reader);
				}
			}
		}

		// Properties
		public abstract class NiProperty : NiObjectNET
		{
			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);
			}
		}
		public class NiTexturingProperty : NiProperty
		{
			public ushort flags;

			public ApplyMode applyMode;
			public uint textureCount;

			public bool hasBaseTexture;
			public TexDesc baseTexture;

			public bool hasDarkTexture;
			public TexDesc darkTexture;

			public bool hasDetailTexture;
			public TexDesc detailTexture;

			public bool hasGlossTexture;
			public TexDesc glossTexture;

			public bool hasGlowTexture;
			public TexDesc glowTexture;

			public bool hasBumpMapTexture;
			public TexDesc bumpMapTexture;

			public bool hasDecal0Texture;
			public TexDesc decal0Texture;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				flags = NiReaderUtils.ReadFlags(reader);

				applyMode = (ApplyMode)reader.ReadLEUInt32();
				textureCount = reader.ReadLEUInt32();

				hasBaseTexture = reader.ReadLEBool32();
				if(hasBaseTexture)
				{
					baseTexture = new TexDesc();
					baseTexture.Deserialize(reader);
				}

				hasDarkTexture = reader.ReadLEBool32();
				if(hasDarkTexture)
				{
					darkTexture = new TexDesc();
					darkTexture.Deserialize(reader);
				}

				hasDetailTexture = reader.ReadLEBool32();
				if(hasDetailTexture)
				{
					detailTexture = new TexDesc();
					detailTexture.Deserialize(reader);
				}

				hasGlossTexture = reader.ReadLEBool32();
				if(hasGlossTexture)
				{
					glossTexture = new TexDesc();
					glossTexture.Deserialize(reader);
				}

				hasGlowTexture = reader.ReadLEBool32();
				if(hasGlowTexture)
				{
					glowTexture = new TexDesc();
					glowTexture.Deserialize(reader);
				}

				hasBumpMapTexture = reader.ReadLEBool32();
				if(hasBumpMapTexture)
				{
					bumpMapTexture = new TexDesc();
					bumpMapTexture.Deserialize(reader);
				}

				hasDecal0Texture = reader.ReadLEBool32();
				if(hasDecal0Texture)
				{
					decal0Texture = new TexDesc();
					decal0Texture.Deserialize(reader);
				}
			}
		}
		public class NiAlphaProperty : NiProperty
		{
			public ushort flags;
			public byte threshold;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				flags = reader.ReadLEUInt16();
				threshold = reader.ReadByte();
			}
		}
		public class NiZBufferProperty : NiProperty
		{
			public ushort flags;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				flags = reader.ReadLEUInt16();
			}
		}
		public class NiVertexColorProperty : NiProperty
		{
			public ushort flags;
			public VertMode vertexMode;
			public LightMode lightingMode;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				flags = NiReaderUtils.ReadFlags(reader);
				vertexMode = (VertMode)reader.ReadLEUInt32();
				lightingMode = (LightMode)reader.ReadLEUInt32();
			}
		}
        public class NiShadeProperty : NiProperty
        {
            public ushort flags;

            public override void Deserialize(UnityBinaryReader reader)
            {
                base.Deserialize(reader);

                flags = NiReaderUtils.ReadFlags(reader);
            }
        }

        // Data
        public class NiUVData : NiObject
		{
			public KeyGroup<float>[] UVGroups;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				UVGroups = new KeyGroup<float>[4];

				for(int i = 0; i < UVGroups.Length; i++)
				{
					UVGroups[i] = new KeyGroup<float>();
					UVGroups[i].Deserialize(reader);
				}
			}
		}
		public class NiKeyframeData : NiObject
		{
			public uint numRotationKeys;
			public KeyType rotationType;
			public QuatKey<Quaternion>[] quaternionKeys;
			public float unknownFloat;
			public KeyGroup<float>[] XYZRotations;
			public KeyGroup<Vector3> translations;
			public KeyGroup<float> scales;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				numRotationKeys = reader.ReadLEUInt32();

				if(numRotationKeys != 0)
				{
					rotationType = (KeyType)reader.ReadLEUInt32();

					if(rotationType != KeyType.XYZ_ROTATION_KEY)
					{
						quaternionKeys = new QuatKey<Quaternion>[numRotationKeys];
						for(int i = 0; i < quaternionKeys.Length; i++)
						{
							quaternionKeys[i] = new QuatKey<Quaternion>();
							quaternionKeys[i].Deserialize(reader, rotationType);
						}
					}
					else
					{
						unknownFloat = reader.ReadLESingle();

						XYZRotations = new KeyGroup<float>[3];
						for(int i = 0; i < XYZRotations.Length; i++)
						{
							XYZRotations[i] = new KeyGroup<float>();
							XYZRotations[i].Deserialize(reader);
						}
					}
				}

				translations = new KeyGroup<Vector3>();
				translations.Deserialize(reader);

				scales = new KeyGroup<float>();
				scales.Deserialize(reader);
			}
		}
		public class NiColorData : NiObject
		{
			public KeyGroup<Color4> data;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				data = new KeyGroup<Color4>();
				data.Deserialize(reader);
			}
		}
		public class NiMorphData : NiObject
		{
			public uint numMorphs;
			public uint numVertices;
			public byte relativeTargets;
			public Morph[] morphs;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				numMorphs = reader.ReadLEUInt32();
				numVertices = reader.ReadLEUInt32();
				relativeTargets = reader.ReadByte();

				morphs = new Morph[numMorphs];
				for(int i = 0; i < morphs.Length; i++)
				{
					morphs[i] = new Morph();
					morphs[i].Deserialize(reader, numVertices);
				}
			}
		}
		public class NiVisData : NiObject
		{
			public uint numKeys;
			public Key<byte>[] keys;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				numKeys = reader.ReadLEUInt32();

				keys = new Key<byte>[numKeys];
				for(int i = 0; i < keys.Length; i++)
				{
					keys[i] = new Key<byte>();
					keys[i].Deserialize(reader, KeyType.LINEAR_KEY);
				}
			}
		}
		public class NiFloatData : NiObject
		{
			public KeyGroup<float> data;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				data = new KeyGroup<float>();
				data.Deserialize(reader);
			}
		}
        public class NiPosData : NiObject
        {
            public KeyGroup<Vector3> data;

            public override void Deserialize(UnityBinaryReader reader)
            {
                base.Deserialize(reader);

                data = new KeyGroup<Vector3>();
                data.Deserialize(reader);
            }
        }

        public class NiExtraData : NiObject
		{
			public Ref<NiExtraData> nextExtraData;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				nextExtraData = NiReaderUtils.ReadRef<NiExtraData>(reader);
			}
		}
		public class NiStringExtraData : NiExtraData
		{
			public uint bytesRemaining;
			public string str;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				bytesRemaining = reader.ReadLEUInt32();
				str = reader.ReadLELength32PrefixedASCIIString();
			}
		}
		public class NiTextKeyExtraData : NiExtraData
		{
			public uint unknownInt1;
            public uint numTextKeys;
            public Key<string>[] textKeys;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				unknownInt1 = reader.ReadLEUInt32();
				numTextKeys = reader.ReadLEUInt32();

				textKeys = new Key<string>[numTextKeys];
				for(int i = 0; i < textKeys.Length; i++)
				{
					textKeys[i] = new Key<string>();
					textKeys[i].Deserialize(reader, KeyType.LINEAR_KEY);
				}
			}
		}
        public class NiVertWeightsExtraData : NiExtraData
        {
            public uint numBytes;
            public ushort numVertices;
            public float[] weights;

            public override void Deserialize(UnityBinaryReader reader)
            {
                base.Deserialize(reader);

                numBytes = reader.ReadLEUInt32();
                numVertices = reader.ReadLEUInt16();

                weights = new float[numVertices];
                for(var i = 0; i < weights.Length; i++)
                {
                    weights[i] = reader.ReadLESingle();
                }
            }
        }

        // Particles
        public class NiParticles : NiGeometry { }
		public class NiParticlesData : NiGeometryData
		{
			public ushort numParticles;
			public float particleRadius;
			public ushort numActive;
			public bool hasSizes;
			public float[] sizes;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				numParticles = reader.ReadLEUInt16();
				particleRadius = reader.ReadLESingle();
				numActive = reader.ReadLEUInt16();

				hasSizes = reader.ReadLEBool32();
				if(hasSizes)
				{
					sizes = new float[numVertices];
					for(int i = 0; i < sizes.Length; i++)
					{
						sizes[i] = reader.ReadLESingle();
					}
				}
			}
		}
		public class NiRotatingParticles : NiParticles { }
		public class NiRotatingParticlesData : NiParticlesData
		{
			public bool hasRotations;
			public Quaternion[] rotations;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				hasRotations = reader.ReadLEBool32();

				if(hasRotations)
				{
					rotations = new Quaternion[numVertices];
					for(int i = 0; i < rotations.Length; i++)
					{
						rotations[i] = reader.ReadLEQuaternionWFirst();
					}
				}
			}
		}
		public class NiAutoNormalParticles : NiParticles { }
		public class NiAutoNormalParticlesData : NiParticlesData { }

		public class NiParticleSystemController : NiTimeController
		{
			public float speed;
			public float speedRandom;
			public float verticalDirection;
			public float verticalAngle;
			public float horizontalDirection;
			public float horizontalAngle;
			public Vector3 unknownNormal;
			public Color4 unknownColor;
			public float size;
			public float emitStartTime;
			public float emitStopTime;
			public byte unknownByte;
			public float emitRate;
			public float lifetime;
			public float lifetimeRandom;
			public ushort emitFlags;
			public Vector3 startRandom;
			public Ptr<NiObject> emitter;
			public ushort unknownShort2;
			public float unknownFloat13;
			public uint unknownInt1;
			public uint unknownInt2;
			public ushort unknownShort3;
			public ushort numParticles;
			public ushort numValid;
			public Particle[] particles;
			public Ref<NiObject> unknownLink;
			public Ref<NiParticleModifier> particleExtra;
			public Ref<NiObject> unknownLink2;
			public byte trailer;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				speed = reader.ReadLESingle();
				speedRandom = reader.ReadLESingle();
				verticalDirection = reader.ReadLESingle();
				verticalAngle = reader.ReadLESingle();
				horizontalDirection = reader.ReadLESingle();
				horizontalAngle = reader.ReadLESingle();
				unknownNormal = reader.ReadLEVector3();

				unknownColor = new Color4();
				unknownColor.Deserialize(reader);

				size = reader.ReadLESingle();
				emitStartTime = reader.ReadLESingle();
				emitStopTime = reader.ReadLESingle();
				unknownByte = reader.ReadByte();
				emitRate = reader.ReadLESingle();
				lifetime = reader.ReadLESingle();
				lifetimeRandom = reader.ReadLESingle();
				emitFlags = reader.ReadLEUInt16();
				startRandom = reader.ReadLEVector3();
				emitter = NiReaderUtils.ReadPtr<NiObject>(reader);
				unknownShort2 = reader.ReadLEUInt16();
				unknownFloat13 = reader.ReadLESingle();
				unknownInt1 = reader.ReadLEUInt32();
				unknownInt2 = reader.ReadLEUInt32();
				unknownShort3 = reader.ReadLEUInt16();
				numParticles = reader.ReadLEUInt16();
				numValid = reader.ReadLEUInt16();

				particles = new Particle[numParticles];
				for(int i = 0; i < particles.Length; i++)
				{
					particles[i] = new Particle();
					particles[i].Deserialize(reader);
				}

				unknownLink = NiReaderUtils.ReadRef<NiObject>(reader);
				particleExtra = NiReaderUtils.ReadRef<NiParticleModifier>(reader);
				unknownLink2 = NiReaderUtils.ReadRef<NiObject>(reader);
				trailer = reader.ReadByte();
			}
		}

        public class NiBSPArrayController : NiParticleSystemController {}

        // Particle Modifiers
        public abstract class NiParticleModifier : NiObject
		{
			public Ref<NiParticleModifier> nextModifier;
			public Ptr<NiParticleSystemController> controller;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				nextModifier = NiReaderUtils.ReadRef<NiParticleModifier>(reader);
				controller = NiReaderUtils.ReadPtr<NiParticleSystemController>(reader);
			}
		}
		public class NiGravity : NiParticleModifier
		{
			public float unknownFloat1;
			public float force;
			public FieldType type;
			public Vector3 position;
			public Vector3 direction;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				unknownFloat1 = reader.ReadLESingle();
				force = reader.ReadLESingle();
				type = (FieldType)reader.ReadLEUInt32();
				position = reader.ReadLEVector3();
				direction = reader.ReadLEVector3();
			}
		}
		public class NiParticleBomb : NiParticleModifier
		{
			public float decay;
			public float duration;
			public float deltaV;
			public float start;
			public DecayType decayType;
			public Vector3 position;
			public Vector3 direction;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				decay = reader.ReadLESingle();
				duration = reader.ReadLESingle();
				deltaV = reader.ReadLESingle();
				start = reader.ReadLESingle();
				decayType = (DecayType)reader.ReadLEUInt32();
				position = reader.ReadLEVector3();
				direction = reader.ReadLEVector3();
			}
		}
		public class NiParticleColorModifier : NiParticleModifier
		{
			public Ref<NiColorData> colorData;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				colorData = NiReaderUtils.ReadRef<NiColorData>(reader);
			}
		}
		public class NiParticleGrowFade : NiParticleModifier
		{
			public float grow;
			public float fade;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				grow = reader.ReadLESingle();
				fade = reader.ReadLESingle();
			}
		}
		public class NiParticleMeshModifier : NiParticleModifier
		{
			public uint numParticleMeshes;
			public Ref<NiAVObject>[] particleMeshes;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				numParticleMeshes = reader.ReadLEUInt32();

				particleMeshes = new Ref<NiAVObject>[numParticleMeshes];
				for(int i = 0; i < particleMeshes.Length; i++)
				{
					particleMeshes[i] = NiReaderUtils.ReadRef<NiAVObject>(reader);
				}
			}
		}
		public class NiParticleRotation : NiParticleModifier
		{
			public byte randomInitialAxis;
			public Vector3 initialAxis;
			public float rotationSpeed;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				randomInitialAxis = reader.ReadByte();
				initialAxis = reader.ReadLEVector3();
				rotationSpeed = reader.ReadLESingle();
			}
		}

		// Controllers
		public abstract class NiTimeController : NiObject
		{
			public Ref<NiTimeController> nextController;
			public ushort flags;
			public float frequency;
			public float phase;
			public float startTime;
			public float stopTime;
			public Ptr<NiObjectNET> target;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				nextController = NiReaderUtils.ReadRef<NiTimeController>(reader);
				flags = reader.ReadLEUInt16();
				frequency = reader.ReadLESingle();
				phase = reader.ReadLESingle();
				startTime = reader.ReadLESingle();
				stopTime = reader.ReadLESingle();
				target = NiReaderUtils.ReadPtr<NiObjectNET>(reader);
			}
		}
		public class NiUVController : NiTimeController
		{
			public ushort unknownShort;
			public Ref<NiUVData> data;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				unknownShort = reader.ReadLEUInt16();
				data = NiReaderUtils.ReadRef<NiUVData>(reader);
			}
		}
		public abstract class NiInterpController : NiTimeController { }
		public abstract class NiSingleInterpController : NiInterpController { }
		public class NiKeyframeController : NiSingleInterpController
		{
			public Ref<NiKeyframeData> data;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				data = NiReaderUtils.ReadRef<NiKeyframeData>(reader);
			}
		}
		public class NiGeomMorpherController : NiInterpController
		{
			public Ref<NiMorphData> data;
			public byte alwaysUpdate;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				data = NiReaderUtils.ReadRef<NiMorphData>(reader);
				alwaysUpdate = reader.ReadByte();
			}
		}
		public abstract class NiBoolInterpController : NiSingleInterpController { }
		public class NiVisController : NiBoolInterpController
		{
			public Ref<NiVisData> data;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				data = NiReaderUtils.ReadRef<NiVisData>(reader);
			}
		}
		public abstract class NiFloatInterpController : NiSingleInterpController { }
		public class NiAlphaController : NiFloatInterpController
		{
			public Ref<NiFloatData> data;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				data = NiReaderUtils.ReadRef<NiFloatData>(reader);
			}
		}

		// Skin Stuff
		public class NiSkinInstance : NiObject
		{
			public Ref<NiSkinData> data;
			public Ptr<NiNode> skeletonRoot;
			public uint numBones;
			public Ptr<NiNode>[] bones;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				data = NiReaderUtils.ReadRef<NiSkinData>(reader);
				skeletonRoot = NiReaderUtils.ReadPtr<NiNode>(reader);
				numBones = reader.ReadLEUInt32();

				bones = new Ptr<NiNode>[numBones];
				for(int i = 0; i < bones.Length; i++)
				{
					bones[i] = NiReaderUtils.ReadPtr<NiNode>(reader);
				}
			}
		}
		public class NiSkinData : NiObject
		{
			public SkinTransform skinTransform;
			public uint numBones;
			public Ref<NiSkinPartition> skinPartition;
			public SkinData[] boneList;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				skinTransform = new SkinTransform();
				skinTransform.Deserialize(reader);

				numBones = reader.ReadLEUInt32();

				skinPartition = NiReaderUtils.ReadRef<NiSkinPartition>(reader);

				boneList = new SkinData[numBones];
				for(int i = 0; i < boneList.Length; i++)
				{
					boneList[i] = new SkinData();
					boneList[i].Deserialize(reader);
				}
			}
		}
		public class NiSkinPartition : NiObject { }

		// Miscellaneous
		public abstract class NiTexture : NiObjectNET
		{
			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);
			}
		}
		public class NiSourceTexture : NiTexture
		{
			public byte useExternal;
			public string fileName;
			public PixelLayout pixelLayout;
			public MipMapFormat useMipMaps;
			public AlphaFormat alphaFormat;
			public byte isStatic;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				useExternal = reader.ReadByte();
				fileName = reader.ReadLELength32PrefixedASCIIString();
				pixelLayout = (PixelLayout)reader.ReadLEUInt32();
				useMipMaps = (MipMapFormat)reader.ReadLEUInt32();
				alphaFormat = (AlphaFormat)reader.ReadLEUInt32();
				isStatic = reader.ReadByte();
			}
		}

        public abstract class NiPoint3InterpController : NiSingleInterpController
        {
            public Ref<NiPosData> data;

            public override void Deserialize(UnityBinaryReader reader)
            {
                base.Deserialize(reader);

                data = NiReaderUtils.ReadRef<NiPosData>(reader);
            }
        }

        public class NiMaterialProperty : NiProperty
		{
			public ushort flags;
			public Color3 ambientColor;
			public Color3 diffuseColor;
			public Color3 specularColor;
			public Color3 emissiveColor;
			public float glossiness;
			public float alpha;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				flags = NiReaderUtils.ReadFlags(reader);

				ambientColor = new Color3();
				ambientColor.Deserialize(reader);

				diffuseColor = new Color3();
				diffuseColor.Deserialize(reader);

				specularColor = new Color3();
				specularColor.Deserialize(reader);

				emissiveColor = new Color3();
				emissiveColor.Deserialize(reader);

				glossiness = reader.ReadLESingle();
				alpha = reader.ReadLESingle();
			}
		}

        public class NiMaterialColorController : NiPoint3InterpController {}
        
        public abstract class NiDynamicEffect : NiAVObject
		{
			uint numAffectedNodeListPointers;
			uint[] affectedNodeListPointers;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				numAffectedNodeListPointers = reader.ReadLEUInt32();

				affectedNodeListPointers = new uint[numAffectedNodeListPointers];
				for(int i = 0; i < affectedNodeListPointers.Length; i++)
				{
					affectedNodeListPointers[i] = reader.ReadLEUInt32();
				}
			}
		}
		public class NiTextureEffect : NiDynamicEffect
		{
			public Matrix4x4 modelProjectionMatrix;
			public Vector3 modelProjectionTransform;
			public TexFilterMode textureFiltering;
			public TexClampMode textureClamping;
			public EffectType textureType;
			public CoordGenType coordinateGenerationType;
			public Ref<NiSourceTexture> sourceTexture;
			public byte clippingPlane;
			public Vector3 unknownVector;
			public float unknownFloat;
			public short PS2L;
			public short PS2K;
			public ushort unknownShort;

			public override void Deserialize(UnityBinaryReader reader)
			{
				base.Deserialize(reader);

				modelProjectionMatrix = NiReaderUtils.Read3x3RotationMatrix(reader);
				modelProjectionTransform = reader.ReadLEVector3();
				textureFiltering = (TexFilterMode)reader.ReadLEUInt32();
				textureClamping = (TexClampMode)reader.ReadLEUInt32();
				textureType = (EffectType)reader.ReadLEUInt32();
				coordinateGenerationType = (CoordGenType)reader.ReadLEUInt32();
				sourceTexture = NiReaderUtils.ReadRef<NiSourceTexture>(reader);
				clippingPlane = reader.ReadByte();
				unknownVector = reader.ReadLEVector3();
				unknownFloat = reader.ReadLESingle();
				PS2L = reader.ReadLEInt16();
				PS2K = reader.ReadLEInt16();
				unknownShort = reader.ReadLEUInt16();
			}
		}
        #endregion










    }
}