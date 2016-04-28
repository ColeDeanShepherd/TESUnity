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

		return textObject;
	}
	public static GameObject CreateTextButton(string text, GameObject parent)
	{
		var button = CreateUIObject("Button", parent);
		button.AddComponent<CanvasRenderer>();

		var buttonImage = button.AddComponent<Image>();
		buttonImage.sprite = TESUnity.instance.UISpriteImg;
		buttonImage.type = Image.Type.Sliced;

		button.AddComponent<Button>();

		var textObj = CreateText(text, button);
		var textObjTransform = textObj.GetComponent<RectTransform>();
		textObjTransform.anchorMin = Vector2.zero;
		textObjTransform.anchorMax = Vector2.one;

		return button;
	}
	public static GameObject CreateScrollView(GameObject parent)
	{
		var scrollView = GameObject.Instantiate(TESUnity.instance.scrollViewPrefab);
		scrollView.transform.SetParent(parent.transform, false);

		return scrollView;

		/*// Create the scroll view.
		var scrollView = CreateUIObject("Scroll View", parent);
		scrollView.AddComponent<CanvasRenderer>();

		var scrollViewScrollRect = scrollView.AddComponent<ScrollRect>();
		scrollViewScrollRect.movementType = ScrollRect.MovementType.Clamped;

		var scrollViewImage = scrollView.AddComponent<Image>();
		scrollViewImage.sprite = TESUnity.instance.UIBackgroundImg;
		scrollViewImage.type = Image.Type.Sliced;
		scrollViewImage.color = new Color32(255, 255, 255, 100);

		// Create the viewport.
		var viewport = CreateUIObject("Viewport", scrollView);
		viewport.AddComponent<Mask>().showMaskGraphic = false;
		viewport.AddComponent<Image>();
		var viewportRectTransform = viewport.GetComponent<RectTransform>();
		viewportRectTransform.anchorMin = Vector2.zero;
		viewportRectTransform.anchorMax = Vector2.one;
		viewportRectTransform.offsetMin = Vector2.zero;
		viewportRectTransform.offsetMax = Vector2.zero;

		// Create the content container.
		var content = CreateUIObject("Content", viewport);
		var contentRectTransform = content.GetComponent<RectTransform>();
		contentRectTransform.pivot = Vector2.up;
		contentRectTransform.anchorMin = Vector2.up;
		contentRectTransform.anchorMax = Vector2.up;
		contentRectTransform.anchoredPosition = Vector2.zero;

		// Create the scroll bars.
		var verticalScrollBar = CreateScrollBar(Scrollbar.Direction.BottomToTop, scrollView);
		var horizontalScrollBar = CreateScrollBar(Scrollbar.Direction.RightToLeft, scrollView);

		scrollViewScrollRect.viewport = viewport.GetComponent<RectTransform>();
		scrollViewScrollRect.content = content.GetComponent<RectTransform>();

		// Link everything together.
		scrollViewScrollRect.verticalScrollbar = verticalScrollBar.GetComponent<Scrollbar>();
		scrollViewScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;

		scrollViewScrollRect.horizontalScrollbar = horizontalScrollBar.GetComponent<Scrollbar>();
		scrollViewScrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;

		return scrollView;*/
	}
	// TODO: Verify everything is the same as Unity's default scrollbar.
	public static GameObject CreateScrollBar(Scrollbar.Direction direction, GameObject parent)
	{
		// Create the scroll bar object.
		var isVertical = (direction == Scrollbar.Direction.BottomToTop) || (direction == Scrollbar.Direction.TopToBottom);
		var scrollBarName = isVertical ? "Scrollbar Vertical" : "Scrollbar Horizontal";
		var scrollBar = CreateUIObject(scrollBarName, parent);
		scrollBar.AddComponent<CanvasRenderer>();

		var scrollBarImage = scrollBar.AddComponent<Image>();
		scrollBarImage.sprite = TESUnity.instance.UIBackgroundImg;
		scrollBarImage.type = Image.Type.Sliced;

		var scrollBarComponent = scrollBar.AddComponent<Scrollbar>();
		scrollBarComponent.direction = direction;

		var scrollBarTransform = scrollBar.GetComponent<RectTransform>();
		scrollBarTransform.sizeDelta = new Vector2(20, 20);
		scrollBarTransform.anchorMin = new Vector2(1, 0);
		scrollBarTransform.anchorMax = new Vector2(1, 1);

		// Create the sliding area.
		var slidingArea = CreateUIObject("Sliding Area", scrollBar);
		var slidingAreaRectTransform = slidingArea.GetComponent<RectTransform>();
		slidingAreaRectTransform.anchorMin = Vector2.zero;
		slidingAreaRectTransform.anchorMax = Vector2.one;
		slidingAreaRectTransform.offsetMin = Vector2.zero;
		slidingAreaRectTransform.offsetMax = Vector2.zero;

		// Create the handle.
		var handle = CreateUIObject("Handle", slidingArea);
		handle.AddComponent<CanvasRenderer>();

		var handleImage = handle.AddComponent<Image>();
		handleImage.sprite = TESUnity.instance.UISpriteImg;
		handleImage.type = Image.Type.Sliced;

		var handleRectTransform = handle.GetComponent<RectTransform>();
		handleRectTransform.offsetMin = Vector2.zero;
		handleRectTransform.offsetMax = Vector2.zero;

		// Link everything together.
		scrollBarComponent.handleRect = handle.GetComponent<RectTransform>();
		scrollBarComponent.targetGraphic = handle.GetComponent<Image>();

		return scrollBar;
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