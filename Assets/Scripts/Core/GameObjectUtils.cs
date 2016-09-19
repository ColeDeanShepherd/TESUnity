using UnityEngine;

public static class GameObjectUtils
{
	/// <summary>
	/// Creates a camera identical to the one added to new scenes by default.
	/// </summary>
	public static GameObject CreateMainCamera(Vector3 position, Quaternion orientation)
	{
		GameObject cameraObject = new GameObject("Main Camera");
		cameraObject.tag = "MainCamera";

		cameraObject.AddComponent<Camera>();
		cameraObject.AddComponent<GUILayer>();
		cameraObject.AddComponent<FlareLayer>();
		cameraObject.AddComponent<AudioListener>();

		cameraObject.transform.position = position;
		cameraObject.transform.rotation = orientation;

		return cameraObject;
	}

	public static GameObject CreateDirectionalLight(Vector3 position, Quaternion orientation)
	{
		var light = new GameObject("Directional Light");

		var lightComponent = light.AddComponent<Light>();
		lightComponent.type = LightType.Directional;

		light.transform.position = position;
		light.transform.rotation = orientation;

		return light;
	}

	/// <summary>
	/// Creates terrain data from heights.
	/// </summary>
	/// <param name="heightPercents">Terrain height percentages ranging from 0 to 1.</param>
	/// <param name="maxHeight">The maximum height of the terrain, corresponding to a height percentage of 1.</param>
	/// <param name="heightSampleDistance">The horizontal/vertical distance between height samples.</param>
	/// <param name="splatPrototypes">The textures used by the terrain.</param>
	/// <param name="alphaMap">Texture blending information.</param>
	/// <returns>A TerrainData instance.</returns>
	public static TerrainData CreateTerrainData(float[,] heightPercents, float maxHeight, float heightSampleDistance, SplatPrototype[] splatPrototypes, float[,,] alphaMap)
	{
		Debug.Assert((heightPercents.GetLength(0) == heightPercents.GetLength(1)) && (maxHeight >= 0) && (heightSampleDistance >= 0));

		// Create the TerrainData.
		var terrainData = new TerrainData();
		terrainData.heightmapResolution = heightPercents.GetLength(0);

		var terrainWidth = (terrainData.heightmapResolution - 1) * heightSampleDistance;

		// If maxHeight is 0, leave all the heights in terrainData at 0 and make the vertical size of the terrain 1 to ensure valid AABBs.
		if(!Mathf.Approximately(maxHeight, 0))
		{
			terrainData.size = new Vector3(terrainWidth, maxHeight, terrainWidth);

			terrainData.SetHeights(0, 0, heightPercents);
		}
		else
		{
			terrainData.size = new Vector3(terrainWidth, 1, terrainWidth);
		}

		// Texture the terrain.
		if((splatPrototypes != null) && (alphaMap != null))
		{
			Debug.Assert(alphaMap.GetLength(0) == alphaMap.GetLength(1));

			terrainData.alphamapResolution = alphaMap.GetLength(0);
			terrainData.splatPrototypes = splatPrototypes;
			terrainData.SetAlphamaps(0, 0, alphaMap);
		}

		return terrainData;
	}

	/// <summary>
	/// Creates a terrain from heights.
	/// </summary>
	/// <param name="heightPercents">Terrain height percentages ranging from 0 to 1.</param>
	/// <param name="maxHeight">The maximum height of the terrain, corresponding to a height percentage of 1.</param>
	/// <param name="heightSampleDistance">The horizontal/vertical distance between height samples.</param>
	/// <param name="splatPrototypes">The textures used by the terrain.</param>
	/// <param name="alphaMap">Texture blending information.</param>
	/// <param name="position">The position of the terrain.</param>
	/// <returns>A terrain GameObject.</returns>
	public static GameObject CreateTerrain(float[,] heightPercents, float maxHeight, float heightSampleDistance, SplatPrototype[] splatPrototypes, float[,,] alphaMap, Vector3 position)
	{
		var terrainData = CreateTerrainData(heightPercents, maxHeight, heightSampleDistance, splatPrototypes, alphaMap);

		return CreateTerrainFromTerrainData(terrainData, position);
	}

