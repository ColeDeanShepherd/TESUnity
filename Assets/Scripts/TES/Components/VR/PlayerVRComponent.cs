#if OSVR
using OSVR.Unity;
#endif
using System.Collections;
using TESUnity.UI;
using UnityEngine;
using UnityEngine.VR;

namespace TESUnity.Components.VR
{
    /// <summary>
    /// This component is responsible to enable the VR feature and deal with VR SDKs.
    /// VR SDKs allows us to provide more support (moving controller, teleportation, etc.)
    /// To enable a VR SDKs, please read the README.md file located in the Vendors folder.
    /// </summary>
    public class PlayerVRComponent : MonoBehaviour
    {
        public enum VRVendor
        {
            OSVR, UnityVR, None
        }

        private VRVendor _vrVendor = VRVendor.None;
        private Transform _camTransform = null;
        private Transform _canvas = null;
        private Transform _pivotCanvas = null;
        private Transform _hud = null;

        [SerializeField]
        private bool _isSpectator = false;
        [SerializeField]
        private Canvas _mainCanvas = null;

        public bool VREnabled
        {
            get { return _vrVendor != VRVendor.None; }
        }

        void Awake()
        {
            if (VRSettings.enabled)
                _vrVendor = VRVendor.UnityVR;
#if OSVR
            var clientKitGO = new GameObject("ClientKit");
            clientKitGO.SetActive(false);

            var clientKit = clientKitGO.AddComponent<ClientKit>();
            clientKit.AppID = "io.github.TESUnity";

#if UNITY_STANDALONE_WIN
            // Prevent OSVR Server to launch
            clientKit.autoStartServer = false;
#endif

            clientKitGO.SetActive(true);

            var osvrEnabled = clientKit != null && clientKit.context != null && clientKit.context.CheckStatus();

            if (osvrEnabled)
            {
                _vrVendor = VRVendor.OSVR;
                VRSettings.enabled = false;
            }
#endif
        }

        /// <summary>
        /// Intialize the VR support for the player.
        /// - The HUD and UI will use a WorldSpace Canvas
        /// - The HUD canvas is not recommanded, it's usefull for small informations
        /// - The UI is for all other UIs: Menu, Life, etc.
        /// </summary>
        void Start()
        {
            if (_vrVendor != VRVendor.None)
            {
                if (_mainCanvas == null)
                    _mainCanvas = FindObjectOfType<Canvas>();

                _canvas = _mainCanvas.GetComponent<Transform>();
                _pivotCanvas = _canvas.parent;

                // Put the Canvas in WorldSpace and Attach it to the camera.
                _camTransform = Camera.main.GetComponent<Transform>();

                // Add a pivot to the UI. It'll help to rotate it in the inverse direction of the camera.
                var uiPivot = new GameObject("UI Pivot");
                _pivotCanvas = uiPivot.GetComponent<Transform>();
                _pivotCanvas.parent = transform;
                _pivotCanvas.localPosition = Vector3.zero;
                _pivotCanvas.localRotation = Quaternion.identity;
                _pivotCanvas.localScale = Vector3.one;
                GUIUtils.SetCanvasToWorldSpace(_canvas.GetComponent<Canvas>(), _pivotCanvas, 1.0f, 0.002f);

                // Add the HUD
                if (!_isSpectator)
                {
                    var hud = GUIUtils.CreateCanvas(false);
                    hud.name = "HUD";
                    _hud = hud.GetComponent<Transform>();
                    GUIUtils.SetCanvasToWorldSpace(hud.GetComponent<Canvas>(), _camTransform, 1.0f, 0.002f);

                    var hudWidgets = _canvas.GetComponentsInChildren<IHUDWidget>(true);
                    for (int i = 0; i < hudWidgets.Length; i++)
                        hudWidgets[i].SetParent(_hud);
                }
                else
                {
                    ShowUICursor(true);
                }

                // Setup the camera
                Camera.main.nearClipPlane = 0.1f;

#if OSVR
                if (_vrVendor == VRVendor.OSVR)
                {
                    // OSVR: Open Source VR Implementation
                    var displayController = _camTransform.parent.gameObject.AddComponent<DisplayController>();
                    displayController.showDirectModePreview = TESUnity.instance.directModePreview;
                    Camera.main.gameObject.AddComponent<VRViewer>();
                }
#endif

#if OCULUS
                // Enable Oculus Features...
#endif

#if STEAMVR
                // Enable SteamVR Features...
#endif
                ResetOrientationAndPosition();
            }
        }

        void Update()
        {
            // At any time, the user might want to reset the orientation and position.
            if (Input.GetButtonDown("Recenter"))
                ResetOrientationAndPosition();
        }

        public void ShowUICursor(bool visible)
        {
            // TODO: Add hand selector for the Touchs and the Vive.
            var uiCursor = GetComponentInChildren<VRGazeUI>(true);
            uiCursor.SetActive(visible);
        }

        /// <summary>
        /// Recenter the Main UI.
        /// </summary>
        private void RecenterUI()
        {
            if (_vrVendor != VRVendor.None)
            {
                var pivotRot = _pivotCanvas.localRotation;
                pivotRot.y = _camTransform.localRotation.y;
                _pivotCanvas.localRotation = pivotRot;

                var camPosition = _camTransform.position;
                var targetPosition = _pivotCanvas.position;
                targetPosition.y = camPosition.y;
                _pivotCanvas.position = targetPosition;
            }
        }

        private IEnumerator ResetOrientationAndPosition(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_vrVendor == VRVendor.UnityVR)
                InputTracking.Recenter();

#if OSVR
            if (_vrVendor == VRVendor.OSVR)
            {
                var clientKit = ClientKit.instance;
                var displayController = FindObjectOfType<DisplayController>();

                if (clientKit != null && displayController != null)
                {
                    if (displayController.UseRenderManager)
                        displayController.RenderManager.SetRoomRotationUsingHead();
                    else
                        clientKit.context.SetRoomRotationUsingHead();
                }
            }
#endif
        }

        /// <summary>
        /// Reset the orientation and the position of the HMD with a delay of 0.1ms.
        /// </summary>
        public void ResetOrientationAndPosition()
        {
            StartCoroutine(ResetOrientationAndPosition(0.1f));
        }

        /// <summary>
        /// Sent by the PlayerComponent when the pause method is called.
        /// </summary>
        /// <param name="paused">Boolean: Indicates if the player is paused.</param>
        void OnPlayerPause(bool paused)
        {
            if (paused)
                RecenterUI();
        }
    }
}
