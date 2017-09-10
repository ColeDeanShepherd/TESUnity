using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
			    prefab = LoadNifPrefabDontAddToPrefabCache(filePath);
				nifPrefabs[filePath] = prefab;
			}

            // Instantiate the prefab.
			return GameObject.Instantiate(prefab);
		}
        public void PreloadNifFileAsync(string filePath)
        {
            // If the NIF prefab has already been created we don't have to load the file again.
            if(nifPrefabs.ContainsKey(filePath)) { return; }
            
            Task<NIF.NiFile> nifFileLoadingTask;

            // Start loading the NIF asynchronously if we haven't already started.
            if(!nifFilePreloadTasks.TryGetValue(filePath, out nifFileLoadingTask))
            {
                nifFileLoadingTask = dataReader.LoadNifAsync(filePath);
                nifFilePreloadTasks[filePath] = nifFileLoadingTask;
            }
        }
		
		private MorrowindDataReader dataReader;
		private MaterialManager materialManager;
		private GameObject prefabContainerObj;

        private Dictionary<string, Task<NIF.NiFile>> nifFilePreloadTasks = new Dictionary<string, Task<NIF.NiFile>>();
		private Dictionary<string, GameObject> nifPrefabs = new Dictionary<string, GameObject>();

	    private void EnsurePrefabContainerObjectExists()
	    {
	        if(prefabContainerObj == null)
	        {
	            prefabContainerObj = new GameObject("NIF Prefabs");
	            prefabContainerObj.SetActive(false);
	        }
        }
	    private GameObject LoadNifPrefabDontAddToPrefabCache(string filePath)
	    {
            Debug.Assert(!nifPrefabs.ContainsKey(filePath));

            PreloadNifFileAsync(filePath);
            var file = nifFilePreloadTasks[filePath].Result;
            nifFilePreloadTasks.Remove(filePath);

            // Start pre-loading all the NIF's textures.
            foreach(var anNiObject in file.blocks)
            {
                if(anNiObject is NIF.NiSourceTexture)
                {
                    var anNiSourceTexture = (NIF.NiSourceTexture)anNiObject;

                    if((anNiSourceTexture.fileName != null) && (anNiSourceTexture.fileName != ""))
                    {
                        materialManager.TextureManager.PreloadTextureFileAsync(anNiSourceTexture.fileName);
                    }
                }
            }

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