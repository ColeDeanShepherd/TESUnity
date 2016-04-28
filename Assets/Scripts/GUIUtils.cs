using UnityEngine;
using UnityEngine.UI;

public static class GUIUtils
{
	public static Font Arial
	{
		get
		{
			if(_Arial == null)
			{
				_Arial = Resources.GetBuiltinResource<Font>("Arial.ttf");
			}

			return _Arial;
		}
	}
	public static Sprite UIBackgroundImage
	{
		get
		{
			return TESUnity.instance.UIBackgroundImg;
		}
	}
	public static Sprite UISpriteImage
	{
		get
		{
			return TESUnity.instance.UISpriteImg;
		}
	}
	public static Sprite UIMaskImage
	{
		get
		{
			return TESUnity.instance.UIMaskImg;
		}
	}

	public static GameObject CreateCanvas()
	{
		var canvas = CreateUIObject("Canvas");
		canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.AddComponent<CanvasScaler>();
		canvas.AddComponent<GraphicRaycaster>();

		return canvas;
	}
	public static GameObject CreateEventSystem()
	{
		var eventSystem = new GameObject("EventSystem");
		eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
		eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

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
		buttonImage.sprite = UISpriteImage;
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

	public static GameObject CreateImage(Sprite sprite, GameObject parent)
	{
		var image = CreateUIObject("Image", parent);
		image.AddComponent<CanvasRenderer>();
		image.AddComponent<Image>().sprite = sprite;

		return image;
	}
	public static GameObject CreateRawImage(Texture texture, GameObject parent)
	{
		var rawImage = CreateUIObject("Raw Image", parent);
		rawImage.AddComponent<CanvasRenderer>();
		rawImage.AddComponent<RawImage>().texture = texture;

		return rawImage;
	}

	public static GameObject CreatePanel(GameObject parent)
	{
		var panel = CreateUIObject("Panel", parent);
		panel.AddComponent<CanvasRenderer>();

		var image = panel.AddComponent<Image>();
		image.sprite = UIBackgroundImage;
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
		scrollBarImage.sprite = UIBackgroundImage;
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
		handleImage.sprite = UISpriteImage;
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
		scrollViewImage.sprite = UIBackgroundImage;
		scrollViewImage.type = Image.Type.Sliced;
		scrollViewImage.color = new Color32(255, 255, 255, 100);

		var scrollViewTransform = scrollView.GetComponent<RectTransform>();
		scrollViewTransform.sizeDelta = new Vector2(200, 200);

		// Create the viewport.
		var viewport = CreateUIObject("Viewport", scrollView);
		viewport.AddComponent<Mask>().showMaskGraphic = false;

		var viewportImage = viewport.AddComponent<Image>();
		viewportImage.sprite = UIMaskImage;
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

	private static Font _Arial;

	private static GameObject CreateUIObject(string name, GameObject parent = null)
	{
		var UIObject = new GameObject(name);
		UIObject.layer = LayerMask.NameToLayer("UI");
		UIObject.AddComponent<RectTransform>();

		if(parent != null)
		{
			UIObject.transform.SetParent(parent.transform, false);
		}

		return UIObject;
	}
}