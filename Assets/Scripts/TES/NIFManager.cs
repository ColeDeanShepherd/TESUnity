using System.Collections.Generic;
using UnityEngine;

namespace TESUnity
{
	/// <summary>
	/// Manages loading and instantiation of NIF models.
	/// </summary>
	public class NIFManager
	{
		public NIFManager(MorrowindDataReader dataReader)
		{
			this.dataReader = dataReader;
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
				NIF.NiFile file = dataReader.LoadNIF(filePath);

				var objBuilder = new NIFObjectBuilder(file, dataReader);
				prefab = objBuilder.BuildObject();

				prefab.transform.parent = prefabContainerObj.transform;

				NIFPrefabs[filePath] = prefab;
			}

			return GameObject.Instantiate(prefab);
		}

		private MorrowindDataReader dataReader;
		
		private GameObject prefabContainerObj;
		private Dictionary<string, GameObject> NIFPrefabs = new Dictionary<string, GameObject>();
	}
}