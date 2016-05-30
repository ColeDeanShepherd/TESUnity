using System.Collections.Generic;
using UnityEngine;

namespace TESUnity
{
	/// <summary>
	/// Manages loading and instantiation of NIF models.
	/// </summary>
	public class NIFManager
	{
		public NIFManager(MorrowindDataReader dataReader, MaterialManager materialManager)
		{
			this.dataReader = dataReader;
			this.materialManager = materialManager;
		}
		public GameObject InstantiateNIF(string filePath)
		{
			// Create a container for the NIF prefabs if it doesn't yet exist.
			if(prefabContainerObj == null)
			{
				prefabContainerObj = new GameObject("NIF Prefabs");
				prefabContainerObj.SetActive(false);
			}

			// Find an existing prefab for the NIF file, or create a new prefab object.
			GameObject prefab;

			if(!NIFPrefabs.TryGetValue(filePath, out prefab))
			{
				// Load the NIF file and instantiate it.
				NIF.NiFile file = dataReader.LoadNIF(filePath);

				var objBuilder = new NIFObjectBuilder(file, dataReader, materialManager);
				prefab = objBuilder.BuildObject();

				prefab.transform.parent = prefabContainerObj.transform;

				// Add LOD support to the prefab.
				var LODComponent = prefab.AddComponent<LODGroup>();

				var LODs = new LOD[1]
				{
					new LOD(0.025f, prefab.GetComponentsInChildren<Renderer>())
				};
				
				LODComponent.SetLODs(LODs);

				// Cache the prefab.
				NIFPrefabs[filePath] = prefab;
			}

			return GameObject.Instantiate(prefab);
		}

		private MorrowindDataReader dataReader;
		private MaterialManager materialManager;

		private GameObject prefabContainerObj;
		private Dictionary<string, GameObject> NIFPrefabs = new Dictionary<string, GameObject>();
	}
}