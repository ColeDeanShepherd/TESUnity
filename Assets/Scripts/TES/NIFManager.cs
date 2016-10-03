using System;
using System.Collections.Generic;
using System.Threading;
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
		/// Pre-load a NIF file asynchronously.
		/// </summary>
		public void PreLoadNIFAsync(string filePath)
		{
			lock(dictionariesLock)
			{
				// If the NIF is already loading or has already loaded, return.
				if(preLoadedNIFFiles.ContainsKey(filePath) || NIFPrefabs.ContainsKey(filePath))
				{
					return;
				}

				// Ensure filePath is a key in the dictionary so that the NIF isn't loaded multiple times.
				preLoadedNIFFiles[filePath] = null;
			}

			var waitCallback = new WaitCallback(PreLoadNIFThreadFunc);
			if(!ThreadPool.QueueUserWorkItem(waitCallback, filePath))
			{
				throw new Exception("Failed adding the NIF preloading task for the file " + filePath + " to the thread pool.");
			}
		}

		/// <summary>
		/// Determines if a NIF file is done pre-loading. Also returns true for cached models that weren't explicitly pre-loaded. Thread-safe.
		/// </summary>
		public bool IsDonePreLoading(string filePath)
		{
			NIF.NiFile preLoadedFile;

			lock(dictionariesLock)
			{
				if(preLoadedNIFFiles.TryGetValue(filePath, out preLoadedFile))
				{
					return preLoadedFile != null;
				}
				else
				{
					return NIFPrefabs.ContainsKey(filePath);
				}
			}
		}

		/// <summary>
		/// Instantiates a NIF file. Must be called from the main thread.
		/// </summary>
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

			lock(dictionariesLock)
			{
				NIFPrefabs.TryGetValue(filePath, out prefab);
			}

			if(prefab == null)
			{
				// Load the NIF file and instantiate it.
				var file = LoadNIFAndRemoveFromCache(filePath);

				var objBuilder = new NIFObjectBuilder(file, dataReader, materialManager);
				prefab = objBuilder.BuildObject();

				prefab.transform.parent = prefabContainerObj.transform;

				// Add LOD support to the prefab.
				var LODComponent = prefab.AddComponent<LODGroup>();

				var LODs = new LOD[1]
				{
					new LOD(0.015f, prefab.GetComponentsInChildren<Renderer>())
				};

				LODComponent.SetLODs(LODs);

				// Cache the prefab.
				lock(dictionariesLock)
				{
					NIFPrefabs[filePath] = prefab;
				}
			}

			return GameObject.Instantiate(prefab);
		}
		
		private MorrowindDataReader dataReader;
		private MaterialManager materialManager;

		private GameObject prefabContainerObj;

		private object dictionariesLock = new object(); // A lock for NIFPrefabs and preLoadedNIFFiles.
		private Dictionary<string, GameObject> NIFPrefabs = new Dictionary<string, GameObject>();
		private Dictionary<string, NIF.NiFile> preLoadedNIFFiles = new Dictionary<string, NIF.NiFile>();

		private void PreLoadNIFThreadFunc(object filePathObj)
		{
			// Load the NIF file without loading textures.
			var filePath = (string)filePathObj;
			NIF.NiFile file = dataReader.LoadNIF(filePath);

			// Pre-load the NIF file's textures.
			foreach(var anNiObject in file.blocks)
			{
				if(anNiObject is NIF.NiSourceTexture)
				{
					var anNiSourceTexture = (NIF.NiSourceTexture)anNiObject;

					if((anNiSourceTexture.fileName != null) && (anNiSourceTexture.fileName != ""))
					{
						materialManager.TextureManager.PreLoadTexture(anNiSourceTexture.fileName);
					}
				}
			}

			lock(dictionariesLock)
			{
				preLoadedNIFFiles[filePath] = file;
			}
		}

		/// <summary>
		/// Loads a NIF file and removes it from the cache if necessary.
		/// Used when instantiating NIF files to take advantage of asynchronously loaded files.
		/// Thread safe.
		/// </summary>
		private NIF.NiFile LoadNIFAndRemoveFromCache(string filePath)
		{
			// Try to get the cached NiFile.
			NIF.NiFile file;

			lock(dictionariesLock)
			{
				preLoadedNIFFiles.TryGetValue(filePath, out file);
			}

			// If there is no cached NiFile.
			if(file == null)
			{
				file = dataReader.LoadNIF(filePath);
			}
			else
			{
				lock(dictionariesLock)
				{
					preLoadedNIFFiles.Remove(filePath);
				}
			}

			return file;
		}
	}
}