	public static GameObject CreateTerrainFromTerrainData(TerrainData terrainData, Vector3 position)
	{
		// Create the terrain game object.
		GameObject terrainObject = new GameObject("terrain");

		var terrain = terrainObject.AddComponent<Terrain>();
		terrain.terrainData = terrainData;

		terrainObject.AddComponent<TerrainCollider>().terrainData = terrainData;

		terrainObject.transform.position = position;

		return terrainObject;
	}

	/// <summary>
	/// Calculate the AABB of an object and it's descendants.
	/// </summary>
	public static Bounds CalcVisualBoundsRecursive(GameObject gameObject)
	{
		Debug.Assert(gameObject != null);

		// Gets all the renderers in the object and it's descendants.
		var renderers = gameObject.transform.GetComponentsInChildren<Renderer>();

		if(renderers.Length > 0)
		{
			// Encapsulate the first renderer.
			var visualBounds = renderers[0].bounds;

			// Encapsulate the rest of the renderers.
			for(int i = 1; i < renderers.Length; i++)
			{
				visualBounds.Encapsulate(renderers[i].bounds);
			}

			return visualBounds;
		}
		// If there are no renderers in the object or any of it's children, simply return a degenerate AABB where the object is.
		else
		{
			return new Bounds(gameObject.transform.position, Vector3.zero);
		}
	}

	/// <summary>
	/// Finds a descendant game object by name.
	/// </summary>
	public static GameObject FindChildRecursively(GameObject parent, string name)
	{
		var resultTransform = parent.transform.Find(name);

		// Search through each of parent's children.
		if(resultTransform != null)
		{
			return resultTransform.gameObject;
		}

		// Perform the search recursively for each child of parent.
		foreach(Transform childTransform in parent.transform)
		{
			var result = FindChildRecursively(childTransform.gameObject, name);

			if(result != null)
			{
				return result;
			}
		}

		return null;
	}

	/// <summary>
	/// Finds a descendant game object with a name containing nameSubstring.
	/// </summary>
	public static GameObject FindChildWithNameSubstringRecursively(GameObject parent, string nameSubstring)
	{
		// Search through each of parent's children.
		foreach(Transform childTransform in parent.transform)
		{
			if(childTransform.name.Contains(nameSubstring))
			{
				return childTransform.gameObject;
			}
		}

		// Perform the search recursively for each child of parent.
		foreach(Transform childTransform in parent.transform)
		{
			var result = FindChildWithNameSubstringRecursively(childTransform.gameObject, nameSubstring);

			if(result != null)
			{
				return result;
			}
		}

		return null;
	}

	/// <summary>
	/// Find an ancestor object, or the object itself, with a tag.
	/// </summary>
	public static GameObject FindObjectWithTagUpHeirarchy(GameObject gameObject, string tag)
	{
		while(gameObject != null)
		{
			if(gameObject.tag == tag)
			{
				return gameObject;
			}

			// Go up one level in the object hierarchy.
			var parentTransform = gameObject.transform.parent;
			gameObject = (parentTransform != null) ? parentTransform.gameObject : null;
		}

		return null;
	}

	/// <summary>
	/// Set the layer of an object and all of it's descendants.
	/// </summary>
	public static void SetLayerRecursively(GameObject gameObject, int layer)
	{
		gameObject.layer = layer;

		foreach(Transform childTransform in gameObject.transform)
		{
			SetLayerRecursively(childTransform.gameObject, layer);
		}
	}

	/// <summary>
	/// Adds mesh colliders to every descandant object with a mesh filter but no mesh collider, including the object itself.
	/// </summary>
	public static void AddMissingMeshCollidersRecursively(GameObject gameObject)
	{
		// If gameObject has a MeshFilter but no Collider, add a MeshCollider.
		if(gameObject.GetComponent<Collider>() == null)
		{
			var meshFilter = gameObject.GetComponent<MeshFilter>();

			if((meshFilter != null) && (meshFilter.mesh != null))
			{
				gameObject.AddComponent<MeshCollider>();
			}
		}

		// Perform the above procedure on gameObject's children recursively.
		foreach(Transform childTransform in gameObject.transform)
		{
			AddMissingMeshCollidersRecursively(childTransform.gameObject);
		}
	}
}