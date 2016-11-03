using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TESUnity
{
    /// <summary>
    /// This component is responsible to start the engine.
    /// </summary>
    public class Boostrap : MonoBehaviour
    {
        private static readonly string ConfigFile = "config.ini";

        [SerializeField]
        private GameObject _mainUI = null;

        void Awake()
        {
            var tes = GetComponent<TESUnity>();
            tes.LoadingCompleted += OnEngineLoaded;

            // We need to do that in the menu and use it only in the editor.
            var path = CheckConfigINI(tes);

            if (!File.Exists(Path.Combine(path, "Morrowind.esm")))
            {
                var lines = File.ReadAllLines(ConfigFile);
                for (var i = 0; i < lines.Length; i++)
                    if (lines[i].Contains("MorrowindDataPath"))
                        lines[i] = string.Format("MorrowindDataPath = ");

                File.WriteAllLines(ConfigFile, lines);
            }

            tes.dataPath = path;
            tes.enabled = true;
        }

        private void OnEngineLoaded()
        {
            _mainUI.SetActive(true);
            Destroy(this);
        }

        /// <summary>
        /// Checks if a file named Config.ini is located left to the main executable.
        /// Open/Parse it and configure default values.
        /// </summary>
        private string CheckConfigINI(TESUnity tes)
        {
            var path = string.Empty;
            var lines = File.ReadAllLines(ConfigFile);
            var temp = new string[2];
            var value = string.Empty;

            foreach (var line in lines)
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
                        case "MorrowindDataPath": path = value; break;
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
            }

            return path;
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