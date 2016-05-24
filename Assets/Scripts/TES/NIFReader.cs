using System;
using System.IO;
using UnityEngine;

namespace TESUnity
{
	namespace NIF
	{
		// Refers to an object before the current one in the hierarchy.
		public struct Ptr<T>
		{
			public int value;
			public bool isNull
			{
				get
				{
					return value < 0;
				}
			}

			public void Deserialize(BinaryReader reader)
			{
				value = reader.ReadInt32();
			}
		}

		// Refers to an object after the current one in the hierarchy.
		public struct Ref<T>
		{
			public int value;
			public bool isNull
			{
				get
				{
					return value < 0;
				}
			}

			public void Deserialize(BinaryReader reader)
			{
				value = reader.ReadInt32();
			}
		}

		public class NiReaderUtils
		{
			public static Ptr<T> ReadPtr<T>(BinaryReader reader)
			{
				var ptr = new Ptr<T>();
				ptr.Deserialize(reader);

				return ptr;
			}
			public static Ref<T> ReadRef<T>(BinaryReader reader)
			{
				var readRef = new Ref<T>();
				readRef.Deserialize(reader);

				return readRef;
			}
			public static Ref<T>[] ReadLengthPrefixedRefs32<T>(BinaryReader reader)
			{
				var refs = new Ref<T>[reader.ReadUInt32()];

				for(int i = 0; i < refs.Length; i++)
				{
					refs[i] = ReadRef<T>(reader);
				}

				return refs;
			}
			public static ushort ReadFlags(BinaryReader reader)
			{
				return reader.ReadUInt16();
			}
			public static T Read<T>(BinaryReader reader)
			{
				if(typeof(T) == typeof(float))
				{
					return (T)((object)reader.ReadSingle());
				}
				else if(typeof(T) == typeof(byte))
				{
					return (T)((object)reader.ReadByte());
				}
				else if(typeof(T) == typeof(string))
				{
					return (T)((object)BinaryReaderExtensions.ReadLength32PrefixedASCIIString(reader));
				}
				else if(typeof(T) == typeof(Vector3))
				{
					return (T)((object)BinaryReaderExtensions.ReadVector3(reader));
				}
				else if(typeof(T) == typeof(Quaternion))
				{
					return (T)((object)BinaryReaderExtensions.ReadQuaternionWFirst(reader));
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
			public static NiObject ReadNiObject(BinaryReader reader)
			{
				var nodeTypeBytes = BinaryReaderExtensions.ReadLength32PrefixedBytes(reader);

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
				else if(StringUtils.Equals(nodeTypeBytes, "NiParticleSystemController"))
				{
					var controller = new NiParticleSystemController();
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
				else
				{
					throw new NotImplementedException("Tried to read an unsupported NiObject type (" + System.Text.Encoding.ASCII.GetString(nodeTypeBytes) + ").");
				}
			}
			public static Matrix4x4 Read3x3RotationMatrix(BinaryReader reader)
			{
				return BinaryReaderExtensions.ReadRowMajorMatrix3x3(reader);
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

			public void Deserialize(BinaryReader reader)
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

		#region Enums
		// texture enums
		public enum ApplyMode : uint
		{
			APPLY_REPLACE = 0,
			APPLY_DECAL = 1,
			APPLY_MODULATE = 2,
			APPLY_HILIGHT = 3,
			APPLY_HILIGHT2 = 4
		}
		public enum TexClampMode : uint
		{
			CLAMP_S_CLAMP_T = 0,
			CLAMP_S_WRAP_T = 1,
			WRAP_S_CLAMP_T = 2,
			WRAP_S_WRAP_T = 3
		}
		public enum TexFilterMode : uint
		{
			FILTER_NEAREST = 0,
			FILTER_BILERP = 1,
			FILTER_TRILERP = 2,
			FILTER_NEAREST_MIPNEAREST = 3,
			FILTER_NEAREST_MIPLERP = 4,
			FILTER_BILERP_MIPNEAREST = 5
		}
		public enum PixelLayout : uint
		{
			PIX_LAY_PALETTISED = 0,
			PIX_LAY_HIGH_COLOR_16 = 1,
			PIX_LAY_TRUE_COLOR_32 = 2,
			PIX_LAY_COMPRESSED = 3,
			PIX_LAY_BUMPMAP = 4,
			PIX_LAY_PALETTISED_4 = 5,
			PIX_LAY_DEFAULT = 6
		}
		public enum MipMapFormat : uint
		{
			MIP_FMT_NO = 0,
			MIP_FMT_YES = 1,
			MIP_FMT_DEFAULT = 2
		}
		public enum AlphaFormat : uint
		{
			ALPHA_NONE = 0,
			ALPHA_BINARY = 1,
			ALPHA_SMOOTH = 2,
			ALPHA_DEFAULT = 3
		}

		// miscellaneous
		public enum VertMode : uint
		{
			VERT_MODE_SRC_IGNORE = 0,
			VERT_MODE_SRC_EMISSIVE = 1,
			VERT_MODE_SRC_AMB_DIF = 2
		}
		public enum LightMode : uint
		{
			LIGHT_MODE_EMISSIVE = 0,
			LIGHT_MODE_EMI_AMB_DIF = 1
		}
		public enum KeyType : uint
		{
			LINEAR_KEY = 1,
			QUADRATIC_KEY = 2,
			TBC_KEY = 3,
			XYZ_ROTATION_KEY = 4,
			CONST_KEY = 5
		}
		public enum EffectType : uint
		{
			EFFECT_PROJECTED_LIGHT = 0,
			EFFECT_PROJECTED_SHADOW = 1,
			EFFECT_ENVIRONMENT_MAP = 2,
			EFFECT_FOG_MAP = 3
		}
		public enum CoordGenType : uint
		{
			CG_WORLD_PARALLEL = 0,
			CG_WORLD_PERSPECTIVE = 1,
			CG_SPHERE_MAP = 2,
			CG_SPECULAR_CUBE_MAP = 3,
			CG_DIFFUSE_CUBE_MAP = 4
		}
		public enum FieldType : uint
		{
			FIELD_WIND = 0,
			FIELD_POINT = 1
		}
		public enum DecayType : uint
		{
			DECAY_NONE = 0,
			DECAY_LINEAR = 1,
			DECAY_EXPONENTIAL = 2
		}
		#endregion // Enums

		#region Misc Classes
		public class BoundingBox
		{
			public uint unknownInt;
			public Vector3 translation;
			public Matrix4x4 rotation;
			public Vector3 radius;

			public void Deserialize(BinaryReader reader)
			{
				unknownInt = reader.ReadUInt32();
				translation = BinaryReaderExtensions.ReadVector3(reader);
				rotation = NiReaderUtils.Read3x3RotationMatrix(reader);
				radius = BinaryReaderExtensions.ReadVector3(reader);
			}
		}
		
		public class Color3
		{
			public float r;
			public float g;
			public float b;

			public void Deserialize(BinaryReader reader)
			{
				r = reader.ReadSingle();
				g = reader.ReadSingle();
				b = reader.ReadSingle();
			}
		}
		public class Color4
		{
			public float r;
			public float g;
			public float b;
			public float a;

			public void Deserialize(BinaryReader reader)
			{
				r = reader.ReadSingle();
				g = reader.ReadSingle();
				b = reader.ReadSingle();
				a = reader.ReadSingle();
			}
		}

		public class TexDesc
		{
			public Ref<NiSourceTexture> source;
			public TexClampMode clampMode;
			public TexFilterMode filterMode;
			public uint UVSet;
			public short PS2L;
			public short PS2K;
			public ushort unknown1;

			public void Deserialize(BinaryReader reader)
			{
				source = NiReaderUtils.ReadRef<NiSourceTexture>(reader);
				clampMode = (TexClampMode)reader.ReadUInt32();
				filterMode = (TexFilterMode)reader.ReadUInt32();
				UVSet = reader.ReadUInt32();
				PS2L = reader.ReadInt16();
				PS2K = reader.ReadInt16();
				unknown1 = reader.ReadUInt16();
			}
		}
		public class TexCoord
		{
			public float u;
			public float v;

			public void Deserialize(BinaryReader reader)
			{
				u = reader.ReadSingle();
				v = reader.ReadSingle();
			}
		}

		public class Triangle
		{
			public ushort v1;
			public ushort v2;
			public ushort v3;

			public void Deserialize(BinaryReader reader)
			{
				v1 = reader.ReadUInt16();
				v2 = reader.ReadUInt16();
				v3 = reader.ReadUInt16();
			}
		}

		public class MatchGroup
		{
			public ushort numVertices;
			public ushort[] vertexIndices;

			public void Deserialize(BinaryReader reader)
			{
				numVertices = reader.ReadUInt16();

				vertexIndices = new ushort[numVertices];
				for(int i = 0; i < vertexIndices.Length; i++)
				{
					vertexIndices[i] = reader.ReadUInt16();
				}
			}
		}
		
		public class TBC
		{
			public float t;
			public float b;
			public float c;

			public void Deserialize(BinaryReader reader)
			{
				t = reader.ReadSingle();
				b = reader.ReadSingle();
				c = reader.ReadSingle();
			}
		}

		public class Key<T>
		{
			public float time;
			public T value;
			public T forward;
			public T backward;
			public TBC TBC;

			public void Deserialize(BinaryReader reader, KeyType keyType)
			{
				time = reader.ReadSingle();
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
		public class KeyGroup<T>
		{
			public uint numKeys;
			public KeyType interpolation;
			public Key<T>[] keys;

			public void Deserialize(BinaryReader reader)
			{
				numKeys = reader.ReadUInt32();

				if(numKeys != 0)
				{
					interpolation = (KeyType)reader.ReadUInt32();
				}

				keys = new Key<T>[numKeys];
				for(int i = 0; i < keys.Length; i++)
				{
					keys[i] = new Key<T>();
					keys[i].Deserialize(reader, interpolation);
				}
			}
		}
		public class QuatKey<T>
		{
			public float time;
			public T value;
			public TBC TBC;

			public void Deserialize(BinaryReader reader, KeyType keyType)
			{
				time = reader.ReadSingle();

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

		public class SkinData
		{
			public SkinTransform skinTransform;
			public Vector3 boundingSphereOffset;
			public float boundingSphereRadius;
			public ushort numVertices;
			public SkinWeight[] vertexWeights;

			public void Deserialize(BinaryReader reader)
			{
				skinTransform = new SkinTransform();
				skinTransform.Deserialize(reader);

				boundingSphereOffset = BinaryReaderExtensions.ReadVector3(reader);
				boundingSphereRadius = reader.ReadSingle();
				numVertices = reader.ReadUInt16();

				vertexWeights = new SkinWeight[numVertices];
				for(int i = 0; i < vertexWeights.Length; i++)
				{
					vertexWeights[i] = new SkinWeight();
					vertexWeights[i].Deserialize(reader);
				}
			}
		}
		public class SkinWeight
		{
			public ushort index;
			public float weight;

			public void Deserialize(BinaryReader reader)
			{
				index = reader.ReadUInt16();
				weight = reader.ReadSingle();
			}
		}
		public class SkinTransform
		{
			public Matrix4x4 rotation;
			public Vector3 translation;
			public float scale;

			public void Deserialize(BinaryReader reader)
			{
				rotation = NiReaderUtils.Read3x3RotationMatrix(reader);
				translation = BinaryReaderExtensions.ReadVector3(reader);
				scale = reader.ReadSingle();
			}
		}

		public class Particle
		{
			public Vector3 velocity;
			public Vector3 unknownVector;
			public float lifetime;
			public float lifespan;
			public float timestamp;
			public ushort unknownShort;
			public ushort vertexID;

			public void Deserialize(BinaryReader reader)
			{
				velocity = BinaryReaderExtensions.ReadVector3(reader);
				unknownVector = BinaryReaderExtensions.ReadVector3(reader);
				lifetime = reader.ReadSingle();
				lifespan = reader.ReadSingle();
				timestamp = reader.ReadSingle();
				unknownShort = reader.ReadUInt16();
				vertexID = reader.ReadUInt16();
			}
		}

		public class Morph
		{
			public uint numKeys;
			public KeyType interpolation;
			public Key<float>[] keys;
			public Vector3[] vectors;

			public void Deserialize(BinaryReader reader, uint numVertices)
			{
				numKeys = reader.ReadUInt32();
				interpolation = (KeyType)reader.ReadUInt32();

				keys = new Key<float>[numKeys];
				for(int i = 0; i < keys.Length; i++)
				{
					keys[i] = new Key<float>();
					keys[i].Deserialize(reader, interpolation);
				}

				vectors = new Vector3[numVertices];
				for(int i = 0; i < vectors.Length; i++)
				{
					vectors[i] = BinaryReaderExtensions.ReadVector3(reader);
				}
			}
		}
		#endregion

		public class NiHeader
		{
			public byte[] str; // 40 bytes (including \n)
			public uint version;
			public uint numBlocks;

			public void Deserialize(BinaryReader reader)
			{
				str = reader.ReadBytes(40);
				version = reader.ReadUInt32();
				numBlocks = reader.ReadUInt32();
			}
		}
		public class NiFooter
		{
			public uint numRoots;
			public int[] roots;

			public void Deserialize(BinaryReader reader)
			{
				numRoots = reader.ReadUInt32();

				roots = new int[numRoots];
				for(int i = 0; i < numRoots; i++)
				{
					roots[i] = reader.ReadInt32();
				}
			}
		}

		public abstract class NiObject
		{
			public virtual void Deserialize(BinaryReader reader) {}
		}
		public abstract class NiObjectNET : NiObject
		{
			public byte[] name;
			public Ref<NiExtraData> extraData;
			public Ref<NiTimeController> controller;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				name = BinaryReaderExtensions.ReadLength32PrefixedBytes(reader);
				extraData = NiReaderUtils.ReadRef<NiExtraData>(reader);
				controller = NiReaderUtils.ReadRef<NiTimeController>(reader);
			}
		}
		public abstract class NiAVObject : NiObjectNET
		{
			public ushort flags;
			public Vector3 translation;
			public Matrix4x4 rotation;
			public float scale;
			public Vector3 velocity;
			//public uint numProperties;
			public Ref<NiProperty>[] properties;
			public bool hasBoundingBox;
			public BoundingBox boundingBox;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				flags = NiReaderUtils.ReadFlags(reader);
				translation = BinaryReaderExtensions.ReadVector3(reader);
				rotation = NiReaderUtils.Read3x3RotationMatrix(reader);
				scale = reader.ReadSingle();
				velocity = BinaryReaderExtensions.ReadVector3(reader);
				properties = NiReaderUtils.ReadLengthPrefixedRefs32<NiProperty>(reader);
				hasBoundingBox = BinaryReaderExtensions.ReadBool32(reader);

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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				children = NiReaderUtils.ReadLengthPrefixedRefs32<NiAVObject>(reader);
				effects = NiReaderUtils.ReadLengthPrefixedRefs32<NiDynamicEffect>(reader);
			}
		}
		public class RootCollisionNode : NiNode { }
		public class NiBSAnimationNode : NiNode { }
		public class NiBSParticleNode : NiNode { }
		public class AvoidNode : NiNode { }

		// Geometry
		public abstract class NiGeometry : NiAVObject
		{
			public Ref<NiGeometryData> data;
			public Ref<NiSkinInstance> skinInstance;

			public override void Deserialize(BinaryReader reader)
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				numVertices = reader.ReadUInt16();
				hasVertices = BinaryReaderExtensions.ReadBool32(reader);

				if(hasVertices)
				{
					vertices = new Vector3[numVertices];
					for(int i = 0; i < vertices.Length; i++)
					{
						vertices[i] = BinaryReaderExtensions.ReadVector3(reader);
					}
				}

				hasNormals = BinaryReaderExtensions.ReadBool32(reader);

				if(hasNormals)
				{
					normals = new Vector3[numVertices];
					for(int i = 0; i < normals.Length; i++)
					{
						normals[i] = BinaryReaderExtensions.ReadVector3(reader);
					}
				}

				center = BinaryReaderExtensions.ReadVector3(reader);
				radius = reader.ReadSingle();
				hasVertexColors = BinaryReaderExtensions.ReadBool32(reader);

				if(hasVertexColors)
				{
					vertexColors = new Color4[numVertices];
					for(int i = 0; i < vertexColors.Length; i++)
					{
						vertexColors[i] = new Color4();
						vertexColors[i].Deserialize(reader);
					}
				}

				numUVSets = reader.ReadUInt16();
				hasUV = BinaryReaderExtensions.ReadBool32(reader);

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
			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);
			}
		}
		public abstract class NiTriBasedGeomData : NiGeometryData
		{
			public ushort numTriangles;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				numTriangles = reader.ReadUInt16();
			}
		}
		public class NiTriShape : NiTriBasedGeom
		{
			public override void Deserialize(BinaryReader reader)
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				numTrianglePoints = reader.ReadUInt32();

				triangles = new Triangle[numTriangles];
				for(int i = 0; i < triangles.Length; i++)
				{
					triangles[i] = new Triangle();
					triangles[i].Deserialize(reader);
				}

				numMatchGroups = reader.ReadUInt16();

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
			public override void Deserialize(BinaryReader reader)
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				flags = NiReaderUtils.ReadFlags(reader);

				applyMode = (ApplyMode)reader.ReadUInt32();
				textureCount = reader.ReadUInt32();

				hasBaseTexture = BinaryReaderExtensions.ReadBool32(reader);
				if(hasBaseTexture)
				{
					baseTexture = new TexDesc();
					baseTexture.Deserialize(reader);
				}

				hasDarkTexture = BinaryReaderExtensions.ReadBool32(reader);
				if(hasDarkTexture)
				{
					darkTexture = new TexDesc();
					darkTexture.Deserialize(reader);
				}

				hasDetailTexture = BinaryReaderExtensions.ReadBool32(reader);
				if(hasDetailTexture)
				{
					detailTexture = new TexDesc();
					detailTexture.Deserialize(reader);
				}

				hasGlossTexture = BinaryReaderExtensions.ReadBool32(reader);
				if(hasGlossTexture)
				{
					glossTexture = new TexDesc();
					glossTexture.Deserialize(reader);
				}

				hasGlowTexture = BinaryReaderExtensions.ReadBool32(reader);
				if(hasGlowTexture)
				{
					glowTexture = new TexDesc();
					glowTexture.Deserialize(reader);
				}

				hasBumpMapTexture = BinaryReaderExtensions.ReadBool32(reader);
				if(hasBumpMapTexture)
				{
					bumpMapTexture = new TexDesc();
					bumpMapTexture.Deserialize(reader);
				}

				hasDecal0Texture = BinaryReaderExtensions.ReadBool32(reader);
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				flags = reader.ReadUInt16();
				threshold = reader.ReadByte();
			}
		}
		public class NiZBufferProperty : NiProperty
		{
			public ushort flags;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				flags = reader.ReadUInt16();
			}
		}
		public class NiVertexColorProperty : NiProperty
		{
			public ushort flags;
			public VertMode vertexMode;
			public LightMode lightingMode;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				flags = reader.ReadUInt16();
				vertexMode = (VertMode)reader.ReadUInt32();
				lightingMode = (LightMode)reader.ReadUInt32();
			}
		}

