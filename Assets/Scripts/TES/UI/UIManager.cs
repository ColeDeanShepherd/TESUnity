using UnityEngine;

namespace TESUnity.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIManager : MonoBehaviour
    {
        private Canvas _canvas = null;

        [Header("HUD Elements")]
        [SerializeField]
        private UICrosshair _crosshair;
        [SerializeField]
        private UIInteractiveText _interactiveText;

        [Header("UI Elements")]
        [SerializeField]
        private UIBook _book;
        [SerializeField]
        private UIScroll _scroll;

        #region Public Fields

        public UIBook Book
        {
            get { return _book; }
        }

        public UIInteractiveText InteractiveText
        {
            get { return _interactiveText; }
        }

        public UIScroll Scroll
        {
            get { return _scroll; }
        }

        public UICrosshair Crosshair
        {
            get { return _crosshair; }
        }

        public bool Visible
        {
            get { return _canvas.enabled; }
            set { _canvas.enabled = value; }
        }

        public bool Active
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
        }

        #endregion

        void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        public Transform GetHUD()
        {
            return transform.Find("HUD");
        }

        public Transform GetUI()
        {
            return transform.Find("UI");
        }
    }
}
