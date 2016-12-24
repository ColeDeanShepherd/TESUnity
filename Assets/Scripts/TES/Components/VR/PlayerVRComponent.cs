#if OSVR
using OSVR.Unity;
#endif
using System.Collections;
using TESUnity.UI;
using UnityEngine;
using UnityEngine.VR;

#if UNITY_5_5 && (UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX) && !UNITY_EDITOR
// A quick polyfill for UnityEngine.VR for Linux and Mac because it's disabled in Unity 5.5 beta.
namespace UnityEngine.VR
{
    public enum VRNode
    {
        LeftEye = 0,
        RightEye = 1,
        CenterEye = 2,
        Head = 3
    }

    public static class VRSettings
    {
        public static bool enabled { get; set; }
        public static readonly string loadedDeviceName = "None";
        public static readonly int eyeTextureHeight = Screen.height;
        public static readonly int eyeTextureWidth = Screen.width;
        public static readonly float renderScale = 1.0f;
    }

    public static class InputTracking
    {
        public static Vector3 GetLocalPosition(VRNode node)
        {
            return Vector3.zero;
        }

        public static Quaternion GetLocalRotation(VRNode node)
        {
            return Quaternion.identity;
        }

        public static void Recenter() { }
    }
}
#endif

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
        private RectTransform _canvas = null;
        private Transform _pivotCanvas = null;
        private RectTransform _hud = null;

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
            UnityEngine.VR.InputTracking.GetLocalPosition(VRNode.CenterEye);
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
                var uiManager = FindObjectOfType<UIManager>();

                if (_mainCanvas == null)
                    _mainCanvas = uiManager.GetComponent<Canvas>();

                if (_mainCanvas == null)
                    throw new UnityException("The Main Canvas Is Null");

                uiManager.Crosshair.Enabled = false;

                _canvas = _mainCanvas.GetComponent<RectTransform>();
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

                // Add the HUD, its fixed to the camera.
                if (_isSpectator)
                    ShowUICursor(true);

                // Setup the camera
                Camera.main.nearClipPlane = 0.1f;

#if OSVR
                SetupOSVR();
#endif
#if OCULUS
                SetupOculus();
#endif
#if OPENVR
                SetupOpenVR();
