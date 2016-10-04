using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TESUnity
{
    public class PathSelectionComponent : MonoBehaviour
    {
        private static readonly string SavePathKey = "keepMWPath";
        private string defaultMWDataPath = "C:/Program Files (x86)/Steam/steamapps/common/Morrowind/Data Files";

        private new GameObject camera;
        private GameObject eventSystem;
        private GameObject canvas;
        private InputField inputField;
        private Toggle toggle;
        private Text errorText;

        private void Start()
        {
            CheckConfigINI();

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
            var path = inputField.text;

            if (toggle.isOn)
                PlayerPrefs.SetString(SavePathKey, path);

            LoadWorld(path);
        }

        private void LoadWorld(string path)
        {
            if (Directory.Exists(path))
            {
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

        /// <summary>
        /// Checks if a file named Config.ini is located left to the main executable.
        /// Open/Parse it and configure default values.
        /// </summary>
        private void CheckConfigINI()
        {
            if (!File.Exists("Config.ini"))
                return;

            using (var savedData = File.OpenText("Config.ini"))
            {
                var text = savedData.ReadToEnd();

                if (text != string.Empty)
                {
                    using (var stream = new StringReader(text))
                    {
                        var line = stream.ReadLine();
                        var temp = new string[2];
                        var tes = GetComponent<TESUnity>();
                        var value = string.Empty;

                        while (line != null)
                        {
                            temp = line.Split('=');

                            if (temp.Length == 2)
                            {
                                value = temp[1].Trim();

                                switch (temp[0].Trim())
                                {
                                    case "SunShadows": tes.renderSunShadows = bool.Parse(value); break;
                                    case "LightShadows": tes.renderLightShadows = bool.Parse(value); break;
                                    case "PlayMusic": tes.playMusic = bool.Parse(value); break;
                                    case "RenderPath":
                                        var renderPathID = int.Parse(value);

                                        if (renderPathID == 1 || renderPathID == 3)
                                            tes.renderPath = (RenderingPath)renderPathID;

                                        break;
                                    case "Shader":
                                        switch (value)
                                        {
                                            case "Default": tes.materialType = TESUnity.MWMaterialType.Default; break;
                                            case "Standard": tes.materialType = TESUnity.MWMaterialType.Standard; break;
                                            case "Unlit": tes.materialType = TESUnity.MWMaterialType.Unlit; break;
                                            default: tes.materialType = TESUnity.MWMaterialType.BumpedDiffuse; break;
                                        }
                                        break;
                                }
                            }

                            line = stream.ReadLine();
                        }
                        stream.Close();
                    }
                }

                savedData.Close();
            }
        }
    }
}