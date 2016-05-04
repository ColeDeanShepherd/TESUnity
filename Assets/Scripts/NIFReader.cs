using System;
using System.IO;
using UnityEngine;

namespace NIF
{
	public class NiUtils
	{
		public static byte[] ReadLengthPrefixedBytes32(BinaryReader reader)
		{
			var length = reader.ReadUInt32();
			return reader.ReadBytes((int)length);
		}
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
		public static Vector3 ReadVector3(BinaryReader reader)
		{
			float x = reader.ReadSingle();
			float y = reader.ReadSingle();
			float z = reader.ReadSingle();

			return new Vector3(x, y, z);
		}
		public static Matrix4x4 ReadMatrix3x3(BinaryReader reader)
		{
			var mat = new Matrix4x4();

			for(int j = 0; j < 4; j++)
			{
				for(int i = 0; i < 4; i++)
				{
					if(i < 3 && j < 3)
					{
						mat[i, j] = reader.ReadSingle();
					}
					else
					{
						mat[i, j] = (i == j) ? 1 : 0;
					}
				}
			}

			return mat;
		}
		public static bool ReadBool(BinaryReader reader)
		{
			return reader.ReadUInt32() != 0;
		}
		public static NiObject ReadNiObject(BinaryReader reader)
		{
			var nodeTypeBytes = ReadLengthPrefixedBytes32(reader); // "NiNode"

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

		public void Deserialize(BinaryReader reader)
		{
			header = new NiHeader();
			header.Deserialize(reader);

			blocks = new NiObject[header.numBlocks];
			for(int i = 0; i < header.numBlocks; i++)
			{
				blocks[i] = NiUtils.ReadNiObject(reader);
			}
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
			translation = NiUtils.ReadVector3(reader);
			rotation = NiUtils.ReadMatrix3x3(reader);
			radius = NiUtils.ReadVector3(reader);
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
			rotation = NiUtils.ReadMatrix3x3(reader);
			translation = NiUtils.ReadVector3(reader);
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

			name = NiUtils.ReadLengthPrefixedBytes32(reader);
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
			translation = NiUtils.ReadVector3(reader);
			rotation = NiUtils.ReadMatrix3x3(reader);
			scale = reader.ReadSingle();
			velocity = NiUtils.ReadVector3(reader);
			propertyRefs = NiUtils.ReadLengthPrefixedRefs32(reader);
			hasBoundingBox = NiUtils.ReadBool(reader);

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
			stringData = NiUtils.ReadLengthPrefixedBytes32(reader);
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

			boundingSphereOffset = NiUtils.ReadVector3(reader);
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
			hasVertices = NiUtils.ReadBool(reader);

			if(hasVertices)
			{
				vertices = new Vector3[numVertices];
				for(int i = 0; i < vertices.Length; i++)
				{
					vertices[i] = NiUtils.ReadVector3(reader);
				}
			}

			hasNormals = NiUtils.ReadBool(reader);

			if(hasNormals)
			{
				normals = new Vector3[numVertices];
				for(int i = 0; i < normals.Length; i++)
				{
					normals[i] = NiUtils.ReadVector3(reader);
				}
			}

			center = NiUtils.ReadVector3(reader);
			radius = reader.ReadSingle();
			hasVertexColors = NiUtils.ReadBool(reader);

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
			hasUV = NiUtils.ReadBool(reader);

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

			hasBaseTexture = NiUtils.ReadBool(reader);
			if(hasBaseTexture)
			{
				baseTexture = new TexDesc();
				baseTexture.Deserialize(reader);
			}

			hasDarkTexture = NiUtils.ReadBool(reader);
			if(hasDarkTexture)
			{
				darkTexture = new TexDesc();
				darkTexture.Deserialize(reader);
			}

			hasDetailTexture = NiUtils.ReadBool(reader);
			if(hasDetailTexture)
			{
				detailTexture = new TexDesc();
				detailTexture.Deserialize(reader);
			}

			hasGlossTexture = NiUtils.ReadBool(reader);
			if(hasGlossTexture)
			{
				glossTexture = new TexDesc();
				glossTexture.Deserialize(reader);
			}

			hasGlowTexture = NiUtils.ReadBool(reader);
			if(hasGlowTexture)
			{
				glowTexture = new TexDesc();
				glowTexture.Deserialize(reader);
			}

			hasBumpMapTexture = NiUtils.ReadBool(reader);
			if(hasBumpMapTexture)
			{
				bumpMapTexture = new TexDesc();
				bumpMapTexture.Deserialize(reader);
			}

			hasDecal0Texture = NiUtils.ReadBool(reader);
			if(hasDecal0Texture)
			{
				decal0Texture = new TexDesc();
				decal0Texture.Deserialize(reader);
			}
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
			fileName = NiUtils.ReadLengthPrefixedBytes32(reader);
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