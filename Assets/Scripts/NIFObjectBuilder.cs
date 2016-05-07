using System;
using UnityEngine;
using NIF;

public class NIFObjectBuilder
{
	public static float NIFScale = 1.0f / 128;

	#region Helper Functions
	public static Vector3 NifVector3ToUnityVector3(Vector3 NIFVector3)
	{
		Utils.Swap(ref NIFVector3.y, ref NIFVector3.z);

		return NIFVector3;
	}
	public static Vector3 NifPointToUnityPoint(Vector3 NIFPoint)
	{
		return NifVector3ToUnityVector3(NIFPoint) * NIFScale;
	}
	public static Quaternion NifMatrix4x4ToUnityQuaternion(Matrix4x4 NIFMatrix4x4)
	{
		var quat = RotationMatrixToQuaternion(NIFMatrix4x4);
		Utils.Swap(ref quat.y, ref quat.z);

		return Quaternion.Inverse(quat);
	}
	public static Quaternion RotationMatrixToQuaternion(Matrix4x4 matrix)
	{
		var fourXSquaredMinus1 = matrix[0, 0] - matrix[1, 1] - matrix[2, 2];
		var fourYSquaredMinus1 = matrix[1, 1] - matrix[0, 0] - matrix[2, 2];
		var fourZSquaredMinus1 = matrix[2, 2] - matrix[0, 0] - matrix[1, 1];
		var fourWSquaredMinus1 = matrix[0, 0] + matrix[1, 1] + matrix[2, 2];

		int biggestIndex = 0;
		var fourBiggestSquaredMinus1 = fourWSquaredMinus1;

		if(fourXSquaredMinus1 > fourBiggestSquaredMinus1)
		{
			fourBiggestSquaredMinus1 = fourXSquaredMinus1;
			biggestIndex = 1;
		}

		if(fourYSquaredMinus1 > fourBiggestSquaredMinus1)
		{
			fourBiggestSquaredMinus1 = fourYSquaredMinus1;
			biggestIndex = 2;
		}

		if(fourZSquaredMinus1 > fourBiggestSquaredMinus1)
		{
			fourBiggestSquaredMinus1 = fourZSquaredMinus1;
			biggestIndex = 3;
		}

		var biggestVal = Mathf.Sqrt(fourBiggestSquaredMinus1 + 1) * 0.5f;
		var mult = 0.25f / biggestVal;

		Quaternion quat = new Quaternion();

		switch(biggestIndex)
		{
			case 0:
				quat.w = biggestVal;
				quat.x = (matrix[1, 2] - matrix[2, 1]) * mult;
				quat.y = (matrix[2, 0] - matrix[0, 2]) * mult;
				quat.z = (matrix[0, 1] - matrix[1, 0]) * mult;
				break;
			case 1:
				quat.w = (matrix[1, 2] - matrix[2, 1]) * mult;
				quat.x = biggestVal;
				quat.y = (matrix[0, 1] + matrix[1, 0]) * mult;
				quat.z = (matrix[2, 0] + matrix[0, 2]) * mult;
				break;
			case 2:
				quat.w = (matrix[2, 0] - matrix[0, 2]) * mult;
				quat.x = (matrix[0, 1] + matrix[1, 0]) * mult;
				quat.y = biggestVal;
				quat.z = (matrix[1, 2] + matrix[2, 1]) * mult;
				break;
			case 3:
				quat.w = (matrix[0, 1] - matrix[1, 0]) * mult;
				quat.x = (matrix[2, 0] + matrix[0, 2]) * mult;
				quat.y = (matrix[1, 2] + matrix[2, 1]) * mult;
				quat.z = biggestVal;
				break;
			default:
				Debug.Assert(false);
				break;
		}

		return quat;
	}
	#endregion

	public NIFObjectBuilder(NiFile file, MorrowindDataReader dataReader)
	{
		this.file = file;
		this.dataReader = dataReader;
	}
	public GameObject BuildObject()
	{
		return InstantiateNiObject(file.blocks[0], null);
	}
	
	private NiFile file;
	private MorrowindDataReader dataReader;

