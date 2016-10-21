using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TESUnity.Components
{
    /// <summary>
    /// Deprecated.
    /// </summary>
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

#if UNITY_EDITOR
        [SerializeField]
        private bool bypassConfigINI = false;
#endif

        private void Start()
        {
#if UNITY_EDITOR
            if (!bypassConfigINI)
                CheckConfigINI();
#else
            CheckConfigINI();
#endif
            var savedPath = PlayerPrefs.GetString(SavePathKey, string.Empty);
            if (savedPath != string.Empty)
                defaultMWDataPath = savedPath;

            camera = GameObjectUtils.CreateMainCamera(Vector3.zero, Quaternion.identity);
            eventSystem = GUIUtils.CreateEventSystem();
            canvas = GUIUtils.CreateCanvas();

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

            // Load the game if the Data Files folder is here
            var path = Path.Combine(System.Environment.CurrentDirectory, "Data Files");
            if (Directory.Exists(path))
            {
                LoadWorld(path);
                return;
            }

            // Or if it's already in the config.ini file.
            var tes = GetComponent<TESUnity>();
            if (Directory.Exists(tes.dataPath))
            {
                LoadWorld(tes.dataPath);
                return;
            }
        }

        private void OnDestroy()
        {
            Destroy(canvas);
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
            if (!File.Exists("config.ini"))
                return;

            using (var savedData = File.OpenText("config.ini"))
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
                                    case "AntiAliasing": tes.antiAliasing = ParseBool(value, tes.antiAliasing); break;
                                    case "AmbientOcclusion": tes.ambientOcclusion = ParseBool(value, tes.ambientOcclusion); break;
                                    case "AnimateLights": tes.animateLights = ParseBool(value, tes.animateLights); break;
                                    case "Bloom": tes.bloom = ParseBool(value, tes.bloom); break;
                                    case "MorrowindPath": tes.dataPath = value; break;
                                    case "FollowHeadDirection": tes.followHeadDirection = ParseBool(value, tes.followHeadDirection); break;
                                    case "DirectModePreview": tes.directModePreview = ParseBool(value, tes.directModePreview); break;
                                    case "SunShadows": tes.renderSunShadows = ParseBool(value, tes.renderSunShadows); break;
                                    case "LightShadows": tes.renderLightShadows = ParseBool(value, tes.renderLightShadows); break;
                                    case "PlayMusic": tes.playMusic = ParseBool(value, tes.playMusic); break;
                                    case "RenderExteriorCellLights": tes.renderExteriorCellLights = ParseBool(value, tes.renderExteriorCellLights); break;
                                    case "WaterBackSideTransparent": tes.waterBackSideTransparent = ParseBool(value, tes.waterBackSideTransparent); break;
                                    case "RenderPath":
                                        var renderPathID = ParseInt(value, 0);
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

        private bool ParseBool(string value, bool defaultValue)
        {
            bool result;

            if (bool.TryParse(value, out result))
                return result;

            return defaultValue;
        }

        private int ParseInt(string value, int defaultValue)
        {
            int result;

            if (int.TryParse(value, out result))
                return result;

            return defaultValue;
        }
    }
}