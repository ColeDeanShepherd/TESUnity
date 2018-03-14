using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace TESUnity
{
    public class GameSettings
    {
        private static readonly string ConfigFile = "config.ini";
        private static readonly string MWDataPathName = "MorrowindDataPath";

        public static void SaveValue(string parameter, string value)
        {
            var lines = File.ReadAllLines(ConfigFile);

            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(parameter))
                {
                    lines[i] = string.Format("{0} = {1}", parameter, value);
                    break;
                }
            }

            File.WriteAllLines(ConfigFile, lines);
        }

        public static bool IsValidPath(string path)
        {
            return File.Exists(Path.Combine(path, "Morrowind.esm"));
        }

        public static void SetDataPath(string dataPath)
        {
            SaveValue(MWDataPathName, dataPath);
        }

        public static string GetDataPath()
        {
            if (!File.Exists("config.ini"))
                return string.Empty;

            var lines = File.ReadAllLines(ConfigFile);
            foreach (var line in lines)
            {
                if (line.Contains(MWDataPathName))
                {
                    var tmp = line.Split('=');
                    if (tmp.Length == 2)
                        return tmp[1].Trim();
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Checks if a file named Config.ini is located left to the main executable.
        /// Open/Parse it and configure default values.
        /// </summary>
        public static string CheckSettings(TESUnity tes)
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
                        case "AntiAliasing":
                            {
                                int result;
                                if (int.TryParse(value, out result))
                                {
                                    if (result >= 0 && result < 4)
                                        tes.antiAliasing = (PostProcessLayer.Antialiasing)result;
                                }
                            }
                            break;
                        case "PostProcessQuality":
                            {
                                int result;
                                if (int.TryParse(value, out result))
                                {
                                    if (result >= 0 && result < 4)
                                        tes.postProcessingQuality = (TESUnity.PostProcessingQuality)result;
                                }
                            }
                            break;
                        case "AnimateLights": tes.animateLights = ParseBool(value, tes.animateLights); break;
                        case "MorrowindDataPath": path = value; break;
                        case "FollowHeadDirection": tes.followHeadDirection = ParseBool(value, tes.followHeadDirection); break;
                        case "DirectModePreview": tes.directModePreview = ParseBool(value, tes.directModePreview); break;
                        case "SunShadows": tes.renderSunShadows = ParseBool(value, tes.renderSunShadows); break;
                        case "LightShadows": tes.renderLightShadows = ParseBool(value, tes.renderLightShadows); break;
                        case "PlayMusic": tes.playMusic = ParseBool(value, tes.playMusic); break;
                        case "RenderExteriorCellLights": tes.renderExteriorCellLights = ParseBool(value, tes.renderExteriorCellLights); break;
                        case "WaterBackSideTransparent": tes.waterBackSideTransparent = ParseBool(value, tes.waterBackSideTransparent); break;
                        case "RenderPath":
                            tes.renderPath = value == "Forward" ? RenderingPath.Forward : RenderingPath.DeferredShading;
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
                        case "RoomScale": tes.roomScale = ParseBool(value, tes.roomScale); break;
                        case "ForceControllers": tes.forceControllers = ParseBool(value, tes.forceControllers); break;
                        case "CreaturesEnabled": tes.creaturesEnabled = ParseBool(value, tes.creaturesEnabled); break;
                        case "CameraFarClip": tes.cameraFarClip = ParseFloat(value, tes.cameraFarClip); break;
                        case "XRVignette": tes.useXRVignette = ParseBool(value, tes.useXRVignette); break;
                        case "WaterQuality":
                            {
                                int result;
                                if (int.TryParse(value, out result))
                                {
                                    if (result > -1 && result < 3)
                                        tes.waterQuality = (UnityStandardAssets.Water.Water.WaterMode)result;
                                }
                            }
                            break;

                        case "DayNightCycle": tes.dayNightCycle = ParseBool(value, tes.dayNightCycle); break;
                        case "GenerateNormalMap": tes.generateNormalMap = ParseBool(value, tes.generateNormalMap); break;
                        case "NormalGeneratorIntensity": tes.normalGeneratorIntensity = ParseFloat(value, tes.normalGeneratorIntensity); break;

                        default: break;
                    }
                }
            }

            return path;
        }

        public static void CreateConfigFile()
        {
            var sb = new StringBuilder();
            sb.Append("# TESUnity Configuration File\r\n");
            sb.Append("\r\n");

            sb.Append("[Global]\r\n");
            sb.Append("PlayMusic = True\r\n");
            sb.Append(string.Format("{0} = \r\n", MWDataPathName));
            sb.Append("\r\n");

            sb.Append("[Rendering]\r\n");
            sb.Append("RenderPath = Deferred\r\n");
            sb.Append("Shader = Standard\r\n");
            sb.Append("CameraFarClip = 500\r\n");
            sb.Append("WaterQuality = 0\r\n");
            sb.Append("\r\n");

            sb.Append("[Lighting]\r\n");
            sb.Append("AnimateLights = True\r\n");
            sb.Append("SunShadows = True\r\n");
            sb.Append("LightShadows = False\r\n");
            sb.Append("RenderExteriorCellLights = True\r\n");
            sb.Append("DayNightCycle = True\r\n");
            sb.Append("GenerateNormalMap = True\r\n");
            sb.Append("NormalGeneratorIntensity = 0.75\r\n");
            sb.Append("\r\n");

            sb.Append("[Effects]\r\n");
            sb.Append("AntiAliasing = True\r\n");
            sb.Append("PostProcessQuality = 3\r\n");
            sb.Append("WaterBackSideTransparent = False\r\n");
            sb.Append("\r\n");

            sb.Append("[VR]\r\n");
            sb.Append("FollowHeadDirection = True\r\n");
            sb.Append("RoomScale = False\r\n");
            sb.Append("ForceControllers = True\r\n");
            sb.Append("XRVignette = False\r\n");
            sb.Append("\r\n");

            sb.Append("[Debug]\r\n");
            sb.Append("CreaturesEnabled = False\r\n");

            File.WriteAllText(ConfigFile, sb.ToString());
        }

        private static bool ParseBool(string value, bool defaultValue)
        {
            var val = value.ToLower();
            if (val == "true" || val == "1")
                return true;
            else if (val == "false" || value == "0")
                return false;

            return defaultValue;
        }

        private static int ParseInt(string value, int defaultValue)
        {
            int result;

            if (int.TryParse(value, out result))
                return result;

            return defaultValue;
        }

        private static float ParseFloat(string value, float defaultValue)
        {
            float result;

            if (float.TryParse(value, out result))
                return result;

            return defaultValue;
        }
    }
}
