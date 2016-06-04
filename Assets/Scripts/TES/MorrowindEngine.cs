using System;
using System.Collections;
using System.Collections.Generic;
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

		public MorrowindEngine(MorrowindDataReader dataReader , bool sunShadows = true)
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
			interactText = interactTextObj.GetComponent<Text>();

			var interactTextCSF = interactTextObj.AddComponent<ContentSizeFitter>();
			interactTextCSF.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			interactTextCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			interactTextObj.SetActive(false);

			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
			RenderSettings.ambientIntensity = 1.5f;

			sunObj = GameObjectUtils.CreateDirectionalLight(Vector3.zero, Quaternion.Euler(new Vector3(50, 330, 0)));
			sunObj.GetComponent<Light>().shadows = sunShadows ? LightShadows.Hard : LightShadows.None;
			sunObj.SetActive(false);

			waterObj = GameObject.Instantiate(TESUnity.instance.waterPrefab);
			waterObj.SetActive(false);
		}

		public Vector2i GetExteriorCellIndices(Vector3 point)
		{
			return new Vector2i(Mathf.FloorToInt(point.x / Convert.exteriorCellSideLengthInMeters), Mathf.FloorToInt(point.z / Convert.exteriorCellSideLengthInMeters));
		}
		public InRangeCellInfo InstantiateCell(CELLRecord CELL)
		{
			Debug.Assert(CELL != null);

			string cellObjName = null;
			LANDRecord LAND = null;

			if(!CELL.isInterior)
			{
				cellObjName = "cell " + CELL.gridCoords.ToString();
				LAND = dataReader.FindLANDRecord(CELL.gridCoords);
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

		/// <summary>
		/// Creates terrain representing a LAND record.
		/// </summary>
		/// <returns>A GameObject, or null if the LAND record does not contain sufficient data.</returns>
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
						var textureFilePath = LTEX.DATA.value;
						var texture = textureManager.LoadTexture(textureFilePath);

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

			playerObj = CreatePlayer(position, out playerCameraObj);

			UpdateExteriorCells(true, cellRadiusOnLoad);
			OnExteriorCell(_currentCell);
		}
		public void SpawnPlayerInside(string interiorCellName, Vector3 position)
		{
			_currentCell = dataReader.FindInteriorCellRecord(interiorCellName);

			Debug.Assert(_currentCell != null);

			playerObj = CreatePlayer(position, out playerCameraObj);

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
			// Cast a ray to see what the camera is looking at.
			Ray ray = new Ray(playerCameraObj.transform.position, playerCameraObj.transform.forward);

			int raycastHitCount = Physics.RaycastNonAlloc(ray, interactRaycastHitBuffer, maxInteractDistance);
			if ( raycastHitCount < 1 )
			{
				RemoveInteractText(); //deactivate text if no objects are hit
			}
			else
			{
				for ( int i = 0 ; i < raycastHitCount ; i++ )
				{
					RaycastHit hitInfo = interactRaycastHitBuffer[ i ];

					// Find the door associated with the hit collider.
					GameObject doorObj = GameObjectUtils.FindObjectWithTagUpHeirarchy( hitInfo.collider.gameObject , "Door" );
					GameObject containerObj = GameObjectUtils.FindObjectWithTagUpHeirarchy( hitInfo.collider.gameObject , "Container" );
					GameObject bookObj = GameObjectUtils.FindObjectWithTagUpHeirarchy( hitInfo.collider.gameObject , "Book" );
					GameObject miscObj = GameObjectUtils.FindObjectWithTagUpHeirarchy( hitInfo.collider.gameObject , "MiscObj" );

					if ( doorObj != null )
					{
						DoorComponent doorComponent = doorObj.GetComponent<DoorComponent>();
						SetInteractText( doorComponent.leadsToAnotherCell ? doorComponent.doorExitName : doorComponent.doorName );

						if ( Input.GetKeyDown( KeyCode.E ) )
						{
							OpenDoor( doorComponent );
						}

						break;
					}
					else if ( containerObj != null )
					{
						ContainerComponent component = containerObj.GetComponentInParent<ContainerComponent>();
						SetInteractText( component.containerName );
						break;
					}
					else if ( bookObj != null )
					{
						BookComponent component = bookObj.GetComponentInParent<BookComponent>();
						SetInteractText( component.bookTitle );
						TryRemoveObject( bookObj );
						break;
					}
					else if ( miscObj != null )
					{
						MiscComponent component = miscObj.GetComponentInParent<MiscComponent>();
						SetInteractText( component.itemName );
						TryRemoveObject( miscObj );
						break;
					}
					else
					{
						RemoveInteractText(); //deactivate text if no interactable [ DOORS ONLY - REQUIRES EXPANSION ] is found
					}
				}
			}
		}

		private void TryRemoveObject ( GameObject obj )
		{
			if ( Input.GetKeyDown( KeyCode.E ) )
			{
				Transform p = obj.transform;
				while ( p.parent != null && p.GetComponent<LODGroup>() == null ) p = p.parent; //kind of a hacky way to check when youre referencing the entirety of an individual object
				GameObject.Destroy( p.gameObject );
			}
		}

		public void ShowInteractiveText()
		{
			if ( !interactTextObj.activeSelf ) interactTextObj.SetActive( true );
		}

		public void RemoveInteractText ()
		{
			if ( interactTextObj.activeSelf ) interactTextObj.SetActive( false );
		}

		public void SetInteractText ( string text )
		{
			if ( interactText.text != text ) interactText.text = text;
			ShowInteractiveText();
		}
		#endregion

		#region Private
		private const float playerHeight = 2;
		private const float playerRadius = 0.4f;

		private float desiredWorkTimePerFrame = 1.0f / 160;

		private Dictionary<Vector2i, InRangeCellInfo> cellObjects = new Dictionary<Vector2i, InRangeCellInfo>();
		private Dictionary<Vector2i, IEnumerator> cellCreationCoroutines = new Dictionary<Vector2i, IEnumerator>();

		private int cellRadius = 4;
		private int detailRadius = 3;
		private int cellRadiusOnLoad = 2;
		private CELLRecord _currentCell;

		private GameObject interactTextObj;
		private Text interactText;
		private GameObject sunObj;
		private GameObject waterObj;
		private GameObject playerObj;
		private GameObject playerCameraObj;

		private Color32 defaultAmbientColor = new Color32(137, 140, 160, 255);

		private RaycastHit[] interactRaycastHitBuffer = new RaycastHit[32];

		private IEnumerator InstantiateCellObjectsCoroutine(CELLRecord CELL, LANDRecord LAND, GameObject cellObj, GameObject cellObjectsContainer)
		{
			// Return before doing any work to provide an IEnumerator handle to the coroutine.
			yield return null;

			// Instantiate terrain.
			if(LAND != null)
			{
				var landObj = InstantiateLAND(LAND);

				if(landObj != null)
				{
					landObj.transform.parent = cellObj.transform;
				}

				yield return null;
			}

			// Extract information about referenced objects. Do this all at once because it's fast.
			RefCellObjInfo[] refCellObjInfos = new RefCellObjInfo[CELL.refObjDataGroups.Count];
			
			for(int i = 0; i < CELL.refObjDataGroups.Count; i++)
			{
				var refObjInfo = new RefCellObjInfo();
				refObjInfo.refObjDataGroup = CELL.refObjDataGroups[i];

				// Get the record the RefObjDataGroup references.
				dataReader.MorrowindESMFile.objectsByIDString.TryGetValue(refObjInfo.refObjDataGroup.NAME.value, out refObjInfo.referencedRecord);

				if(refObjInfo.referencedRecord != null)
				{
					var modelFileName = ESM.RecordUtils.GetModelFileName(refObjInfo.referencedRecord);

					// If the model file name is valid, store the model file path.
					if((modelFileName != null) && (modelFileName != ""))
					{
						refObjInfo.modelFilePath = "meshes\\" + modelFileName;
					}
				}

				refCellObjInfos[i] = refObjInfo;
			}

			yield return null;

			// Start loading all required assets in background threads.
			foreach(var refCellObjInfo in refCellObjInfos)
			{
				if(refCellObjInfo.modelFilePath != null)
				{
					theNIFManager.PreLoadNIFAsync(refCellObjInfo.modelFilePath);

					yield return null;
				}
			}

			yield return null;

			// Instantiate objects when they are done loading, or if they don't need to load.
			int instantiatedObjectCount = 0;

			while(instantiatedObjectCount < refCellObjInfos.Length)
			{
				foreach(var refCellObjInfo in refCellObjInfos)
				{
					// Ignore objects that have already been instantiated.
					if(refCellObjInfo.isInstantiated)
					{
						continue;
					}

					// If the referenced object has a model and it has just finished pre-loading, instantiate the model.
					if(refCellObjInfo.modelFilePath != null)
					{
						Debug.Assert(!refCellObjInfo.isDonePreLoading);

						// Update isDonePreloading.
						refCellObjInfo.isDonePreLoading = theNIFManager.IsDonePreLoading(refCellObjInfo.modelFilePath);

						// If the model just finished pre-loading, instantiate it.
						if(refCellObjInfo.isDonePreLoading)
						{
							InstantiatePreLoadedCellObject(CELL, cellObjectsContainer, refCellObjInfo);
							refCellObjInfo.isInstantiated = true;

							instantiatedObjectCount++;

							yield return null;
						}
					}
					else // If the referenced object doesn't have a model, there is no loading to be done, so try to instantiate it.
					{
						InstantiatePreLoadedCellObject(CELL, cellObjectsContainer, refCellObjInfo);
						refCellObjInfo.isInstantiated = true;

						instantiatedObjectCount++;

						yield return null;
					}
				}

				// Yield after every foreach to prevent spinning if all models are loading.
				yield return null;
			}
		}
		private void InstantiatePreLoadedCellObject(CELLRecord CELL, GameObject parent, RefCellObjInfo refCellObjInfo)
		{
			if(refCellObjInfo.referencedRecord != null)
			{
				GameObject modelObj = null;

				// If the object has a model, instantiate it.
				if(refCellObjInfo.modelFilePath != null)
				{
					modelObj = theNIFManager.InstantiateNIF(refCellObjInfo.modelFilePath);
					PostProcessInstantiatedCellObject(modelObj, refCellObjInfo);

					modelObj.transform.parent = parent.transform;
				}
				
				// If the object has a light, instantiate it.
				if(refCellObjInfo.referencedRecord is LIGHRecord)
				{
					var lightObj = InstantiateLight((LIGHRecord)refCellObjInfo.referencedRecord, CELL.isInterior);

					// If the object also has a model, parent the model to the light.
					if(modelObj != null)
					{
						// Some NIF files have nodes named "AttachLight". Parent it to the light if it exists.
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
						else // If there is no "AttachLight", center the light in the model's bounds.
						{
							lightObj.transform.position = GameObjectUtils.GetVisualBoundsRecursive(modelObj).center;
							lightObj.transform.rotation = modelObj.transform.rotation;

							lightObj.transform.parent = modelObj.transform;
						}
					}
					else // If the light has no associated model, instantiate the light as a standalone object.
					{
						PostProcessInstantiatedCellObject(lightObj, refCellObjInfo);
						lightObj.transform.parent = parent.transform;
					}
				}
			}
			/*else
			{
				Debug.Log("Unknown Object: " + refCellObjInfo.refObjDataGroup.NAME.value);
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
		private void PostProcessInstantiatedCellObject(GameObject gameObject, RefCellObjInfo refCellObjInfo)
		{
			var refObjDataGroup = refCellObjInfo.refObjDataGroup;

			// Handle object transforms.
			if(refObjDataGroup.XSCL != null)
			{
				gameObject.transform.localScale = Vector3.one * refObjDataGroup.XSCL.value;
			}
			
			gameObject.transform.position += Convert.NifPointToUnityPoint(refObjDataGroup.DATA.position);
			gameObject.transform.rotation *= Convert.NifEulerAnglesToUnityQuaternion(refObjDataGroup.DATA.eulerAngles);

			#region doors
			// Handle doors.
			if(refCellObjInfo.referencedRecord is DOORRecord)
			{
				gameObject.tag = "Door";

				// Add a door component.
				var DOOR = (DOORRecord)refCellObjInfo.referencedRecord;
				var doorComponent = gameObject.AddComponent<DoorComponent>();

				if(DOOR.FNAM != null)
				{
					doorComponent.doorName = DOOR.FNAM.value;
				}

				// If the door leads to another cell (as opposed to just rotating to open).
				if((refObjDataGroup.DNAM != null) || (refObjDataGroup.DODT != null))
				{
					doorComponent.leadsToAnotherCell = true;

					// Does the door lead to an exterior cell or an interior cell?
					if(refObjDataGroup.DNAM != null)
					{
						doorComponent.doorExitName = refObjDataGroup.DNAM.value;
						doorComponent.leadsToInteriorCell = true;
					}
					else
					{
						doorComponent.leadsToInteriorCell = false;
					}

					// Store the door's exit position and orientation.
					if(refObjDataGroup.DODT != null)
					{
						doorComponent.doorExitPos = Convert.NifPointToUnityPoint(refObjDataGroup.DODT.position);
						doorComponent.doorExitOrientation = Convert.NifEulerAnglesToUnityQuaternion(refObjDataGroup.DODT.eulerAngles);

						// If the door leads to an exterior cell, store the name of the region containing the cell as the door's exit name.
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
			#endregion

			#region containers
			if ( refCellObjInfo.referencedRecord is CONTRecord )
			{
				gameObject.tag = "Container";
				var CONT = ( CONTRecord )refCellObjInfo.referencedRecord;

				ContainerComponent component = gameObject.AddComponent<ContainerComponent>();
				component.containerName = CONT.FNAM.value;

				/*
				Do More Containery Stuff
				*/
			}
			#endregion

			#region book items
			if ( refCellObjInfo.referencedRecord is BOOKRecord )
			{
				Collider coll = gameObject.GetComponentInChildren<Collider>();
				if ( coll != null ) coll.gameObject.tag = "Book";
				gameObject.tag = "Book";
				var BOOK = ( BOOKRecord )refCellObjInfo.referencedRecord;

				BookComponent component = gameObject.AddComponent<BookComponent>();
				component.bookTitle = BOOK.FNAM.value;

				/*
				Do More Containery Stuff
				*/
			}
			#endregion

			#region misc items
			if ( refCellObjInfo.referencedRecord is MISCRecord )
			{
				Collider coll = gameObject.GetComponentInChildren<Collider>();
				if ( coll != null )
					coll.gameObject.tag = "MiscObj";
				gameObject.tag = "MiscObj";
				var MISC = ( MISCRecord )refCellObjInfo.referencedRecord;

				MiscComponent component = gameObject.AddComponent<MiscComponent>();
				component.itemName = MISC.FNAM.value;

				/*
				Do More Containery Stuff
				*/
			}
			#endregion
		}

		private void UpdateExteriorCells(bool immediate = false, int cellRadiusOverride = -1)
		{
			var cameraCellIndices = GetExteriorCellIndices(playerCameraObj.transform.position);

			_currentCell = dataReader.FindExteriorCellRecord(cameraCellIndices);

			var cellRadius = (cellRadiusOverride >= 0) ? cellRadiusOverride : this.cellRadius;
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
			for(int r = 0; r <= cellRadius; r++)
			{
				for(int x = minCellX; x <= maxCellX; x++)
				{
					for(int y = minCellY; y <= maxCellY; y++)
					{
						var cellIndices = new Vector2i(x, y);

						var cellXDistance = Mathf.Abs(cameraCellIndices.x - cellIndices.x);
						var cellYDistance = Mathf.Abs(cameraCellIndices.y - cellIndices.y);
						var cellDistance = Mathf.Max(cellXDistance, cellYDistance);

						if((cellDistance == r) && !cellObjects.ContainsKey(cellIndices))
						{
							var cellInfo = CreateExteriorCell(cellIndices);

							if((cellInfo != null) && immediate)
							{
								temporalLoadBalancer.WaitForTask(cellInfo.creationCoroutine);
							}
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
					if ( !cellInfo.objectsContainerGameObject.activeSelf )
						cellInfo.objectsContainerGameObject.SetActive(true);
				}
				else
				{
					if ( cellInfo.objectsContainerGameObject.activeSelf )
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
				doorComponent.UseDoor();
			}
			else
			{
				// The door leads to another cell, so destroy all currently loaded cells.
				DestroyAllCells();

				// Move the player.
				playerObj.transform.position = doorComponent.doorExitPos;
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

					UpdateExteriorCells(true, cellRadiusOnLoad);

					OnExteriorCell(newCell);
				}

				_currentCell = newCell;
			}
		}

		private GameObject CreatePlayer(Vector3 position, out GameObject playerCamera)
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
			playerCamera = CreatePlayerCamera(position);
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
			Camera.main.renderingPath = RenderingPath.DeferredShading;
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

	internal class RefCellObjInfo
	{
		public CELLRecord.RefObjDataGroup refObjDataGroup;
		public Record referencedRecord;
		public string modelFilePath;
		public bool isDonePreLoading;
		public bool isInstantiated;
	}
}