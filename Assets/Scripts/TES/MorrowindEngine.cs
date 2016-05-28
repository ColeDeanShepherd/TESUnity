using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace TESUnity
{
	using ESM;

	public class InRangeCellInfo
	{
		public GameObject gameObject;
		public GameObject objectsContainerGameObject;
		public IEnumerator creationCoroutine;

		public InRangeCellInfo(GameObject gameObject, GameObject objectsContainerGameObject, IEnumerator creationCoroutine)
		{
			this.gameObject = gameObject;
			this.objectsContainerGameObject = objectsContainerGameObject;
			this.creationCoroutine = creationCoroutine;
		}
	}

	public class MorrowindEngine
	{
		#region Public
		public static MorrowindEngine instance;

		public const float maxInteractDistance = 3;
		public static int markerLayer
		{
			get
			{
				return LayerMask.NameToLayer("Marker");
			}
		}

		public static bool IsMarkerName(string name)
		{
			var lowerName = name.ToLower();

			return	lowerName == "prisonmarker" ||
					lowerName == "divinemarker" ||
					lowerName == "templemarker" ||
					lowerName == "northmarker" ||
					lowerName == "doormarker" ||
					lowerName == "travelmarker" ||
					lowerName == "editormarker";
		}

		public MorrowindDataReader dataReader;
		public TextureManager textureManager;
		public MaterialManager materialManager;
		public NIFManager theNIFManager;
		public TemporalLoadBalancer temporalLoadBalancer = new TemporalLoadBalancer();

		public GameObject canvasObj;
		public CELLRecord currentCell
		{
			get
			{
				return _currentCell;
			}
		}

		public MorrowindEngine(MorrowindDataReader dataReader)
		{
			Debug.Assert(instance == null);

			instance = this;
			this.dataReader = dataReader;
			textureManager = new TextureManager(this.dataReader);
			materialManager = new MaterialManager(textureManager);
			theNIFManager = new NIFManager(this.dataReader, materialManager);

			canvasObj = GUIUtils.CreateCanvas();
			GUIUtils.CreateEventSystem();

			interactTextObj = GUIUtils.CreateText("", canvasObj);
			interactTextObj.GetComponent<Text>().color = Color.white;

			var interactTextCSF = interactTextObj.AddComponent<ContentSizeFitter>();
			interactTextCSF.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			interactTextCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			interactTextObj.SetActive(false);

			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
			RenderSettings.ambientIntensity = 1.5f;

			sunObj = GameObjectUtils.CreateDirectionalLight(Vector3.zero, Quaternion.Euler(new Vector3(50, 330, 0)));
			sunObj.GetComponent<Light>().shadows = LightShadows.Hard;
			sunObj.SetActive(false);

			waterObj = GameObject.Instantiate(TESUnity.instance.waterPrefab);
			waterObj.SetActive(false);
		}

		public InRangeCellInfo InstantiateCell(CELLRecord CELL)
		{
			Debug.Assert(CELL != null);

			string cellObjName = null;
			LANDRecord LAND = null;

			if(!CELL.isInterior)
			{
				LAND = dataReader.FindLANDRecord(CELL.gridCoords);

				if(LAND != null)
				{
					cellObjName = "cell " + CELL.gridCoords.ToString();
				}
				else
				{
					return null;
				}
			}
			else
			{
				cellObjName = CELL.NAME.value;
			}

			var cellObj = new GameObject(cellObjName);
			cellObj.tag = "Cell";

			var cellObjectsContainer = new GameObject("objects");
			cellObjectsContainer.transform.parent = cellObj.transform;

			var cellCreationCoroutine = InstantiateCellObjectsCoroutine(CELL, LAND, cellObj, cellObjectsContainer);
			temporalLoadBalancer.AddTask(cellCreationCoroutine);

			return new InRangeCellInfo(cellObj, cellObjectsContainer, cellCreationCoroutine);
		}
		public GameObject InstantiateLAND(LANDRecord LAND)
		{
			Debug.Assert(LAND != null);

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
						var texture = textureManager.LoadTexture(textureName);

						// Create the splat prototype.
						var splat = new SplatPrototype();
						splat.texture = texture;
						splat.smoothness = 0;
						splat.metallic = 0;
						splat.tileSize = new Vector2(6, 6);

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

			playerObj = CreatePlayer(position);

			UpdateExteriorCells(true);
			OnExteriorCell(_currentCell);
		}
		public void SpawnPlayerInside(string interiorCellName, Vector3 position)
		{
			_currentCell = dataReader.FindInteriorCellRecord(interiorCellName);

			Debug.Assert(_currentCell != null);

			playerObj = CreatePlayer(position);

			var cellInfo = CreateInteriorCell(interiorCellName);
			temporalLoadBalancer.WaitForTask(cellInfo.creationCoroutine);

			OnInteriorCell(_currentCell);
		}
		public void Update()
		{
			// The current cell can be null if the player is outside of the defined game world.
			if((_currentCell == null) || !_currentCell.isInterior)
			{
				UpdateExteriorCells();
			}

			temporalLoadBalancer.RunTasks(desiredWorkTimePerFrame);

			CastInteractRay();
		}
		public void CastInteractRay()
		{
			interactTextObj.SetActive(false);

			// Cast a ray to see what the camera is looking at.
			var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

			var raycastHitCount = Physics.RaycastNonAlloc(ray, interactRaycastHitBuffer, maxInteractDistance);

			for(int i = 0; i < raycastHitCount; i++)
			{
				var hitInfo = interactRaycastHitBuffer[i];

				// Find the door associated with the hit collider.
				GameObject doorObj = GameObjectUtils.FindObjectWithTagUpHeirarchy(hitInfo.collider.gameObject, "Door");

				if(doorObj != null)
				{
					var doorComponent = doorObj.GetComponent<DoorComponent>();

					interactTextObj.SetActive(true);

					if(doorComponent.leadsToAnotherCell)
					{
						interactTextObj.GetComponent<Text>().text = doorComponent.doorExitName;
					}
					else
					{
						interactTextObj.GetComponent<Text>().text = doorComponent.doorName;
					}

					if(Input.GetKeyDown(KeyCode.E))
					{
						OpenDoor(doorComponent);
					}

					break;
				}
			}
		}
		#endregion

		#region Private
		private const float playerHeight = 2;
		private const float playerRadius = 0.4f;

		private float desiredWorkTimePerFrame = 1.0f / 100;

		private Dictionary<Vector2i, InRangeCellInfo> cellObjects = new Dictionary<Vector2i, InRangeCellInfo>();
		private Dictionary<Vector2i, IEnumerator> cellCreationCoroutines = new Dictionary<Vector2i, IEnumerator>();

		private int cellRadius = 4;
		private int detailRadius = 2;
		private CELLRecord _currentCell;

		private GameObject interactTextObj;
		private GameObject sunObj;
		private GameObject waterObj;
		private GameObject playerObj;

		private Color32 defaultAmbientColor = new Color32(137, 140, 160, 255);

		private RaycastHit[] interactRaycastHitBuffer = new RaycastHit[32];

		private IEnumerator InstantiateCellObjectsCoroutine(CELLRecord CELL, LANDRecord LAND, GameObject cellObj, GameObject cellObjectsContainer)
		{
			yield return null;

			if(LAND != null)
			{
				var landObj = InstantiateLAND(LAND);

				if(landObj != null)
				{
					landObj.transform.parent = cellObj.transform;
					yield return null;
				}
			}

			foreach(var refObjDataGroup in CELL.refObjDataGroups)
			{
				InstantiateCellObject(CELL, cellObjectsContainer, refObjDataGroup);

				yield return null;
			}
		}
		private void InstantiateCellObject(CELLRecord CELL, GameObject parent, CELLRecord.RefObjDataGroup refObjDataGroup)
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

					modelObj = theNIFManager.InstantiateNIF(modelFilePath);
					PostProcessInstantiatedCellObject(modelObj, objRecord, refObjDataGroup);

					modelObj.transform.parent = parent.transform;
				}

				if(objRecord is LIGHRecord)
				{
					var lightObj = InstantiateLight((LIGHRecord)objRecord, CELL.isInterior);

					if(modelObj != null)
					{
						GameObject attachLightObj = GameObjectUtils.FindChildRecursively(modelObj, "AttachLight");

						if(attachLightObj == null)
						{
							attachLightObj = GameObjectUtils.FindChildWithNameSubstringRecursively(modelObj, "Emitter");
						}

						if(attachLightObj != null)
						{
							lightObj.transform.position = attachLightObj.transform.position;
							lightObj.transform.rotation = attachLightObj.transform.rotation;

							lightObj.transform.parent = attachLightObj.transform;
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

				// Add a door component.
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
						doorComponent.leadsToInteriorCell = true;
					}
					else
					{
						doorComponent.leadsToInteriorCell = false;
					}

					if(refObjDataGroup.DODT != null)
					{
						doorComponent.doorExitPos = Convert.NifPointToUnityPoint(refObjDataGroup.DODT.position);
						doorComponent.doorExitOrientation = Convert.NifEulerAnglesToUnityQuaternion(refObjDataGroup.DODT.eulerAngles);

						if(!doorComponent.leadsToInteriorCell)
						{
							var doorExitCell = dataReader.FindExteriorCellRecord(GetExteriorCellIndices(doorComponent.doorExitPos));
							doorComponent.doorExitName = (doorExitCell != null) ? doorExitCell.RGNN.value : doorComponent.doorName;
						}
					}
				}
				else
				{
					doorComponent.leadsToAnotherCell = false;
				}
			}

			if(IsMarkerName(refObjDataGroup.NAME.value))
			{
				GameObjectUtils.SetLayerRecursively(gameObject, markerLayer);
			}
		}

		private Vector2i GetExteriorCellIndices(Vector3 point)
		{
			return new Vector2i(Mathf.FloorToInt(point.x / Convert.exteriorCellSideLengthInMeters), Mathf.FloorToInt(point.z / Convert.exteriorCellSideLengthInMeters));
		}
		private void UpdateExteriorCells(bool immediate = false)
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
						var cellInfo = CreateExteriorCell(cellIndices);
						
						if(immediate)
						{
							temporalLoadBalancer.WaitForTask(cellInfo.creationCoroutine);
						}
					}
				}
			}

			// Update LODs.
			foreach(var keyValuePair in cellObjects)
			{
				Vector2i cellIndices = keyValuePair.Key;
				InRangeCellInfo cellInfo = keyValuePair.Value;

				var cellXDistance = Mathf.Abs(cameraCellIndices.x - cellIndices.x);
				var cellYDistance = Mathf.Abs(cameraCellIndices.y - cellIndices.y);
				var cellDistance = Mathf.Max(cellXDistance, cellYDistance);

				if(cellDistance <= detailRadius)
				{
					cellInfo.objectsContainerGameObject.SetActive(true);
				}
				else
				{
					cellInfo.objectsContainerGameObject.SetActive(false);
				}
			}
		}
		private InRangeCellInfo CreateExteriorCell(Vector2i cellIndices)
		{
			var CELL = dataReader.FindExteriorCellRecord(cellIndices);

			if(CELL != null)
			{
				var cellInfo = InstantiateCell(CELL);
				cellObjects[cellIndices] = cellInfo;

				return cellInfo;
			}
			else
			{
				return null;
			}
		}
		private void DestroyExteriorCell(Vector2i indices)
		{
			InRangeCellInfo cellInfo;

			if(cellObjects.TryGetValue(indices, out cellInfo))
			{
				temporalLoadBalancer.CancelTask(cellInfo.creationCoroutine);
				GameObject.Destroy(cellInfo.gameObject);
				cellObjects.Remove(indices);
			}
			else
			{
				Debug.LogError("Tried to destroy a cell that isn't created.");
			}
		}
		private InRangeCellInfo CreateInteriorCell(string cellName)
		{
			var CELL = dataReader.FindInteriorCellRecord(cellName);

			if(CELL != null)
			{
				var cellInfo = InstantiateCell(CELL);
				cellObjects[Vector2i.zero] = cellInfo;

				return cellInfo;
			}
			else
			{
				return null;
			}
		}
		private void DestroyAllCells()
		{
			foreach(var keyValuePair in cellObjects)
			{
				temporalLoadBalancer.CancelTask(keyValuePair.Value.creationCoroutine);
				GameObject.Destroy(keyValuePair.Value.gameObject);
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

		private void OpenDoor(DoorComponent doorComponent)
		{
			if(!doorComponent.leadsToAnotherCell)
			{
				if(!doorComponent.isOpen)
				{
					doorComponent.gameObject.transform.Rotate(new Vector3(0, 90, 0));
					doorComponent.isOpen = true;
				}
				else
				{
					doorComponent.gameObject.transform.Rotate(new Vector3(0, -90, 0));
					doorComponent.isOpen = false;
				}
			}
			else
			{
				// The door leads to another cell, so destroy all currently loaded cells.
				DestroyAllCells();

				// Move the player.
				playerObj.transform.position = doorComponent.doorExitPos + new Vector3(0, playerHeight / 2, 0);
				playerObj.transform.localEulerAngles = new Vector3(0, doorComponent.doorExitOrientation.eulerAngles.y, 0);

				// Load the new cell.
				CELLRecord newCell;

				if(doorComponent.leadsToInteriorCell)
				{
					newCell = dataReader.FindInteriorCellRecord(doorComponent.doorExitName);

					var cellInfo = InstantiateCell(newCell);
					temporalLoadBalancer.WaitForTask(cellInfo.creationCoroutine);

					cellObjects[Vector2i.zero] = cellInfo;

					OnInteriorCell(newCell);
				}
				else
				{
					var cellIndices = GetExteriorCellIndices(doorComponent.doorExitPos);
					newCell = dataReader.FindExteriorCellRecord(cellIndices);

					UpdateExteriorCells(true);

					OnExteriorCell(newCell);
				}

				_currentCell = newCell;
			}
		}

		private GameObject CreatePlayer(Vector3 position)
		{
			// Create the player.
			var player = new GameObject();
			player.name = "Player";

			var capsuleCollider = player.AddComponent<CapsuleCollider>();
			capsuleCollider.height = playerHeight;
			capsuleCollider.radius = playerRadius;

			var rigidbody = player.AddComponent<Rigidbody>();
			rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

			var playerComponent = player.AddComponent<PlayerComponent>();

			// Create the camera point object.
			var eyeHeight = 0.9f * capsuleCollider.height;

			var cameraPoint = new GameObject("Camera Point");
			cameraPoint.transform.localPosition = new Vector3(0, eyeHeight - (capsuleCollider.height / 2), 0);
			cameraPoint.transform.SetParent(player.transform, false);

			player.transform.position = position;

			// Create the player camera.
			var playerCamera = CreatePlayerCamera(position);
			playerCamera.transform.localPosition = Vector3.zero;
			playerCamera.transform.SetParent(cameraPoint.transform, false);

			// Create the player's lantern.
			var lantern = new GameObject("Lantern");

			var lightComponent = lantern.AddComponent<Light>();
			lightComponent.range = 20;
			lightComponent.intensity = 3;
			lightComponent.color = new Color32(245, 140, 40, 255);
			lightComponent.enabled = false;

			lantern.transform.localPosition = cameraPoint.transform.localPosition;
			lantern.transform.SetParent(playerComponent.transform, false);

			playerComponent.camera = playerCamera;
			playerComponent.lantern = lantern;

			return player;
		}
		private GameObject CreatePlayerCamera(Vector3 position)
		{
			var camera = GameObjectUtils.CreateMainCamera(position, Quaternion.identity);
			camera.GetComponent<Camera>().cullingMask = ~(1 << markerLayer);

			return camera;
		}
		private GameObject CreateFlyingCamera(Vector3 position)
		{
			var camera = GameObjectUtils.CreateMainCamera(position, Quaternion.identity);
			camera.AddComponent<FlyingCameraComponent>();
			camera.GetComponent<Camera>().cullingMask = ~(1 << markerLayer);

			return camera;
		}
		#endregion
	}
}