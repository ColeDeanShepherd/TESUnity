using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace TESUnity
{
	public class PathSelectionComponent : MonoBehaviour
	{
		private string defaultMWDataPath = "C:/Program Files (x86)/Steam/steamapps/common/Morrowind/Data Files";

		private void Start()
		{
			camera = GameObjectUtils.CreateMainCamera(Vector3.zero, Quaternion.identity);
			eventSystem = GUIUtils.CreateEventSystem();
			canvas = GUIUtils.CreateCanvas();

			inputField = GUIUtils.CreateInputField(defaultMWDataPath, Vector3.zero, new Vector2(620, 30), canvas);

			var button = GUIUtils.CreateTextButton("Load World", canvas);
			button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40);
			button.GetComponent<Button>().onClick.AddListener(LoadWorld);
		}
		private void OnDestroy()
		{
			Destroy(canvas);
			Destroy(eventSystem);
			Destroy(camera);
		}
		private void LoadWorld()
		{
			var MWDataPath = inputField.GetComponent<InputField>().text;

			if(Directory.Exists(MWDataPath))
			{
				var TESUnityComponent = GetComponent<TESUnity>();
				TESUnityComponent.MWDataPath = MWDataPath;

				TESUnityComponent.enabled = true;
				Destroy(this);
			}
			else
			{
				Debug.Log("Invalid path.");
			}
		}

		private new GameObject camera;
		private GameObject eventSystem, canvas, inputField;
	}
}