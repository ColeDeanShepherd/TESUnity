using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TESUnity.Components.VR
{
    /// <summary>
    /// Display a crosshair in a world space canvas and use it to interact with the UI.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public sealed class VRGazeUI : MonoBehaviour
    {
        public delegate bool IsActionPressedDelegate();

        private List<IsActionPressedDelegate> _inputCallbacks = new List<IsActionPressedDelegate>(1);
        private List<RaycastResult> _raycasts = null;
        private PointerEventData _pointer = null;
        private EventSystem _eventSystem = null;
        private RectTransform _selected = null;
        private Transform _transform = null;

        void Start()
        {
            if (EventSystem.current == null)
                throw new UnityException("[VRGazeUI] EventSystem is null.");

            var scaling = UnityEngine.XR.XRSettings.eyeTextureResolutionScale;
            var screenCenter = new Vector2(Screen.width * 0.5f * scaling, Screen.height * 0.5f * scaling);

            if (UnityEngine.XR.XRSettings.enabled)
            {
                screenCenter.x = UnityEngine.XR.XRSettings.eyeTextureWidth * 0.5f * scaling;
                screenCenter.y = UnityEngine.XR.XRSettings.eyeTextureHeight * 0.5f * scaling;
            }

            _eventSystem = EventSystem.current;
            _pointer = new PointerEventData(_eventSystem);
            _transform = GetComponent<Transform>();
            _transform.localScale = Vector3.one;
            _raycasts = new List<RaycastResult>();
            _pointer.position = screenCenter;
        }

        void Update()
        {
            _eventSystem.RaycastAll(_pointer, _raycasts);
            _selected = GetFirstValidUI();

            if (_selected != null)
            {
                if (Input.GetButtonDown("Use") || Input.GetButtonDown("Attack") || Input.GetKeyDown(KeyCode.Return))
                    Click(_selected.gameObject);
                else if (_eventSystem.currentSelectedGameObject != _selected.gameObject)
                    SelectGameObject(_selected.gameObject);
            }
            else if (_eventSystem.currentSelectedGameObject != null)
                SelectGameObject(null);
        }

        public void SetActive(bool isActive)
        {
            StopAllCoroutines();
            gameObject.SetActive(isActive);
        }

        private void Click(GameObject selected)
        {
            var uiElement = selected.GetComponent<IPointerClickHandler>();
            var clicked = uiElement != null;

            if (!clicked)
            {
                var toggle = selected.GetComponent(typeof(Toggle)) as Toggle;
                if (toggle == null)
                    toggle = selected.GetComponentInParent(typeof(Toggle)) as Toggle;

                if (toggle != null)
                {
                    toggle.isOn = !toggle.isOn;
                    clicked = true;
                }
            }
            else
                uiElement.OnPointerClick(_pointer);

            if (clicked)
                SelectGameObject(null);
        }

        private RectTransform GetFirstValidUI()
        {
            IPointerClickHandler pointer = null;

            for (int i = 0, l = _raycasts.Count; i < l; i++)
            {
                pointer = _raycasts[i].gameObject.GetComponent(typeof(IPointerClickHandler)) as IPointerClickHandler;
                if (pointer != null)
                    return _raycasts[i].gameObject.GetComponent(typeof(RectTransform)) as RectTransform;

                pointer = _raycasts[i].gameObject.GetComponentInParent(typeof(IPointerClickHandler)) as IPointerClickHandler;
                if (pointer != null)
                    return _raycasts[i].gameObject.transform.parent.GetComponent(typeof(RectTransform)) as RectTransform;
            }

            return null;
        }

        private void SelectGameObject(GameObject go)
        {
            _eventSystem.SetSelectedGameObject(go);

            var targetScale = go == null ? 1.0f : 1.5f;

            _transform.localScale = new Vector3(targetScale, targetScale, targetScale); 
        }
    }
}