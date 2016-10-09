using UnityEngine;
using UnityEngine.UI;

namespace TESUnity.UI
{
    public class UIInteractiveText : MonoBehaviour
    {
        private const int WidgetWidth = 200;
        private const int WidgetHeight = 100;

        private bool _opened;

        [SerializeField]
        private GameObject _container = null;
        [SerializeField]
        private Image _icon = null;
        [SerializeField]
        private Text _title = null;

        [SerializeField]
        private GameObject _inventoryInfos = null;
        [SerializeField]
        private Text _weight = null;
        [SerializeField]
        private Text _value = null;

        void Start()
        {
            transform.localPosition = Vector3.zero;
            _opened = false;
            _container.SetActive(false);
        }

        public void Show(Sprite icon, string prefixTitle, string title, string weight, string value)
        {
            _icon.enabled = icon != null;
            if (_icon.enabled)
                _icon.sprite = icon;

            _title.text = string.IsNullOrEmpty(prefixTitle) ? title : prefixTitle + title;

            var showInventoryInfos = !string.IsNullOrEmpty(weight) && !string.IsNullOrEmpty(value);

            _inventoryInfos.SetActive(showInventoryInfos);
            _container.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, showInventoryInfos ? WidgetHeight : WidgetHeight / 2.0f);

            if (showInventoryInfos)
            {
                _weight.text = "Weight: " + weight;
                _value.text = "Value: " + value;
            }

            transform.SetAsLastSibling();
            _container.SetActive(true);
            _opened = true;
        }

        public void Close()
        {
            if (_opened)
            {
                _container.SetActive(false);
                _opened = false;
            }
        }

        public static UIInteractiveText Create(GameObject parent)
        {
            // 1. Global Container
            var globalContainer = new GameObject("InteractiveText");
            var gcTransform = globalContainer.AddComponent<RectTransform>();
            gcTransform.SetParent(parent.transform);
            gcTransform.localPosition = Vector3.zero;
            gcTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, WidgetWidth);
            gcTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, WidgetHeight);

            var mcImage = globalContainer.AddComponent<Image>();
            mcImage.color = new Color(144.0f / 255.0f, 133.0f / 255.0f, 99.0f / 255.0f);

            var uiInteractive = globalContainer.AddComponent<UIInteractiveText>();

            // 2. Background
            var background = new GameObject("Background");
            var bgTransform = background.AddComponent<RectTransform>();
            bgTransform.SetParent(gcTransform);
            bgTransform.localPosition = Vector3.zero;
            bgTransform.anchorMin = Vector2.zero;
            bgTransform.anchorMax = Vector2.one;
            bgTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, WidgetWidth - 2);
            bgTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, WidgetHeight - 2);

            var bgImg = background.AddComponent<Image>();
            bgImg.color = Color.black;

            // 3. MainContainer
            var mainContainer = new GameObject("MainContainer");
            var mcTransform = mainContainer.AddComponent<RectTransform>();
            mcTransform.SetParent(gcTransform);
            mcTransform.localPosition = Vector3.zero;
            mcTransform.anchorMin = Vector2.zero;
            mcTransform.anchorMax = Vector2.one;
            mcTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, WidgetWidth - 10);
            mcTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, WidgetHeight - 10);

            var mcVerticalLayout = mainContainer.AddComponent<VerticalLayoutGroup>();
            mcVerticalLayout.spacing = 10;

            // 4. Header
            var header = new GameObject("Header");
            var hTransform = header.AddComponent<RectTransform>();
            hTransform.SetParent(mcTransform);

            var hHorizontalLayout = header.AddComponent<HorizontalLayoutGroup>();
            hHorizontalLayout.spacing = 10;

            var hLayoutElement = header.AddComponent<LayoutElement>();
            hLayoutElement.minHeight = 30;

            // 5. Header Content
            var iconGO = new GameObject("Icon");
            var iconTransform = iconGO.AddComponent<RectTransform>();
            iconTransform.SetParent(hTransform);

            var imgIcon = iconGO.AddComponent<Image>();

            var titleGO = new GameObject("Title");
            var titleTransform = titleGO.AddComponent<RectTransform>();
            titleTransform.SetParent(hTransform);

            var titleText = titleGO.AddComponent<Text>();
            titleText.color = new Color(223.0f / 255.0f, 200.0f / 255.0f, 158.0f / 255.0f);
            titleText.font = GUIUtils.Arial;

            var titleLayoutElement = titleGO.AddComponent<LayoutElement>();
            titleLayoutElement.minWidth = 120;

            // 6. Content
            var content = new GameObject("Content");
            var cTransform = content.AddComponent<RectTransform>();
            cTransform.SetParent(mcTransform);
            content.AddComponent<VerticalLayoutGroup>();

            // 7. Content Content
            var weightGO = new GameObject("Title");
            var weightTransform = weightGO.AddComponent<RectTransform>();
            weightTransform.SetParent(cTransform);

            var wText = weightGO.AddComponent<Text>();
            wText.color = new Color(202.0f / 255.0f, 165.0f / 255.0f, 96.0f / 255.0f);
            wText.font = GUIUtils.Arial;
            wText.alignment = TextAnchor.MiddleCenter;

            var valueGO = new GameObject("Title");
            var valueTransform = valueGO.AddComponent<RectTransform>();
            valueTransform.SetParent(cTransform);

            var vText = valueGO.AddComponent<Text>();
            vText.color = new Color(202.0f / 255.0f, 165.0f / 255.0f, 96.0f / 255.0f);
            vText.font = GUIUtils.Arial;
            vText.alignment = TextAnchor.MiddleCenter;

            // 8. Binding
            uiInteractive._container = globalContainer;
            uiInteractive._icon = imgIcon;
            uiInteractive._title = titleText;
            uiInteractive._inventoryInfos = content;
            uiInteractive._weight = wText;
            uiInteractive._value = vText;

            return uiInteractive;
        }
    }
}