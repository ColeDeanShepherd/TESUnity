using UnityEngine;
using UnityEngine.UI;

namespace TESUnity.UI
{
    [RequireComponent(typeof(Image))]
    public class UICrosshair : MonoBehaviour, IHUDWidget
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

        public void SetParent(Transform transform)
        {
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.SetParent(transform, false);

            var image = GetComponent<Image>();
            image.enabled = false;
        }

        public static UICrosshair Create(Transform parent)
        {
            var crossGO = new GameObject("Crosshair");
            var crossRect = crossGO.AddComponent<RectTransform>();
            crossRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 35);
            crossRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 35);
            crossRect.SetParent(parent);
            crossRect.localPosition = Vector3.zero;
            crossRect.localRotation = Quaternion.identity;
            crossRect.localScale = Vector3.one;
            crossGO.AddComponent<Image>();

            return crossGO.AddComponent<UICrosshair>();
        }
    }
}
