using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class GUIUtils
{
    private static GameObject mainCanvas;

    public static Font Arial
    {
        get
        {
            if (_Arial == null)
            {
                _Arial = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return _Arial;
        }
    }

    public static Sprite backgroundImg
    {
        get
        {
            return TESUnity.TESUnity.instance.UIBackgroundImg;
        }
    }
    public static Sprite checkmarkImg
    {
        get
        {
            return TESUnity.TESUnity.instance.UICheckmarkImg;
        }
    }
    public static Sprite dropdownArrowImg
    {
        get
        {
            return TESUnity.TESUnity.instance.UIDropdownArrowImg;
        }
    }
    public static Sprite inputFieldBackgroundImg
    {
        get
        {
            return TESUnity.TESUnity.instance.UIInputFieldBackgroundImg;
        }
    }
    public static Sprite knobImg
    {
        get
        {
            return TESUnity.TESUnity.instance.UIKnobImg;
        }
    }
    public static Sprite maskImg
    {
        get
        {
            return TESUnity.TESUnity.instance.UIMaskImg;
        }
    }
    public static Sprite spriteImg
    {
        get
        {
            return TESUnity.TESUnity.instance.UISpriteImg;
        }
    }

    public static GameObject MainCanvas
    {
        get
        {
            if (mainCanvas == null)
            {
                var canvas = MonoBehaviour.FindObjectOfType<Canvas>();
                if (canvas != null)
                    mainCanvas = canvas.gameObject;
            }
            return mainCanvas;
        }
    }

    /// <summary>
    /// Create a canvas node.
    /// </summary>
    /// <param name="isMainCanvas">Indicates if this canvas will be the main canvas.</param>
    /// <returns>Returns the GameObject that contains the Canvas element.</returns>
    public static GameObject CreateCanvas(bool isMainCanvas = true)
    {
        var canvasGO = CreateUIObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 1.0f;

        canvasGO.AddComponent<GraphicRaycaster>();

        if (isMainCanvas)
            mainCanvas = canvasGO;

        return canvasGO;
    }

    public static void SetCanvasToWorldSpace(Canvas canvas, Transform parent, float depth, float scale)
    {
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        var canvasTransform = canvas.GetComponent<RectTransform>();
        canvasTransform.SetParent(parent);
        canvasTransform.localPosition = new Vector3(0.0f, 0.0f, depth);
        canvasTransform.localRotation = Quaternion.identity;
        canvasTransform.localScale = new Vector3(scale, scale, scale);

        var canvasScaler = canvas.GetComponent<CanvasScaler>();
        if (canvasScaler != null)
        {
            canvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvasScaler.referenceResolution.x);
            canvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasScaler.referenceResolution.y);
        }
    }

    public static GameObject CreateEventSystem()
    {
        if (EventSystem.current != null)
            return EventSystem.current.gameObject;

        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        return eventSystem;
    }

    public static GameObject CreateText(string text, GameObject parent)
    {
        var textObject = CreateUIObject("Text", parent);
        textObject.AddComponent<CanvasRenderer>();

        var textComponent = textObject.AddComponent<Text>();
        textComponent.font = Arial;
        textComponent.text = text;
        textComponent.color = new Color32(50, 50, 50, 255);

        textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 30);

        return textObject;
    }
    public static GameObject CreateButton(GameObject parent)
    {
        var button = CreateUIObject("Button", parent);
        button.AddComponent<CanvasRenderer>();

        var buttonImage = button.AddComponent<Image>();
        buttonImage.sprite = spriteImg;
        buttonImage.type = Image.Type.Sliced;

        button.AddComponent<Button>();
        button.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 30);

        return button;
    }
    public static GameObject CreateTextButton(string text, GameObject parent)
    {
        var button = CreateButton(parent);

        var textObj = CreateText(text, button);
        var textObjTransform = textObj.GetComponent<RectTransform>();
        textObjTransform.anchorMin = Vector2.zero;
        textObjTransform.anchorMax = Vector2.one;
        textObjTransform.offsetMin = Vector2.zero;
        textObjTransform.offsetMax = Vector2.zero;

        textObj.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

        return button;
    }

    public static GameObject CreateImage(Sprite sprite, GameObject parent, int width = 0, int height = 0)
    {
        var gameObject = CreateUIObject("Image", parent);
        gameObject.AddComponent<CanvasRenderer>();

        var image = gameObject.AddComponent<Image>();
        image.sprite = sprite;

        if (width > 0)
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

        if (height > 0)
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        return gameObject;
    }
    public static GameObject CreateRawImage(Texture texture, GameObject parent)
    {
        var rawImage = CreateUIObject("Raw Image", parent);
        rawImage.AddComponent<CanvasRenderer>();
        rawImage.AddComponent<RawImage>().texture = texture;

        return rawImage;
    }

    public static Sprite CreateSprite(Texture2D texture)
    {
        if (texture == null)
            return null;

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector3.zero);
    }

    public static GameObject CreatePanel(GameObject parent)
    {
        var panel = CreateUIObject("Panel", parent);
        panel.AddComponent<CanvasRenderer>();

        var image = panel.AddComponent<Image>();
        image.sprite = backgroundImg;
        image.type = Image.Type.Sliced;
        image.color = new Color32(255, 255, 255, 100);

        var transform = panel.GetComponent<RectTransform>();
        transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.one;
        transform.offsetMin = Vector2.zero;
        transform.offsetMax = Vector2.zero;

        return panel;
    }

    public static GameObject CreateScrollBar(Scrollbar.Direction direction, GameObject parent)
    {
        // Create the scroll bar object.
        var scrollBar = CreateUIObject("Scrollbar", parent);
        scrollBar.AddComponent<CanvasRenderer>();

        var isHorizontal = (direction == Scrollbar.Direction.LeftToRight) || (direction == Scrollbar.Direction.RightToLeft);
        float length = 160;
        scrollBar.GetComponent<RectTransform>().sizeDelta = isHorizontal ? new Vector2(length, 20) : new Vector2(20, length);

        var scrollBarImage = scrollBar.AddComponent<Image>();
        scrollBarImage.sprite = backgroundImg;
        scrollBarImage.type = Image.Type.Sliced;

        var scrollBarComponent = scrollBar.AddComponent<Scrollbar>();
        scrollBarComponent.direction = direction;

        // Create the sliding area.
        var slidingArea = CreateUIObject("Sliding Area", scrollBar);

        var slidingAreaRectTransform = slidingArea.GetComponent<RectTransform>();
        slidingAreaRectTransform.anchorMin = Vector2.zero;
        slidingAreaRectTransform.anchorMax = Vector2.one;
        slidingAreaRectTransform.offsetMin = new Vector2(10, 10);
        slidingAreaRectTransform.offsetMax = new Vector2(-10, -10);

        // Create the handle.
        var handle = CreateUIObject("Handle", slidingArea);
        handle.AddComponent<CanvasRenderer>();

        var handleImage = handle.AddComponent<Image>();
        handleImage.sprite = spriteImg;
        handleImage.type = Image.Type.Sliced;

        var handleRectTransform = handle.GetComponent<RectTransform>();
        handleRectTransform.offsetMin = new Vector2(-10, -10);
        handleRectTransform.offsetMax = new Vector2(10, 10);

        // Link everything together.
        scrollBarComponent.handleRect = handle.GetComponent<RectTransform>();
        scrollBarComponent.targetGraphic = handle.GetComponent<Image>();

        return scrollBar;
    }
    public static GameObject CreateHorizontalScrollBar(GameObject parent)
    {
        return CreateScrollBar(Scrollbar.Direction.LeftToRight, parent);
    }
    public static GameObject CreateVerticalScrollBar(GameObject parent)
    {
        return CreateScrollBar(Scrollbar.Direction.BottomToTop, parent);
    }

    public static GameObject CreateScrollView(GameObject parent)
    {
        // Create the scroll view.
        var scrollView = CreateUIObject("Scroll View", parent);
        var scrollViewScrollRect = scrollView.AddComponent<ScrollRect>();
        scrollView.AddComponent<CanvasRenderer>();

        var scrollViewImage = scrollView.AddComponent<Image>();
        scrollViewImage.sprite = backgroundImg;
        scrollViewImage.type = Image.Type.Sliced;
        scrollViewImage.color = new Color32(255, 255, 255, 100);

        var scrollViewTransform = scrollView.GetComponent<RectTransform>();
        scrollViewTransform.sizeDelta = new Vector2(200, 200);

        // Create the viewport.
        var viewport = CreateUIObject("Viewport", scrollView);
        viewport.AddComponent<Mask>().showMaskGraphic = false;

        var viewportImage = viewport.AddComponent<Image>();
        viewportImage.sprite = maskImg;
        viewportImage.type = Image.Type.Sliced;

        var viewportRectTransform = viewport.GetComponent<RectTransform>();
        viewportRectTransform.pivot = Vector2.up;

        // Create the content container.
        var content = CreateUIObject("Content", viewport);
        var contentRectTransform = content.GetComponent<RectTransform>();
        contentRectTransform.anchorMin = Vector2.up;
        contentRectTransform.anchorMax = Vector2.one;
        contentRectTransform.pivot = Vector2.up;
        contentRectTransform.sizeDelta = new Vector2(0, 300);

        // Create the scroll bars.
        var horizontalScrollBar = CreateHorizontalScrollBar(scrollView);
        horizontalScrollBar.name = "Scrollbar Horizontal";

        var horizontalScrollBarTransform = horizontalScrollBar.GetComponent<RectTransform>();
        horizontalScrollBarTransform.anchorMin = Vector2.zero;
        horizontalScrollBarTransform.anchorMax = Vector2.right;
        horizontalScrollBarTransform.pivot = Vector2.zero;

        var verticalScrollBar = CreateVerticalScrollBar(scrollView);
        verticalScrollBar.name = "Scrollbar Vertical";

        var verticalScrollBarTransform = verticalScrollBar.GetComponent<RectTransform>();
        verticalScrollBarTransform.anchorMin = Vector2.right;
        verticalScrollBarTransform.anchorMax = Vector2.one;
        verticalScrollBarTransform.pivot = Vector2.one;

        // Link everything together.
        scrollViewScrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollViewScrollRect.content = content.GetComponent<RectTransform>();

        scrollViewScrollRect.horizontalScrollbar = horizontalScrollBar.GetComponent<Scrollbar>();
        scrollViewScrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        scrollViewScrollRect.horizontalScrollbarSpacing = -3;

        scrollViewScrollRect.verticalScrollbar = verticalScrollBar.GetComponent<Scrollbar>();
        scrollViewScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        scrollViewScrollRect.verticalScrollbarSpacing = -3;

        return scrollView;
    }

    public static void AddToScrollView(GameObject UIObject)
    {
        UIObject.transform.SetParent(UIObject.transform, false);
    }
    public static GameObject GetScrollViewContent(GameObject scrollView)
    {
        return scrollView.transform.FindChild("Viewport/Content").gameObject;
    }

    public static GameObject CreateDropdown(GameObject parent)
    {
        // Create the dropdown object.
        var dropdown = CreateUIObject("Dropdown", parent);
        dropdown.AddComponent<CanvasRenderer>();

        var image = dropdown.AddComponent<Image>();
        image.sprite = spriteImg;
        image.type = Image.Type.Sliced;

        var dropdownComponent = dropdown.AddComponent<Dropdown>();
        dropdownComponent.targetGraphic = image;

        dropdownComponent.options = new System.Collections.Generic.List<Dropdown.OptionData>();
        dropdownComponent.options.Add(new Dropdown.OptionData("Option A"));
        dropdownComponent.options.Add(new Dropdown.OptionData("Option B"));
        dropdownComponent.options.Add(new Dropdown.OptionData("Option C"));

        dropdown.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 30);

        // Create the label.
        var label = CreateText("", dropdown);
        label.name = "Label";

        var labelTransform = label.GetComponent<RectTransform>();
        labelTransform.anchorMin = Vector2.zero;
        labelTransform.anchorMax = Vector2.one;
        labelTransform.offsetMin = new Vector2(10, 6);
        labelTransform.offsetMax = new Vector2(-25, -7);

        label.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

        // Create the arrow.
        var arrow = CreateImage(dropdownArrowImg, dropdown);
        arrow.name = "Arrow";

        var arrowTransform = arrow.GetComponent<RectTransform>();
        arrowTransform.anchorMin = new Vector2(1, 0.5f);
        arrowTransform.anchorMax = new Vector2(1, 0.5f);
        arrowTransform.anchoredPosition = new Vector2(-15, 0);
        arrowTransform.sizeDelta = new Vector2(20, 20);

        // Create the scroll view.
        var scrollView = CreateScrollView(dropdown);
        scrollView.name = "Template";

        var scrollViewTransform = scrollView.GetComponent<RectTransform>();
        scrollViewTransform.anchorMin = Vector2.zero;
        scrollViewTransform.anchorMax = new Vector2(1, 0);
        scrollViewTransform.pivot = new Vector2(0.5f, 1);
        scrollViewTransform.offsetMin = Vector2.zero;
        scrollViewTransform.anchoredPosition = new Vector2(0, 2);
        scrollViewTransform.sizeDelta = new Vector2(0, 150);

        var scrollViewImage = scrollView.GetComponent<Image>();
        scrollViewImage.sprite = spriteImg;
        scrollViewImage.color = new Color32(255, 255, 255, 255);

        var scrollViewScrollRect = scrollView.GetComponent<ScrollRect>();
        scrollViewScrollRect.horizontal = false;
        scrollViewScrollRect.movementType = ScrollRect.MovementType.Clamped;

        var scrollViewContent = GetScrollViewContent(scrollView);
        var scrollViewContentTransform = scrollViewContent.GetComponent<RectTransform>();
        scrollViewContentTransform.sizeDelta = new Vector2(0, 28);
        scrollViewContentTransform.pivot = new Vector2(0.5f, 1);

        scrollView.SetActive(false);

        // Create the dropdown item.
        var item = CreateDropdownItem("Option A", GetScrollViewContent(scrollView));

        // Link everything together.
        dropdownComponent.template = scrollView.GetComponent<RectTransform>();
        dropdownComponent.captionText = label.GetComponent<Text>();
        dropdownComponent.itemText = item.transform.FindChild("Item Label").gameObject.GetComponent<Text>();

        return dropdown;
    }
    public static GameObject CreateDropdownItem(string text, GameObject parent)
    {
        // Create the item object.
        var item = CreateUIObject("Item", parent);

        var itemTransform = item.GetComponent<RectTransform>();
        itemTransform.anchorMin = new Vector2(0, 0.5f);
        itemTransform.anchorMax = new Vector2(1, 0.5f);
        itemTransform.offsetMin = Vector2.zero;
        itemTransform.offsetMax = Vector2.zero;
        itemTransform.sizeDelta = new Vector2(0, 20);

        var itemToggle = item.AddComponent<Toggle>();

        // Create the item background.
        var itemBackground = CreateImage(null, item);
        itemBackground.name = "Item Background";

        var itemBackgroundTransform = itemBackground.GetComponent<RectTransform>();
        itemBackgroundTransform.anchorMin = Vector2.zero;
        itemBackgroundTransform.anchorMax = Vector2.one;
        itemBackgroundTransform.offsetMin = Vector2.zero;
        itemBackgroundTransform.offsetMax = Vector2.zero;

        // Create the item checkmark.
        var itemCheckmark = CreateImage(checkmarkImg, item);
        itemCheckmark.name = "Item Checkmark";

        var itemCheckmarkTransform = itemCheckmark.GetComponent<RectTransform>();
        itemCheckmarkTransform.anchorMin = new Vector2(0, 0.5f);
        itemCheckmarkTransform.anchorMax = new Vector2(0, 0.5f);
        itemCheckmarkTransform.sizeDelta = new Vector2(20, 20);
        itemCheckmarkTransform.anchoredPosition = new Vector2(10, 0);

        // Create the item label.
        var itemLabel = CreateText(text, item);
        itemLabel.name = "Item Label";

        var itemLabelTransform = itemLabel.GetComponent<RectTransform>();
        itemLabelTransform.anchorMin = Vector2.zero;
        itemLabelTransform.anchorMax = Vector2.one;
        itemLabelTransform.offsetMin = new Vector2(20, 1);
        itemLabelTransform.offsetMax = new Vector2(-10, -2);

        itemLabel.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

        // Link everything together.
        itemToggle.targetGraphic = itemBackground.GetComponent<Image>();
        itemToggle.graphic = itemCheckmark.GetComponent<Image>();
        itemToggle.isOn = true;

        return item;
    }

    public static GameObject CreateInputField(string value, Vector3 position, Vector2 size, GameObject parent)
    {
        var inputField = CreateUIObject("Input Field", parent);

        var inputFieldTransform = inputField.GetComponent<RectTransform>();
        inputFieldTransform.anchoredPosition = position;
        inputFieldTransform.sizeDelta = size;

        inputField.AddComponent<CanvasRenderer>();

        var inputFieldImage = inputField.AddComponent<Image>();
        inputFieldImage.sprite = inputFieldBackgroundImg;
        inputFieldImage.type = Image.Type.Sliced;

        var inputFieldComponent = inputField.AddComponent<InputField>();

        // Create the caret.
        var caret = CreateUIObject("Input Caret", inputField);
        caret.AddComponent<CanvasRenderer>();
        caret.AddComponent<LayoutElement>().ignoreLayout = true;

        var caretTransform = caret.GetComponent<RectTransform>();
        caretTransform.anchorMin = Vector2.zero;
        caretTransform.anchorMax = Vector2.one;
        caretTransform.offsetMin = new Vector2(10, 6);
        caretTransform.offsetMax = new Vector2(-10, -7);

        // Create the placeholder text.
        var placeholderText = CreateText("Enter text...", inputField);
        placeholderText.name = "Placeholder";

        var placeholderTextTransform = placeholderText.GetComponent<RectTransform>();
        placeholderTextTransform.anchorMin = Vector2.zero;
        placeholderTextTransform.anchorMax = Vector2.one;
        placeholderTextTransform.offsetMin = new Vector2(10, 6);
        placeholderTextTransform.offsetMax = new Vector2(-10, -7);

        var placeholderTextTextComponent = placeholderText.GetComponent<Text>();
        placeholderTextTextComponent.fontStyle = FontStyle.Italic;
        placeholderTextTextComponent.color = new Color32(50, 50, 50, 128);

        // Create the text.
        var text = CreateText("", inputField);

        var textTextTransform = text.GetComponent<RectTransform>();
        textTextTransform.anchorMin = Vector2.zero;
        textTextTransform.anchorMax = Vector2.one;
        textTextTransform.offsetMin = new Vector2(10, 6);
        textTextTransform.offsetMax = new Vector2(-10, -7);

        var textTextComponent = text.GetComponent<Text>();
        textTextComponent.supportRichText = false;

        // Link everything together.
        inputFieldComponent.textComponent = textTextComponent;
        inputFieldComponent.placeholder = placeholderTextTextComponent;
        inputFieldComponent.text = value;

        return inputField;
    }

    public static GameObject CreateSlider(GameObject parent)
    {
        var slider = CreateUIObject("Slider", parent);
        slider.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 20);
        var sliderComponent = slider.AddComponent<Slider>();

        // Create the background.
        var background = CreateImage(backgroundImg, slider);
        background.name = "Background";

        var backgroundTransform = background.GetComponent<RectTransform>();
        backgroundTransform.anchorMin = new Vector2(0, 0.25f);
        backgroundTransform.anchorMax = new Vector2(1, 0.75f);
        backgroundTransform.offsetMin = Vector2.zero;
        backgroundTransform.offsetMax = Vector2.zero;

        background.GetComponent<Image>().type = Image.Type.Sliced;

        // Create the fill area.
        var fillArea = CreateUIObject("Fill Area", slider);

        var fillAreaTransform = fillArea.GetComponent<RectTransform>();
        fillAreaTransform.anchorMin = new Vector2(0, 0.25f);
        fillAreaTransform.anchorMax = new Vector2(1, 0.75f);
        fillAreaTransform.offsetMin = new Vector2(5, 0);
        fillAreaTransform.offsetMax = new Vector2(-15, 0);

        // Create the fill.
        var fill = CreateImage(spriteImg, fillArea);
        fill.name = "Fill";

        var fillTransform = fill.GetComponent<RectTransform>();
        fillTransform.anchoredPosition = Vector2.zero;
        fillTransform.offsetMin = Vector2.zero;
        fillTransform.offsetMax = Vector2.zero;
        fillTransform.sizeDelta = new Vector2(10, 0);

        fill.GetComponent<Image>().type = Image.Type.Sliced;

        // Create the handle slide area.
        var handleSlideArea = CreateUIObject("Handle Slide Area", slider);

        var handleSlideAreaTransform = handleSlideArea.GetComponent<RectTransform>();
        handleSlideAreaTransform.anchorMin = Vector2.zero;
        handleSlideAreaTransform.anchorMax = Vector2.one;
        handleSlideAreaTransform.offsetMin = new Vector2(10, 0);
        handleSlideAreaTransform.offsetMax = new Vector2(-10, 0);

        // Create the handle.
        var handle = CreateImage(knobImg, handleSlideArea);
        handle.name = "Handle";

        var handleTransform = handle.GetComponent<RectTransform>();
        handleTransform.anchoredPosition = Vector2.zero;
        handleTransform.offsetMin = Vector2.zero;
        handleTransform.offsetMax = Vector2.zero;
        handleTransform.sizeDelta = new Vector2(20, 0);

        // Link everything together.
        sliderComponent.targetGraphic = handle.GetComponent<Image>();
        sliderComponent.fillRect = fill.GetComponent<RectTransform>();
        sliderComponent.handleRect = handle.GetComponent<RectTransform>();

        return slider;
    }

    public static GameObject CreateToggle(string labelContent, GameObject parent)
    {
        var toggle = CreateUIObject("Toggle", parent);
        toggle.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 20);

        var toggleComponent = toggle.AddComponent<Toggle>();
        toggleComponent.isOn = true;

        // Create the background.
        var background = CreateImage(spriteImg, toggle);
        background.name = "Background";

        var backgroundTransform = background.GetComponent<RectTransform>();
        backgroundTransform.sizeDelta = new Vector2(20, 20);
        backgroundTransform.anchorMin = new Vector2(0, 1);
        backgroundTransform.anchorMax = new Vector2(0, 1);
        backgroundTransform.anchoredPosition = new Vector2(10, -10);

        background.GetComponent<Image>().type = Image.Type.Sliced;

        // Create the checkmark.
        var checkmark = CreateImage(checkmarkImg, background);
        checkmark.name = "Checkmark";

        checkmark.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);

        // Create the label.
        var label = CreateText(labelContent, toggle);
        label.name = "Label";

        var labelTransform = label.GetComponent<RectTransform>();
        labelTransform.anchorMin = Vector2.zero;
        labelTransform.anchorMax = Vector2.one;
        labelTransform.offsetMin = new Vector2(23, 1);
        labelTransform.offsetMax = new Vector2(-5, -2);

        // Link everything together.
        toggleComponent.targetGraphic = background.GetComponent<Image>();
        toggleComponent.graphic = checkmark.GetComponent<Image>();

        return toggle;
    }

    private static Font _Arial;

    private static GameObject CreateUIObject(string name, GameObject parent = null)
    {
        var UIObject = new GameObject(name);
        UIObject.layer = LayerMask.NameToLayer("UI");
        UIObject.AddComponent<RectTransform>();

        if (parent != null)
        {
            UIObject.transform.SetParent(parent.transform, false);
        }

        return UIObject;
    }
}