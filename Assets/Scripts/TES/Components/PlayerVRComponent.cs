#if OSVR
using OSVR.Unity;
#endif
using System.Collections;
using TESUnity.UI;
using UnityEngine;
using UnityEngine.VR;

namespace TESUnity.Components
{
    /// <summary>
    /// This component is responsible to enable the VR feature and deal with VR SDKs.
    /// VR SDKs allows us to provide more support (moving controller, teleportation, etc.)
    /// To enable a VR SDKs, please read the README.md file located in the Vendors folder.
    /// </summary>
    [RequireComponent(typeof(PlayerComponent))]
    public class PlayerVRComponent : MonoBehaviour
    {
        private bool _vrEnabled = false;
        private Transform _camTransform = null;
        private Transform _canvas = null;
        private Transform _pivotCanvas = null;
        private Transform _hud = null;

        void Awake()
        {
            _vrEnabled = VRSettings.enabled;
#if OSVR
            var clientKitGO = new GameObject("ClientKit");
            clientKitGO.SetActive(false);

            var clientKit = clientKitGO.AddComponent<ClientKit>();
            clientKit.AppID = "io.github.TESUnity";
            clientKitGO.SetActive(true);

            _vrEnabled = clientKit != null && clientKit.context != null && clientKit.context.CheckStatus();

            VRSettings.enabled = false;
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
            if (_vrEnabled)
            {
                var canvas = FindObjectOfType<Canvas>();
                _canvas = canvas.GetComponent<Transform>();
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
                var hud = GUIUtils.CreateCanvas(false);
                hud.name = "HUD";
                _hud = hud.GetComponent<Transform>();
                GUIUtils.SetCanvasToWorldSpace(hud.GetComponent<Canvas>(), _camTransform, 1.0f, 0.002f);

                var hudWidgets = _canvas.GetComponentsInChildren<IHUDWidget>(true);
                for (int i = 0; i < hudWidgets.Length; i++)
                    hudWidgets[i].SetParent(_hud);

                // Setup the camera
                Camera.main.nearClipPlane = 0.1f;

#if OSVR
                // OSVR: Open Source VR Implementation
                var displayController = _camTransform.parent.gameObject.AddComponent<DisplayController>();
                displayController.showDirectModePreview = TESUnity.instance.directModePreview;
                Camera.main.gameObject.AddComponent<VRViewer>();
#else
                VRSettings.showDeviceView = TESUnity.instance.directModePreview;
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

        /// <summary>
        /// Recenter the Main UI.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RecenterUI()
        {
            yield return new WaitForSeconds(0.1f);

            if (_vrEnabled)
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

            if (_vrEnabled)
            {
#if OSVR
                var clientKit = ClientKit.instance;
                var displayController = FindObjectOfType<DisplayController>();

                if (clientKit != null && displayController != null)
                {
                    if (displayController.UseRenderManager)
                        displayController.RenderManager.SetRoomRotationUsingHead();
                    else
                        clientKit.context.SetRoomRotationUsingHead();
                }
#else
                InputTracking.Recenter();
#endif
            }
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
        void OnPlayerPause(object paused)
        {
            if (((bool)paused))
                StartCoroutine(RecenterUI());
        }
    }
}
