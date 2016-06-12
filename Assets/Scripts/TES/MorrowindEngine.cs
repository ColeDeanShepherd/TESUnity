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

		public MorrowindEngine( MorrowindDataReader dataReader )
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
			RenderSettings.ambientIntensity = TESUnity.instance.AmbientIntensity;

			sunObj = GameObjectUtils.CreateDirectionalLight(Vector3.zero, Quaternion.Euler(new Vector3(50, 330, 0)));
			sunObj.GetComponent<Light>().shadows = TESUnity.instance.EnableSunShadows ? LightShadows.Hard : LightShadows.None;
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
			var ray = new Ray(playerCameraObj.transform.position, playerCameraObj.transform.forward);

			int raycastHitCount = Physics.RaycastNonAlloc(ray , interactRaycastHitBuffer , maxInteractDistance );

			if ( raycastHitCount > 0 )
			{
				for ( int i = 0 ; i < raycastHitCount ; i++ )
				{
					var hitInfo = interactRaycastHitBuffer[ i ];
					var hitObj = hitInfo.collider.gameObject;

					var component = hitObj.gameObject.GetComponentInParent<GenericObjectComponent>();
					if ( component != null )
					{
						if ( !string.IsNullOrEmpty( component.objData.name ) )
						{
							switch ( component.gameObject.tag )
							{
								case "Door": SetInteractText(component.objData.name); if ( Input.GetKeyDown(KeyCode.E) ) OpenDoor(component); break;
								case "Container": SetInteractText("Open " + component.objData.name); break;
								case "Activator": SetInteractText("" + component.objData.name); break;
								case "Lock": SetInteractText("Locked: " + component.objData.name); break;
								case "Light":
								case "Probe":
								case "RepairTool":
								case "Clothing":
								case "Armor":
								case "Weapon":
								case "Ingredient":
								case "Alchemical":
								case "Apparatus":
								case "MiscObj": SetInteractText("Take " + component.objData.name); TryRemoveObject(component.gameObject); break;
								case "Book": SetInteractText("" + component.objData.name); TryRemoveObject(component.gameObject); if ( Input.GetKeyDown(KeyCode.F) ) component.Interact(); break;
							}
							break;
						}
					}
					else
					{
						RemoveInteractText(); //deactivate text if no interactable [ DOORS ONLY - REQUIRES EXPANSION ] is found
					}
				}
			}
			else
			{
				RemoveInteractText(); //deactivate text if nothing is raycasted against
			}
		}

		private void TryRemoveObject ( GameObject obj ) // temp utility function representing character adding items to inventory
		{
			if ( Input.GetKeyDown( KeyCode.E ) )
			{
				var p = obj.transform;
				while ( p.parent != null && p.parent.gameObject.name != "objects" ) p = p.parent; //kind of a hacky way to reference the entirety of an individual object
				UnityEngine.Object.Destroy( p.gameObject );
			}
		}

		public void ShowInteractText()
		{
			if ( !interactTextObj.activeSelf ) interactTextObj.SetActive( true );
		}

		public void RemoveInteractText ()
		{
			if ( interactTextObj.activeSelf ) interactTextObj.SetActive( false );
		}

		public void SetInteractText ( string text )
		{
			if ( text == "" ) return; // remove this if we change to clearing text rather than activating/deactivating the interactText text object
			if ( interactText.text != text ) interactText.text = text;
			ShowInteractText();
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

		/// <summary>
		/// A coroutine that instantiates the terrain for, and all objects in, a cell.
		/// </summary>
		private IEnumerator InstantiateCellObjectsCoroutine(CELLRecord CELL, LANDRecord LAND, GameObject cellObj, GameObject cellObjectsContainer)
		{
			// Return before doing any work to provide an IEnumerator handle to the coroutine.
			yield return null;

			// Instantiate terrain.
			if(LAND != null)
			{
				var instantiateLANDTaskEnumerator = InstantiateLANDCoroutine(LAND, cellObj);
				
				// Run the LAND instantiation coroutine.
				while(instantiateLANDTaskEnumerator.MoveNext())
				{
					// Yield every time InstantiateLANDCoroutine does to avoid doing too much work in one frame.
					yield return null;
				}

				// Yield after InstantiateLANDCoroutine has finished to avoid doing too much work in one frame.
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

		/// <summary>
		/// Instantiates an object in a cell. Called by InstantiateCellObjectsCoroutine after the object's assets have been pre-loaded.
		/// </summary>
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
							//attachLightObj = GameObjectUtils.FindChildWithNameSubstringRecursively(modelObj, "Emitter");
							attachLightObj = modelObj;
						}

						if(attachLightObj != null)
						{
							lightObj.transform.position = attachLightObj.transform.position;
							lightObj.transform.rotation = attachLightObj.transform.rotation;

							lightObj.transform.parent = attachLightObj.transform;
						}
						else // If there is no "AttachLight", center the light in the model's bounds.
						{
							lightObj.transform.position = GameObjectUtils.CalcVisualBoundsRecursive(modelObj).center;
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
			lightComponent.bounceIntensity = 0f;
			lightComponent.shadows = ( TESUnity.instance.EnableLightShadows ) ? LightShadows.Soft : LightShadows.None;


			if ( !indoors && !TESUnity.instance.EnableExteriorLights )//disabling exterior cell lights because there is no day/night cycle
			{
				lightComponent.enabled = false;
			}

			return lightObj;
		}

		/// <summary>
		/// Finishes initializing an instantiated cell object.
		/// </summary>
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

			var tagTarget = gameObject;
			var coll = gameObject.GetComponentInChildren<Collider>(); // if the collider is on a child object and not on the object with the component, we need to set that object's tag instead.
			if ( coll != null ) tagTarget = coll.gameObject;

			ProcessObjectType<DOORRecord>( tagTarget , refCellObjInfo , "Door");
			ProcessObjectType<ACTIRecord>( tagTarget , refCellObjInfo , "Activator");
			ProcessObjectType<CONTRecord>( tagTarget , refCellObjInfo , "Container");
			ProcessObjectType<LIGHRecord>( tagTarget , refCellObjInfo , "Light");
			ProcessObjectType<LOCKRecord>( tagTarget , refCellObjInfo , "Lock");
			ProcessObjectType<PROBRecord>( tagTarget , refCellObjInfo , "Probe");
			ProcessObjectType<REPARecord>( tagTarget , refCellObjInfo , "RepairTool");
			ProcessObjectType<WEAPRecord>( tagTarget , refCellObjInfo , "Weapon");
			ProcessObjectType<CLOTRecord>( tagTarget , refCellObjInfo , "Clothing");
			ProcessObjectType<ARMORecord>( tagTarget , refCellObjInfo , "Armor");
			ProcessObjectType<INGRRecord>( tagTarget , refCellObjInfo , "Ingredient");
			ProcessObjectType<ALCHRecord>( tagTarget , refCellObjInfo , "Alchemical");
			ProcessObjectType<APPARecord>( tagTarget , refCellObjInfo , "Apparatus");
			ProcessObjectType<BOOKRecord>( tagTarget , refCellObjInfo , "Book");
			ProcessObjectType<MISCRecord>( tagTarget , refCellObjInfo , "MiscObj");

		}

		/// <summary>
		/// Creates terrain representing a LAND record.
		/// </summary>
		private IEnumerator InstantiateLANDCoroutine(LANDRecord LAND, GameObject parent)
		{
			Debug.Assert(LAND != null);

			// Don't create anything if the LAND doesn't have height data.
			if(LAND.VHGT == null)
			{
				// End execution of the coroutine.
				yield break;
			}

			// Return before doing any work to provide an IEnumerator handle to the coroutine.
			yield return null;

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

						// Yield after loading each texture to avoid doing too much work on one frame.
						yield return null;

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

			// Yield before creating the terrain GameObject because it takes a while.
			yield return null;

			// Create the terrain.
			var heightRange = maxHeight - minHeight;
			var terrainPosition = new Vector3(Convert.exteriorCellSideLengthInMeters * LAND.gridCoords.x, minHeight / Convert.meterInMWUnits, Convert.exteriorCellSideLengthInMeters * LAND.gridCoords.y);

			var heightSampleDistance = Convert.exteriorCellSideLengthInMeters / (LAND_SIDE_LENGTH_IN_SAMPLES - 1);

			var terrain = GameObjectUtils.CreateTerrain(heights, heightRange / Convert.meterInMWUnits, heightSampleDistance, splatPrototypes, alphaMap, terrainPosition);
			terrain.GetComponent<Terrain>().materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;

			terrain.transform.parent = parent.transform;
		}

		public static GameObject FindTopLevelObject(GameObject baseObject)
		{
			if(baseObject.transform.parent == null) return baseObject;
			var p = baseObject.transform;
			while(p.parent != null)
			{
				if(p.parent.gameObject.name == "objects")
					break;
				p = p.parent;
			}
			return p.gameObject;
		}

		private void ProcessObjectType <RecordType> ( GameObject gameObject , RefCellObjInfo info , string tag ) where RecordType : Record
		{
			var record = info.referencedRecord;
			if ( record is RecordType )
			{
				var obj = FindTopLevelObject(gameObject);
				if ( obj == null ) return;
				var component = obj.AddComponent< GenericObjectComponent>();

				if ( record is DOORRecord ) component.refObjDataGroup = info.refObjDataGroup; //only door records need access to the cell object data group so far
				component.init(( RecordType )record , tag);
			}
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

		private void OpenDoor(GenericObjectComponent component)
		{
			if(!component.doorData.leadsToAnotherCell)
			{
				component.Interact();
			}
			else
			{
				// The door leads to another cell, so destroy all currently loaded cells.
				DestroyAllCells();

				// Move the player.
				playerObj.transform.position = component.doorData.doorExitPos;
				playerObj.transform.localEulerAngles = new Vector3(0, component.doorData.doorExitOrientation.eulerAngles.y, 0);

				// Load the new cell.
				CELLRecord newCell;

				if( component.doorData.leadsToInteriorCell )
				{
					newCell = dataReader.FindInteriorCellRecord(component.doorData.doorExitName);

					var cellInfo = InstantiateCell(newCell);
					temporalLoadBalancer.WaitForTask(cellInfo.creationCoroutine);

					cellObjects[Vector2i.zero] = cellInfo;

					OnInteriorCell(newCell);
				}
				else
				{
					var cellIndices = GetExteriorCellIndices(component.doorData.doorExitPos);
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
			lightComponent.range = 20f;
			lightComponent.intensity = 1.5f;
			lightComponent.color = new Color32(245, 140, 40, 255);
			lightComponent.enabled = false;
			lightComponent.shadows = TESUnity.instance.EnableLightShadows ? LightShadows.Hard : LightShadows.None;

			lantern.transform.localPosition = cameraPoint.transform.localPosition - Vector3.up * 0.5f;
			lantern.transform.SetParent(playerComponent.transform, false);

			playerComponent.camera = playerCamera;
			playerComponent.lantern = lantern;

			return player;
		}
		private GameObject CreatePlayerCamera(Vector3 position)
		{
			var camera = GameObjectUtils.CreateMainCamera(position, Quaternion.identity);
			camera.GetComponent<Camera>().cullingMask = ~(1 << markerLayer);
			camera.GetComponent<Camera>().renderingPath = TESUnity.instance.RenderPath;
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