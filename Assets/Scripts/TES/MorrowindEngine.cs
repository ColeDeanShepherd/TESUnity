using System.Collections;
using System.Collections.Generic;
using TESUnity.Components;
using TESUnity.Components.Records;
using TESUnity.Effects;
using TESUnity.ESM;
using TESUnity.UI;
using UnityEngine;

namespace TESUnity
{
    public class MorrowindEngine
    {
        public static MorrowindEngine instance;

        public const float maxInteractDistance = 3;

        #region Private Fields

        private const float playerHeight = 2;
        private const float playerRadius = 0.4f;
        private const float desiredWorkTimePerFrame = 1.0f / 160;
        private const int cellRadiusOnLoad = 2;
        private CELLRecord _currentCell;
        private UIManager _uiManager;
        private GameObject sunObj;
        private GameObject waterObj;
        private Transform playerTransform;
        private PlayerComponent playerComponent;
        private PlayerInventory playerInventory;
        private GameObject playerCameraObj;
        private UnderwaterEffect underwaterEffect;
        private Color32 defaultAmbientColor = new Color32(137, 140, 160, 255);
        private RaycastHit[] interactRaycastHitBuffer = new RaycastHit[32];

        #endregion

        #region Public Fields

        public MorrowindDataReader dataReader;
        public TextureManager textureManager;
        public MaterialManager materialManager;
        public NIFManager nifManager;
        public CellManager cellManager;
        public CELLRecord currentCell
        {
            get { return _currentCell; }
        }

        public static int markerLayer
        {
            get { return LayerMask.NameToLayer("Marker"); }
        }

        #endregion

        public MorrowindEngine(MorrowindDataReader mwDataReader, UIManager uiManager)
        {
            Debug.Assert(instance == null);

            instance = this;
            dataReader = mwDataReader;
            textureManager = new TextureManager(dataReader);
            materialManager = new MaterialManager(textureManager);
            nifManager = new NIFManager(dataReader, materialManager);
            cellManager = new CellManager(dataReader, textureManager, nifManager);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientIntensity = TESUnity.instance.ambientIntensity;

            sunObj = GameObjectUtils.CreateDirectionalLight(Vector3.zero, Quaternion.Euler(new Vector3(50, 330, 0)));
            sunObj.GetComponent<Light>().shadows = TESUnity.instance.renderSunShadows ? LightShadows.Soft : LightShadows.None;
            sunObj.SetActive(false);

            waterObj = GameObject.Instantiate(TESUnity.instance.waterPrefab);
            waterObj.SetActive(false);

            if (!TESUnity.instance.waterBackSideTransparent)
            {
                var side = waterObj.transform.GetChild(0);
                var sideMaterial = side.GetComponent<Renderer>().sharedMaterial;
                sideMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                sideMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                sideMaterial.SetInt("_ZWrite", 1);
                sideMaterial.DisableKeyword("_ALPHATEST_ON");
                sideMaterial.DisableKeyword("_ALPHABLEND_ON");
                sideMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                sideMaterial.renderQueue = -1;
            }

            Cursor.SetCursor(textureManager.LoadTexture("tx_cursor", true), Vector2.zero, CursorMode.Auto);

            _uiManager = uiManager;
            _uiManager.Active = true;
        }

        #region Player Spawn

        /// <summary>
        /// Spawns the player inside. Be carefull, the name of the cell is not the same for each languages.
        /// Use it with the correct name.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="interiorCellName">The name of the desired cell.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayerInside(GameObject playerPrefab, string interiorCellName, Vector3 position)
        {
            _currentCell = dataReader.FindInteriorCellRecord(interiorCellName);

            Debug.Assert(_currentCell != null);

            CreatePlayer(playerPrefab, position, out playerCameraObj);
            var cellInfo = cellManager.CreateInteriorCell(interiorCellName);

            OnInteriorCell(_currentCell);
        }

        /// <summary>
        /// Spawns the player inside using the cell's grid coordinates.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="gridCoords">The grid coordinates.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayerInside(GameObject playerPrefab, Vector2i gridCoords, Vector3 position)
        {
            _currentCell = dataReader.FindInteriorCellRecord(gridCoords);

            Debug.Assert(_currentCell != null);

            CreatePlayer(playerPrefab, position, out playerCameraObj);
            var cellInfo = cellManager.CreateInteriorCell(gridCoords);

            OnInteriorCell(_currentCell);
        }

        /// <summary>
        /// Spawns the player outside using the cell's grid coordinates.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="gridCoords">The grid coordinates.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayerOutside(GameObject playerPrefab, Vector2i gridCoords, Vector3 position)
        {
            _currentCell = dataReader.FindExteriorCellRecord(gridCoords);

            Debug.Assert(_currentCell != null);

            CreatePlayer(playerPrefab, position, out playerCameraObj);
            var cellInfo = cellManager.CreateExteriorCell(gridCoords);

            OnExteriorCell(_currentCell);
        }

