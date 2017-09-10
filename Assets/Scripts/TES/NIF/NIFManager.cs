using System;
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

		/// <summary>
		/// Instantiates a NIF file.
		/// </summary>
		public GameObject InstantiateNIF(string filePath)
		{
            EnsurePrefabContainerObjectExists();
            
            // Get the prefab.
			GameObject prefab;
			if(!nifPrefabs.TryGetValue(filePath, out prefab))
			{
                // Load & cache the NIF prefab.
			    prefab = LoadNifPrefabDontAddToCache(filePath);
				nifPrefabs[filePath] = prefab;
			}

            // Instantiate the prefab.
			return GameObject.Instantiate(prefab);
		}
		
		private MorrowindDataReader dataReader;
		private MaterialManager materialManager;
		private GameObject prefabContainerObj;
		private Dictionary<string, GameObject> nifPrefabs = new Dictionary<string, GameObject>();

	    private void EnsurePrefabContainerObjectExists()
	    {
	        if(prefabContainerObj == null)
	        {
	            prefabContainerObj = new GameObject("NIF Prefabs");
	            prefabContainerObj.SetActive(false);
	        }
        }
	    private GameObject LoadNifPrefabDontAddToCache(string filePath)
	    {
	        var file = dataReader.LoadNIF(filePath);

	        var objBuilder = new NIFObjectBuilder(file, materialManager);
	        var prefab = objBuilder.BuildObject();

	        prefab.transform.parent = prefabContainerObj.transform;

	        // Add LOD support to the prefab.
	        var LODComponent = prefab.AddComponent<LODGroup>();
	        var LODs = new LOD[1]
	        {
	            new LOD(0.015f, prefab.GetComponentsInChildren<Renderer>())
	        };
	        LODComponent.SetLODs(LODs);

	        return prefab;
	    }
	}
}