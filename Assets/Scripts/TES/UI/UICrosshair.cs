using UnityEngine;
using UnityEngine.UI;

namespace TESUnity.UI
{
    [RequireComponent(typeof(Image))]
    public class UICrosshair : MonoBehaviour
    {
        private Image _crosshair = null;

        void Start()
        {
            var textureManager = TESUnity.instance.TextureManager;
            var crosshairTexture = textureManager.LoadTexture("target", true);
            _crosshair = GetComponent<Image>();
            _crosshair.sprite = GUIUtils.CreateSprite(crosshairTexture);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