#endif

                ResetOrientationAndPosition();
            }
        }

        void Update()
        {
            // At any time, the user might want to reset the orientation and position.
            if (Input.GetButtonDown("Recenter"))
                ResetOrientationAndPosition();

            RecenterUI(true);
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
        private void RecenterUI(bool onlyPosition = false)
        {
            if (_vrVendor != VRVendor.None)
            {
                if (!onlyPosition)
                {
                    var pivotRot = _pivotCanvas.localRotation;
                    pivotRot.y = _camTransform.localRotation.y;
                    _pivotCanvas.localRotation = pivotRot;
                }

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

            RecenterUI();
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

        #region SDK Integration

#if OSVR
        private void SetupOSVR()
        {

            if (_vrVendor == VRVendor.OSVR)
            {
                // OSVR: Open Source VR Implementation
                var displayController = _camTransform.parent.gameObject.AddComponent<DisplayController>();
                displayController.showDirectModePreview = TESUnity.instance.directModePreview;
                Camera.main.gameObject.AddComponent<VRViewer>();
            }
        }
#endif

#if OCULUS
        private void SetupOculus()
        {
            if (_vrVendor == VRVendor.UnityVR && VRSettings.loadedDeviceName == "Oculus")
            {
                var player = GetComponent<PlayerComponent>();

                _camTransform.name = "CenterEyeAnchor";

                var trackingSpace = _camTransform.parent;
                trackingSpace.name = "TrackingSpace";

                var head = trackingSpace.parent.gameObject;
                head.name = "OVRCameraRig";
                head.AddComponent<OVRCameraRig>();
                head.AddComponent<OVRManager>();

                CreateNode(head.transform.parent, "ForwardDirection");

                var anchorNames = new string[]
                {
                    "LeftEyeAnchor",
                    "RightEyeAnchor",
                    "TrackerAnchor",
                    "LeftHandAnchor",
                    "RightHandAnchor"
                };

                // If Oculus Touch are connected, using them as hand
                if (OVRInput.GetActiveController() == OVRInput.Controller.Touch)
                {
                    Transform nodeTransform = null;
                    OculusHand oculusHand = null;

                    for (int i = 0, l = anchorNames.Length; i < l; i++)
                    {
                        nodeTransform = CreateNode(trackingSpace, anchorNames[i]);

                        if (i == 0)
                        {
                            Destroy(player.leftHand);
                            player.leftHand = nodeTransform;
                            oculusHand = nodeTransform.gameObject.AddComponent<OculusHand>();
                            oculusHand.TouchID = OVRInput.Controller.LTouch;
                        }
                        else if (i == 1)
                        {
                            Destroy(player.rightHand);
                            player.rightHand = nodeTransform;
                            oculusHand = nodeTransform.gameObject.AddComponent<OculusHand>();
                            oculusHand.TouchID = OVRInput.Controller.RTouch;
                        }
                    }
                }
            }
        }

        private Transform CreateNode(Transform parent, string name)
        {
            if (!parent.Find(name))
            {
                var node = new GameObject(name);
                var nodeTransform = node.GetComponent<Transform>();
                nodeTransform.parent = parent;
                nodeTransform.localPosition = Vector3.zero;
                nodeTransform.localRotation = Quaternion.identity;
                return nodeTransform;
            }

            return null;
        }
#endif

#if OPENVR
        private void SetupOpenVR()
        {
            if (_vrVendor == VRVendor.UnityVR && VRSettings.loadedDeviceName == "OpenVR")
            {
                var player = GetComponent<PlayerComponent>();
                var mTransform = GetComponent<Transform>();
                var trackingSpace = _camTransform.parent;
                var head = trackingSpace != null ? trackingSpace.parent : trackingSpace;

                // Creates the SteamVR's main camera.
                var steamCamera = _camTransform.gameObject.AddComponent<SteamVR_Camera>();

                // The controllers.
                var controllerGameObject = new GameObject("SteamVR_Controllers");
                var controllerTransform = controllerGameObject.transform;
                controllerTransform.parent = head;
                controllerTransform.localPosition = Vector3.zero;
                controllerTransform.localRotation = Quaternion.identity;

                // We need to disable the gameobject because the SteamVR_ControllerManager component
                // Will check attached controllers in the awake method.
                // Here we don't have attached controllers yet.
                controllerGameObject.SetActive(false);

                var controllerManager = controllerGameObject.AddComponent<SteamVR_ControllerManager>();
                controllerManager.left = CreateController(controllerManager.transform, "Controller (left)");
                controllerManager.right = CreateController(controllerManager.transform, "Controller (right)");

                // Now that controllers are attached, we can enable the GameObject
                controllerGameObject.SetActive(true);

                // Attach the controllers to the player.
                SetupVRHand(player, true, controllerManager.left.GetComponent<Transform>());
                SetupVRHand(player, false, controllerManager.right.GetComponent<Transform>());

                // And finally the play area.
                var playArea = new GameObject("SteamVR_PlayArea");
                playArea.transform.parent = mTransform;
                playArea.AddComponent<MeshRenderer>();
                playArea.AddComponent<MeshFilter>();
                playArea.AddComponent<SteamVR_PlayArea>();
            }
        }

        private GameObject CreateController(Transform parent, string name)
        {
            if (!parent.Find(name))
            {
                var node = new GameObject(name);
                node.transform.parent = parent;

                var trackedObject = node.AddComponent<SteamVR_TrackedObject>();
                trackedObject.index = SteamVR_TrackedObject.EIndex.None;

                var model = new GameObject("Model");
                model.transform.parent = node.transform;

                var renderModel = model.AddComponent<SteamVR_RenderModel>();
                renderModel.index = SteamVR_TrackedObject.EIndex.None;

                return node;
            }

            return null;
        }
#endif

        private void SetupVRHand(PlayerComponent player, bool leftHand, Transform target)
        {
            target.position = leftHand ? player.leftHand.position : player.rightHand.position;
            target.rotation = leftHand ? player.leftHand.rotation : player.rightHand.rotation;

            if (leftHand)
            {
                Destroy(player.leftHand);
                player.leftHand = target;
            }
            else
            {
                Destroy(player.rightHand);
                player.rightHand = target;
            }
        }

        #endregion
    }
}
