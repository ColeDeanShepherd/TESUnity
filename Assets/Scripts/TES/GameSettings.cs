using System.IO;
using System.Text;

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
                if (lines[i].Contains(parameter))
                    lines[i] = string.Format("{0} = {1}", parameter, value);

            File.WriteAllLines(ConfigFile, lines);
        }

        public static bool IsValidPath(string path)
        {
            return File.Exists(Path.Combine(path, "Morrowind.esm"));
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

        private static void CreateINIFile()
        {
            var sb = new StringBuilder();
            sb.Append("# TESUnity Configuration File\r\n");
            sb.Append("\r\n");

            sb.Append("[Global]\r\n");
            sb.Append("PlayMusic = True\r\n");
            sb.Append(string.Format("{0} = \r\n", MWDataPathName));
            sb.Append("\r\n");

            sb.Append("[Rendering]\r\n");
            sb.Append("RenderPath = 1\r\n");
            sb.Append("Shader = Standard\r\n");
            sb.Append("\r\n");

            sb.Append("[Lighting]\r\n");
            sb.Append("AnimateLights = False\r\n");
            sb.Append("SunShadows = True\r\n");
            sb.Append("LightShadows = True\r\n");
            sb.Append("RenderExteriorCellLights = False\r\n");
            sb.Append("\r\n");

            sb.Append("[Effects]\r\n");
            sb.Append("AntiAliasing = False\r\n");
            sb.Append("AmbientOcclusion = False\r\n");
            sb.Append("Bloom = True\r\n");
            sb.Append("WaterBackSideTransparent = False\r\n");
            sb.Append("\r\n");

            sb.Append("[VR]\r\n");
            sb.Append("FollowHeadDirection = False\r\n");
            sb.Append("DirectModePreview = False\r\n");
            sb.Append("\r\n");

            sb.Append("[Debug]\r\n");
            sb.Append("CreaturesEnabled = True\r\n");

            File.WriteAllText(ConfigFile, sb.ToString());
        }
    }
}