		// Data
		public class NiUVData : NiObject
		{
			public KeyGroup<float>[] UVGroups;

			public override void Deserialize(BinaryReader reader)
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				numRotationKeys = reader.ReadUInt32();

				if(numRotationKeys != 0)
				{
					rotationType = (KeyType)reader.ReadUInt32();

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
						unknownFloat = reader.ReadSingle();

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

			public override void Deserialize(BinaryReader reader)
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				numMorphs = reader.ReadUInt32();
				numVertices = reader.ReadUInt32();
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				numKeys = reader.ReadUInt32();

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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				data = new KeyGroup<float>();
				data.Deserialize(reader);
			}
		}

		public class NiExtraData : NiObject
		{
			public Ref<NiExtraData> nextExtraData;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				nextExtraData = NiReaderUtils.ReadRef<NiExtraData>(reader);
			}
		}
		public class NiStringExtraData : NiExtraData
		{
			public uint bytesRemaining;
			public string stringData;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				bytesRemaining = reader.ReadUInt32();
				stringData = BinaryReaderExtensions.ReadLength32PrefixedASCIIString(reader);
			}
		}
		public class NiTextKeyExtraData : NiExtraData
		{
			uint unknownInt1;
			uint numTextKeys;
			Key<string>[] textKeys;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				unknownInt1 = reader.ReadUInt32();
				numTextKeys = reader.ReadUInt32();

