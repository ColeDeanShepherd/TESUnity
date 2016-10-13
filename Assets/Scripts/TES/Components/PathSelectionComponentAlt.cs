using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace TESUnity.Components
{
    public class PathSelectionComponentAlt : MonoBehaviour
    {
        private static readonly string SavePathKey = "keepMWPath";
        private string defaultMWDataPath = "C:/Program Files (x86)/Steam/steamapps/common/Morrowind/Data Files";

        [SerializeField]
        private InputField _path;
        [SerializeField]
        private Toggle _keepPath;
        [SerializeField]
        private Text _errorText;

        public TESUnity TESUnity
        {
            get
            {
                if (TESUnity.instance != null)
                    return TESUnity.instance;

                var tes = FindObjectOfType<TESUnity>();
                if (tes == null)
                {
                    var go = new GameObject("TESUnity");
                    tes = go.AddComponent<TESUnity>();
                    tes.enabled = false;
                }

                return tes;
            }
        }

        private void Start()
        {
            CheckConfigINI();

            var savedPath = PlayerPrefs.GetString(SavePathKey, string.Empty);
            if (savedPath != string.Empty)
                defaultMWDataPath = savedPath;

            var path = Path.Combine(System.Environment.CurrentDirectory, "Data Files");
            if (Directory.Exists(path))
            {
                LoadWorld(path);
                return;
            }
        }

        private void LoadWorld()
        {
            var path = _path.text;

            if (_keepPath.isOn)
                PlayerPrefs.SetString(SavePathKey, path);

            LoadWorld(path);
        }

        private void LoadWorld(string path)
        {
            if (Directory.Exists(path))
            {
                TESUnity.dataPath = path;
                TESUnity.enabled = true;
                Destroy(this);
            }
            else
                StartCoroutine(ShowErrorMessage("Invalid path."));
        }

        private IEnumerator ShowErrorMessage(string message)
        {
            _errorText.text = message;
            _errorText.enabled = true;
            yield return new WaitForSeconds(5.0f);
            _errorText.enabled = false;
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
                        var tes = TESUnity;
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
                                    case "FollowHeadDirection": tes.followHeadDirection = ParseBool(value, tes.followHeadDirection); break;
                                    case "SunShadows": tes.renderSunShadows = ParseBool(value, tes.renderSunShadows); break;
                                    case "LightShadows": tes.renderLightShadows = ParseBool(value, tes.renderLightShadows); break;
                                    case "PlayMusic": tes.playMusic = ParseBool(value, tes.playMusic); break;
                                    case "RenderExteriorCellLights": tes.renderExteriorCellLights = ParseBool(value, tes.renderExteriorCellLights); break;
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