        /// <summary>
        /// Spawns the player outside using the position of the player.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayerOutside(GameObject playerPrefab, Vector3 position)
        {
            var cellIndices = cellManager.GetExteriorCellIndices(position);
            _currentCell = dataReader.FindExteriorCellRecord(cellIndices);

            CreatePlayer(playerPrefab, position, out playerCameraObj);
            cellManager.UpdateExteriorCells(playerCameraObj.transform.position, true, cellRadiusOnLoad);
            OnExteriorCell(_currentCell);
        }

        #endregion

        public void Update()
        {
            // The current cell can be null if the player is outside of the defined game world.
            if ((_currentCell == null) || !_currentCell.isInterior)
            {
                cellManager.UpdateExteriorCells(playerCameraObj.transform.position);
            }

            CastInteractRay();
        }

        public void CastInteractRay()
        {
            // Cast a ray to see what the camera is looking at.
            var ray = new Ray(playerCameraObj.transform.position, playerCameraObj.transform.forward);
            var raycastHitCount = Physics.RaycastNonAlloc(ray, interactRaycastHitBuffer, maxInteractDistance);

            if (raycastHitCount > 0 && !playerComponent.Paused)
            {
                for (int i = 0; i < raycastHitCount; i++)
                {
                    var hitInfo = interactRaycastHitBuffer[i];
                    var component = hitInfo.collider.GetComponentInParent<GenericObjectComponent>();

                    if (component != null)
                    {
                        if (string.IsNullOrEmpty(component.objData.name))
                            return;

                        ShowInteractiveText(component);

                        if (Input.GetButtonDown("Use"))
                        {
                            if (component is DoorComponent)
                                OpenDoor((DoorComponent)component);

                            else if (component.usable)
                                component.Interact();

                            else if (component.pickable)
                                playerInventory.Add(component);
                        }
                        break;
                    }
                    else
                        CloseInteractiveText(); //deactivate text if no interactable [ DOORS ONLY - REQUIRES EXPANSION ] is found
                }
            }
            else
                CloseInteractiveText(); //deactivate text if nothing is raycasted against
        }

        public void ShowInteractiveText(GenericObjectComponent component)
        {
            var data = component.objData;
            _uiManager.InteractiveText.Show(GUIUtils.CreateSprite(data.icon), data.interactionPrefix, data.name, data.value, data.weight);
        }

        public void CloseInteractiveText()
        {
            _uiManager.InteractiveText.Close();
        }

        #region Private

        private void OnExteriorCell(CELLRecord CELL)
        {
            RenderSettings.ambientLight = defaultAmbientColor;

            sunObj.SetActive(true);

            waterObj.transform.position = Vector3.zero;
            waterObj.SetActive(true);
            underwaterEffect.enabled = true;
            underwaterEffect.Level = 0.0f;
        }

        private void OnInteriorCell(CELLRecord CELL)
        {
            if (CELL.AMBI != null)
                RenderSettings.ambientLight = ColorUtils.B8G8R8ToColor32(CELL.AMBI.ambientColor);

            sunObj.SetActive(false);

            underwaterEffect.enabled = CELL.WHGT != null;

            if (CELL.WHGT != null)
            {
                var offset = 1.6f; // Interiors cells needs this offset to render at the correct location.
                waterObj.transform.position = new Vector3(0, (CELL.WHGT.value / Convert.meterInMWUnits) - offset, 0);
                waterObj.SetActive(true);
                underwaterEffect.Level = waterObj.transform.position.y;
            }
            else
            {
                waterObj.SetActive(false);
            }
        }

        private void OpenDoor(DoorComponent component)
        {
            if (!component.doorData.leadsToAnotherCell)
            {
                component.Interact();
            }
            else
            {
                // The door leads to another cell, so destroy all currently loaded cells.
                cellManager.DestroyAllCells();

                // Move the player.
                playerTransform.position = component.doorData.doorExitPos;
                playerTransform.localEulerAngles = new Vector3(0, component.doorData.doorExitOrientation.eulerAngles.y, 0);

                // Load the new cell.
                CELLRecord newCell;

                if (component.doorData.leadsToInteriorCell)
                {
                    var cellInfo = cellManager.CreateInteriorCell(component.doorData.doorExitName);
                    newCell = cellInfo.cellRecord;

                    OnInteriorCell(cellInfo.cellRecord);
                }
                else
                {
                    var cellIndices = cellManager.GetExteriorCellIndices(component.doorData.doorExitPos);
                    newCell = dataReader.FindExteriorCellRecord(cellIndices);

                    cellManager.UpdateExteriorCells(playerCameraObj.transform.position, true, cellRadiusOnLoad);

                    OnExteriorCell(newCell);
                }

                _currentCell = newCell;
            }
        }

        private GameObject CreatePlayer(GameObject playerPrefab, Vector3 position, out GameObject playerCamera)
        {
            var player = (GameObject)GameObject.Instantiate(playerPrefab);
            player.name = "Player";
            player.transform.position = position;
            
            playerTransform = player.GetComponent<Transform>();
            playerCamera = player.GetComponentInChildren<Camera>().gameObject;
            playerComponent = player.GetComponent<PlayerComponent>();
            playerInventory = player.GetComponent<PlayerInventory>();
            underwaterEffect = playerCamera.GetComponent<UnderwaterEffect>();

            return player;
        }

        #endregion
    }
}