// Automatically generated.

using System;

namespace NIFReader
{
	public enum AlphaFormat : uint
	{
		ALPHA_NONE = 0,
		ALPHA_BINARY = 1,
		ALPHA_SMOOTH = 2,
		ALPHA_DEFAULT = 3,
		
	}
	public enum ApplyMode : uint
	{
		APPLY_REPLACE = 0,
		APPLY_DECAL = 1,
		APPLY_MODULATE = 2,
		APPLY_HILIGHT = 3,
		APPLY_HILIGHT2 = 4,
		
	}
	public enum TexType : uint
	{
		BASE_MAP = 0,
		DARK_MAP = 1,
		DETAIL_MAP = 2,
		GLOSS_MAP = 3,
		GLOW_MAP = 4,
		BUMP_MAP = 5,
		NORMAL_MAP = 6,
		UNKNOWN2_MAP = 7,
		DECAL_0_MAP = 8,
		DECAL_1_MAP = 9,
		DECAL_2_MAP = 10,
		DECAL_3_MAP = 11,
		
	}
	public enum KeyType : uint
	{
		LINEAR_KEY = 1,
		QUADRATIC_KEY = 2,
		TBC_KEY = 3,
		XYZ_ROTATION_KEY = 4,
		CONST_KEY = 5,
		
	}
	public enum LightMode : uint
	{
		LIGHT_MODE_EMISSIVE = 0,
		LIGHT_MODE_EMI_AMB_DIF = 1,
		
	}
	public enum MoppDataBuildType : byte
	{
		BUILT_WITH_CHUNK_SUBDIVISION = 0,
		BUILT_WITHOUT_CHUNK_SUBDIVISION = 1,
		BUILD_NOT_SET = 2,
		
	}
	public enum MipMapFormat : uint
	{
		MIP_FMT_NO = 0,
		MIP_FMT_YES = 1,
		MIP_FMT_DEFAULT = 2,
		
	}
	public enum PixelFormat : uint
	{
		PX_FMT_RGB8 = 0,
		PX_FMT_RGBA8 = 1,
		PX_FMT_PAL8 = 2,
		PX_FMT_DXT1 = 4,
		PX_FMT_DXT5 = 5,
		PX_FMT_DXT5_ALT = 6,
		
	}
	public enum PixelLayout : uint
	{
		PIX_LAY_PALETTISED = 0,
		PIX_LAY_HIGH_COLOR_16 = 1,
		PIX_LAY_TRUE_COLOR_32 = 2,
		PIX_LAY_COMPRESSED = 3,
		PIX_LAY_BUMPMAP = 4,
		PIX_LAY_PALETTISED_4 = 5,
		PIX_LAY_DEFAULT = 6,
		
	}
	public enum TexClampMode : uint
	{
		CLAMP_S_CLAMP_T = 0,
		CLAMP_S_WRAP_T = 1,
		WRAP_S_CLAMP_T = 2,
		WRAP_S_WRAP_T = 3,
		
	}
	public enum TexFilterMode : uint
	{
		FILTER_NEAREST = 0,
		FILTER_BILERP = 1,
		FILTER_TRILERP = 2,
		FILTER_NEAREST_MIPNEAREST = 3,
		FILTER_NEAREST_MIPLERP = 4,
		FILTER_BILERP_MIPNEAREST = 5,
		
	}
	public enum VertMode : uint
	{
		VERT_MODE_SRC_IGNORE = 0,
		VERT_MODE_SRC_EMISSIVE = 1,
		VERT_MODE_SRC_AMB_DIF = 2,
		
	}
	public enum CycleType : uint
	{
		CYCLE_LOOP = 0,
		CYCLE_REVERSE = 1,
		CYCLE_CLAMP = 2,
		
	}
	public enum FieldType : uint
	{
		FIELD_WIND = 0,
		FIELD_POINT = 1,
		
	}
	public enum BillboardMode : ushort
	{
		ALWAYS_FACE_CAMERA = 0,
		ROTATE_ABOUT_UP = 1,
		RIGID_FACE_CAMERA = 2,
		ALWAYS_FACE_CENTER = 3,
		RIGID_FACE_CENTER = 4,
		BSROTATE_ABOUT_UP = 5,
		ROTATE_ABOUT_UP2 = 9,
		
	}
	public enum StencilCompareMode : uint
	{
		TEST_NEVER = 0,
		TEST_LESS = 1,
		TEST_EQUAL = 2,
		TEST_LESS_EQUAL = 3,
		TEST_GREATER = 4,
		TEST_NOT_EQUAL = 5,
		TEST_GREATER_EQUAL = 6,
		TEST_ALWAYS = 7,
		
	}
	public enum ZCompareMode : uint
	{
		ZCOMP_ALWAYS = 0,
		ZCOMP_LESS = 1,
		ZCOMP_EQUAL = 2,
		ZCOMP_LESS_EQUAL = 3,
		ZCOMP_GREATER = 4,
		ZCOMP_NOT_EQUAL = 5,
		ZCOMP_GREATER_EQUAL = 6,
		ZCOMP_NEVER = 7,
		
	}
	public enum StencilAction : uint
	{
		ACTION_KEEP = 0,
		ACTION_ZERO = 1,
		ACTION_REPLACE = 2,
		ACTION_INCREMENT = 3,
		ACTION_DECREMENT = 4,
		ACTION_INVERT = 5,
		
	}
	public enum FaceDrawMode : uint
	{
		DRAW_CCW_OR_BOTH = 0,
		DRAW_CCW = 1,
		DRAW_CW = 2,
		DRAW_BOTH = 3,
		
	}
	public enum MotionSystem : byte
	{
		MO_SYS_INVALID = 0,
		MO_SYS_DYNAMIC = 1,
		MO_SYS_SPHERE = 2,
		MO_SYS_SPHERE_INERTIA = 3,
		MO_SYS_BOX = 4,
		MO_SYS_BOX_STABILIZED = 5,
		MO_SYS_KEYFRAMED = 6,
		MO_SYS_FIXED = 7,
		MO_SYS_THIN_BOX = 8,
		MO_SYS_CHARACTER = 9,
		
	}
	public enum DeactivatorType : byte
	{
		DEACTIVATOR_INVALID = 0,
		DEACTIVATOR_NEVER = 1,
		DEACTIVATOR_SPATIAL = 2,
		
	}
	public enum SolverDeactivation : byte
	{
		SOLVER_DEACTIVATION_INVALID = 0,
		SOLVER_DEACTIVATION_OFF = 1,
		SOLVER_DEACTIVATION_LOW = 2,
		SOLVER_DEACTIVATION_MEDIUM = 3,
		SOLVER_DEACTIVATION_HIGH = 4,
		SOLVER_DEACTIVATION_MAX = 5,
		
	}
	public enum MotionQuality : byte
	{
		MO_QUAL_INVALID = 0,
		MO_QUAL_FIXED = 1,
		MO_QUAL_KEYFRAMED = 2,
		MO_QUAL_DEBRIS = 3,
		MO_QUAL_MOVING = 4,
		MO_QUAL_CRITICAL = 5,
		MO_QUAL_BULLET = 6,
		MO_QUAL_USER = 7,
		MO_QUAL_CHARACTER = 8,
		MO_QUAL_KEYFRAMED_REPORT = 9,
		
	}
	public enum ForceType : uint
	{
		FORCE_PLANAR = 0,
		FORCE_SPHERICAL = 1,
		FORCE_UNKNOWN = 2,
		
	}
	public enum TexTransform : uint
	{
		TT_TRANSLATE_U = 0,
		TT_TRANSLATE_V = 1,
		TT_ROTATE = 2,
		TT_SCALE_U = 3,
		TT_SCALE_V = 4,
		
	}
	public enum DecayType : uint
	{
		DECAY_NONE = 0,
		DECAY_LINEAR = 1,
		DECAY_EXPONENTIAL = 2,
		
	}
	public enum SymmetryType : uint
	{
		SPHERICAL_SYMMETRY = 0,
		CYLINDRICAL_SYMMETRY = 1,
		PLANAR_SYMMETRY = 2,
		
	}
	public enum VelocityType : uint
	{
		VELOCITY_USE_NORMALS = 0,
		VELOCITY_USE_RANDOM = 1,
		VELOCITY_USE_DIRECTION = 2,
		
	}
	public enum EmitFrom : uint
	{
		EMIT_FROM_VERTICES = 0,
		EMIT_FROM_FACE_CENTER = 1,
		EMIT_FROM_EDGE_CENTER = 2,
		EMIT_FROM_FACE_SURFACE = 3,
		EMIT_FROM_EDGE_SURFACE = 4,
		
	}
	public enum EffectType : uint
	{
		EFFECT_PROJECTED_LIGHT = 0,
		EFFECT_PROJECTED_SHADOW = 1,
		EFFECT_ENVIRONMENT_MAP = 2,
		EFFECT_FOG_MAP = 3,
		
	}
	public enum CoordGenType : uint
	{
		CG_WORLD_PARALLEL = 0,
		CG_WORLD_PERSPECTIVE = 1,
		CG_SPHERE_MAP = 2,
		CG_SPECULAR_CUBE_MAP = 3,
		CG_DIFFUSE_CUBE_MAP = 4,
		
	}
	public enum EndianType : byte
	{
		ENDIAN_BIG = 0,
		ENDIAN_LITTLE = 1,
		
	}
	public enum TargetColor : ushort
	{
		TC_AMBIENT = 0,
		TC_DIFFUSE = 1,
		TC_SPECULAR = 2,
		TC_SELF_ILLUM = 3,
		
	}
	public enum ConsistencyType : ushort
	{
		CT_MUTABLE = 0x0000,
		CT_STATIC = 0x4000,
		CT_VOLATILE = 0x8000,
		
	}
	public enum SortingMode : uint
	{
		SORTING_INHERIT = 0,
		SORTING_OFF = 1,
		
	}
	public enum PropagationMode : uint
	{
		PROPAGATE_ON_SUCCESS = 0,
		PROPAGATE_ON_FAILURE = 1,
		PROPAGATE_ALWAYS = 2,
		PROPAGATE_NEVER = 3,
		
	}
	public enum CollisionMode : uint
	{
		CM_USE_OBB = 0,
		CM_USE_TRI = 1,
		CM_USE_ABV = 2,
		CM_NOTEST = 3,
		CM_USE_NIBOUND = 4,
		
	}
	public enum BoundVolumeType : uint
	{
		BASE_BV = 0xffffffff,
		SPHERE_BV = 0,
		BOX_BV = 1,
		CAPSULE_BV = 2,
		UNION_BV = 4,
		HALFSPACE_BV = 5,
		
	}
	public enum hkResponseType : byte
	{
		RESPONSE_INVALID = 0,
		RESPONSE_SIMPLE_CONTACT = 1,
		RESPONSE_REPORTING = 2,
		RESPONSE_NONE = 3,
		
	}
	public enum BSDismemberBodyPartType : ushort
	{
		BP_TORSO = 0,
		BP_HEAD = 1,
		BP_HEAD2 = 2,
		BP_LEFTARM = 3,
		BP_LEFTARM2 = 4,
		BP_RIGHTARM = 5,
		BP_RIGHTARM2 = 6,
		BP_LEFTLEG = 7,
		BP_LEFTLEG2 = 8,
		BP_LEFTLEG3 = 9,
		BP_RIGHTLEG = 10,
		BP_RIGHTLEG2 = 11,
		BP_RIGHTLEG3 = 12,
		BP_BRAIN = 13,
		SBP_30_HEAD = 30,
		SBP_31_HAIR = 31,
		SBP_32_BODY = 32,
		SBP_33_HANDS = 33,
		SBP_34_FOREARMS = 34,
		SBP_35_AMULET = 35,
		SBP_36_RING = 36,
		SBP_37_FEET = 37,
		SBP_38_CALVES = 38,
		SBP_39_SHIELD = 39,
		SBP_40_TAIL = 40,
		SBP_41_LONGHAIR = 41,
		SBP_42_CIRCLET = 42,
		SBP_43_EARS = 43,
		SBP_44_DRAGON_BLOODHEAD_OR_MOD_MOUTH = 44,
		SBP_45_DRAGON_BLOODWINGL_OR_MOD_NECK = 45,
		SBP_46_DRAGON_BLOODWINGR_OR_MOD_CHEST_PRIMARY = 46,
		SBP_47_DRAGON_BLOODTAIL_OR_MOD_BACK = 47,
		SBP_48_MOD_MISC1 = 48,
		SBP_49_MOD_PELVIS_PRIMARY = 49,
		SBP_50_DECAPITATEDHEAD = 50,
		SBP_51_DECAPITATE = 51,
		SBP_52_MOD_PELVIS_SECONDARY = 52,
		SBP_53_MOD_LEG_RIGHT = 53,
		SBP_54_MOD_LEG_LEFT = 54,
		SBP_55_MOD_FACE_JEWELRY = 55,
		SBP_56_MOD_CHEST_SECONDARY = 56,
		SBP_57_MOD_SHOULDER = 57,
		SBP_58_MOD_ARM_LEFT = 58,
		SBP_59_MOD_ARM_RIGHT = 59,
		SBP_60_MOD_MISC2 = 60,
		SBP_61_FX01 = 61,
		BP_SECTIONCAP_HEAD = 101,
		BP_SECTIONCAP_HEAD2 = 102,
		BP_SECTIONCAP_LEFTARM = 103,
		BP_SECTIONCAP_LEFTARM2 = 104,
		BP_SECTIONCAP_RIGHTARM = 105,
		BP_SECTIONCAP_RIGHTARM2 = 106,
		BP_SECTIONCAP_LEFTLEG = 107,
		BP_SECTIONCAP_LEFTLEG2 = 108,
		BP_SECTIONCAP_LEFTLEG3 = 109,
		BP_SECTIONCAP_RIGHTLEG = 110,
		BP_SECTIONCAP_RIGHTLEG2 = 111,
		BP_SECTIONCAP_RIGHTLEG3 = 112,
		BP_SECTIONCAP_BRAIN = 113,
		SBP_130_HEAD = 130,
		SBP_131_HAIR = 131,
		SBP_141_LONGHAIR = 141,
		SBP_142_CIRCLET = 142,
		SBP_143_EARS = 143,
		SBP_150_DECAPITATEDHEAD = 150,
		BP_TORSOCAP_HEAD = 201,
		BP_TORSOCAP_HEAD2 = 202,
		BP_TORSOCAP_LEFTARM = 203,
		BP_TORSOCAP_LEFTARM2 = 204,
		BP_TORSOCAP_RIGHTARM = 205,
		BP_TORSOCAP_RIGHTARM2 = 206,
		BP_TORSOCAP_LEFTLEG = 207,
		BP_TORSOCAP_LEFTLEG2 = 208,
		BP_TORSOCAP_LEFTLEG3 = 209,
		BP_TORSOCAP_RIGHTLEG = 210,
		BP_TORSOCAP_RIGHTLEG2 = 211,
		BP_TORSOCAP_RIGHTLEG3 = 212,
		BP_TORSOCAP_BRAIN = 213,
		SBP_230_HEAD = 230,
		BP_TORSOSECTION_HEAD = 1000,
		BP_TORSOSECTION_HEAD2 = 2000,
		BP_TORSOSECTION_LEFTARM = 3000,
		BP_TORSOSECTION_LEFTARM2 = 4000,
		BP_TORSOSECTION_RIGHTARM = 5000,
		BP_TORSOSECTION_RIGHTARM2 = 6000,
		BP_TORSOSECTION_LEFTLEG = 7000,
		BP_TORSOSECTION_LEFTLEG2 = 8000,
		BP_TORSOSECTION_LEFTLEG3 = 9000,
		BP_TORSOSECTION_RIGHTLEG = 10000,
		BP_TORSOSECTION_RIGHTLEG2 = 11000,
		BP_TORSOSECTION_RIGHTLEG3 = 12000,
		BP_TORSOSECTION_BRAIN = 13000,
		
	}
	[Flags]
	public enum BSSegmentFlags : uint
	{
		BSSEG_WATER = 9,
		
	}
	public enum BSLightingShaderPropertyShaderType : uint
	{
		Default = 0,
		Environment_Map = 1,
		Glow_Shader = 2,
		Heightmap = 3,
		Face_Tint = 4,
		Skin_Tint = 5,
		Hair_Tint = 6,
		Parallax_Occ_Material = 7,
		World_Multitexture = 8,
		WorldMap1 = 9,
		Unknown_10 = 10,
		MultiLayer_Parallax = 11,
		Unknown_12 = 12,
		WorldMap2 = 13,
		Sparkle_Snow = 14,
		WorldMap3 = 15,
		Eye_Envmap = 16,
		Unknown_17 = 17,
		WorldMap4 = 18,
		World_LOD_Multitexture = 19,
		
	}
	public enum EffectShaderControlledVariable : uint
	{
		EmissiveMultiple = 0,
		Falloff_Start_Angle_degrees = 1,
		Falloff_Stop_Angle_degrees = 2,
		Falloff_Start_Opacity = 3,
		Falloff_Stop_Opacity = 4,
		Alpha_Transparency_Emissive_alpha = 5,
		U_Offset = 6,
		U_Scale = 7,
		V_Offset = 8,
		V_Scale = 9,
		
	}
	public enum EffectShaderControlledColor : uint
	{
		Emissive_Color = 0,
		
	}
	public enum LightingShaderControlledVariable : uint
	{
		Refraction_Strength = 0,
		Environment_Map_Scale = 8,
		Glossiness = 9,
		Specular_Strength = 10,
		Emissive_Multiple = 11,
		Alpha = 12,
		U_Offset = 20,
		U_Scale = 21,
		V_Offset = 22,
		V_Scale = 23,
		
	}
	public enum LightingShaderControlledColor : uint
	{
		Specular_Color = 0,
		Emissive_Color = 1,
		
	}
	public enum ExtraVectorsFlags : byte
	{
		None = 0,
		Tangents_Bitangents = 16,
		
	}
	public enum hkConstraintType : uint
	{
		BallAndSocket = 0,
		Hinge = 1,
		Limited_Hinge = 2,
		Prismatic = 6,
		Ragdoll = 7,
		StiffSpring = 8,
		
	}
	public class SizedString
	{
		uint Length;
		byte[] Value;
		
	}
	public class NIFString
	{
		SizedString String;
		
	}
	public class ByteArray
	{
		uint Data_Size;
		byte[] Data;
		
	}
	public class ByteMatrix
	{
		uint Data_Size_1;
		uint Data_Size_2;
		byte[,] Data;
		
	}
	public class Color3
	{
		float r;
		float g;
		float b;
		
	}
	public class ByteColor3
	{
		byte r;
		byte g;
		byte b;
		
	}
	public class Color4
	{
		float r;
		float g;
		float b;
		float a;
		
	}
	public class ByteColor4
	{
		byte r;
		byte g;
		byte b;
		byte a;
		
	}
	public class FilePath
	{
		SizedString String;
		
	}
	public class Footer
	{
		uint Num_Roots;
		int[] Roots;
		
	}
	public class LODRange
	{
		float Near_Extent;
		float Far_Extent;
		
	}
	public class MatchGroup
	{
		ushort Num_Vertices;
		ushort[] Vertex_Indices;
		
	}
	public class ByteVector3
	{
		byte x;
		byte y;
		byte z;
		
	}
	public class HalfVector3
	{
		float x;
		float y;
		float z;
		
	}
	public class Vector3
	{
		float x;
		float y;
		float z;
		
	}
	public class Vector4
	{
		float x;
		float y;
		float z;
		float w;
		
	}
	public class Quaternion
	{
		float w;
		float x;
		float y;
		float z;
		
	}
	public class QuaternionXYZW
	{
		float x;
		float y;
		float z;
		float w;
		
	}
	public class Matrix22
	{
		float m11;
		float m21;
		float m12;
		float m22;
		
	}
	public class Matrix33
	{
		float m11;
		float m21;
		float m31;
		float m12;
		float m22;
		float m32;
		float m13;
		float m23;
		float m33;
		
	}
	public class Matrix44
	{
		float m11;
		float m21;
		float m31;
		float m41;
		float m12;
		float m22;
		float m32;
		float m42;
		float m13;
		float m23;
		float m33;
		float m43;
		float m14;
		float m24;
		float m34;
		float m44;
		
	}
	public class MipMap
	{
		uint Width;
		uint Height;
		uint Offset;
		
	}
	public class NodeGroup
	{
		uint Num_Nodes;
		int[] Nodes;
		
	}
	public class ShortString
	{
		byte Length;
		byte[] Value;
		
	}
	public class SkinShape
	{
		int Shape;
		int Skin_Instance;
		
	}
	public class SkinShapeGroup
	{
		uint Num_Link_Pairs;
		SkinShape[] Link_Pairs;
		
	}
	public class SkinWeight
	{
		ushort Index;
		float Weight;
		
	}
	public class AVObject
	{
		SizedString Name;
		int AV_Object;
		
	}
	public class ControllerLink
	{
		NIFString Target_Name;
		int Controller;
		
	}
	public class ExportInfo
	{
		uint Unknown;
		ShortString Creator;
		ShortString Export_Info_1;
		ShortString Export_Info_2;
		
	}
	public class Header
	{
		string Header_String;
		uint Version;
		uint Num_Blocks;
		ShortString Export_Info_3;
		
	}
	public class StringPalette
	{
		SizedString Palette;
		uint Length;
		
	}
	public class TBC
	{
		float t;
		float b;
		float c;
		
	}
	public class Key<T>
	{
		float Time;
		T Value;
		T Forward;
		T Backward;
		TBC TBC;
		
	}
	public class KeyGroup<T>
	{
		uint Num_Keys;
		KeyType Interpolation;
		Key<T>[] Keys;
		
	}
	public class QuatKey<T>
	{
		float Time;
		T Value;
		TBC TBC;
		
	}
	public class TexCoord
	{
		float u;
		float v;
		
	}
	public class HalfTexCoord
	{
		float u;
		float v;
		
	}
	public class TexDesc
	{
		int Source;
		TexClampMode Clamp_Mode;
		TexFilterMode Filter_Mode;
		uint UV_Set;
		short PS2_L;
		short PS2_K;
		ushort Unknown1;
		
	}
	public class ShaderTexDesc
	{
		bool Is_Used;
		TexDesc Texture_Data;
		uint Map_Index;
		
	}
	public class TexSource
	{
		byte Use_External;
		FilePath File_Name;
		byte Unknown_Byte;
		int Pixel_Data;
		
	}
	public class Triangle
	{
		ushort v1;
		ushort v2;
		ushort v3;
		
	}
	public class SkinPartition
	{
		ushort Num_Vertices;
		ushort Num_Triangles;
		ushort Num_Bones;
		ushort Num_Strips;
		ushort Num_Weights_Per_Vertex;
		ushort[] Bones;
		ushort[] Vertex_Map;
		float[,] Vertex_Weights;
		ushort[] Strip_Lengths;
		ushort[,] Strips;
		Triangle[] Triangles;
		bool Has_Bone_Indices;
		byte[,] Bone_Indices;
		ushort Unknown_Short;
		
	}
	public class QTransform
	{
		Vector3 Translation;
		Quaternion Rotation;
		float Scale;
		
	}
	public class MTransform
	{
		Vector3 Translation;
		Matrix33 Rotation;
		float Scale;
		
	}
	public class SkinTransform
	{
		Matrix33 Rotation;
		Vector3 Translation;
		float Scale;
		
	}
	public class BoundingBox
	{
		uint Unknown_Int;
		Vector3 Translation;
		Matrix33 Rotation;
		Vector3 Radius;
		
	}
	[Flags]
	public enum FurnitureEntryPoints : ushort
	{
		Front = 0,
		Behind = 1,
		Right = 2,
		Left = 3,
		Up = 4,
		
	}
	public enum AnimationType : ushort
	{
		Sit = 1,
		Sleep = 2,
		Lean = 4,
		
	}
	public class FurniturePosition
	{
		Vector3 Offset;
		ushort Orientation;
		byte Position_Ref_1;
		byte Position_Ref_2;
		float Heading;
		AnimationType Animation_Type;
		FurnitureEntryPoints Entry_Properties;
		
	}
	public class hkTriangle
	{
		Triangle Triangle;
		ushort Welding_Info;
		Vector3 Normal;
		
	}
	public class Morph
	{
		uint Num_Keys;
		KeyType Interpolation;
		Key<float>[] Keys;
		Vector3[] Vectors;
		
	}
	public class Particle
	{
		Vector3 Velocity;
		Vector3 Unknown_Vector;
		float Lifetime;
		float Lifespan;
		float Timestamp;
		ushort Unknown_Short;
		ushort Vertex_ID;
		
	}
	public class SkinData
	{
		SkinTransform Skin_Transform;
		Vector3 Bounding_Sphere_Offset;
		float Bounding_Sphere_Radius;
		ushort Num_Vertices;
		SkinWeight[] Vertex_Weights;
		
	}
	public class SphereBV
	{
		Vector3 Center;
		float Radius;
		
	}
	public class HavokColFilter
	{
		byte Flags_and_Part_Number;
		ushort Unknown_Short;
		
	}
	public class HavokMaterial
	{
		
	}
	public class MotorDescriptor
	{
		float Unknown_Float_1;
		float Unknown_Float_2;
		float Unknown_Float_3;
		float Unknown_Float_4;
		float Unknown_Float_5;
		float Unknown_Float_6;
		byte Unknown_Byte_1;
		
	}
	public class RagdollDescriptor
	{
		float Cone_Max_Angle;
		float Plane_Min_Angle;
		float Plane_Max_Angle;
		float Twist_Min_Angle;
		float Twist_Max_Angle;
		float Max_Friction;
		
	}
	public class LimitedHingeDescriptor
	{
		float Min_Angle;
		float Max_Angle;
		float Max_Friction;
		
	}
	public class HingeDescriptor
	{
		
	}
	public class BallAndSocketDescriptor
	{
		byte[] Unknown_4_bytes;
		Vector3 Unknown_Floats_1;
		Vector3 Unknown_Floats_2;
		uint Unknown_Int_1;
		
	}
	public class PrismaticDescriptor
	{
		float Min_Distance;
		float Max_Distance;
		float Friction;
		
	}
	public class StiffSpringDescriptor
	{
		Vector4 Pivot_A;
		Vector4 Pivot_B;
		float Length;
		
	}
	public class OldSkinData
	{
		float Vertex_Weight;
		ushort Vertex_Index;
		Vector3 Unknown_Vector;
		
	}
	public class MultiTextureElement
	{
		bool Has_Image;
		int Image;
		TexClampMode Clamp;
		TexFilterMode Filter;
		uint UV_Set;
		short PS2_L;
		short PS2_K;
		short Unknown_Short_3;
		
	}
	public enum ImageType : uint
	{
		RGB = 1,
		RGBA = 2,
		
	}
	public class BoxBV
	{
		Vector3 Center;
		Vector3[] Axis;
		float[] Extent;
		
	}
	public class CapsuleBV
	{
		Vector3 Center;
		Vector3 Origin;
		float Unknown_Float_1;
		float Unknown_Float_2;
		
	}
	public class HalfSpaceBV
	{
		Vector3 Normal;
		Vector3 Center;
		float Unknown_Float_1;
		
	}
	public class BoundingVolume
	{
		BoundVolumeType Collision_Type;
		SphereBV Sphere;
		BoxBV Box;
		CapsuleBV Capsule;
		UnionBV Union;
		HalfSpaceBV HalfSpace;
		
	}
	public class UnionBV
	{
		uint Num_BV;
		BoundingVolume[] Bounding_Volumes;
		
	}
	public class MorphWeight
	{
		int Interpolator;
		float Weight;
		
	}
	public class ArkTexture
	{
		NIFString Texture_Name;
		int Unknown_Int_3;
		int Unknown_Int_4;
		int Texturing_Property;
		byte[] Unknown_Bytes;
		
	}
	public class InertiaMatrix
	{
		float m11;
		float m12;
		float m13;
		float m14;
		float m21;
		float m22;
		float m23;
		float m24;
		float m31;
		float m32;
		float m33;
		float m34;
		
	}
	public class DecalVectorArray
	{
		short Num_Vectors;
		Vector3[] Points;
		Vector3[] Normals;
		
	}
	[Flags]
	public enum BSPartFlag : ushort
	{
		PF_EDITOR_VISIBLE = 0,
		PF_START_NET_BONESET = 8,
		
	}
	public class BodyPartList
	{
		BSPartFlag Part_Flag;
		BSDismemberBodyPartType Body_Part;
		
	}
	public class BSSegment
	{
		int Internal_index;
		BSSegmentFlags Flags;
		byte Unknown_Byte_1;
		
	}
	public class BoneLOD
	{
		uint Distance;
		NIFString Bone_Name;
		
	}
	public class bhkCMSDMaterial
	{
		byte Byte_set_to_0;
		ushort Short_set_to_0;
		
	}
	public class bhkCMSDBigTris
	{
		ushort Triangle_1;
		ushort Triangle_2;
		ushort Triangle_3;
		uint Unknown_Int_1;
		ushort Unknown_Short_1;
		
	}
	public class bhkCMSDTransform
	{
		Vector4 Translation;
		QuaternionXYZW Rotation;
		
	}
	public class bhkCMSDChunk
	{
		Vector4 Translation;
		uint Material_Index;
		ushort Unknown_Short_1;
		ushort Transform_Index;
		uint Num_Vertices;
		ushort[] Vertices;
		uint Num_Indices;
		ushort[] Indices;
		uint Num_Strips;
		ushort[] Strips;
		uint Num_Indices_2;
		ushort[] Indices_2;
		
	}
	public class SubConstraint
	{
		hkConstraintType Type;
		uint Num_Entities;
		int[] Entities;
		uint Priority;
		BallAndSocketDescriptor Ball_and_Socket;
		HingeDescriptor Hinge;
		LimitedHingeDescriptor Limited_Hinge;
		PrismaticDescriptor Prismatic;
		RagdollDescriptor Ragdoll;
		StiffSpringDescriptor StiffSpring;
		
	}
	public class bhkRDTConstraint
	{
		uint Type;
		uint Unknown_Int;
		int Entity_A;
		int Entity_B;
		uint Priority;
		RagdollDescriptor Ragdoll;
		bhkRDTMalleableConstraint Malleable_Constraint;
		
	}
	public class bhkRDTMalleableConstraint
	{
		uint Type;
		uint Unknown_Int;
		int Entity_A;
		int Entity_B;
		uint Priority;
		HingeDescriptor Hinge;
		RagdollDescriptor Ragdoll;
		LimitedHingeDescriptor Limited_Hinge;
		float Damping;
		
	}
	public abstract class NiObject
	{
		
	}
	public class Ni3dsAlphaAnimator : NiObject
	{
		byte[] Unknown_1;
		int Parent;
		uint Num_1;
		uint Num_2;
		uint[,,] Unknown_2;
		
	}
	public class Ni3dsAnimationNode : NiObject
	{
		NIFString Name;
		bool Has_Data;
		float[] Unknown_Floats_1;
		ushort Unknown_Short;
		int Child;
		float[] Unknown_Floats_2;
		uint Count;
		byte[,] Unknown_Array;
		
	}
	public class Ni3dsColorAnimator : NiObject
	{
		byte[] Unknown_1;
		
	}
	public class Ni3dsMorphShape : NiObject
	{
		byte[] Unknown_1;
		
	}
	public class Ni3dsParticleSystem : NiObject
	{
		byte[] Unknown_1;
		
	}
	public class Ni3dsPathController : NiObject
	{
		byte[] Unknown_1;
		
	}
	public abstract class NiParticleModifier : NiObject
	{
		int Next_Modifier;
		int Controller;
		
	}
	public abstract class NiPSysCollider : NiObject
	{
		float Bounce;
		bool Spawn_on_Collide;
		bool Die_on_Collide;
		int Spawn_Modifier;
		int Parent;
		int Next_Collider;
		int Collider_Object;
		
	}
	public abstract class bhkRefObject : NiObject
	{
		
	}
	public abstract class bhkSerializable : bhkRefObject
	{
		
	}
	public abstract class bhkWorldObject : bhkSerializable
	{
		int Shape;
		HavokColFilter Havok_Col_Filter;
		
	}
	public abstract class bhkPhantom : bhkWorldObject
	{
		
	}
	public abstract class bhkShapePhantom : bhkPhantom
	{
		
	}
	public class bhkSimpleShapePhantom : bhkShapePhantom
	{
		float[] Unkown_Floats;
		float[,] Unknown_Floats_2;
		float Unknown_Float;
		
	}
	public abstract class bhkEntity : bhkWorldObject
	{
		
	}
	public class bhkRigidBody : bhkEntity
	{
		int Unknown_Int_1;
		int Unknown_Int_2;
		int[] Unknown_3_Ints;
		hkResponseType Collision_Response;
		byte Unknown_Byte;
		ushort Process_Contact_Callback_Delay;
		ushort[] Unknown_2_Shorts;
		HavokColFilter Havok_Col_Filter_Copy;
		ushort[] Unknown_6_Shorts;
		Vector4 Translation;
		QuaternionXYZW Rotation;
		Vector4 Linear_Velocity;
		Vector4 Angular_Velocity;
		InertiaMatrix Inertia;
		Vector4 Center;
		float Mass;
		float Linear_Damping;
		float Angular_Damping;
		float Unknown_TimeFactor_or_GravityFactor_1;
		float Unknown_TimeFactor_or_GravityFactor_2;
		float Friction;
		float RollingFrictionMultiplier;
		float Restitution;
		float Max_Linear_Velocity;
		float Max_Angular_Velocity;
		float Penetration_Depth;
		MotionSystem Motion_System;
		DeactivatorType Deactivator_Type;
		SolverDeactivation Solver_Deactivation;
		MotionQuality Quality_Type;
		uint Unknown_Int_6;
		uint Unknown_Int_7;
		uint Unknown_Int_8;
		uint Num_Constraints;
		int[] Constraints;
		uint Unknown_Int_9;
		ushort Unknown_Int_91;
		
	}
	public class bhkRigidBodyT : bhkRigidBody
	{
		
	}
	public abstract class bhkConstraint : bhkSerializable
	{
		uint Num_Entities;
		int[] Entities;
		uint Priority;
		
	}
	public class bhkLimitedHingeConstraint : bhkConstraint
	{
		LimitedHingeDescriptor Limited_Hinge;
		
	}
	public class bhkMalleableConstraint : bhkConstraint
	{
		SubConstraint Sub_Constraint;
		float Tau;
		float Damping;
		
	}
	public class bhkStiffSpringConstraint : bhkConstraint
	{
		StiffSpringDescriptor StiffSpring;
		
	}
	public class bhkRagdollConstraint : bhkConstraint
	{
		RagdollDescriptor Ragdoll;
		
	}
	public class bhkPrismaticConstraint : bhkConstraint
	{
		PrismaticDescriptor Prismatic;
		
	}
	public class bhkHingeConstraint : bhkConstraint
	{
		HingeDescriptor Hinge;
		
	}
	public class bhkBallAndSocketConstraint : bhkConstraint
	{
		BallAndSocketDescriptor Ball_and_Socket;
		
	}
	public class bhkBallSocketConstraintChain : bhkSerializable
	{
		uint Num_Floats;
		Vector4[] Floats_1;
		float Unknown_Float_1;
		float Unknown_Float_2;
		uint Unknown_Int_1;
		uint Unknown_Int_2;
		uint Num_Links;
		int[] Links;
		uint Num_Links_2;
		int[] Links_2;
		uint Unknown_Int_3;
		
	}
	public abstract class bhkShape : bhkSerializable
	{
		
	}
	public class bhkTransformShape : bhkShape
	{
		int Shape;
		HavokMaterial Material;
		float Unknown_Float_1;
		byte[] Unknown_8_Bytes;
		Matrix44 Transform;
		
	}
	public abstract class bhkSphereRepShape : bhkShape
	{
		HavokMaterial Material;
		float Radius;
		
	}
	public abstract class bhkConvexShape : bhkSphereRepShape
	{
		
	}
	public class bhkSphereShape : bhkConvexShape
	{
		
	}
	public class bhkCapsuleShape : bhkConvexShape
	{
		byte[] Unknown_8_Bytes;
		Vector3 First_Point;
		float Radius_1;
		Vector3 Second_Point;
		float Radius_2;
		
	}
	public class bhkBoxShape : bhkConvexShape
	{
		byte[] Unknown_8_Bytes;
		Vector3 Dimensions;
		float Minimum_Size;
		
	}
	public class bhkConvexVerticesShape : bhkConvexShape
	{
		float[] Unknown_6_Floats;
		uint Num_Vertices;
		Vector4[] Vertices;
		uint Num_Normals;
		Vector4[] Normals;
		
	}
	public class bhkConvexTransformShape : bhkTransformShape
	{
		
	}
	public class bhkMultiSphereShape : bhkSphereRepShape
	{
		float Unknown_Float_1;
		float Unknown_Float_2;
		uint Num_Spheres;
		SphereBV[] Spheres;
		
	}
	public abstract class bhkBvTreeShape : bhkShape
	{
		
	}
	public class bhkMoppBvTreeShape : bhkBvTreeShape
	{
		int Shape;
		uint Unknown_Int;
		byte[] Unknown_8_Bytes;
		float Unknown_Float;
		uint MOPP_Data_Size;
		Vector3 Origin;
		float Scale;
		byte[] Old_MOPP_Data;
		
	}
	public abstract class bhkShapeCollection : bhkShape
	{
		
	}
	public class bhkListShape : bhkShapeCollection
	{
		uint Num_Sub_Shapes;
		int[] Sub_Shapes;
		HavokMaterial Material;
		float[] Unknown_Floats;
		uint Num_Unknown_Ints;
		uint[] Unknown_Ints;
		
	}
	public class bhkPackedNiTriStripsShape : bhkShapeCollection
	{
		uint Unknown_Int_1;
		uint Unknown_Int_2;
		float Unknown_Float_1;
		uint Unknown_Int_3;
		Vector3 Scale_Copy;
		float Unknown_Float_2;
		float Unknown_Float_3;
		Vector3 Scale;
		float Unknown_Float_4;
		int Data;
		
	}
	public class bhkNiTriStripsShape : bhkShapeCollection
	{
		HavokMaterial Material;
		float Unknown_Float_1;
		uint Unknown_Int_1;
		uint[] Unknown_Ints_1;
		uint Unknown_Int_2;
		Vector3 Scale;
		uint Unknown_Int_3;
		uint Num_Strips_Data;
		int[] Strips_Data;
		uint Num_Data_Layers;
		HavokColFilter[] Data_Layers;
		
	}
	public class NiExtraData : NiObject
	{
		int Next_Extra_Data;
		
	}
	public abstract class NiInterpolator : NiObject
	{
		
	}
	public abstract class NiKeyBasedInterpolator : NiInterpolator
	{
		
	}
	public class NiFloatInterpolator : NiKeyBasedInterpolator
	{
		float Float_Value;
		int Data;
		
	}
	public class NiTransformInterpolator : NiKeyBasedInterpolator
	{
		Vector3 Translation;
		Quaternion Rotation;
		float Scale;
		int Data;
		
	}
	public class NiPoint3Interpolator : NiKeyBasedInterpolator
	{
		Vector3 Point_3_Value;
		int Data;
		
	}
	public class NiPathInterpolator : NiKeyBasedInterpolator
	{
		ushort Unknown_Short;
		uint Unknown_Int;
		float Unknown_Float_1;
		float Unknown_Float_2;
		ushort Unknown_Short_2;
		int Pos_Data;
		int Float_Data;
		
	}
	public class NiBoolInterpolator : NiKeyBasedInterpolator
	{
		bool Bool_Value;
		int Data;
		
	}
	public class NiBoolTimelineInterpolator : NiBoolInterpolator
	{
		
	}
	public abstract class NiBlendInterpolator : NiInterpolator
	{
		ushort Unknown_Short;
		uint Unknown_Int;
		
	}
	public abstract class NiBSplineInterpolator : NiInterpolator
	{
		float Start_Time;
		float Stop_Time;
		int Spline_Data;
		int Basis_Data;
		
	}
	public abstract class NiObjectNET : NiObject
	{
		NIFString Name;
		int Extra_Data;
		int Controller;
		
	}
	public class NiCollisionObject : NiObject
	{
		int Target;
		
	}
	public class NiCollisionData : NiCollisionObject
	{
		PropagationMode Propagation_Mode;
		byte Use_ABV;
		BoundingVolume Bounding_Volume;
		
	}
	public abstract class bhkNiCollisionObject : NiCollisionObject
	{
		ushort Flags;
		int Body;
		
	}
	public class bhkCollisionObject : bhkNiCollisionObject
	{
		
	}
	public class bhkBlendCollisionObject : bhkCollisionObject
	{
		float Unknown_Float_1;
		float Unknown_Float_2;
		
	}
	public class bhkPCollisionObject : bhkNiCollisionObject
	{
		
	}
	public class bhkSPCollisionObject : bhkPCollisionObject
	{
		
	}
	public abstract class NiAVObject : NiObjectNET
	{
		ushort Flags;
		Vector3 Translation;
		Matrix33 Rotation;
		float Scale;
		Vector3 Velocity;
		uint Num_Properties;
		int[] Properties;
		bool Has_Bounding_Box;
		BoundingBox Bounding_Box;
		
	}
	public abstract class NiDynamicEffect : NiAVObject
	{
		uint Num_Affected_Node_List_Pointers;
		uint[] Affected_Node_List_Pointers;
		
	}
	public abstract class NiLight : NiDynamicEffect
	{
		float Dimmer;
		Color3 Ambient_Color;
		Color3 Diffuse_Color;
		Color3 Specular_Color;
		
	}
	public abstract class NiProperty : NiObjectNET
	{
		
	}
	public class NiTransparentProperty : NiProperty
	{
		byte[] Unknown;
		
	}
	public abstract class NiPSysModifier : NiObject
	{
		NIFString Name;
		uint Order;
		int Target;
		bool Active;
		
	}
	public abstract class NiPSysEmitter : NiPSysModifier
	{
		float Speed;
		float Speed_Variation;
		float Declination;
		float Declination_Variation;
		float Planar_Angle;
		float Planar_Angle_Variation;
		Color4 Initial_Color;
		float Initial_Radius;
		float Life_Span;
		float Life_Span_Variation;
		
	}
	public abstract class NiPSysVolumeEmitter : NiPSysEmitter
	{
		
	}
	public abstract class NiTimeController : NiObject
	{
		int Next_Controller;
		ushort Flags;
		float Frequency;
		float Phase;
		float Start_Time;
		float Stop_Time;
		int Target;
		
	}
	public abstract class NiInterpController : NiTimeController
	{
		
	}
	public class NiMultiTargetTransformController : NiInterpController
	{
		ushort Num_Extra_Targets;
		int[] Extra_Targets;
		
	}
	public class NiGeomMorpherController : NiInterpController
	{
		int Data;
		byte Always_Update;
		
	}
	public class NiMorphController : NiInterpController
	{
		
	}
	public class NiMorpherController : NiInterpController
	{
		int Data;
		
	}
	public abstract class NiSingleInterpController : NiInterpController
	{
		
	}
	public class NiKeyframeController : NiSingleInterpController
	{
		int Data;
		
	}
	public class NiTransformController : NiKeyframeController
	{
		
	}
	public abstract class NiPSysModifierCtlr : NiSingleInterpController
	{
		NIFString Modifier_Name;
		
	}
	public class NiPSysEmitterCtlr : NiPSysModifierCtlr
	{
		int Data;
		
	}
	public abstract class NiPSysModifierBoolCtlr : NiPSysModifierCtlr
	{
		
	}
	public class NiPSysModifierActiveCtlr : NiPSysModifierBoolCtlr
	{
		int Data;
		
	}
	public abstract class NiPSysModifierFloatCtlr : NiPSysModifierCtlr
	{
		int Data;
		
	}
	public class NiPSysEmitterDeclinationCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysEmitterDeclinationVarCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysEmitterInitialRadiusCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysEmitterLifeSpanCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysEmitterSpeedCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysGravityStrengthCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public abstract class NiFloatInterpController : NiSingleInterpController
	{
		
	}
	public class NiFlipController : NiFloatInterpController
	{
		TexType Texture_Slot;
		uint Unknown_Int_2;
		float Delta;
		uint Num_Sources;
		int[] Sources;
		
	}
	public class NiAlphaController : NiFloatInterpController
	{
		int Data;
		
	}
	public class NiTextureTransformController : NiFloatInterpController
	{
		byte Unknown2;
		TexType Texture_Slot;
		TexTransform Operation;
		int Data;
		
	}
	public class NiLightDimmerController : NiFloatInterpController
	{
		
	}
	public abstract class NiBoolInterpController : NiSingleInterpController
	{
		
	}
	public class NiVisController : NiBoolInterpController
	{
		int Data;
		
	}
	public abstract class NiPoint3InterpController : NiSingleInterpController
	{
		int Data;
		
	}
	public class NiMaterialColorController : NiPoint3InterpController
	{
		
	}
	public class NiLightColorController : NiPoint3InterpController
	{
		
	}
	public abstract class NiExtraDataController : NiSingleInterpController
	{
		
	}
	public class NiFloatExtraDataController : NiExtraDataController
	{
		byte Num_Extra_Bytes;
		byte[] Unknown_Bytes;
		byte[] Unknown_Extra_Bytes;
		
	}
	public class NiBoneLODController : NiTimeController
	{
		uint Unknown_Int_1;
		uint Num_Node_Groups;
		uint Num_Node_Groups_2;
		NodeGroup[] Node_Groups;
		
	}
	public class NiBSBoneLODController : NiBoneLODController
	{
		
	}
	public abstract class NiGeometry : NiAVObject
	{
		int Data;
		int Skin_Instance;
		byte Unknown_Byte;
		bool Dirty_Flag;
		int Unknown_Integer_3;
		
	}
	public abstract class NiTriBasedGeom : NiGeometry
	{
		
	}
	public abstract class NiGeometryData : NiObject
	{
		ushort Num_Vertices;
		ushort BS_Max_Vertices;
		bool Has_Vertices;
		Vector3[] Vertices;
		ExtraVectorsFlags Extra_Vectors_Flags;
		bool Has_Normals;
		Vector3[] Normals;
		Vector3 Center;
		float Radius;
		bool Has_Vertex_Colors;
		Color4[] Vertex_Colors;
		ushort Num_UV_Sets;
		bool Has_UV;
		TexCoord[,] UV_Sets;
		
	}
	public abstract class AbstractAdditionalGeometryData : NiObject
	{
		
	}
	public abstract class NiTriBasedGeomData : NiGeometryData
	{
		ushort Num_Triangles;
		
	}
	public class bhkBlendController : NiTimeController
	{
		uint Unknown_Int;
		
	}
	public class BSBound : NiExtraData
	{
		Vector3 Center;
		Vector3 Dimensions;
		
	}
	public class BSFurnitureMarker : NiExtraData
	{
		uint Num_Positions;
		FurniturePosition[] Positions;
		
	}
	public class BSParentVelocityModifier : NiPSysModifier
	{
		float Damping;
		
	}
	public class BSPSysArrayEmitter : NiPSysVolumeEmitter
	{
		
	}
	public class BSWindModifier : NiPSysModifier
	{
		float Strength;
		
	}
	public class hkPackedNiTriStripsData : bhkShapeCollection
	{
		uint Num_Triangles;
		hkTriangle[] Triangles;
		uint Num_Vertices;
		Vector3[] Vertices;
		
	}
	public class NiAlphaProperty : NiProperty
	{
		ushort Flags;
		byte Threshold;
		
	}
	public class NiAmbientLight : NiLight
	{
		
	}
	public class NiParticlesData : NiGeometryData
	{
		ushort Num_Particles;
		float Particle_Radius;
		ushort Num_Active;
		bool Has_Sizes;
		float[] Sizes;
		byte Unknown_Byte_1;
		int Unknown_Link;
		float[] Rotation_Angles;
		bool Has_UV_Quadrants;
		byte Num_UV_Quadrants;
		Vector4[] UV_Quadrants;
		byte Unknown_Byte_2;
		
	}
	public class NiRotatingParticlesData : NiParticlesData
	{
		bool Has_Rotations_2;
		Quaternion[] Rotations_2;
		
	}
	public class NiAutoNormalParticlesData : NiParticlesData
	{
		
	}
	public class ParticleDesc
	{
		Vector3 Translation;
		float[] Unknown_Floats_1;
		float Unknown_Float_1;
		float Unknown_Float_2;
		float Unknown_Float_3;
		int Unknown_Int_1;
		
	}
	public class NiPSysData : NiRotatingParticlesData
	{
		ParticleDesc[] Particle_Descriptions;
		ushort Unknown_Short_1;
		ushort Unknown_Short_2;
		bool Has_Subtexture_Offset_UVs;
		uint Num_Subtexture_Offset_UVs;
		float Aspect_Ratio;
		Vector4[] Subtexture_Offset_UVs;
		uint Unknown_Int_4;
		uint Unknown_Int_5;
		uint Unknown_Int_6;
		ushort Unknown_Short_3;
		byte Unknown_Byte_4;
		
	}
	public class NiMeshPSysData : NiPSysData
	{
		int Unknown_Node;
		
	}
	public class NiBinaryExtraData : NiExtraData
	{
		ByteArray Binary_Data;
		
	}
	public class NiBinaryVoxelExtraData : NiExtraData
	{
		uint Unknown_Int;
		int Data;
		
	}
	public class NiBinaryVoxelData : NiObject
	{
		ushort Unknown_Short_1;
		ushort Unknown_Short_2;
		ushort Unknown_Short_3;
		float[] Unknown_7_Floats;
		byte[,] Unknown_Bytes_1;
		uint Num_Unknown_Vectors;
		Vector4[] Unknown_Vectors;
		uint Num_Unknown_Bytes_2;
		byte[] Unknown_Bytes_2;
		uint[] Unknown_5_Ints;
		
	}
	public class NiBlendBoolInterpolator : NiBlendInterpolator
	{
		byte Bool_Value;
		
	}
	public class NiBlendFloatInterpolator : NiBlendInterpolator
	{
		float Float_Value;
		
	}
	public class NiBlendPoint3Interpolator : NiBlendInterpolator
	{
		Vector3 Point_Value;
		
	}
	public class NiBlendTransformInterpolator : NiBlendInterpolator
	{
		
	}
	public class NiBoolData : NiObject
	{
		KeyGroup<byte> Data;
		
	}
	public class NiBooleanExtraData : NiExtraData
	{
		byte Boolean_Data;
		
	}
	public class NiBSplineBasisData : NiObject
	{
		uint Num_Control_Points;
		
	}
	public abstract class NiBSplineFloatInterpolator : NiBSplineInterpolator
	{
		
	}
	public class NiBSplineCompFloatInterpolator : NiBSplineFloatInterpolator
	{
		float Base;
		uint Offset;
		float Bias;
		float Multiplier;
		
	}
	public abstract class NiBSplinePoint3Interpolator : NiBSplineInterpolator
	{
		float[] Unknown_Floats;
		
	}
	public class NiBSplineCompPoint3Interpolator : NiBSplinePoint3Interpolator
	{
		
	}
	public class NiBSplineTransformInterpolator : NiBSplineInterpolator
	{
		Vector3 Translation;
		Quaternion Rotation;
		float Scale;
		uint Translation_Offset;
		uint Rotation_Offset;
		uint Scale_Offset;
		
	}
	public class NiBSplineCompTransformInterpolator : NiBSplineTransformInterpolator
	{
		float Translation_Bias;
		float Translation_Multiplier;
		float Rotation_Bias;
		float Rotation_Multiplier;
		float Scale_Bias;
		float Scale_Multiplier;
		
	}
	public class BSRotAccumTransfInterpolator : NiTransformInterpolator
	{
		
	}
	public class NiBSplineData : NiObject
	{
		uint Num_Float_Control_Points;
		float[] Float_Control_Points;
		uint Num_Short_Control_Points;
		short[] Short_Control_Points;
		
	}
	public class NiCamera : NiAVObject
	{
		float Frustum_Left;
		float Frustum_Right;
		float Frustum_Top;
		float Frustum_Bottom;
		float Frustum_Near;
		float Frustum_Far;
		float Viewport_Left;
		float Viewport_Right;
		float Viewport_Top;
		float Viewport_Bottom;
		float LOD_Adjust;
		int Unknown_Link;
		uint Unknown_Int;
		
	}
	public class NiColorData : NiObject
	{
		KeyGroup<Color4> Data;
		
	}
	public class NiColorExtraData : NiExtraData
	{
		Color4 Data;
		
	}
	public class NiControllerManager : NiTimeController
	{
		bool Cumulative;
		uint Num_Controller_Sequences;
		int[] Controller_Sequences;
		int Object_Palette;
		
	}
	public class NiSequence : NiObject
	{
		NIFString Name;
		NIFString Text_Keys_Name;
		int Text_Keys;
		uint Num_Controlled_Blocks;
		ControllerLink[] Controlled_Blocks;
		
	}
	public class NiControllerSequence : NiSequence
	{
		
	}
	public abstract class NiAVObjectPalette : NiObject
	{
		
	}
	public class NiDefaultAVObjectPalette : NiAVObjectPalette
	{
		uint Unknown_Int;
		uint Num_Objs;
		AVObject[] Objs;
		
	}
	public class NiDirectionalLight : NiLight
	{
		
	}
	public class NiDitherProperty : NiProperty
	{
		ushort Flags;
		
	}
	public class NiRollController : NiSingleInterpController
	{
		int Data;
		
	}
	public class NiFloatData : NiObject
	{
		KeyGroup<float> Data;
		
	}
	public class NiFloatExtraData : NiExtraData
	{
		float Float_Data;
		
	}
	public class NiFloatsExtraData : NiExtraData
	{
		uint Num_Floats;
		float[] Data;
		
	}
	public class NiFogProperty : NiProperty
	{
		ushort Flags;
		float Fog_Depth;
		Color3 Fog_Color;
		
	}
	public class NiGravity : NiParticleModifier
	{
		float Unknown_Float_1;
		float Force;
		FieldType Type;
		Vector3 Position;
		Vector3 Direction;
		
	}
	public class NiIntegerExtraData : NiExtraData
	{
		uint Integer_Data;
		
	}
	public class BSXFlags : NiIntegerExtraData
	{
		
	}
	public class NiIntegersExtraData : NiExtraData
	{
		uint Num_Integers;
		uint[] Data;
		
	}
	public class BSKeyframeController : NiKeyframeController
	{
		int Data_2;
		
	}
	public class NiKeyframeData : NiObject
	{
		uint Num_Rotation_Keys;
		KeyType Rotation_Type;
		QuatKey<Quaternion>[] Quaternion_Keys;
		float Unknown_Float;
		KeyGroup<float>[] XYZ_Rotations;
		KeyGroup<Vector3> Translations;
		KeyGroup<float> Scales;
		
	}
	public class NiLookAtController : NiTimeController
	{
		int Look_At_Node;
		
	}
	public class NiLookAtInterpolator : NiInterpolator
	{
		ushort Unknown_Short;
		int Look_At;
		NIFString Target;
		Vector3 Translation;
		Quaternion Rotation;
		float Scale;
		int Unknown_Link_1;
		int Unknown_Link_2;
		int Unknown_Link_3;
		
	}
	public class NiMaterialProperty : NiProperty
	{
		ushort Flags;
		Color3 Ambient_Color;
		Color3 Diffuse_Color;
		Color3 Specular_Color;
		Color3 Emissive_Color;
		float Glossiness;
		float Alpha;
		float Emit_Multi;
		
	}
	public class NiMorphData : NiObject
	{
		uint Num_Morphs;
		uint Num_Vertices;
		byte Relative_Targets;
		Morph[] Morphs;
		
	}
	public class NiNode : NiAVObject
	{
		uint Num_Children;
		int[] Children;
		uint Num_Effects;
		int[] Effects;
		
	}
	public class NiBone : NiNode
	{
		
	}
	public class AvoidNode : NiNode
	{
		
	}
	public class FxWidget : NiNode
	{
		byte Unknown_3;
		byte[] Unknown_292_Bytes;
		
	}
	public class FxButton : FxWidget
	{
		
	}
	public class FxRadioButton : FxWidget
	{
		uint Unknown_Int_1;
		uint Unknown_Int__2;
		uint Unknown_Int_3;
		uint Num_Buttons;
		int[] Buttons;
		
	}
	public class NiBillboardNode : NiNode
	{
		
	}
	public class NiBSAnimationNode : NiNode
	{
		
	}
	public class NiBSParticleNode : NiNode
	{
		
	}
	public class NiSwitchNode : NiNode
	{
		int Unknown_Int_1;
		
	}
	public class NiLODNode : NiSwitchNode
	{
		Vector3 LOD_Center;
		uint Num_LOD_Levels;
		LODRange[] LOD_Levels;
		
	}
	public class NiPalette : NiObject
	{
		byte Unknown_Byte;
		uint Num_Entries;
		ByteColor4[] Palette;
		
	}
	public class NiParticleBomb : NiParticleModifier
	{
		float Decay;
		float Duration;
		float DeltaV;
		float Start;
		DecayType Decay_Type;
		Vector3 Position;
		Vector3 Direction;
		
	}
	public class NiParticleColorModifier : NiParticleModifier
	{
		int Color_Data;
		
	}
	public class NiParticleGrowFade : NiParticleModifier
	{
		float Grow;
		float Fade;
		
	}
	public class NiParticleMeshModifier : NiParticleModifier
	{
		uint Num_Particle_Meshes;
		int[] Particle_Meshes;
		
	}
	public class NiParticleRotation : NiParticleModifier
	{
		byte Random_Initial_Axis;
		Vector3 Initial_Axis;
		float Rotation_Speed;
		
	}
	public class NiParticles : NiGeometry
	{
		
	}
	public class NiAutoNormalParticles : NiParticles
	{
		
	}
	public class NiParticleMeshes : NiParticles
	{
		
	}
	public class NiParticleMeshesData : NiRotatingParticlesData
	{
		int Unknown_Link_2;
		
	}
	public class NiParticleSystem : NiParticles
	{
		ushort Unknown_Short_2;
		ushort Unknown_Short_3;
		uint Unknown_Int_1;
		int Unknown_Int_2;
		int Unknown_Int_3;
		int Data;
		
	}
	public class NiMeshParticleSystem : NiParticleSystem
	{
		
	}
	public class NiParticleSystemController : NiTimeController
	{
		float Speed;
		float Speed_Random;
		float Vertical_Direction;
		float Vertical_Angle;
		float Horizontal_Direction;
		float Horizontal_Angle;
		Vector3 Unknown_Normal;
		Color4 Unknown_Color;
		float Size;
		float Emit_Start_Time;
		float Emit_Stop_Time;
		byte Unknown_Byte;
		float Emit_Rate;
		float Lifetime;
		float Lifetime_Random;
		ushort Emit_Flags;
		Vector3 Start_Random;
		int Emitter;
		ushort Unknown_Short_2;
		float Unknown_Float_13;
		uint Unknown_Int_1;
		uint Unknown_Int_2;
		ushort Unknown_Short_3;
		ushort Num_Particles;
		ushort Num_Valid;
		Particle[] Particles;
		int Unknown_Link;
		int Particle_Extra;
		int Unknown_Link_2;
		byte Trailer;
		
	}
	public class NiBSPArrayController : NiParticleSystemController
	{
		
	}
	public class NiPathController : NiTimeController
	{
		uint Unknown_Int_1;
		float Unknown_Float_2;
		float Unknown_Float_3;
		ushort Unknown_Short;
		int Pos_Data;
		int Float_Data;
		
	}
	public enum ChannelType : uint
	{
		CHNL_RED = 0,
		CHNL_GREEN = 1,
		CHNL_BLUE = 2,
		CHNL_ALPHA = 3,
		CHNL_COMPRESSED = 4,
		CHNL_INDEX = 16,
		CHNL_EMPTY = 19,
		
	}
	public enum ChannelConvention : uint
	{
		CC_FIXED = 0,
		CC_INDEX = 3,
		CC_COMPRESSED = 4,
		CC_EMPTY = 5,
		
	}
	public class ChannelData
	{
		ChannelType Type;
		ChannelConvention Convention;
		byte Bits_Per_Channel;
		byte Unknown_Byte_1;
		
	}
	public abstract class ATextureRenderData : NiObject
	{
		PixelFormat Pixel_Format;
		uint Red_Mask;
		uint Green_Mask;
		uint Blue_Mask;
		uint Alpha_Mask;
		byte Bits_Per_Pixel;
		byte[] Unknown_3_Bytes;
		byte[] Unknown_8_Bytes;
		int Palette;
		uint Num_Mipmaps;
		uint Bytes_Per_Pixel;
		MipMap[] Mipmaps;
		
	}
	public class NiPersistentSrcTextureRendererData : ATextureRenderData
	{
		uint Num_Pixels;
		uint Unknown_Int_6;
		uint Num_Faces;
		uint Unknown_Int_7;
		byte[,] Pixel_Data;
		
	}
	public class NiPixelData : ATextureRenderData
	{
		uint Num_Pixels;
		byte[,] Pixel_Data;
		
	}
	public class NiPlanarCollider : NiParticleModifier
	{
		float Unknown_Float_1;
		float Unknown_Float_2;
		float Unknown_Float_3;
		float Unknown_Float_4;
		float Unknown_Float_5;
		float Unknown_Float_6;
		float Unknown_Float_7;
		float Unknown_Float_8;
		float Unknown_Float_9;
		float Unknown_Float_10;
		float Unknown_Float_11;
		float Unknown_Float_12;
		float Unknown_Float_13;
		float Unknown_Float_14;
		float Unknown_Float_15;
		float Unknown_Float_16;
		
	}
	public class NiPointLight : NiLight
	{
		float Constant_Attenuation;
		float Linear_Attenuation;
		float Quadratic_Attenuation;
		
	}
	public class NiPosData : NiObject
	{
		KeyGroup<Vector3> Data;
		
	}
	public class NiPSysAgeDeathModifier : NiPSysModifier
	{
		bool Spawn_on_Death;
		int Spawn_Modifier;
		
	}
	public class NiPSysBombModifier : NiPSysModifier
	{
		int Bomb_Object;
		Vector3 Bomb_Axis;
		float Decay;
		float Delta_V;
		DecayType Decay_Type;
		SymmetryType Symmetry_Type;
		
	}
	public class NiPSysBoundUpdateModifier : NiPSysModifier
	{
		ushort Update_Skip;
		
	}
	public class NiPSysBoxEmitter : NiPSysVolumeEmitter
	{
		float Width;
		float Height;
		float Depth;
		
	}
	public class NiPSysColliderManager : NiPSysModifier
	{
		int Collider;
		
	}
	public class NiPSysColorModifier : NiPSysModifier
	{
		int Data;
		
	}
	public class NiPSysCylinderEmitter : NiPSysVolumeEmitter
	{
		float Radius;
		float Height;
		
	}
	public class NiPSysDragModifier : NiPSysModifier
	{
		int Parent;
		Vector3 Drag_Axis;
		float Percentage;
		float Range;
		float Range_Falloff;
		
	}
	public class NiPSysEmitterCtlrData : NiObject
	{
		KeyGroup<float> Float_Keys;
		uint Num_Visibility_Keys;
		Key<byte>[] Visibility_Keys;
		
	}
	public class NiPSysGravityModifier : NiPSysModifier
	{
		int Gravity_Object;
		Vector3 Gravity_Axis;
		float Decay;
		float Strength;
		ForceType Force_Type;
		float Turbulence;
		float Turbulence_Scale;
		
	}
	public class NiPSysGrowFadeModifier : NiPSysModifier
	{
		float Grow_Time;
		ushort Grow_Generation;
		float Fade_Time;
		ushort Fade_Generation;
		
	}
	public class NiPSysMeshEmitter : NiPSysEmitter
	{
		uint Num_Emitter_Meshes;
		int[] Emitter_Meshes;
		VelocityType Initial_Velocity_Type;
		EmitFrom Emission_Type;
		Vector3 Emission_Axis;
		
	}
	public class NiPSysMeshUpdateModifier : NiPSysModifier
	{
		uint Num_Meshes;
		int[] Meshes;
		
	}
	public class BSPSysInheritVelocityModifier : NiPSysModifier
	{
		uint Unknown_Int_1;
		float Unknown_Float_1;
		float Unknown_Float_2;
		float Unknown_Float_3;
		
	}
	public class BSPSysHavokUpdateModifier : NiPSysModifier
	{
		uint Num_Nodes;
		int[] Nodes;
		int Modifier;
		
	}
	public class BSPSysRecycleBoundModifier : NiPSysModifier
	{
		float Unknown_Float_1;
		float Unknown_Float_2;
		float Unknown_Float_3;
		float Unknown_Float_4;
		float Unknown_Float_5;
		float Unknown_Float_6;
		uint Unknown_Int_1;
		
	}
	public class BSPSysSubTexModifier : NiPSysModifier
	{
		uint Start_Frame;
		float Start_Frame_Fudge;
		float End_Frame;
		float Loop_Start_Frame;
		float Loop_Start_Frame_Fudge;
		float Frame_Count;
		float Frame_Count_Fudge;
		
	}
	public class NiPSysPlanarCollider : NiPSysCollider
	{
		float Width;
		float Height;
		Vector3 X_Axis;
		Vector3 Y_Axis;
		
	}
	public class NiPSysSphericalCollider : NiPSysCollider
	{
		float Radius;
		
	}
	public class NiPSysPositionModifier : NiPSysModifier
	{
		
	}
	public class NiPSysResetOnLoopCtlr : NiTimeController
	{
		
	}
	public class NiPSysRotationModifier : NiPSysModifier
	{
		float Initial_Rotation_Speed;
		bool Random_Initial_Axis;
		Vector3 Initial_Axis;
		
	}
	public class NiPSysSpawnModifier : NiPSysModifier
	{
		ushort Num_Spawn_Generations;
		float Percentage_Spawned;
		ushort Min_Num_to_Spawn;
		ushort Max_Num_to_Spawn;
		float Spawn_Speed_Chaos;
		float Spawn_Dir_Chaos;
		float Life_Span;
		float Life_Span_Variation;
		
	}
	public class NiPSysSphereEmitter : NiPSysVolumeEmitter
	{
		float Radius;
		
	}
	public class NiPSysUpdateCtlr : NiTimeController
	{
		
	}
	public abstract class NiPSysFieldModifier : NiPSysModifier
	{
		int Field_Object;
		float Magnitude;
		float Attenuation;
		bool Use_Max_Distance;
		float Max_Distance;
		
	}
	public class NiPSysVortexFieldModifier : NiPSysFieldModifier
	{
		Vector3 Direction;
		
	}
	public class NiPSysGravityFieldModifier : NiPSysFieldModifier
	{
		Vector3 Direction;
		
	}
	public class NiPSysDragFieldModifier : NiPSysFieldModifier
	{
		bool Use_Direction;
		Vector3 Direction;
		
	}
	public class NiPSysTurbulenceFieldModifier : NiPSysFieldModifier
	{
		float Frequency;
		
	}
	public class BSPSysLODModifier : NiPSysModifier
	{
		float Uknown_Float_1;
		float Uknown_Float_2;
		float Uknown_Float_3;
		float Uknown_Float_4;
		
	}
	public class BSPSysScaleModifier : NiPSysModifier
	{
		uint Num_Floats;
		float[] Floats;
		
	}
	public class NiPSysFieldMagnitudeCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysFieldAttenuationCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysFieldMaxDistanceCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysAirFieldAirFrictionCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysAirFieldInheritVelocityCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysAirFieldSpreadCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysInitialRotSpeedCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysInitialRotSpeedVarCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysInitialRotAngleCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysInitialRotAngleVarCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysEmitterPlanarAngleCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysEmitterPlanarAngleVarCtlr : NiPSysModifierFloatCtlr
	{
		
	}
	public class NiPSysAirFieldModifier : NiPSysFieldModifier
	{
		Vector3 Direction;
		float Unknown_Float_2;
		float Unknown_Float_3;
		bool Unknown_Boolean_1;
		bool Unknown_Boolean_2;
		bool Unknown_Boolean_3;
		float Unknown_Float_4;
		
	}
	public class NiPSysTrailEmitter : NiPSysEmitter
	{
		int Unknown_Int_1;
		float Unknown_Float_1;
		float Unknown_Float_2;
		float Unknown_Float_3;
		int Unknown_Int_2;
		float Unknown_Float_4;
		int Unknown_Int_3;
		float Unknown_Float_5;
		int Unknown_Int_4;
		float Unknown_Float_6;
		float Unknown_Float_7;
		
	}
	public class NiLightIntensityController : NiFloatInterpController
	{
		
	}
	public class NiPSysRadialFieldModifier : NiPSysFieldModifier
	{
		int Radial_Type;
		
	}
	public abstract class NiLODData : NiObject
	{
		
	}
	public class NiRangeLODData : NiLODData
	{
		Vector3 LOD_Center;
		uint Num_LOD_Levels;
		LODRange[] LOD_Levels;
		
	}
	public class NiScreenLODData : NiLODData
	{
		Vector3 Bound_Center;
		float Bound_Radius;
		Vector3 World_Center;
		float World_Radius;
		uint Proportion_Count;
		float[] Proportion_Levels;
		
	}
	public class NiRotatingParticles : NiParticles
	{
		
	}
	public class NiSequenceStreamHelper : NiObjectNET
	{
		
	}
	public class NiShadeProperty : NiProperty
	{
		ushort Flags;
		
	}
	public class NiSkinData : NiObject
	{
		SkinTransform Skin_Transform;
		uint Num_Bones;
		int Skin_Partition;
		SkinData[] Bone_List;
		
	}
	public class NiSkinInstance : NiObject
	{
		int Data;
		int Skeleton_Root;
		uint Num_Bones;
		int[] Bones;
		
	}
	public class NiTriShapeSkinController : NiTimeController
	{
		uint Num_Bones;
		uint[] Vertex_Counts;
		int[] Bones;
		OldSkinData[,] Bone_Data;
		
	}
	public class NiClodSkinInstance : NiSkinInstance
	{
		
	}
	public class NiSkinPartition : NiObject
	{
		uint Num_Skin_Partition_Blocks;
		SkinPartition[] Skin_Partition_Blocks;
		
	}
	public abstract class NiTexture : NiObjectNET
	{
		
	}
	public class NiSourceTexture : NiTexture
	{
		byte Use_External;
		FilePath File_Name;
		byte Unknown_Byte;
		int Pixel_Data;
		PixelLayout Pixel_Layout;
		MipMapFormat Use_Mipmaps;
		AlphaFormat Alpha_Format;
		byte Is_Static;
		
	}
	public class NiSpecularProperty : NiProperty
	{
		ushort Flags;
		
	}
	public class NiSphericalCollider : NiParticleModifier
	{
		float Unknown_Float_1;
		ushort Unknown_Short_1;
		float Unknown_Float_2;
		ushort Unknown_Short_2;
		float Unknown_Float_4;
		float Unknown_Float_5;
		
	}
	public class NiSpotLight : NiPointLight
	{
		float Cutoff_Angle;
		float Exponent;
		
	}
	public class NiStencilProperty : NiProperty
	{
		ushort Flags;
		byte Stencil_Enabled;
		StencilCompareMode Stencil_Function;
		uint Stencil_Ref;
		uint Stencil_Mask;
		StencilAction Fail_Action;
		StencilAction Z_Fail_Action;
		StencilAction Pass_Action;
		FaceDrawMode Draw_Mode;
		
	}
	public class NiStringExtraData : NiExtraData
	{
		uint Bytes_Remaining;
		NIFString String_Data;
		
	}
	public class NiStringPalette : NiObject
	{
		StringPalette Palette;
		
	}
	public class NiStringsExtraData : NiExtraData
	{
		uint Num_Strings;
		SizedString[] Data;
		
	}
	public class NiTextKeyExtraData : NiExtraData
	{
		uint Unknown_Int_1;
		uint Num_Text_Keys;
		Key<NIFString>[] Text_Keys;
		
	}
	public class NiTextureEffect : NiDynamicEffect
	{
		Matrix33 Model_Projection_Matrix;
		Vector3 Model_Projection_Transform;
		TexFilterMode Texture_Filtering;
		TexClampMode Texture_Clamping;
		EffectType Texture_Type;
		CoordGenType Coordinate_Generation_Type;
		int Source_Texture;
		byte Clipping_Plane;
		Vector3 Unknown_Vector;
		float Unknown_Float;
		short PS2_L;
		short PS2_K;
		ushort Unknown_Short;
		
	}
	public class NiTextureModeProperty : NiProperty
	{
		short Unknown_Short;
		short PS2_L;
		short PS2_K;
		
	}
	public class NiImage : NiObject
	{
		byte Use_External;
		FilePath File_Name;
		int Image_Data;
		uint Unknown_Int;
		float Unknown_Float;
		
	}
	public class NiTextureProperty : NiProperty
	{
		ushort Flags;
		int Image;
		
	}
	public class NiMultiTextureProperty : NiProperty
	{
		ushort Flags;
		uint Unknown_Int;
		MultiTextureElement[] Texture_Elements;
		
	}
	public class NiTexturingProperty : NiProperty
	{
		ushort Flags;
		ApplyMode Apply_Mode;
		uint Texture_Count;
		bool Has_Base_Texture;
		TexDesc Base_Texture;
		bool Has_Dark_Texture;
		TexDesc Dark_Texture;
		bool Has_Detail_Texture;
		TexDesc Detail_Texture;
		bool Has_Gloss_Texture;
		TexDesc Gloss_Texture;
		bool Has_Glow_Texture;
		TexDesc Glow_Texture;
		bool Has_Bump_Map_Texture;
		TexDesc Bump_Map_Texture;
		float Bump_Map_Luma_Scale;
		float Bump_Map_Luma_Offset;
		Matrix22 Bump_Map_Matrix;
		float Unknown2_Float;
		bool Has_Decal_0_Texture;
		TexDesc Decal_0_Texture;
		bool Has_Decal_1_Texture;
		TexDesc Decal_1_Texture;
		bool Has_Decal_2_Texture;
		TexDesc Decal_2_Texture;
		bool Has_Decal_3_Texture;
		TexDesc Decal_3_Texture;
		
	}
	public class NiTransformData : NiKeyframeData
	{
		
	}
	public class NiTriShape : NiTriBasedGeom
	{
		
	}
	public class NiTriShapeData : NiTriBasedGeomData
	{
		uint Num_Triangle_Points;
		Triangle[] Triangles;
		ushort Num_Match_Groups;
		MatchGroup[] Match_Groups;
		
	}
	public class NiTriStrips : NiTriBasedGeom
	{
		
	}
	public class NiTriStripsData : NiTriBasedGeomData
	{
		ushort Num_Strips;
		ushort[] Strip_Lengths;
		ushort[,] Points;
		
	}
	public class NiEnvMappedTriShape : NiObjectNET
	{
		ushort Unknown_1;
		Matrix44 Unknown_Matrix;
		uint Num_Children;
		int[] Children;
		int Child_2;
		int Child_3;
		
	}
	public class NiEnvMappedTriShapeData : NiTriShapeData
	{
		
	}
	public class NiBezierTriangle4 : NiObject
	{
		uint[] Unknown_1;
		ushort Unknown_2;
		Matrix33 Matrix;
		Vector3 Vector_1;
		Vector3 Vector_2;
		short[] Unknown_3;
		byte Unknown_4;
		uint Unknown_5;
		short[] Unknown_6;
		
	}
	public class NiBezierMesh : NiAVObject
	{
		uint Num_Bezier_Triangles;
		int[] Bezier_Triangle;
		uint Unknown_3;
		ushort Count_1;
		ushort Unknown_4;
		Vector3[] Points_1;
		uint Unknown_5;
		float[,] Points_2;
		uint Unknown_6;
		ushort Count_2;
		ushort[,] Data_2;
		
	}
	public class NiClod : NiTriBasedGeom
	{
		
	}
	public class NiClodData : NiTriBasedGeomData
	{
		ushort Unknown_Shorts;
		ushort Unknown_Count_1;
		ushort Unknown_Count_2;
		ushort Unknown_Count_3;
		float Unknown_Float;
		ushort Unknown_Short;
		ushort[,] Unknown_Clod_Shorts_1;
		ushort[] Unknown_Clod_Shorts_2;
		ushort[,] Unknown_Clod_Shorts_3;
		
	}
	public class NiUVController : NiTimeController
	{
		ushort Unknown_Short;
		int Data;
		
	}
	public class NiUVData : NiObject
	{
		KeyGroup<float>[] UV_Groups;
		
	}
	public class NiVectorExtraData : NiExtraData
	{
		Vector3 Vector_Data;
		float Unknown_Float;
		
	}
	public class NiVertexColorProperty : NiProperty
	{
		ushort Flags;
		VertMode Vertex_Mode;
		LightMode Lighting_Mode;
		
	}
	public class NiVertWeightsExtraData : NiExtraData
	{
		uint Num_Bytes;
		ushort Num_Vertices;
		float[] Weight;
		
	}
	public class NiVisData : NiObject
	{
		uint Num_Keys;
		Key<byte>[] Keys;
		
	}
	public class NiWireframeProperty : NiProperty
	{
		ushort Flags;
		
	}
	public class NiZBufferProperty : NiProperty
	{
		ushort Flags;
		
	}
	public class RootCollisionNode : NiNode
	{
		
	}
	public class NiRawImageData : NiObject
	{
		uint Width;
		uint Height;
		ImageType Image_Type;
		ByteColor3[,] RGB_Image_Data;
		ByteColor4[,] RGBA_Image_Data;
		
	}
	public class NiSortAdjustNode : NiNode
	{
		SortingMode Sorting_Mode;
		int Unknown_Int_2;
		
	}
	public class NiSourceCubeMap : NiSourceTexture
	{
		
	}
	public class NiPhysXProp : NiObjectNET
	{
		float Unknown_Float_1;
		uint Unknown_Int_1;
		int[] Unknown_Refs_1;
		int Num_Dests;
		int[] Transform_Dests;
		byte Unknown_Byte;
		int Prop_Description;
		
	}
	public class physXMaterialRef
	{
		byte Number;
		byte Unknown_Byte_1;
		int Material_Desc;
		
	}
	public class NiPhysXPropDesc : NiObject
	{
		int Num_Dests;
		int[] Actor_Descs;
		uint Num_Joints;
		int[] Joint_Descs;
		int Unknown_Int_1;
		uint Num_Materials;
		physXMaterialRef[] Material_Descs;
		uint Unknown_Int_2;
		
	}
	public class NiPhysXActorDesc : NiObject
	{
		int Unknown_Int_1;
		int Unknown_Int_2;
		Quaternion Unknown_Quat_1;
		Quaternion Unknown_Quat_2;
		Quaternion Unknown_Quat_3;
		int Unknown_Ref_0;
		float Unknown_Int_4;
		int Unknown_Int_5;
		byte Unknown_Byte_1;
		byte Unknown_Byte_2;
		int Unknown_Int_6;
		int Shape_Description;
		int Unknown_Ref_1;
		int Unknown_Ref_2;
		int[] Unknown_Refs_3;
		
	}
	public class NiPhysXBodyDesc : NiObject
	{
		
	}
	public class NiPhysXD6JointDesc : NiObject
	{
		
	}
	public class NiPhysXShapeDesc : NiObject
	{
		int Unknown_Int_1;
		Quaternion Unknown_Quat_1;
		Quaternion Unknown_Quat_2;
		Quaternion Unknown_Quat_3;
		short Unknown_Short_1;
		int Unknown_Int_2;
		short Unknown_Short_2;
		float Unknown_Float_1;
		float Unknown_Float_2;
		float Unknown_Float_3;
		int Unknown_Int_3;
		int Unknown_Int_4;
		int Unknown_Int_5;
		int Unknown_Int_7;
		int Unknown_Int_8;
		int Mesh_Description;
		
	}
	public class NiPhysXMeshDesc : NiObject
	{
		short Unknown_Short_1;
		float Unknown_Float_1;
		short Unknown_Short_2;
		byte[] Unknown_Bytes_0;
		byte Unknown_Byte_1;
		byte[] Unknown_Bytes_1;
		byte[] Unknown_Bytes_2;
		float Unknown_Float_2;
		int Unknown_Int_1;
		int Unknown_Int_2;
		int Num_Vertices;
		int Unknown_Int_4;
		Vector3[] Vertices;
		byte[] Unknown_Bytes_3;
		short[] Unknown_Shorts_1;
		uint[] Unknown_Ints_1;
		byte Unknown_Byte_2;
		
	}
	public class NiPhysXMaterialDesc : NiObject
	{
		uint[] Unknown_Int;
		byte Unknown_Byte_1;
		byte Unknown_Byte_2;
		
	}
	public class NiPhysXKinematicSrc : NiObject
	{
		
	}
	public class NiPhysXTransformDest : NiObject
	{
		byte Unknown_Byte_1;
		byte Unknown_Byte_2;
		int Node;
		
	}
	public class NiArkAnimationExtraData : NiExtraData
	{
		int[] Unknown_Ints;
		byte[] Unknown_Bytes;
		
	}
	public class NiArkImporterExtraData : NiExtraData
	{
		int Unknown_Int_1;
		int Unknown_Int_2;
		NIFString Importer_Name;
		byte[] Unknown_Bytes;
		float[] Unknown_Floats;
		
	}
	public class NiArkTextureExtraData : NiExtraData
	{
		int[] Unknown_Ints_1;
		byte Unknown_Byte;
		int Unknown_Int_2;
		int Num_Textures;
		ArkTexture[] Textures;
		
	}
	public class NiArkViewportInfoExtraData : NiExtraData
	{
		byte[] Unknown_Bytes;
		
	}
	public class NiArkShaderExtraData : NiExtraData
	{
		int Unknown_Int;
		NIFString Unknown_String;
		
	}
	public class NiLines : NiTriBasedGeom
	{
		
	}
	public class NiLinesData : NiGeometryData
	{
		bool[] Lines;
		
	}
	public class Polygon
	{
		ushort Num_Vertices;
		ushort Vertex_Offset;
		ushort Num_Triangles;
		ushort Triangle_Offset;
		
	}
	public class NiScreenElementsData : NiTriShapeData
	{
		ushort Max_Polygons;
		Polygon[] Polygons;
		ushort[] Polygon_Indices;
		ushort Unknown_UShort_1;
		ushort Num_Polygons;
		ushort Used_Vertices;
		ushort Unknown_UShort_2;
		ushort Used_Triangle_Points;
		ushort Unknown_UShort_3;
		
	}
	public class NiScreenElements : NiTriShape
	{
		
	}
	public class NiRoomGroup : NiNode
	{
		int Shell_Link;
		int Num_Rooms;
		int[] Rooms;
		
	}
	public class NiRoom : NiNode
	{
		int Num_Walls;
		Vector4[] Wall_Plane;
		int Num_In_Portals;
		int[] In_Portals;
		int Num_Portals_2;
		int[] Portals_2;
		int Num_Items;
		int[] Items;
		
	}
	public class NiPortal : NiAVObject
	{
		ushort Unknown_Flags;
		short Unknown_Short_2;
		ushort Num_Vertices;
		Vector3[] Vertices;
		int Target;
		
	}
	public class BSFadeNode : NiNode
	{
		
	}
	public enum BSShaderType : uint
	{
		SHADER_TALL_GRASS = 0,
		SHADER_DEFAULT = 1,
		SHADER_SKY = 10,
		SHADER_SKIN = 14,
		SHADER_WATER = 17,
		SHADER_LIGHTING30 = 29,
		SHADER_TILE = 32,
		SHADER_NOLIGHTING = 33,
		
	}
	[Flags]
	public enum BSShaderFlags : uint
	{
		SF_Specular = 0,
		SF_Skinned = 1,
		SF_LowDetail = 2,
		SF_Vertex_Alpha = 3,
		SF_Unknown_1 = 4,
		SF_Single_Pass = 5,
		SF_Empty = 6,
		SF_Environment_Mapping = 7,
		SF_Alpha_Texture = 8,
		SF_Unknown_2 = 9,
		SF_FaceGen = 10,
		SF_Parallax_Shader_Index_15 = 11,
		SF_Unknown_3 = 12,
		SF_Non_Projective_Shadows = 13,
		SF_Unknown_4 = 14,
		SF_Refraction = 15,
		SF_Fire_Refraction = 16,
		SF_Eye_Environment_Mapping = 17,
		SF_Hair = 18,
		SF_Dynamic_Alpha = 19,
		SF_Localmap_Hide_Secret = 20,
		SF_Window_Environment_Mapping = 21,
		SF_Tree_Billboard = 22,
		SF_Shadow_Frustum = 23,
		SF_Multiple_Textures = 24,
		SF_Remappable_Textures = 25,
		SF_Decal_Single_Pass = 26,
		SF_Dynamic_Decal_Single_Pass = 27,
		SF_Parallax_Occulsion = 28,
		SF_External_Emittance = 29,
		SF_Shadow_Map = 30,
		SF_ZBuffer_Test = 31,
		
	}
	[Flags]
	public enum BSShaderFlags2 : uint
	{
		SF2_ZBuffer_Write = 0,
		SF2_LOD_Landscape = 1,
		SF2_LOD_Building = 2,
		SF2_No_Fade = 3,
		SF2_Refraction_Tint = 4,
		SF2_Vertex_Colors = 5,
		SF2_Unknown1 = 6,
		SF2_1st_Light_is_Point_Light = 7,
		SF2_2nd_Light = 8,
		SF2_3rd_Light = 9,
		SF2_Vertex_Lighting = 10,
		SF2_Uniform_Scale = 11,
		SF2_Fit_Slope = 12,
		SF2_Billboard_and_Envmap_Light_Fade = 13,
		SF2_No_LOD_Land_Blend = 14,
		SF2_Envmap_Light_Fade = 15,
		SF2_Wireframe = 16,
		SF2_VATS_Selection = 17,
		SF2_Show_in_Local_Map = 18,
		SF2_Premult_Alpha = 19,
		SF2_Skip_Normal_Maps = 20,
		SF2_Alpha_Decal = 21,
		SF2_No_Transparecny_Multisampling = 22,
		SF2_Unknown2 = 23,
		SF2_Unknown3 = 24,
		SF2_Unknown4 = 25,
		SF2_Unknown5 = 26,
		SF2_Unknown6 = 27,
		SF2_Unknown7 = 28,
		SF2_Unknown8 = 29,
		SF2_Unknown9 = 30,
		SF2_Unknown10 = 31,
		
	}
	public class BSShaderProperty : NiProperty
	{
		ushort Smooth;
		BSShaderType Shader_Type;
		BSShaderFlags Shader_Flags;
		BSShaderFlags2 Shader_Flags_2;
		float Environment_Map_Scale;
		
	}
	public abstract class BSShaderLightingProperty : BSShaderProperty
	{
		TexClampMode Texture_Clamp_Mode;
		
	}
	public class BSShaderNoLightingProperty : BSShaderLightingProperty
	{
		SizedString File_Name;
		float Falloff_Start_Angle;
		float Falloff_Stop_Angle;
		float Falloff_Start_Opacity;
		float Falloff_Stop_Opacity;
		
	}
	public class BSShaderPPLightingProperty : BSShaderLightingProperty
	{
		int Texture_Set;
		float Refraction_Strength;
		int Refraction_Fire_Period;
		float Unknown_Float_4;
		float Unknown_Float_5;
		Color4 Emissive_Color;
		
	}
	public class BSShaderTextureSet : NiObject
	{
		int Num_Textures;
		SizedString[] Textures;
		
	}
	public class WaterShaderProperty : BSShaderProperty
	{
		
	}
	public enum SkyObjectType : uint
	{
		BSSM_SKY_TEXTURE = 0,
		BSSM_SKY_SUNGLARE = 1,
		BSSM_SKY = 2,
		BSSM_SKY_CLOUDS = 3,
		BSSM_SKY_STARS = 5,
		BSSM_SKY_MOON_STARS_MASK = 7,
		
	}
	public class SkyShaderProperty : BSShaderLightingProperty
	{
		SizedString File_Name;
		SkyObjectType Sky_Object_Type;
		
	}
	public class TileShaderProperty : BSShaderLightingProperty
	{
		SizedString File_Name;
		
	}
	public class DistantLODShaderProperty : BSShaderProperty
	{
		
	}
	public class BSDistantTreeShaderProperty : BSShaderProperty
	{
		
	}
	public class TallGrassShaderProperty : BSShaderProperty
	{
		SizedString File_Name;
		
	}
	public class VolumetricFogShaderProperty : BSShaderProperty
	{
		
	}
	public class HairShaderProperty : BSShaderProperty
	{
		
	}
	public class Lighting30ShaderProperty : BSShaderPPLightingProperty
	{
		
	}
	public class BSDismemberSkinInstance : NiSkinInstance
	{
		int Num_Partitions;
		BodyPartList[] Partitions;
		
	}
	public class BSDecalPlacementVectorExtraData : NiExtraData
	{
		float Unknown_Float_1;
		short Num_Vector_Blocks;
		DecalVectorArray[] Vector_Blocks;
		
	}
	public class BSPSysSimpleColorModifier : NiPSysModifier
	{
		float Fade_In_Percent;
		float Fade_out_Percent;
		float Color_1_End_Percent;
		float Color_1_Start_Percent;
		float Color_2_End_Percent;
		float Color_2_Start_Percent;
		Color4[] Colors;
		
	}
	public class BSValueNode : NiNode
	{
		int Value;
		byte Unknown_byte;
		
	}
	public class BSStripParticleSystem : NiParticleSystem
	{
		
	}
	public class BSStripPSysData : NiPSysData
	{
		short Unknown_Short_5;
		byte Unknown_Byte_6;
		int Unknown_Int_7;
		float Unknown_Float_8;
		
	}
	public class BSPSysStripUpdateModifier : NiPSysModifier
	{
		float Update_Delta_Time;
		
	}
	public class BSMaterialEmittanceMultController : NiFloatInterpController
	{
		
	}
	public class BSMasterParticleSystem : NiNode
	{
		ushort Max_Emitter_Objects;
		int Num_Particle_Systems;
		int[] Particle_Systems;
		
	}
	public class BSPSysMultiTargetEmitterCtlr : NiPSysModifierCtlr
	{
		int Data;
		short Unknown_Short_1;
		int Unknown_Int_1;
		
	}
	public class BSRefractionStrengthController : NiFloatInterpController
	{
		
	}
	public class BSOrderedNode : NiNode
	{
		Vector4 Alpha_Sort_Bound;
		byte Is_Static_Bound;
		
	}
	public class BSBlastNode : NiNode
	{
		byte Unknown_Byte_1;
		short Unknown_Short_2;
		
	}
	public class BSDamageStage : NiNode
	{
		byte Unknown_Byte_1;
		short Unknown_Short_2;
		
	}
	public class BSRefractionFirePeriodController : NiTimeController
	{
		
	}
	public class bhkConvexListShape : bhkShape
	{
		uint Num_Sub_Shapes;
		int[] Sub_Shapes;
		HavokMaterial Material;
		float[] Unknown_Floats;
		byte Unknown_Byte_1;
		float Unknown_Float_1;
		
	}
	public class BSTreadTransformData
	{
		Vector3 Translation;
		Quaternion Rotation;
		float Scale;
		
	}
	public class BSTreadTransform
	{
		NIFString Name;
		BSTreadTransformData Transform_1;
		BSTreadTransformData Transform_2;
		
	}
	public class BSTreadTransfInterpolator : NiInterpolator
	{
		int Num_Tread_Transforms;
		BSTreadTransform[] Tread_Transforms;
		int Data;
		
	}
	public class BSAnimNotes : NiObject
	{
		short Unknown_Short_1;
		
	}
	public class bhkLiquidAction : bhkSerializable
	{
		int Unknown_Int_1;
		int Unknown_Int_2;
		int Unknown_Int_3;
		float Unknown_Float_1;
		float Unknown_Float_2;
		float Unknown_Float_3;
		float Unknown_Float_4;
		
	}
	public class BSMultiBoundNode : NiNode
	{
		int Multi_Bound;
		
	}
	public class BSMultiBound : NiObject
	{
		int Data;
		
	}
	public class BSMultiBoundData : NiObject
	{
		
	}
	public class BSMultiBoundOBB : BSMultiBoundData
	{
		Vector3 Center;
		Vector3 Size;
		Matrix33 Rotation;
		
	}
	public class BSMultiBoundSphere : BSMultiBoundData
	{
		int Unknown_Int_1;
		int Unknown_Int_2;
		int Unknown_Int_3;
		float Radius;
		
	}
	public class BSSegmentedTriShape : NiTriShape
	{
		int Num_Segments;
		BSSegment[] Segment;
		
	}
	public class BSMultiBoundAABB : BSMultiBoundData
	{
		Vector3 Position;
		Vector3 Extent;
		
	}
	public class AdditionalDataInfo
	{
		int Data_Type;
		int Num_Channel_Bytes_Per_Element;
		int Num_Channel_Bytes;
		int Num_Total_Bytes_Per_Element;
		int Block_Index;
		int Channel_Offset;
		byte Unknown_Byte_1;
		
	}
	public class AdditionalDataBlock
	{
		bool Has_Data;
		int Block_Size;
		int Num_Blocks;
		int[] Block_Offsets;
		int Num_Data;
		int[] Data_Sizes;
		byte[,] Data;
		
	}
	public class NiAdditionalGeometryData : AbstractAdditionalGeometryData
	{
		ushort Num_Vertices;
		uint Num_Block_Infos;
		AdditionalDataInfo[] Block_Infos;
		int Num_Blocks;
		AdditionalDataBlock[] Blocks;
		
	}
	public class BSWArray : NiExtraData
	{
		int Num_Items;
		int[] Items;
		
	}
	public class bhkAabbPhantom : bhkShapePhantom
	{
		int[] Unknown_Ints_1;
		
	}
	public class BSFrustumFOVController : NiTimeController
	{
		int Interpolator;
		
	}
	public class BSDebrisNode : NiNode
	{
		byte Unknown_byte_1;
		short Unknown_Short_2;
		
	}
	public class bhkBreakableConstraint : bhkConstraint
	{
		SubConstraint Sub_Constraint;
		float Threshold;
		bool Remove_if_Broken;
		
	}
	public class bhkOrientHingedBodyAction : bhkSerializable
	{
		int[] Unknown_Ints_1;
		
	}
	public class Region
	{
		uint Start_Index;
		uint Num_Indices;
		
	}
	public enum CloningBehavior : uint
	{
		CLONING_SHARE = 0,
		CLONING_COPY = 1,
		CLONING_BLANK_COPY = 2,
		
	}
	public enum ComponentFormat : uint
	{
		F_UNKNOWN = 0x00000000,
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
		F_UINT_10_10_10_2 = 0x0001043E,
		
	}
	public enum DataStreamUsage : uint
	{
		USAGE_VERTEX_INDEX = 0,
		USAGE_VERTEX = 1,
		USAGE_SHADER_CONSTANT = 2,
		USAGE_USER = 3,
		
	}
	[Flags]
	public enum DataStreamAccess : uint
	{
		CPU_Read = 0,
		CPU_Write_Static = 1,
		CPU_Write_Mutable = 2,
		CPU_Write_Volatile = 3,
		GPU_Read = 4,
		GPU_Write = 5,
		CPU_Write_Static_Inititialized = 6,
		
	}
	public class NiDataStream : NiObject
	{
		DataStreamUsage Usage;
		DataStreamAccess Access;
		uint Num_Bytes;
		CloningBehavior Cloning_Behavior;
		uint Num_Regions;
		Region[] Regions;
		uint Num_Components;
		ComponentFormat[] Component_Formats;
		byte[] Data;
		bool Streamable;
		
	}
	public class SemanticData
	{
		NIFString Name;
		uint Index;
		
	}
	public class MeshData
	{
		int Stream;
		bool Is_Per_Instance;
		ushort Num_Submeshes;
		ushort[] Submesh_To_Region_Map;
		int Num_Components;
		SemanticData[] Component_Semantics;
		
	}
	public class MaterialData
	{
		NIFString Material_Name;
		uint Material_Extra_Data;
		
	}
	public class NiRenderObject : NiAVObject
	{
		uint Num_Materials;
		MaterialData[] Material_Data;
		int Active_Material;
		bool Material_Needs_Update_Default;
		
	}
	public enum MeshPrimitiveType : uint
	{
		MESH_PRIMITIVE_TRIANGLES = 0,
		MESH_PRIMITIVE_TRISTRIPS = 1,
		MESH_PRIMITIVE_LINESTRIPS = 2,
		MESH_PRIMITIVE_QUADS = 3,
		MESH_PRIMITIVE_POINTS = 4,
		
	}
	public enum SyncPoint : ushort
	{
		SYNC_ANY = 0x8000,
		SYNC_UPDATE = 0x8010,
		SYNC_POST_UPDATE = 0x8020,
		SYNC_VISIBLE = 0x8030,
		SYNC_RENDER = 0x8040,
		SYNC_PHYSICS_SIMULATE = 0x8050,
		SYNC_PHYSICS_COMPLETED = 0x8060,
		SYNC_REFLECTIONS = 0x8070,
		
	}
	public class NiMeshModifier : NiObject
	{
		uint Num_Submit_Points;
		SyncPoint[] Submit_Points;
		uint Num_Complete_Points;
		SyncPoint[] Complete_Points;
		
	}
	public class ExtraMeshDataEpicMickey
	{
		int Unknown_Int_1;
		int Unknown_Int_2;
		int Unknown_Int_3;
		float Unknown_Int_4;
		float Unknown_Int_5;
		float Unknown_Int_6;
		
	}
	public class ExtraMeshDataEpicMickey2
	{
		int Start;
		int End;
		short[] Unknown_Shorts;
		
	}
	public class NiMesh : NiRenderObject
	{
		MeshPrimitiveType Primitive_Type;
		int Unknown_51;
		int Unknown_52;
		int Unknown_53;
		int Unknown_54;
		float Unknown_55;
		int Unknown_56;
		ushort Num_Submeshes;
		bool Instancing_Enabled;
		SphereBV Bound;
		uint Num_Datas;
		MeshData[] Datas;
		uint Num_Modifiers;
		int[] Modifiers;
		byte Unknown_100;
		int Unknown_101;
		uint Unknown_102;
		float[] Unknown_103;
		int Unknown_200;
		ExtraMeshDataEpicMickey[] Unknown_201;
		int Unknown_250;
		int[] Unknown_251;
		int Unknown_300;
		short Unknown_301;
		int Unknown_302;
		byte[] Unknown_303;
		int Unknown_350;
		ExtraMeshDataEpicMickey2[] Unknown_351;
		int Unknown_400;
		
	}
	public class NiMorphWeightsController : NiInterpController
	{
		int Unknown_2;
		uint Num_Interpolators;
		int[] Interpolators;
		uint Num_Targets;
		NIFString[] Target_Names;
		
	}
	public class ElementReference
	{
		SemanticData Semantic;
		uint Normalize_Flag;
		
	}
	public class NiMorphMeshModifier : NiMeshModifier
	{
		byte Flags;
		ushort Num_Targets;
		uint Num_Elements;
		ElementReference[] Elements;
		
	}
	public class NiSkinningMeshModifier : NiMeshModifier
	{
		ushort Flags;
		int Skeleton_Root;
		SkinTransform Skeleton_Transform;
		uint Num_Bones;
		int[] Bones;
		SkinTransform[] Bone_Transforms;
		SphereBV[] Bone_Bounds;
		
	}
	public class NiInstancingMeshModifier : NiMeshModifier
	{
		
	}
	public class NiSkinningLODController : NiTimeController
	{
		
	}
	public class NiPSParticleSystem : NiAVObject
	{
		int Unknown_3;
		int[] Unknown_38;
		int Unknown_4;
		int Unknown_5;
		int[] Unknown_39;
		int Unknown_6;
		int Unknown_7;
		int Unknown_8;
		int Unknown_9;
		float Unknown_10;
		int Unknown_11;
		int Unknown_12;
		int Simulator;
		int Generator;
		int Unknown_15;
		int Unknown_16;
		int Unknown_17;
		int Emitter;
		int Unknown_19;
		int Unknown_20;
		int Unknown_21;
		byte[] Unknown_22;
		
	}
	public class NiPSMeshParticleSystem : NiPSParticleSystem
	{
		int Unknown_23;
		int Unknown_24;
		int Unknown_25;
		byte Unknown_26;
		
	}
	public class NiPSEmitParticlesCtlr : NiPSysEmitterCtlr
	{
		
	}
	public class NiPSForceActiveCtlr : NiTimeController
	{
		int Interpolator;
		int Unknown_2;
		
	}
	public class NiPSSimulator : NiMeshModifier
	{
		uint Num_Simulation_Steps;
		int[] Simulation_Steps;
		
	}
	public abstract class NiPSSimulatorStep : NiObject
	{
		
	}
	public enum PSLoopBehavior : uint
	{
		PS_LOOP_CLAMP_BIRTH = 0,
		PS_LOOP_CLAMP_DEATH = 1,
		PS_LOOP_AGESCALE = 2,
		PS_LOOP_LOOP = 3,
		PS_LOOP_REFLECT = 4,
		
	}
	public class NiPSSimulatorGeneralStep : NiPSSimulatorStep
	{
		byte Num_Size_Keys;
		Key<float>[] Size_Keys;
		float Unknown_1;
		float Unknown_2;
		float Unknown_3;
		
	}
	public class NiPSSimulatorForcesStep : NiPSSimulatorStep
	{
		uint Num_Forces;
		int[] Forces;
		
	}
	public class NiPSSimulatorCollidersStep : NiPSSimulatorStep
	{
		uint Num_Colliders;
		int[] Colliders;
		
	}
	public class NiPSSimulatorMeshAlignStep : NiPSSimulatorStep
	{
		byte Num_Rotation_Keys;
		QuatKey<Quaternion>[] Rotation_Keys;
		PSLoopBehavior Rotation_Loop_Behavior;
		
	}
	public class NiPSSimulatorFinalStep : NiPSSimulatorStep
	{
		
	}
	public class NiPSFacingQuadGenerator : NiObject
	{
		byte Unknown_1;
		byte Unknown_2;
		byte Unknown_3;
		byte Unknown_4;
		byte Unknown_5;
		byte Unknown_6;
		byte Unknown_7;
		byte Unknown_8;
		byte Unknown_9;
		byte Unknown_10;
		byte Unknown_11;
		byte Unknown_12;
		
	}
	public class NiShadowGenerator : NiObject
	{
		NIFString Name;
		ushort Unknown_Flags;
		uint Num_Unknown_Links_1;
		int[] Unknown_Links_1;
		int Unkown_Int_2;
		int Target;
		float Unkown_Float_4;
		byte Unkown_Byte_5;
		int Unkown_Int_6;
		int Unkown_Int_7;
		int Unkown_Int_8;
		byte Unkown_Byte_9;
		
	}
	public class NiPSBoundUpdater : NiObject
	{
		byte Unknown_1;
		byte Unknown_2;
		
	}
	public class NiPSDragForce : NiObject
	{
		int Unknown_1;
		int Unknown_2;
		byte Unknown_3;
		float Unknown_4;
		float Unknown_5;
		float Unknown_6;
		float Unknown_7;
		float Unknown_8;
		float Unknown_9;
		int Unknown_10;
		
	}
	public class NiPSGravityForce : NiObject
	{
		byte Unknown_1;
		byte Unknown_2;
		byte Unknown_3;
		byte Unknown_4;
		byte Unknown_5;
		byte Unknown_6;
		byte Unknown_7;
		byte Unknown_8;
		byte Unknown_9;
		byte Unknown_10;
		byte Unknown_11;
		byte Unknown_12;
		byte Unknown_13;
		byte Unknown_14;
		byte Unknown_15;
		byte Unknown_16;
		byte Unknown_17;
		float Unknown_18;
		byte Unknown_19;
		byte Unknown_20;
		byte Unknown_21;
		byte Unknown_22;
		byte Unknown_23;
		byte Unknown_24;
		byte Unknown_25;
		byte Unknown_26;
		byte Unknown_27;
		byte Unknown_28;
		byte Unknown_29;
		byte Unknown_30;
		byte Unknown_31;
		byte Unknown_32;
		byte Unknown_33;
		byte Unknown_34;
		float Unknown_35;
		int Unknown_36;
		
	}
	public class NiPSBoxEmitter : NiObject
	{
		NIFString Name;
		float Unknown_1;
		float Unknown_2;
		byte Unknown_3;
		byte Unknown_4;
		byte Unknown_5;
		byte Unknown_6;
		float Unknown_7;
		byte Unknown_8;
		byte Unknown_9;
		byte Unknown_10;
		byte Unknown_11;
		float Unknown_12;
		int Unknown_13;
		float Unknown_14;
		float Unknown_15;
		float Unknown_16;
		float Unknown_17;
		float Unknown_18;
		float Unknown_19;
		float Unknown_20;
		float Unknown_21;
		float Unknown_22;
		byte Unknown_23;
		byte Unknown_24;
		byte Unknown_25;
		byte Unknown_26;
		byte Unknown_27;
		byte Unknown_28;
		byte Unknown_29;
		byte Unknown_30;
		byte Unknown_31;
		byte Unknown_32;
		byte Unknown_33;
		byte Unknown_34;
		byte Unknown_35;
		byte Unknown_36;
		byte Unknown_37;
		byte Unknown_38;
		byte Unknown_39;
		byte Unknown_40;
		byte Unknown_41;
		byte Unknown_42;
		byte Unknown_43;
		byte Unknown_44;
		byte Unknown_45;
		byte Unknown_46;
		byte Unknown_47;
		byte Unknown_48;
		
	}
	public class NiPSMeshEmitter : NiObject
	{
		NIFString Name;
		int Unknown_1;
		int Unknown_2;
		int Unknown_3;
		float Unknown_4;
		float Unknown_5;
		float Unknown_6;
		int Unknown_7;
		float Unknown_8;
		float Unknown_9;
		float Unknown_10;
		float Unknown_11;
		float Unknown_12;
		int Unknown_13;
		float Unknown_14;
		float Unknown_15;
		float Unknown_16;
		int Unknown_17;
		int Unknown_18;
		short Unknown_19;
		int Unknown_20;
		int Unknown_21;
		float Unknown_22;
		int Unknown_23;
		int Unknown_24;
		int Unknown_25;
		int Unknown_26;
		
	}
	public class NiPSGravityStrengthCtlr : NiTimeController
	{
		int Unknown_2;
		int Unknown_3;
		
	}
	public class NiPSPlanarCollider : NiObject
	{
		NIFString Name;
		int Unknown_Int_1;
		int Unknown_Int_2;
		short Unknown_Short_3;
		byte Unknown_Byte_4;
		float[] Unknown_Floats_5;
		int Unknown_Link_6;
		
	}
	public class NiPSEmitterSpeedCtlr : NiTimeController
	{
		int Interpolator;
		int Unknown_3;
		
	}
	public class NiPSEmitterRadiusCtlr : NiTimeController
	{
		int Interpolator;
		int Unknown_2;
		
	}
	public class NiPSResetOnLoopCtlr : NiTimeController
	{
		
	}
	public class NiPSSphereEmitter : NiObject
	{
		NIFString Name;
		int Unknown_2;
		int Unknown_3;
		int Unknown_4;
		int Unknown_5;
		float Unknown_6;
		int Unknown_7;
		float Unknown_8;
		float Unknown_9;
		int Unknown_10;
		float Unknown_11;
		int Unknown_12;
		int Unknown_13;
		int Unknown_14;
		int Unknown_15;
		int Unknown_16;
		float Unknown_17;
		int Unknown_18;
		int Unknown_19;
		short Unknown_20;
		int Unknown_21;
		float Unknown_22;
		
	}
	public class NiPSCylinderEmitter : NiPSSphereEmitter
	{
		float Unknown_23;
		
	}
	public class NiPSEmitterDeclinationCtlr : NiPSysModifierCtlr
	{
		
	}
	public class NiPSEmitterDeclinationVarCtlr : NiPSEmitterDeclinationCtlr
	{
		
	}
	public class NiPSEmitterPlanarAngleCtlr : NiPSysModifierCtlr
	{
		
	}
	public class NiPSEmitterPlanarAngleVarCtlr : NiPSEmitterPlanarAngleCtlr
	{
		
	}
	public class NiPSEmitterRotAngleCtlr : NiPSysModifierCtlr
	{
		
	}
	public class NiPSEmitterRotAngleVarCtlr : NiPSEmitterRotAngleCtlr
	{
		
	}
	public class NiPSEmitterRotSpeedCtlr : NiPSysModifierCtlr
	{
		
	}
	public class NiPSEmitterRotSpeedVarCtlr : NiPSEmitterRotSpeedCtlr
	{
		
	}
	public class NiPSEmitterLifeSpanCtlr : NiPSysModifierCtlr
	{
		
	}
	public class NiPSBombForce : NiObject
	{
		NIFString Name;
		byte Unknown_1;
		int Unknown_2;
		int Unknown_3;
		int Unknown_4;
		int Unknown_5;
		int Unknown_6;
		int Unknown_7;
		int Unknown_8;
		int Unknown_9;
		int Unknown_10;
		
	}
	public class NiPSSphericalCollider : NiObject
	{
		int Unknown_1;
		int Unknown_2;
		byte Unknown_3;
		float Unknown_4;
		int Unknown_5;
		short Unknown_6;
		int Unknown_7;
		
	}
	public class NiPSSpawner : NiObject
	{
		
	}
	public class NiSequenceData : NiObject
	{
		
	}
	public class NiTransformEvaluator : NiObject
	{
		
	}
	public class NiBSplineCompTransformEvaluator : NiObject
	{
		
	}
	public class NiMeshHWInstance : NiObject
	{
		
	}
	public class NiFurSpringController : NiTimeController
	{
		float Unknown_Float;
		float Unknown_Float_2;
		uint Num_Bones;
		int[] Bones;
		uint Num_Bones_2;
		int[] Bones_2;
		
	}
	
	public static class NIFBinaryReader
	{
		
	}
}