				textKeys = new Key<string>[numTextKeys];
				for(int i = 0; i < textKeys.Length; i++)
				{
					textKeys[i] = new Key<string>();
					textKeys[i].Deserialize(reader, KeyType.LINEAR_KEY);
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				numParticles = reader.ReadUInt16();
				particleRadius = reader.ReadSingle();
				numActive = reader.ReadUInt16();

				hasSizes = BinaryReaderExtensions.ReadBool32(reader);
				if(hasSizes)
				{
					sizes = new float[numVertices];
					for(int i = 0; i < sizes.Length; i++)
					{
						sizes[i] = reader.ReadSingle();
					}
				}
			}
		}
		public class NiRotatingParticles : NiParticles { }
		public class NiRotatingParticlesData : NiParticlesData
		{
			public bool hasRotations;
			public Quaternion[] rotations;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				hasRotations = BinaryReaderExtensions.ReadBool32(reader);

				if(hasRotations)
				{
					rotations = new Quaternion[numVertices];
					for(int i = 0; i < rotations.Length; i++)
					{
						rotations[i] = BinaryReaderExtensions.ReadQuaternionWFirst(reader);
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				speed = reader.ReadSingle();
				speedRandom = reader.ReadSingle();
				verticalDirection = reader.ReadSingle();
				verticalAngle = reader.ReadSingle();
				horizontalDirection = reader.ReadSingle();
				horizontalAngle = reader.ReadSingle();
				unknownNormal = BinaryReaderExtensions.ReadVector3(reader);

				unknownColor = new Color4();
				unknownColor.Deserialize(reader);

				size = reader.ReadSingle();
				emitStartTime = reader.ReadSingle();
				emitStopTime = reader.ReadSingle();
				unknownByte = reader.ReadByte();
				emitRate = reader.ReadSingle();
				lifetime = reader.ReadSingle();
				lifetimeRandom = reader.ReadSingle();
				emitFlags = reader.ReadUInt16();
				startRandom = BinaryReaderExtensions.ReadVector3(reader);
				emitter = NiReaderUtils.ReadPtr<NiObject>(reader);
				unknownShort2 = reader.ReadUInt16();
				unknownFloat13 = reader.ReadSingle();
				unknownInt1 = reader.ReadUInt32();
				unknownInt2 = reader.ReadUInt32();
				unknownShort3 = reader.ReadUInt16();
				numParticles = reader.ReadUInt16();
				numValid = reader.ReadUInt16();

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

		// Particle Modifiers
		public abstract class NiParticleModifier : NiObject
		{
			public Ref<NiParticleModifier> nextModifier;
			public Ptr<NiParticleSystemController> controller;

			public override void Deserialize(BinaryReader reader)
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				unknownFloat1 = reader.ReadSingle();
				force = reader.ReadSingle();
				type = (FieldType)reader.ReadUInt32();
				position = BinaryReaderExtensions.ReadVector3(reader);
				direction = BinaryReaderExtensions.ReadVector3(reader);
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				decay = reader.ReadSingle();
				duration = reader.ReadSingle();
				deltaV = reader.ReadSingle();
				start = reader.ReadSingle();
				decayType = (DecayType)reader.ReadUInt32();
				position = BinaryReaderExtensions.ReadVector3(reader);
				direction = BinaryReaderExtensions.ReadVector3(reader);
			}
		}
		public class NiParticleColorModifier : NiParticleModifier
		{
			public Ref<NiColorData> colorData;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				colorData = NiReaderUtils.ReadRef<NiColorData>(reader);
			}
		}
		public class NiParticleGrowFade : NiParticleModifier
		{
			public float grow;
			public float fade;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				grow = reader.ReadSingle();
				fade = reader.ReadSingle();
			}
		}
		public class NiParticleMeshModifier : NiParticleModifier
		{
			public uint numParticleMeshes;
			public Ref<NiAVObject>[] particleMeshes;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				numParticleMeshes = reader.ReadUInt32();

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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				randomInitialAxis = reader.ReadByte();
				initialAxis = BinaryReaderExtensions.ReadVector3(reader);
				rotationSpeed = reader.ReadSingle();
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				nextController = NiReaderUtils.ReadRef<NiTimeController>(reader);
				flags = reader.ReadUInt16();
				frequency = reader.ReadSingle();
				phase = reader.ReadSingle();
				startTime = reader.ReadSingle();
				stopTime = reader.ReadSingle();
				target = NiReaderUtils.ReadPtr<NiObjectNET>(reader);
			}
		}
		public class NiUVController : NiTimeController
		{
			public ushort unknownShort;
			public Ref<NiUVData> data;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				unknownShort = reader.ReadUInt16();
				data = NiReaderUtils.ReadRef<NiUVData>(reader);
			}
		}
		public abstract class NiInterpController : NiTimeController { }
		public abstract class NiSingleInterpController : NiInterpController { }
		public class NiKeyframeController : NiSingleInterpController
		{
			public Ref<NiKeyframeData> data;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				data = NiReaderUtils.ReadRef<NiKeyframeData>(reader);
			}
		}
		public class NiGeomMorpherController : NiInterpController
		{
			public Ref<NiMorphData> data;
			public byte alwaysUpdate;

			public override void Deserialize(BinaryReader reader)
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				data = NiReaderUtils.ReadRef<NiVisData>(reader);
			}
		}
		public abstract class NiFloatInterpController : NiSingleInterpController { }
		public class NiAlphaController : NiFloatInterpController
		{
			public Ref<NiFloatData> data;

			public override void Deserialize(BinaryReader reader)
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				data = NiReaderUtils.ReadRef<NiSkinData>(reader);
				skeletonRoot = NiReaderUtils.ReadPtr<NiNode>(reader);
				numBones = reader.ReadUInt32();

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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				skinTransform = new SkinTransform();
				skinTransform.Deserialize(reader);

				numBones = reader.ReadUInt32();

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
			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);
			}
		}
		public class NiSourceTexture : NiTexture
		{
			public byte useExternal;
			public byte[] fileName;
			public PixelLayout pixelLayout;
			public MipMapFormat useMipMaps;
			public AlphaFormat alphaFormat;
			public byte isStatic;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				useExternal = reader.ReadByte();
				fileName = BinaryReaderExtensions.ReadLength32PrefixedBytes(reader);
				pixelLayout = (PixelLayout)reader.ReadUInt32();
				useMipMaps = (MipMapFormat)reader.ReadUInt32();
				alphaFormat = (AlphaFormat)reader.ReadUInt32();
				isStatic = reader.ReadByte();
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

			public override void Deserialize(BinaryReader reader)
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

				glossiness = reader.ReadSingle();
				alpha = reader.ReadSingle();
			}
		}

		public abstract class NiDynamicEffect : NiAVObject
		{
			uint numAffectedNodeListPointers;
			uint[] affectedNodeListPointers;

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				numAffectedNodeListPointers = reader.ReadUInt32();

				affectedNodeListPointers = new uint[numAffectedNodeListPointers];
				for(int i = 0; i < affectedNodeListPointers.Length; i++)
				{
					affectedNodeListPointers[i] = reader.ReadUInt32();
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

			public override void Deserialize(BinaryReader reader)
			{
				base.Deserialize(reader);

				modelProjectionMatrix = NiReaderUtils.Read3x3RotationMatrix(reader);
				modelProjectionTransform = BinaryReaderExtensions.ReadVector3(reader);
				textureFiltering = (TexFilterMode)reader.ReadUInt32();
				textureClamping = (TexClampMode)reader.ReadUInt32();
				textureType = (EffectType)reader.ReadUInt32();
				coordinateGenerationType = (CoordGenType)reader.ReadUInt32();
				sourceTexture = NiReaderUtils.ReadRef<NiSourceTexture>(reader);
				clippingPlane = reader.ReadByte();
				unknownVector = BinaryReaderExtensions.ReadVector3(reader);
				unknownFloat = reader.ReadSingle();
				PS2L = reader.ReadInt16();
				PS2K = reader.ReadInt16();
				unknownShort = reader.ReadUInt16();
			}
		}
	}
}