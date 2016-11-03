using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TESUnity.Components
{
    public class PathSelectionComponentAlt : MonoBehaviour
    {
        private static readonly string ConfigFile = "config.ini";
        private static readonly string MWDataPathName = "MorrowindDataPath";

        [SerializeField]
        private InputField _path = null;
        [SerializeField]
        private Button _button = null;
        [SerializeField]
        private Text _infoMessage = null;

        private void Awake()
        {
            var savedPath = CheckForPath();
            if (savedPath != string.Empty)
                StartCoroutine(LoadWorld(savedPath));
        }

        public void LoadWorld()
        {
            var path = _path.text;

            if (IsValidPath(path))
            {
                CreateINIFile();

                var lines = File.ReadAllLines(ConfigFile);
                for (var i = 0; i < lines.Length; i++)
                    if (lines[i].Contains(MWDataPathName))
                        lines[i] = string.Format("{0} = {1}", MWDataPathName, path);

                File.WriteAllLines(ConfigFile, lines);

                StartCoroutine(LoadWorld(_path.text));
            }
            else
                StartCoroutine(ShowErrorMessage("This path is not valid."));

        }

        private IEnumerator LoadWorld(string path)
        {
            if (IsValidPath(path))
            {
                _path.gameObject.SetActive(false);
                _button.gameObject.SetActive(false);
                _infoMessage.text = "Loading...";
                _infoMessage.enabled = true;
                
                var asyncOperation = SceneManager.LoadSceneAsync("Scene");
                var waitForSeconds = new WaitForSeconds(0.1f);

                while (!asyncOperation.isDone)
                {
                    _infoMessage.text = string.Format("Loading {0}%", Mathf.RoundToInt(asyncOperation.progress * 100.0f));
                    yield return waitForSeconds;
                }
            }
            else
                StartCoroutine(ShowErrorMessage("Invalid path."));
        }

        private IEnumerator ShowErrorMessage(string message)
        {
            _infoMessage.text = message;
            _infoMessage.enabled = true;
            yield return new WaitForSeconds(5.0f);
            _infoMessage.enabled = false;
        }

        private bool IsValidPath(string path)
        {
            return File.Exists(Path.Combine(path, "Morrowind.esm"));
        }

        /// <summary>
        /// Checks if a file named Config.ini is located left to the main executable.
        /// Open/Parse it and configure default values.
        /// </summary>
        private string CheckForPath()
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