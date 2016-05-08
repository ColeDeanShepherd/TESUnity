using System;
using System.IO;
using UnityEngine;

namespace NIF
{
	public class NiUtils
	{
		public static int ReadRef(BinaryReader reader)
		{
			return reader.ReadInt32();
		}
		public static int ReadPtr(BinaryReader reader)
		{
			return reader.ReadInt32();
		}
		public static int[] ReadLengthPrefixedRefs32(BinaryReader reader)
		{
			var refs = new int[reader.ReadUInt32()];

			for(int i = 0; i < refs.Length; i++)
			{
				refs[i] = ReadRef(reader);
			}

			return refs;
		}
		public static ushort ReadFlags(BinaryReader reader)
		{
			return reader.ReadUInt16();
		}
		public static NiObject ReadNiObject(BinaryReader reader)
		{
			var nodeTypeBytes = BinaryReaderUtils.ReadLengthPrefixedBytes32(reader); // "NiNode"

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
			else if(StringUtils.Equals(nodeTypeBytes, "NiAutoNormalParticlesData"))
			{
				var data = new NiAutoNormalParticlesData();
				data.Deserialize(reader);

				return data;
			}
			else
			{
				throw new NotImplementedException("Tried to read an unsupported NiObject type (" + System.Text.Encoding.ASCII.GetString(nodeTypeBytes) + ").");
			}
		}
	}

	public class NiFile
	{
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
				blocks[i] = NiUtils.ReadNiObject(reader);
			}

