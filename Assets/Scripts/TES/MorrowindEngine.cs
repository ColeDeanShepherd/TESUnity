using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TESUnity
{
	using ESM;
	
	public class MorrowindEngine
	{
		public const float maxInteractDistance = 2;

		public CELLRecord currentCell
		{
			get
			{
				return _currentCell;
			}
		}

		public MorrowindEngine(MorrowindDataReader dataReader)
		{
			this.dataReader = dataReader;

			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

			sunObj = GameObjectUtils.CreateDirectionalLight();
			sunObj.transform.rotation = Quaternion.Euler(new Vector3(50, 330, 0));
			sunObj.SetActive(false);

			waterObj = GameObject.Instantiate(TESUnity.instance.waterPrefab);
			waterObj.SetActive(false);
		}

		public GameObject InstantiateNIF(string filePath)
		{
			NIF.NiFile file = dataReader.LoadNIF(filePath);

			if(prefabObj == null)
			{
				prefabObj = new GameObject("Prefabs");
				prefabObj.SetActive(false);
			}

			GameObject prefab;

			if(!loadedNIFObjects.TryGetValue(filePath, out prefab))
			{
				var objBuilder = new NIFObjectBuilder(file, dataReader);
				prefab = objBuilder.BuildObject();

				prefab.transform.parent = prefabObj.transform;

				loadedNIFObjects[filePath] = prefab;
			}

			return GameObject.Instantiate(prefab);
		}
		public GameObject InstantiateCell(CELLRecord CELL)
		{
			Debug.Assert(CELL != null);

			if(!CELL.isInterior)
			{
				var cellIndices = new Vector2i(CELL.DATA.gridX, CELL.DATA.gridY);
				var LAND = dataReader.FindLANDRecord(cellIndices);

				if(LAND != null)
				{
					var cellObj = new GameObject("cell " + cellIndices.ToString());
					cellObj.tag = "Cell";

					var landObj = InstantiateLAND(LAND);

					if(landObj != null)
					{
						landObj.transform.parent = cellObj.transform;
					}

					InstantiateCellObjects(CELL, cellObj);

					return cellObj;
				}
				else
				{
					return null;
				}
			}
			else
			{
				GameObject cellObj = new GameObject(CELL.NAME.value);
				cellObj.tag = "Cell";

				InstantiateCellObjects(CELL, cellObj);

				return cellObj;
			}
		}
		public GameObject InstantiateExteriorCell(Vector2i cellIndices)
		{
			var CELL = dataReader.FindExteriorCellRecord(cellIndices);

			if(CELL != null)
			{
				return InstantiateCell(CELL);
			}
			else
			{
				return null;
			}
		}
		public GameObject InstantiateInteriorCell(string cellName)
		{
			var CELL = dataReader.FindInteriorCellRecord(cellName);

			if(CELL != null)
			{
				return InstantiateCell(CELL);
			}
			else
			{
				return null;
			}
		}
		public GameObject InstantiateLAND(LANDRecord LAND)
		{
			// Don't create anything if the LAND doesn't have height data.
			if(LAND.VHGT == null)
			{
				return null;
			}

			int LAND_SIDE_LENGTH_IN_SAMPLES = 65;
			var heights = new float[LAND_SIDE_LENGTH_IN_SAMPLES, LAND_SIDE_LENGTH_IN_SAMPLES];

			// Read in the heights in Morrowind units.
			const int VHGTIncrementToMWUnits = 8;
			float rowOffset = LAND.VHGT.referenceHeight;

			for(int y = 0; y < LAND_SIDE_LENGTH_IN_SAMPLES; y++)
			{
				rowOffset += LAND.VHGT.heightOffsets[y * LAND_SIDE_LENGTH_IN_SAMPLES];
				heights[y, 0] = VHGTIncrementToMWUnits * rowOffset;

				float colOffset = rowOffset;

				for(int x = 1; x < LAND_SIDE_LENGTH_IN_SAMPLES; x++)
				{
					colOffset += LAND.VHGT.heightOffsets[(y * LAND_SIDE_LENGTH_IN_SAMPLES) + x];
					heights[y, x] = VHGTIncrementToMWUnits * colOffset;
				}
			}

			// Change the heights to percentages.
			float minHeight, maxHeight;
			Utils.GetExtrema(heights, out minHeight, out maxHeight);

			for(int y = 0; y < LAND_SIDE_LENGTH_IN_SAMPLES; y++)
			{
				for(int x = 0; x < LAND_SIDE_LENGTH_IN_SAMPLES; x++)
				{
					heights[y, x] = Utils.ChangeRange(heights[y, x], minHeight, maxHeight, 0, 1);
				}
			}

			// Texture the terrain.
			SplatPrototype[] splatPrototypes = null;
			float[,,] alphaMap = null;

			if(LAND.VTEX != null)
			{
				// Create splat prototypes.
				var splatPrototypeList = new List<SplatPrototype>();
				var texInd2splatInd = new Dictionary<ushort, int>();

				for(int i = 0; i < LAND.VTEX.textureIndices.Length; i++)
				{
					short textureIndex = (short)((short)LAND.VTEX.textureIndices[i] - 1);

					if(textureIndex < 0)
					{
						continue;
					}

					if(!texInd2splatInd.ContainsKey((ushort)textureIndex))
					{
						// Load terrain texture.
						var LTEX = dataReader.FindLTEXRecord(textureIndex);
						var textureFileName = LTEX.DATA.value;
						var textureName = Path.GetFileNameWithoutExtension(textureFileName);
						var texture = dataReader.LoadTexture(textureName);

						// Create the splat prototype.
						var splat = new SplatPrototype();
						splat.texture = texture;
						splat.smoothness = 0;
						splat.metallic = 0;

						// Update collections.
						var splatIndex = splatPrototypeList.Count;
						splatPrototypeList.Add(splat);
						texInd2splatInd.Add((ushort)textureIndex, splatIndex);
					}
				}

				splatPrototypes = splatPrototypeList.ToArray();

				// Create the alpha map.
				int VTEX_ROWS = 16;
				int VTEX_COLUMNS = VTEX_ROWS;
				alphaMap = new float[VTEX_ROWS, VTEX_COLUMNS, splatPrototypes.Length];

				for(int y = 0; y < VTEX_ROWS; y++)
				{
					var yMajor = y / 4;
					var yMinor = y - (yMajor * 4);

					for(int x = 0; x < VTEX_COLUMNS; x++)
					{
						var xMajor = x / 4;
						var xMinor = x - (xMajor * 4);

						var texIndex = (short)((short)LAND.VTEX.textureIndices[(yMajor * 64) + (xMajor * 16) + (yMinor * 4) + xMinor] - 1);

						if(texIndex >= 0)
						{
							var splatIndex = texInd2splatInd[(ushort)texIndex];

							alphaMap[y, x, splatIndex] = 1;
						}
						else
						{
							alphaMap[y, x, 0] = 1;
						}
					}
				}
			}

			// Create the terrain.
			var heightRange = maxHeight - minHeight;
			var terrainPosition = new Vector3(Convert.exteriorCellSideLengthInMeters * LAND.gridCoords.x, minHeight / Convert.meterInMWUnits, Convert.exteriorCellSideLengthInMeters * LAND.gridCoords.y);

			var heightSampleDistance = Convert.exteriorCellSideLengthInMeters / (LAND_SIDE_LENGTH_IN_SAMPLES - 1);
			var terrain = GameObjectUtils.CreateTerrain(heights, heightRange / Convert.meterInMWUnits, heightSampleDistance, splatPrototypes, alphaMap, terrainPosition);
			terrain.GetComponent<Terrain>().materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;
			return terrain;
		}

		public void SpawnPlayerOutside(Vector3 position)
		{
			var cellIndices = GetExteriorCellIndices(position);
			_currentCell = dataReader.FindExteriorCellRecord(cellIndices);

			var camera = CreateFlyingCamera(position);

			OnExteriorCell(_currentCell);
		}
		public void SpawnPlayerInside(string interiorCellName, Vector3 position)
		{
			_currentCell = dataReader.FindInteriorCellRecord(interiorCellName);

			Debug.Assert(_currentCell != null);

			var camera = CreateFlyingCamera(position);

			CreateInteriorCell(interiorCellName);

			OnInteriorCell(_currentCell);
		}
		public void Update()
		{
			// The current cell can be null if the player is outside of the defined game world.
			if((_currentCell == null) || !_currentCell.isInterior)
			{
				UpdateExteriorCells();
			}
		}
		public void TryOpenDoor()
		{
			// Cast a ray to see what the camera is looking at.
			RaycastHit hitInfo;
			var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

			if(Physics.Raycast(ray, out hitInfo, maxInteractDistance))
			{
				// Find the door associated with the hit collider.
				GameObject doorObj = GameObjectUtils.FindObjectWithTagUpHeirarchy(hitInfo.collider.gameObject, "Door");

				if(doorObj != null)
				{
					var doorComponent = doorObj.GetComponent<DoorComponent>();
					
					if(doorComponent.leadsToAnotherCell)
					{
						// The door leads to another cell, so destroy all currently loaded cells.
						DestroyAllCells();

						// Load the new cell.
						CELLRecord newCell;

						if((doorComponent.doorExitName != null) && (doorComponent.doorExitName != ""))
						{
							newCell = dataReader.FindInteriorCellRecord(doorComponent.doorExitName);
							Debug.Assert(newCell.isInterior);
							cellObjects[Vector2i.zero] = InstantiateCell(newCell);

							OnInteriorCell(newCell);
						}
						else
						{
							var cellIndices = GetExteriorCellIndices(doorComponent.doorExitPos);
							newCell = dataReader.FindExteriorCellRecord(cellIndices);
							Debug.Assert(!newCell.isInterior);

							OnExteriorCell(newCell);
						}

						Camera.main.transform.position = doorComponent.doorExitPos;
						Camera.main.transform.rotation = doorComponent.doorExitOrientation;

						_currentCell = newCell;
					}
				}
			}
		}

		private MorrowindDataReader dataReader;

		private Dictionary<string, GameObject> loadedNIFObjects = new Dictionary<string, GameObject>();
		private GameObject prefabObj;

		private Dictionary<Vector2i, GameObject> cellObjects = new Dictionary<Vector2i, GameObject>();
		private int cellRadius = 1;
		private CELLRecord _currentCell;

		private GameObject sunObj;
		private GameObject waterObj;

		private Color32 defaultAmbientColor = new Color32(137, 140, 160, 255);

		private void InstantiateCellObjects(CELLRecord CELL, GameObject parent)
		{
			foreach(var refObjDataGroup in CELL.refObjDataGroups)
			{
				Record objRecord;

				// Find the ESM record associated with the referenced object.
				if(dataReader.MorrowindESMFile.objectsByIDString.TryGetValue(refObjDataGroup.NAME.value, out objRecord))
				{
					var modelFileName = ESM.RecordUtils.GetModelFileName(objRecord);
					GameObject modelObj = null;

					// If the model file name is valid, instantiate it.
					if((modelFileName != null) && (modelFileName != ""))
					{
						var modelFilePath = "meshes\\" + modelFileName;

						modelObj = InstantiateNIF(modelFilePath);
						PostProcessInstantiatedCellObject(modelObj, objRecord, refObjDataGroup);
						modelObj.transform.parent = parent.transform;
					}

					if(objRecord is LIGHRecord)
					{
						var lightObj = InstantiateLight((LIGHRecord)objRecord, CELL.isInterior);

						if(modelObj != null)
						{
							var attachLightTransform = modelObj.transform.FindChild("AttachLight");

							if(attachLightTransform != null)
							{
								lightObj.transform.position = attachLightTransform.position;
								lightObj.transform.rotation = attachLightTransform.rotation;

								lightObj.transform.parent = attachLightTransform;
							}
							else
							{
								lightObj.transform.position = GameObjectUtils.GetVisualBoundsRecursive(modelObj).center;
								lightObj.transform.rotation = modelObj.transform.rotation;

								lightObj.transform.parent = modelObj.transform;
							}
						}
						else
						{
							PostProcessInstantiatedCellObject(lightObj, objRecord, refObjDataGroup);
							lightObj.transform.parent = parent.transform;
						}
					}
				}
				/*else
				{
					Debug.Log("Unknown Object: " + refObjDataGroup.NAME.value);
				}*/
			}
		}
		private GameObject InstantiateLight(LIGHRecord LIGH, bool indoors)
		{
			var lightObj = new GameObject("Light");

			var lightComponent = lightObj.AddComponent<Light>();
			lightComponent.range = 3 * (LIGH.LHDT.radius / Convert.meterInMWUnits);
			lightComponent.color = new Color32(LIGH.LHDT.red, LIGH.LHDT.green, LIGH.LHDT.blue, 255);
			lightComponent.intensity = 1.5f;
			//lightComponent.shadows = LightShadows.Soft;

			if(!indoors)
			{
				lightComponent.enabled = false;
			}

			return lightObj;
		}
		// Called by InstantiateCellObjects.
		private void PostProcessInstantiatedCellObject(GameObject gameObject, ESM.Record record, CELLRecord.RefObjDataGroup refObjDataGroup)
		{
			if(refObjDataGroup.XSCL != null)
			{
				gameObject.transform.localScale = Vector3.one * refObjDataGroup.XSCL.value;
			}
			
			gameObject.transform.position += Convert.NifPointToUnityPoint(refObjDataGroup.DATA.position);
			gameObject.transform.rotation *= Convert.NifEulerAnglesToUnityQuaternion(refObjDataGroup.DATA.eulerAngles);

			if(record is DOORRecord)
			{
				gameObject.tag = "Door";

				var DOOR = (DOORRecord)record;
				var doorComponent = gameObject.AddComponent<DoorComponent>();

				if(DOOR.FNAM != null)
				{
					doorComponent.doorName = DOOR.FNAM.value;
				}

				if((refObjDataGroup.DNAM != null) || (refObjDataGroup.DODT != null))
				{
					doorComponent.leadsToAnotherCell = true;

					if(refObjDataGroup.DNAM != null)
					{
						doorComponent.doorExitName = refObjDataGroup.DNAM.value;
					}

					if(refObjDataGroup.DODT != null)
					{
						doorComponent.doorExitPos = Convert.NifPointToUnityPoint(refObjDataGroup.DODT.position);
						doorComponent.doorExitOrientation = Convert.NifEulerAnglesToUnityQuaternion(refObjDataGroup.DODT.eulerAngles);
					}
				}
				else
				{
					doorComponent.leadsToAnotherCell = false;
				}
			}
		}

		private Vector2i GetExteriorCellIndices(Vector3 point)
		{
			return new Vector2i(Mathf.FloorToInt(point.x / Convert.exteriorCellSideLengthInMeters), Mathf.FloorToInt(point.z / Convert.exteriorCellSideLengthInMeters));
		}
		private void UpdateExteriorCells()
		{
			var cameraCellIndices = GetExteriorCellIndices(Camera.main.transform.position);

			_currentCell = dataReader.FindExteriorCellRecord(cameraCellIndices);

			var minCellX = cameraCellIndices.x - cellRadius;
			var maxCellX = cameraCellIndices.x + cellRadius;
			var minCellY = cameraCellIndices.y - cellRadius;
			var maxCellY = cameraCellIndices.y + cellRadius;

			// Destroy out of range cells.
			var outOfRangeCellIndices = new List<Vector2i>();

			foreach(var KVPair in cellObjects)
			{
				if((KVPair.Key.x < minCellX) || (KVPair.Key.x > maxCellX) || (KVPair.Key.y < minCellY) || (KVPair.Key.y > maxCellY))
				{
					outOfRangeCellIndices.Add(KVPair.Key);
				}
			}

			foreach(var cellIndices in outOfRangeCellIndices)
			{
				DestroyExteriorCell(cellIndices);
			}

			// Create new cells.
			for(int x = minCellX; x <= maxCellX; x++)
			{
				for(int y = minCellY; y <= maxCellY; y++)
				{
					var cellIndices = new Vector2i(x, y);

					if(!cellObjects.ContainsKey(cellIndices))
					{
						CreateExteriorCell(cellIndices);
					}
				}
			}
		}
		private GameObject CreateExteriorCell(Vector2i indices)
		{
			var cellObj = InstantiateExteriorCell(indices);
			cellObjects[indices] = cellObj;

			return cellObj;
		}
		private void DestroyExteriorCell(Vector2i indices)
		{
			GameObject cellObj;

			if(cellObjects.TryGetValue(indices, out cellObj))
			{
				cellObjects.Remove(indices);
				GameObject.Destroy(cellObj);
			}
			else
			{
				Debug.LogError("Tried to destroy a cell that isn't created.");
			}
		}
		private GameObject CreateInteriorCell(string cellName)
		{
			var cellObj = InstantiateInteriorCell(cellName);
			cellObjects[Vector2i.zero] = cellObj;

			return cellObj;
		}
		private void DestroyAllCells()
		{
			foreach(var keyValuePair in cellObjects)
			{
				GameObject.Destroy(keyValuePair.Value);
			}

			cellObjects.Clear();
		}

		private void OnExteriorCell(CELLRecord CELL)
		{
			RenderSettings.ambientLight = defaultAmbientColor;

			sunObj.SetActive(true);

			waterObj.transform.position = Vector3.zero;
			waterObj.SetActive(true);
		}
		private void OnInteriorCell(CELLRecord CELL)
		{
			RenderSettings.ambientLight = ColorUtils.B8G8R8ToColor32(CELL.AMBI.ambientColor);

			sunObj.SetActive(false);

			if(CELL.WHGT != null)
			{
				waterObj.transform.position = new Vector3(0, CELL.WHGT.value / Convert.meterInMWUnits, 0);
				waterObj.SetActive(true);
			}
			else
			{
				waterObj.SetActive(false);
			}
		}

		private GameObject CreateFlyingCamera(Vector3 position)
		{
			var camera = GameObjectUtils.CreateMainCamera();
			camera.AddComponent<FlyingCameraComponent>();

			camera.transform.position = position;

			return camera;
		}
	}
}