	private GameObject InstantiateNiObject(NiObject NIFObj, GameObject parent)
	{
		if(NIFObj.GetType() == typeof(NiNode))
		{
			return InstantiateNiNode((NiNode)NIFObj, parent);
		}
		else if(NIFObj.GetType() == typeof(NiTriShape))
		{
			return InstantiateNiTriShape((NiTriShape)NIFObj, parent);
		}
		else if(NIFObj.GetType() == typeof(RootCollisionNode))
		{
			return null;
		}
		else
		{
			throw new NotImplementedException("Tried to instantiate an unsupported NiObject (" + NIFObj.GetType().Name + ").");
		}
	}
	private GameObject InstantiateNiNode(NiNode node, GameObject parent)
	{
		GameObject obj = new GameObject(System.Text.Encoding.ASCII.GetString(node.name));
		obj.transform.parent = (parent != null) ? parent.transform : null;

		obj.transform.localPosition = NifPointToUnityPoint(node.translation);
		obj.transform.localRotation = NifMatrix4x4ToUnityQuaternion(node.rotation);

		for(int i = 0; i < node.children.Length; i++)
		{
			var childIndex = node.children[i];

			if(childIndex >= 0)
			{
				InstantiateNiObject(file.blocks[childIndex], obj);
			}
		}

		return obj;
	}
	private GameObject InstantiateNiTriShape(NiTriShape triShape, GameObject parent)
	{
		var mesh = NiTriShapeDataToMesh((NiTriShapeData)file.blocks[triShape.dataRef]);
		var material = NiAVObjectPropertiesToMaterial(triShape);

		var obj = new GameObject(System.Text.Encoding.ASCII.GetString(triShape.name));
		obj.AddComponent<MeshFilter>().mesh = mesh;
		obj.AddComponent<MeshRenderer>().material = material;

		obj.transform.parent = parent.transform;

		obj.transform.localPosition = NifPointToUnityPoint(triShape.translation);
		obj.transform.localRotation = NifMatrix4x4ToUnityQuaternion(triShape.rotation);

		return obj;
	}

	private Mesh NiTriShapeDataToMesh(NiTriShapeData data)
	{
		var vertices = new Vector3[data.vertices.Length];
		for(int i = 0; i < vertices.Length; i++)
		{
			vertices[i] = NifPointToUnityPoint(data.vertices[i]);
		}

		Vector3[] normals = null;
		if(data.hasNormals)
		{
			normals = new Vector3[vertices.Length];

			for(int i = 0; i < normals.Length; i++)
			{
				normals[i] = NifVector3ToUnityVector3(data.normals[i]);
			}
		}

		Vector2[] UVs = null;
		if(data.hasUV)
		{
			UVs = new Vector2[vertices.Length];

			for(int i = 0; i < UVs.Length; i++)
			{
				var NiTexCoord = data.UVSets[0, i];

				UVs[i] = new Vector2(NiTexCoord.u, NiTexCoord.v);
			}
		}

		var triangles = new int[data.numTrianglePoints];
		for(int i = 0; i < data.triangles.Length; i++)
		{
			int baseI = 3 * i;

			triangles[baseI] = data.triangles[i].v1;
			triangles[baseI + 1] = data.triangles[i].v3;
			triangles[baseI + 2] = data.triangles[i].v2;
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = UVs;
		mesh.triangles = triangles;

		mesh.RecalculateBounds();

		if(!data.hasNormals)
		{
			mesh.RecalculateNormals();
		}

		return mesh;
	}
	private Material NiAVObjectPropertiesToMaterial(NiAVObject obj)
	{
		// Find relevant properties.
		NiTexturingProperty texturingProperty = null;
		NiMaterialProperty materialProperty = null;

		foreach(var propRef in obj.propertyRefs)
		{
			var prop = file.blocks[propRef];

			if(prop is NiTexturingProperty)
			{
				texturingProperty = (NiTexturingProperty)prop;
			}
			else if(prop is NiMaterialProperty)
			{
				materialProperty = (NiMaterialProperty)prop;
			}
		}

		// Create the material.
		var material = new Material(TESUnity.instance.defaultMaterial);

		if(texturingProperty.hasBaseTexture)
		{
			var srcTexture = (NiSourceTexture)file.blocks[texturingProperty.baseTexture.sourceRef];
			var fileNameWithExt = System.Text.Encoding.ASCII.GetString(srcTexture.fileName);
			var fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fileNameWithExt);

			material.mainTexture = dataReader.LoadTexture(fileNameWithoutExt);
		}

		return material;
	}
}