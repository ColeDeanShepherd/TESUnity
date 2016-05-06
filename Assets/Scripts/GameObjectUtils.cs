using UnityEngine;

public static class GameObjectUtils
{
	public static GameObject CreateTerrain(float[,] heightPercents, float maxHeight)
	{
		var terrainData = new TerrainData();
		terrainData.heightmapResolution = heightPercents.GetLength(0);
		terrainData.size = new Vector3(terrainData.size.x, maxHeight, terrainData.size.z);
		terrainData.SetHeights(0, 0, heightPercents);

		GameObject terrain = new GameObject();
		terrain.AddComponent<Terrain>().terrainData = terrainData;

		return terrain;
	}
}