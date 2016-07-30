using System;
using UnityEngine;

namespace TESUnity
{
	using NIF;

	// TODO: Investigate merging meshes.
	// TODO: Investigate merging collision nodes with visual nodes.
	public class NIFObjectBuilder
	{
		public NIFObjectBuilder(NiFile file, MorrowindDataReader dataReader, MaterialManager materialManager)
		{
			this.file = file;
			this.dataReader = dataReader;
			this.materialManager = materialManager;
		}
		public GameObject BuildObject()
		{
			Debug.Assert((file.name != null) && (file.footer.roots.Length > 0));

			// NIF files can have any number of root NiObjects.
			// If there is only one root, instantiate that directly.
			// If there are multiple roots, create a container GameObject and parent it to the roots.

			if(file.footer.roots.Length == 1)
			{
				var rootNiObject = file.blocks[file.footer.roots[0]];

				GameObject gameObject = InstantiateRootNiObject(rootNiObject);

				// If the file doesn't contain any NiObjects we are looking for, return an empty GameObject.
				if(gameObject == null)
				{
					Debug.Log(file.name + " resulted in a null GameObject when instantiated.");

					gameObject = new GameObject(file.name);
				}
				// If gameObject != null and the root NiObject is an NiNode, discard any transformations (Morrowind apparently does).
				else if(rootNiObject is NiNode)
				{
					gameObject.transform.position = Vector3.zero;
					gameObject.transform.rotation = Quaternion.identity;
					gameObject.transform.localScale = Vector3.one;
				}

				return gameObject;
			}
			else
			{
				Debug.Log(file.name + " has multiple roots.");

				GameObject gameObject = new GameObject(file.name);

				foreach(var rootRef in file.footer.roots)
				{
					var child = InstantiateRootNiObject(file.blocks[rootRef]);

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
		private MaterialManager materialManager;

		private GameObject InstantiateRootNiObject(NiObject obj)
		{
			var gameObject = InstantiateNiObject(obj);

			bool shouldAddMissingColliders, isMarker;
			ProcessExtraData(obj, out shouldAddMissingColliders, out isMarker);

			if((file.name != null) && IsMarkerFileName(file.name))
			{
				shouldAddMissingColliders = false;
				isMarker = true;
			}

			// Add colliders to the object if it doesn't already contain one.
			if(shouldAddMissingColliders && (gameObject.GetComponentInChildren<Collider>() == null))
			{
				GameObjectUtils.AddMissingMeshCollidersRecursively(gameObject);
			}

			if(isMarker)
			{
				GameObjectUtils.SetLayerRecursively(gameObject, MorrowindEngine.markerLayer);
			}

			return gameObject;
		}
		private void ProcessExtraData(NiObject obj, out bool shouldAddMissingColliders, out bool isMarker)
		{
			shouldAddMissingColliders = true;
			isMarker = false;

			if(obj is NiObjectNET)
			{
				var objNET = (NiObjectNET)obj;
				var extraData = (objNET.extraData.value >= 0) ? (NiExtraData)file.blocks[objNET.extraData.value] : null;

				while(extraData != null)
				{
					if(extraData is NiStringExtraData)
					{
						var strExtraData = (NiStringExtraData)extraData;

						if(strExtraData.str == "NCO" || strExtraData.str == "NCC")
						{
							shouldAddMissingColliders = false;
						}
						else if(strExtraData.str == "MRK")
						{
							shouldAddMissingColliders = false;
							isMarker = true;
						}
					}

					// Move to the next NiExtraData.
					if(extraData.nextExtraData.value >= 0)
					{
						extraData = (NiExtraData)file.blocks[extraData.nextExtraData.value];
					}
					else
					{
						extraData = null;
					}
				}
			}
		}

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
			else if(obj.GetType() == typeof(NiBSAnimationNode))
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
			GameObject obj = new GameObject(node.name);

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

			ApplyNiAVObject(node, obj);

			return obj;
		}
		private GameObject InstantiateNiTriShape(NiTriShape triShape, bool visual, bool collidable)
		{
			Debug.Assert(visual || collidable);

			var mesh = NiTriShapeDataToMesh((NiTriShapeData)file.blocks[triShape.data.value]);
			var obj = new GameObject(triShape.name);

			if(visual)
			{
				obj.AddComponent<MeshFilter>().mesh = mesh;

				var materialProps = NiAVObjectPropertiesToMWMaterialProperties(triShape);

				var meshRenderer = obj.AddComponent<MeshRenderer>();
				meshRenderer.material = materialManager.BuildMaterialFromProperties(materialProps);

				if(Utils.ContainsBitFlags(triShape.flags, (uint)NiAVObject.Flags.Hidden))
				{
					meshRenderer.enabled = false;
				}
			}
			
			if(collidable)
			{
				obj.AddComponent<MeshCollider>().sharedMesh = mesh;
				if(TESUnity.instance.UseKinematicRigidbodies)
				{
					obj.AddComponent<Rigidbody>().isKinematic = true;
				}
			}

			ApplyNiAVObject(triShape, obj);

			return obj;
		}
		private GameObject InstantiateRootCollisionNode(RootCollisionNode collisionNode)
		{
			GameObject obj = new GameObject("Root Collision Node");

			foreach(var childIndex in collisionNode.children)
			{
				// NiNodes can have child references < 0 meaning null.
				if(!childIndex.isNull)
				{
					AddColliderFromNiObject(file.blocks[childIndex.value], obj);
				}
			}

			ApplyNiAVObject(collisionNode, obj);

			return obj;
		}

		private void ApplyNiAVObject(NiAVObject anNiAVObject, GameObject obj)
		{
			obj.transform.position = Convert.NifPointToUnityPoint(anNiAVObject.translation);
			obj.transform.rotation = Convert.NifRotationMatrixToUnityQuaternion(anNiAVObject.rotation);
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
					normals[i] = Convert.NifVectorToUnityVector(data.normals[i]);
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
			
			if(!data.hasNormals)
			{
				mesh.RecalculateNormals();
			}

			mesh.RecalculateBounds();

			return mesh;
		}
		private MWMaterialProps NiAVObjectPropertiesToMWMaterialProperties(NiAVObject obj)
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

			// Create the material properties.
			MWMaterialProps mp = new MWMaterialProps();

			#region AlphaProperty Cheat Sheet
			/*
			14 bits used:

			1 bit for alpha blend bool
			4 bits for src blend mode
			4 bits for dest blend mode
			1 bit for alpha test bool
			3 bits for alpha test mode
			1 bit for zwrite bool ( opposite value )

			Bit 0 : alpha blending enable
            Bits 1-4 : source blend mode 
            Bits 5-8 : destination blend mode
            Bit 9 : alpha test enable
            Bit 10-12 : alpha test mode
            Bit 13 : no sorter flag ( disables triangle sorting ) ( Unity ZWrite )

			blend modes (glBlendFunc):
            0000 GL_ONE
            0001 GL_ZERO
            0010 GL_SRC_COLOR
            0011 GL_ONE_MINUS_SRC_COLOR
            0100 GL_DST_COLOR
            0101 GL_ONE_MINUS_DST_COLOR
            0110 GL_SRC_ALPHA
            0111 GL_ONE_MINUS_SRC_ALPHA
            1000 GL_DST_ALPHA
            1001 GL_ONE_MINUS_DST_ALPHA
            1010 GL_SRC_ALPHA_SATURATE

            test modes (glAlphaFunc):
            000 GL_ALWAYS
            001 GL_LESS
            010 GL_EQUAL
            011 GL_LEQUAL
            100 GL_GREATER
            101 GL_NOTEQUAL
            110 GL_GEQUAL
            111 GL_NEVER
			*/
			#endregion

			if ( alphaProperty != null)
			{
				ushort flags = alphaProperty.flags;
				ushort oldflags = flags;
				byte srcbm = (byte)(BitConverter.GetBytes( flags >> 1 )[0] & 15);
				byte dstbm = ( byte )( BitConverter.GetBytes( flags >> 5 )[ 0 ] & 15);
				mp.zWrite = BitConverter.GetBytes( flags >> 15 )[ 0 ] == 1;//smush

				if ( Utils.ContainsBitFlags( flags , 0x01) ) // if flags contain the alpha blend flag at bit 0 in byte 0
				{
					mp.alphaBlended = true;
					mp.srcBlendMode = FigureBlendMode( srcbm );
					mp.dstBlendMode = FigureBlendMode( dstbm );
				}

				else if(Utils.ContainsBitFlags( flags , 0x100)) // if flags contain the alpha test flag
				{
					mp.alphaTest = true;
					mp.alphaCutoff = (float)alphaProperty.threshold / 255;
				}
			}
			else
			{
				mp.alphaBlended = false;
				mp.alphaTest= false;
			}

			// Apply textures.
			if(texturingProperty != null) mp.textures = ConfigureTextureProperties( texturingProperty );

			return mp;
		}

		private MWMaterialTextures ConfigureTextureProperties ( NiTexturingProperty ntp )
		{
			MWMaterialTextures tp = new MWMaterialTextures();
			if ( ntp.textureCount < 1 ) return tp;
			if ( ntp.hasBaseTexture )
			{
				NiSourceTexture src = ( NiSourceTexture )file.blocks[ ntp.baseTexture.source.value ];
				tp.mainFilePath = src.fileName;
			}
			if ( ntp.hasDarkTexture )
			{
				NiSourceTexture src = ( NiSourceTexture )file.blocks[ ntp.darkTexture.source.value ];
				tp.darkFilePath = src.fileName;
			}
			if ( ntp.hasDetailTexture )
			{
				NiSourceTexture src = ( NiSourceTexture )file.blocks[ ntp.detailTexture.source.value ];
				tp.detailFilePath = src.fileName;
			}
			if ( ntp.hasGlossTexture )
			{
				NiSourceTexture src = ( NiSourceTexture )file.blocks[ ntp.glossTexture.source.value ];
				tp.glossFilePath = src.fileName;
			}
			if ( ntp.hasGlowTexture )
			{
				NiSourceTexture src = ( NiSourceTexture )file.blocks[ ntp.glowTexture.source.value ];
				tp.glowFilePath = src.fileName;
			}
			if ( ntp.hasBumpMapTexture )
			{
				NiSourceTexture src = ( NiSourceTexture )file.blocks[ ntp.bumpMapTexture.source.value ];
				tp.bumpFilePath = src.fileName;
			}
			return tp;
		}

		private UnityEngine.Rendering.BlendMode FigureBlendMode ( byte b )
		{
			return ( UnityEngine.Rendering.BlendMode )Mathf.Min( b , 10 );
		}

		private MatTestMode FigureTestMode ( byte b )
		{
			return ( MatTestMode )Mathf.Min( b , 7 );
		}

		private void AddColliderFromNiObject(NiObject anNiObject, GameObject gameObject)
		{
			if(anNiObject.GetType() == typeof(NiTriShape))
			{
				var colliderObj = InstantiateNiTriShape((NiTriShape)anNiObject, false, true);
				colliderObj.transform.SetParent(gameObject.transform, false);
			}
			else if(anNiObject.GetType() == typeof(AvoidNode)) { }
			else
			{
				Debug.Log("Unsupported collider NiObject: " + anNiObject.GetType().Name);
			}
		}

		private bool IsMarkerFileName(string name)
		{
			var lowerName = name.ToLower();

			return	lowerName == "marker_light" ||
					lowerName == "marker_north" ||
					lowerName == "marker_error" ||
					lowerName == "marker_arrow" ||
					lowerName == "editormarker" ||
					lowerName == "marker_creature" ||
					lowerName == "marker_travel" ||
					lowerName == "marker_temple" ||
					lowerName == "marker_prison" ||
					lowerName == "marker_radius" ||
					lowerName == "marker_divine" ||
					lowerName == "editormarker_box_01";
		}
	}
}