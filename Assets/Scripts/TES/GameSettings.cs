﻿using System.IO;
using System.Text;
using UnityEngine;

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

        public static void CreateConfigFile()
        {
            var sb = new StringBuilder();
            sb.Append("# TESUnity Configuration File\r\n");
            sb.Append("\r\n");

            sb.Append("[Global]\r\n");
            sb.Append("PlayMusic = \r\n");
            sb.Append(string.Format("{0} = \r\n", MWDataPathName));
            sb.Append("\r\n");

            sb.Append("[Rendering]\r\n");
            sb.Append("RenderPath = \r\n");
            sb.Append("Shader = \r\n");
            sb.Append("\r\n");

            sb.Append("[Lighting]\r\n");
            sb.Append("AnimateLights = \r\n");
            sb.Append("SunShadows = \r\n");
            sb.Append("LightShadows = \r\n");
            sb.Append("RenderExteriorCellLights = \r\n");
            sb.Append("\r\n");

            sb.Append("[Effects]\r\n");
            sb.Append("AntiAliasing = \r\n");
            sb.Append("AmbientOcclusion = \r\n");
            sb.Append("Bloom = \r\n");
            sb.Append("WaterBackSideTransparent = \r\n");
            sb.Append("\r\n");

            sb.Append("[VR]\r\n");
            sb.Append("FollowHeadDirection = \r\n");
            sb.Append("DirectModePreview = \r\n");
            sb.Append("\r\n");

            sb.Append("[Debug]\r\n");
            sb.Append("CreaturesEnabled = \r\n");

            File.WriteAllText(ConfigFile, sb.ToString());
        }

        private static bool ParseBool(string value, bool defaultValue)
        {
            bool result;

            if (bool.TryParse(value, out result))
                return result;

            return defaultValue;
        }

        private static int ParseInt(string value, int defaultValue)
        {
            int result;

            if (int.TryParse(value, out result))
                return result;

            return defaultValue;
        }
    }
}
