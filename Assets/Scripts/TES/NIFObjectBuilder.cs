using System;
using UnityEngine;

namespace TESUnity
{
	using NIF;

	// TODO: Investigate merging meshes.
	// TODO: Investigate merging collision nodes with visual nodes.
	public class NIFObjectBuilder
	{
		public NIFObjectBuilder(NiFile file, MorrowindDataReader dataReader)
		{
			this.file = file;
			this.dataReader = dataReader;
		}
		public GameObject BuildObject()
		{
			Debug.Assert(file.name != null);

			// NIF files can have any number of root NiObjects.
			// If there is only one root, instantiate that directly.
			// If there are multiple roots, create a container GameObject and parent it to the roots.

			if(file.footer.roots.Length == 1)
			{
				GameObject gameObject = InstantiateNiObject(file.blocks[file.footer.roots[0]]);

				if(gameObject == null)
				{
					gameObject = new GameObject(file.name);
				}

				return gameObject;
			}
			else
			{
				GameObject gameObject = new GameObject(file.name);

				foreach(var rootRef in file.footer.roots)
				{
					var child = InstantiateNiObject(file.blocks[rootRef]);

					if(child != null)
					{
						child.transform.SetParent(gameObject.transform, false);
					}
				}

				return gameObject;
			}
		}

		private NiFile file;
		private MorrowindDataReader dataReader;

		/// <summary>
		/// Creates a GameObject representation of an NiObject.
		/// </summary>
		/// <returns>Returns the created GameObject, or null if the NiObject does not need its own GameObject.</returns>
		private GameObject InstantiateNiObject(NiObject obj)
		{
			if(obj.GetType() == typeof(NiNode))
			{
				return InstantiateNiNode((NiNode)obj);
			}
			else if(obj.GetType() == typeof(NiTriShape))
			{
				return InstantiateNiTriShape((NiTriShape)obj, true, false);
			}
			else if(obj.GetType() == typeof(RootCollisionNode))
			{
				return InstantiateRootCollisionNode((RootCollisionNode)obj);
			}
			else if(obj.GetType() == typeof(NiTextureEffect))
			{
				return null;
			}
			else if(obj.GetType() == typeof(NiBSAnimationNode))
			{
				return null;
			}
			else if(obj.GetType() == typeof(NiBSParticleNode))
			{
				return null;
			}
			else if(obj.GetType() == typeof(NiRotatingParticles))
			{
				return null;
			}
			else if(obj.GetType() == typeof(NiAutoNormalParticles))
			{
				return null;
			}
			else
			{
				throw new NotImplementedException("Tried to instantiate an unsupported NiObject (" + obj.GetType().Name + ").");
			}
		}

		private GameObject InstantiateNiNode(NiNode node)
		{
			GameObject obj = new GameObject(System.Text.Encoding.ASCII.GetString(node.name));
			ApplyNiAVObject(node, obj);

			foreach(var childIndex in node.children)
			{
				// NiNodes can have child references < 0 meaning null.
				if(!childIndex.isNull)
				{
					var child = InstantiateNiObject(file.blocks[childIndex.value]);

					if(child != null)
					{
						child.transform.SetParent(obj.transform, false);
					}
				}
			}

			return obj;
		}
		private GameObject InstantiateNiTriShape(NiTriShape triShape, bool visual, bool collidable)
		{
			Debug.Assert(visual || collidable);

			var mesh = NiTriShapeDataToMesh((NiTriShapeData)file.blocks[triShape.data.value]);
			var obj = new GameObject(System.Text.Encoding.ASCII.GetString(triShape.name));

			if(visual)
			{
				obj.AddComponent<MeshFilter>().mesh = mesh;
				obj.AddComponent<MeshRenderer>().material = NiAVObjectPropertiesToMaterial(triShape);
			}
			
			if(collidable)
			{
				obj.AddComponent<MeshCollider>().sharedMesh = mesh;
			}

			ApplyNiAVObject(triShape, obj);

			return obj;
		}
		private GameObject InstantiateRootCollisionNode(RootCollisionNode collisionNode)
		{
			GameObject obj = new GameObject("Root Collision Node");
			ApplyNiAVObject(collisionNode, obj);

			foreach(var childIndex in collisionNode.children)
			{
				// NiNodes can have child references < 0 meaning null.
				if(!childIndex.isNull)
				{
					AddColliderFromNiObject(file.blocks[childIndex.value], obj);
				}
			}

			return obj;
		}

