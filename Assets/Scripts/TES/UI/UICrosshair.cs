using UnityEngine;
using UnityEngine.UI;

namespace TESUnity.UI
{
    [RequireComponent(typeof(Image))]
    public class UICrosshair : MonoBehaviour
    {
        private Image _crosshair = null;

        public bool Enabled
        {
            get { return _crosshair.enabled; }
            set { _crosshair.enabled = value; }
        }

        private void Awake()
        {
            _crosshair = GetComponent<Image>();
        }

        private void Start()
        {
            var textureManager = TESUnity.instance.TextureManager;
            var crosshairTexture = textureManager.LoadTexture("target", true);
            _crosshair.sprite = GUIUtils.CreateSprite(crosshairTexture);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
