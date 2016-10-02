using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TESUnity
{
    public class PathSelectionComponent : MonoBehaviour
    {
        private static readonly string SavePathKey = "TESUnity.PathSelection.Path";
        private string defaultMWDataPath = "C:/Program Files (x86)/Steam/steamapps/common/Morrowind/Data Files";

        private new GameObject camera;
        private GameObject eventSystem;
        private GameObject canvas;
        private InputField inputField;
        private Toggle toggle;
        private Text errorText;

        private void Start()
        {
            var savedPath = PlayerPrefs.GetString(SavePathKey, string.Empty);
            if (savedPath != string.Empty)
                defaultMWDataPath = savedPath;

            camera = GameObjectUtils.CreateMainCamera(Vector3.zero, Quaternion.identity);
            eventSystem = GUIUtils.CreateEventSystem();
            canvas = GUIUtils.CreateCanvas();

            var path = Path.Combine(System.Environment.CurrentDirectory, "Data Files");
            if (Directory.Exists(path))
            {
                LoadWorld(path);
                return;
            }

            var inputFieldGO = GUIUtils.CreateInputField(defaultMWDataPath, Vector3.zero, new Vector2(620, 30), canvas);
            inputField = inputFieldGO.GetComponent<InputField>();

            var toggleGO = GUIUtils.CreateToggle("Remember this path", canvas);
            toggle = toggleGO.GetComponent<Toggle>();
            toggle.isOn = savedPath != string.Empty;
            toggle.gameObject.AddComponent<Outline>();
            toggle.GetComponentInChildren<Text>().color = Color.white;
            toggle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40);

            var button = GUIUtils.CreateTextButton("Load World", canvas);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80);
            button.GetComponent<Button>().onClick.AddListener(LoadWorld);

            eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(button);

            var errorTextGo = GUIUtils.CreateText("", canvas);
            errorText = errorTextGo.GetComponent<Text>();
            errorText.rectTransform.anchoredPosition = new Vector2(0, -120);
            errorText.color = Color.white;
            errorText.fontSize = 26;
            errorText.gameObject.AddComponent<Outline>();
            errorText.enabled = false;
        }

        private void OnDestroy()
        {
            Destroy(canvas);
            Destroy(eventSystem);
            Destroy(camera);
        }

        private void LoadWorld()
        {
            LoadWorld(inputField.text);
        }

        private void LoadWorld(string path)
        {
            if (Directory.Exists(path))
            {
                if (toggle.isOn)
                    PlayerPrefs.SetString(SavePathKey, path);

                var TESUnityComponent = GetComponent<TESUnity>();
                TESUnityComponent.dataPath = path;
                TESUnityComponent.enabled = true;

                Destroy(this);
            }
            else
                StartCoroutine(ShowErrorMessage("Invalid path."));
        }

        private IEnumerator ShowErrorMessage(string message)
        {
            errorText.text = message;
            errorText.enabled = true;
            yield return new WaitForSeconds(5.0f);
            errorText.enabled = false;
        }
    }
}