			footer = new NiFooter();
			footer.Deserialize(reader);
		}
	}

	#region Enums

	// An unsigned 32-bit integer, describing the apply mode of a texture.
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
			translation = BinaryReaderUtils.ReadVector3(reader);
			rotation = BinaryReaderUtils.ReadMatrix3x3(reader);
			radius = BinaryReaderUtils.ReadVector3(reader);
		}
	}

	public class TexDesc
	{
		public int sourceRef;
		public TexClampMode clampMode;
		public TexFilterMode filterMode;
		public uint UVSet;
		public short PS2L;
		public short PS2K;
		public ushort unknown1;

		public void Deserialize(BinaryReader reader)
		{
			sourceRef = NiUtils.ReadRef(reader);
			clampMode = (TexClampMode)reader.ReadUInt32();
			filterMode = (TexFilterMode)reader.ReadUInt32();
			UVSet = reader.ReadUInt32();
			PS2L = reader.ReadInt16();
			PS2K = reader.ReadInt16();
			unknown1 = reader.ReadUInt16();
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

	public class SkinTransform
	{
		public Matrix4x4 rotation;
		public Vector3 translation;
		public float scale;

		public void Deserialize(BinaryReader reader)
		{
			rotation = BinaryReaderUtils.ReadMatrix3x3(reader);
			translation = BinaryReaderUtils.ReadVector3(reader);
			scale = reader.ReadSingle();
		}
	}

	#endregion // Misc Classes

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
		public int[] rootRefs;

		public void Deserialize(BinaryReader reader)
		{
			numRoots = reader.ReadUInt32();

			rootRefs = new int[numRoots];
			for(int i = 0; i < numRoots; i++)
			{
				rootRefs[i] = reader.ReadInt32();
			}
		}
	}

	public class NiObject
	{
		public virtual void Deserialize(BinaryReader reader)
		{
		}
	}

	public class NiObjectNET : NiObject
	{
		public byte[] name;
		public int extraDataRef;
		public int controllerRef;

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			name = BinaryReaderUtils.ReadLengthPrefixedBytes32(reader);
			extraDataRef = NiUtils.ReadRef(reader);
			controllerRef = NiUtils.ReadRef(reader);
		}
	}

	public class NiAVObject : NiObjectNET
	{
		public ushort flags;
		public Vector3 translation;
		public Matrix4x4 rotation;
		public float scale;
		public Vector3 velocity;
		//public uint numProperties;
		public int[] propertyRefs;
		public bool hasBoundingBox;
		public BoundingBox boundingBox;

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			flags = NiUtils.ReadFlags(reader);
			translation = BinaryReaderUtils.ReadVector3(reader);
			rotation = BinaryReaderUtils.ReadMatrix3x3(reader);
			scale = reader.ReadSingle();
			velocity = BinaryReaderUtils.ReadVector3(reader);
			propertyRefs = NiUtils.ReadLengthPrefixedRefs32(reader);
			hasBoundingBox = BinaryReaderUtils.ReadBool32(reader);

			if(hasBoundingBox)
			{
				boundingBox = new BoundingBox();
				boundingBox.Deserialize(reader);
			}
		}
	}

	public class NiNode : NiAVObject
	{
		//public uint numChildren;
		public int[] children;
		//public uint numEffects;
		public int[] effects;

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			children = NiUtils.ReadLengthPrefixedRefs32(reader);
			effects = NiUtils.ReadLengthPrefixedRefs32(reader);
		}
	}

	public class NiBSParticleNode : NiNode {}

	public class NiParticles : NiGeometry {}

	public class NiParticlesData : NiGeometryData
	{
		public ushort numParticles;
		public float particleRadius;
		public ushort numActive;
		public bool hasSizes;
		public float[] sizes;
		public byte unknownByte1;
		public int unknownLink;
		public float[] rotationAngles;
		public bool hasUVQuadrants;
		public byte numUVQuadrants;
		public Vector4[] UVQuadrants;
		public byte unknownByte2;

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			numParticles = reader.ReadUInt16();
			particleRadius = reader.ReadSingle();
			numActive = reader.ReadUInt16();
			hasSizes = BinaryReaderUtils.ReadBool32(reader);

			if(hasSizes)
			{
				sizes = new float[numVertices];
				for(int i = 0; i < sizes.Length; i++)
				{
					sizes[i] = reader.ReadSingle();
				}
			}

			unknownByte1 = reader.ReadByte();
			unknownLink = NiUtils.ReadRef(reader);

			rotationAngles = new float[numVertices];
			for(int i = 0; i < rotationAngles.Length; i++)
			{
				rotationAngles[i] = reader.ReadSingle();
			}

			hasUVQuadrants = BinaryReaderUtils.ReadBool32(reader);
			numUVQuadrants = reader.ReadByte();

			if(hasUVQuadrants)
			{
				UVQuadrants = new Vector4[numUVQuadrants];

				for(int i = 0; i < UVQuadrants.Length; i++)
				{
					UVQuadrants[i] = BinaryReaderUtils.ReadVector4(reader);
				}
			}

			unknownByte2 = reader.ReadByte();
		}
	}

	public class NiRotatingParticles : NiParticles {}

	public class NiRotatingParticlesData : NiParticlesData
	{
		public bool hasRotations;
		public Quaternion[] rotations;

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			hasRotations = BinaryReaderUtils.ReadBool32(reader);

			if(hasRotations)
			{
				rotations = new Quaternion[numVertices];
				for(int i = 0; i < rotations.Length; i++)
				{
					rotations[i] = BinaryReaderUtils.ReadQuaternionWFirst(reader);
				}
			}
		}
	}
	public class NiAutoNormalParticlesData : NiParticlesData {}

	public class RootCollisionNode : NiNode
	{
	}

	public class NiExtraData : NiObject
	{
		public int nextExtraDataRef;

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			nextExtraDataRef = NiUtils.ReadRef(reader);
		}
	}
	public class NiStringExtraData : NiExtraData
	{
		public uint bytesRemaining;
		public byte[] stringData;

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			bytesRemaining = reader.ReadUInt32();
			stringData = BinaryReaderUtils.ReadLengthPrefixedBytes32(reader);
		}
	}

	public class NiSkinInstance : NiObject
	{
		public int dataRef;
		public int skeletonRootPtr;
		public uint numBones;
		public int[] bonePtrs;

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			dataRef = NiUtils.ReadRef(reader);
			skeletonRootPtr = NiUtils.ReadPtr(reader);
			numBones = reader.ReadUInt32();

			bonePtrs = new int[numBones];
			for(int i = 0; i < bonePtrs.Length; i++)
			{
				bonePtrs[i] = NiUtils.ReadPtr(reader);
			}
		}
	}

	public class NiSkinData : NiObject
	{
		public SkinTransform skinTransform;
		public uint numBones;
		public int skinPartitionRef;
		public SkinData[] boneList;

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			skinTransform = new SkinTransform();
			skinTransform.Deserialize(reader);

			numBones = reader.ReadUInt32();

			skinPartitionRef = NiUtils.ReadRef(reader);

			boneList = new SkinData[numBones];
			for(int i = 0; i < boneList.Length; i++)
			{
				boneList[i] = new SkinData();
				boneList[i].Deserialize(reader);
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

			boundingSphereOffset = BinaryReaderUtils.ReadVector3(reader);
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

	// Geometry
	public class NiGeometry : NiAVObject
	{
		public int dataRef;
		public int skinInstanceRef;

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			dataRef = reader.ReadInt32();
			skinInstanceRef = reader.ReadInt32();
		}
	}

	public class NiTriBasedGeom : NiGeometry
	{
		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);
		}
	}

	public class NiTriShape : NiTriBasedGeom
	{
		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);
		}
	}

	public class NiGeometryData : NiObject
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
			hasVertices = BinaryReaderUtils.ReadBool32(reader);

			if(hasVertices)
			{
				vertices = new Vector3[numVertices];
				for(int i = 0; i < vertices.Length; i++)
				{
					vertices[i] = BinaryReaderUtils.ReadVector3(reader);
				}
			}

			hasNormals = BinaryReaderUtils.ReadBool32(reader);

			if(hasNormals)
			{
				normals = new Vector3[numVertices];
				for(int i = 0; i < normals.Length; i++)
				{
					normals[i] = BinaryReaderUtils.ReadVector3(reader);
				}
			}

			center = BinaryReaderUtils.ReadVector3(reader);
			radius = reader.ReadSingle();
			hasVertexColors = BinaryReaderUtils.ReadBool32(reader);

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
			hasUV = BinaryReaderUtils.ReadBool32(reader);

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

	public class NiTriBasedGeomData : NiGeometryData
	{
		public ushort numTriangles;

		public override void Deserialize(BinaryReader reader)
		{
			base.Deserialize(reader);

			numTriangles = reader.ReadUInt16();
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
	public class NiProperty : NiObjectNET
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

			flags = NiUtils.ReadFlags(reader);

			applyMode = (ApplyMode)reader.ReadUInt32();
			textureCount = reader.ReadUInt32();

			hasBaseTexture = BinaryReaderUtils.ReadBool32(reader);
			if(hasBaseTexture)
			{
				baseTexture = new TexDesc();
				baseTexture.Deserialize(reader);
			}

			hasDarkTexture = BinaryReaderUtils.ReadBool32(reader);
			if(hasDarkTexture)
			{
				darkTexture = new TexDesc();
				darkTexture.Deserialize(reader);
			}

			hasDetailTexture = BinaryReaderUtils.ReadBool32(reader);
			if(hasDetailTexture)
			{
				detailTexture = new TexDesc();
				detailTexture.Deserialize(reader);
			}

			hasGlossTexture = BinaryReaderUtils.ReadBool32(reader);
			if(hasGlossTexture)
			{
				glossTexture = new TexDesc();
				glossTexture.Deserialize(reader);
			}

			hasGlowTexture = BinaryReaderUtils.ReadBool32(reader);
			if(hasGlowTexture)
			{
				glowTexture = new TexDesc();
				glowTexture.Deserialize(reader);
			}

			hasBumpMapTexture = BinaryReaderUtils.ReadBool32(reader);
			if(hasBumpMapTexture)
			{
				bumpMapTexture = new TexDesc();
				bumpMapTexture.Deserialize(reader);
			}

			hasDecal0Texture = BinaryReaderUtils.ReadBool32(reader);
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

	public class NiTexture : NiObjectNET
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
			fileName = BinaryReaderUtils.ReadLengthPrefixedBytes32(reader);
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

			flags = NiUtils.ReadFlags(reader);

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
}