		private void ApplyNiAVObject(NiAVObject anNiAVObject, GameObject obj)
		{
			obj.transform.position = Convert.NifPointToUnityPoint(anNiAVObject.translation);
			obj.transform.rotation = Convert.NifMatrix4x4ToUnityQuaternion(anNiAVObject.rotation);
			obj.transform.localScale = anNiAVObject.scale * Vector3.one;
		}

		private Mesh NiTriShapeDataToMesh(NiTriShapeData data)
		{
			// vertex positions
			var vertices = new Vector3[data.vertices.Length];
			for(int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = Convert.NifPointToUnityPoint(data.vertices[i]);
			}

			// vertex normals
			Vector3[] normals = null;
			if(data.hasNormals)
			{
				normals = new Vector3[vertices.Length];

				for(int i = 0; i < normals.Length; i++)
				{
					normals[i] = Convert.NifVector3ToUnityVector3(data.normals[i]);
				}
			}

			// vertex UV coordinates
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

			// triangle vertex indices
			var triangles = new int[data.numTrianglePoints];
			for(int i = 0; i < data.triangles.Length; i++)
			{
				int baseI = 3 * i;

				// Reverse triangle winding order.
				triangles[baseI] = data.triangles[i].v1;
				triangles[baseI + 1] = data.triangles[i].v3;
				triangles[baseI + 2] = data.triangles[i].v2;
			}

			// Create the mesh.
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
			NiAlphaProperty alphaProperty = null;

			foreach(var propRef in obj.properties)
			{
				var prop = file.blocks[propRef.value];

				if(prop is NiTexturingProperty)
				{
					texturingProperty = (NiTexturingProperty)prop;
				}
				else if(prop is NiMaterialProperty)
				{
					materialProperty = (NiMaterialProperty)prop;
				}
				else if(prop is NiAlphaProperty)
				{
					alphaProperty = (NiAlphaProperty)prop;
				}
			}

			// Create the material.
			Material material;

			if(alphaProperty != null)
			{
				if(Utils.ContainsBitFlags(alphaProperty.flags, 1))
				{
					material = new Material(TESUnity.instance.fadeMaterial);
				}
				else if(Utils.ContainsBitFlags(alphaProperty.flags, 0x100))
				{
					material = new Material(TESUnity.instance.cutoutMaterial);
					material.SetFloat("alphaCutoff", (float)alphaProperty.threshold / 255);
				}
				else
				{
					material = new Material(TESUnity.instance.defaultMaterial);
				}
			}
			else
			{
				material = new Material(TESUnity.instance.defaultMaterial);
			}

			// Apply textures.
			if(texturingProperty != null && texturingProperty.hasBaseTexture)
			{
				var srcTexture = (NiSourceTexture)file.blocks[texturingProperty.baseTexture.source.value];
				var fileNameWithExt = System.Text.Encoding.ASCII.GetString(srcTexture.fileName);
				var fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fileNameWithExt);

				material.mainTexture = dataReader.LoadTexture(fileNameWithoutExt);
			}

			return material;
		}

		private void AddColliderFromNiObject(NiObject anNiObject, GameObject gameObject)
		{
			if(anNiObject.GetType() == typeof(NiTriShape))
			{
				var colliderObj = InstantiateNiTriShape((NiTriShape)anNiObject, false, true);
				colliderObj.transform.SetParent(gameObject.transform, false);
			}
			else
			{
				Debug.Log("Unsupported collider NiObject: " + anNiObject.GetType().Name);
			}
		}
	}
}