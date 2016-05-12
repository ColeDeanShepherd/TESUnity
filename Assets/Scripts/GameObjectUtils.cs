using UnityEngine;

public static class GameObjectUtils
{
	/// <summary>
	/// Creates terrain from runtime data.
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

			terrainData.splatPrototypes = splatPrototypes;
			
			terrainData.alphamapResolution = alphaMap.GetLength(0);
			terrainData.SetAlphamaps(0, 0, alphaMap);
		}

		// Create the terrain game object.
		GameObject terrainObject = new GameObject("terrain");

		var terrain = terrainObject.AddComponent<Terrain>();
		terrain.terrainData = terrainData;

		terrainObject.AddComponent<TerrainCollider>();

		terrainObject.transform.position = position;

		return terrainObject;
	}

	public static Bounds GetLocalVisualBoundsOfParent(GameObject gameObject)
	{
		var childMeshFilters = gameObject.GetComponentsInChildren<MeshFilter>();

		if(childMeshFilters.Length == 0)
		{
			return new Bounds();
		}

		Bounds combinedBounds = childMeshFilters[0].mesh.bounds;

		for(int i = 1; i < childMeshFilters.Length; i++)
		{
			combinedBounds.Encapsulate(childMeshFilters[i].mesh.bounds);
		}

		return combinedBounds;
	}
}