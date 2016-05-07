using UnityEngine;

public static class GameObjectUtils
{
	public static GameObject CreateTerrain(float[,] heightPercents, float maxHeight, float heightSampleDistance, SplatPrototype[] splatPrototypes, float[,,] alphaMap, Vector3 position)
	{
		Debug.Assert(heightPercents.GetLength(0) == heightPercents.GetLength(1));
		
		var terrainData = new TerrainData();
		terrainData.heightmapResolution = heightPercents.GetLength(0);

		var terrainWidth = (terrainData.heightmapResolution - 1) * heightSampleDistance;

		if(!Mathf.Approximately(maxHeight, 0))
		{
			terrainData.size = new Vector3(terrainWidth, maxHeight, terrainWidth);

			terrainData.SetHeights(0, 0, heightPercents);
		}
		else
		{
			terrainData.size = new Vector3(terrainWidth, 1, terrainWidth);
		}

		if((splatPrototypes != null) && (alphaMap != null))
		{
			terrainData.splatPrototypes = splatPrototypes;

			Debug.Assert(alphaMap.GetLength(0) == alphaMap.GetLength(1));
			terrainData.alphamapResolution = alphaMap.GetLength(0);
			terrainData.SetAlphamaps(0, 0, alphaMap);
		}

		GameObject terrainObject = new GameObject();

		var terrain = terrainObject.AddComponent<Terrain>();
		terrain.terrainData = terrainData;
		terrain.materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;

		terrainObject.AddComponent<TerrainCollider>();

		terrainObject.transform.position = position;

		return terrainObject;
	}
}