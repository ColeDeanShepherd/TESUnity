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

        private void LoadWorld()
        {
            // TODO: Add the path to the ini file.
            StartCoroutine(LoadWorld(_path.text));
        }

        private IEnumerator LoadWorld(string path)
        {
            if (!path.Contains("Data Files"))
                path = Path.Combine(path, "Data Files");

            if (Directory.Exists(path))
            {
                _path.gameObject.SetActive(false);
                _button.gameObject.SetActive(false);
                _infoMessage.text = "Loading...";
                _infoMessage.enabled = true;

                var asyncOperation = SceneManager.LoadSceneAsync("Scene"); // TODO: Replace by Menu
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

        /// <summary>
        /// Checks if a file named Config.ini is located left to the main executable.
        /// Open/Parse it and configure default values.
        /// </summary>
        private string CheckForPath()
        {
            var path = string.Empty;

            if (!File.Exists("config.ini"))
            {
                CreateINIFile();
                return string.Empty;
            }

            using (var savedData = File.OpenText("config.ini"))
            {
                var text = savedData.ReadToEnd();

                if (text != string.Empty)
                {
                    using (var stream = new StringReader(text))
                    {
                        var line = stream.ReadLine();
                        var temp = new string[2];

                        while (path == string.Empty)
                        {
                            temp = line.Split('=');

                            if (temp.Length == 2 && temp[0].Trim() == "MorrowindPath")
                                path = temp[1].Trim();

                            line = stream.ReadLine();
                        }
                        stream.Close();
                    }
                }

                savedData.Close();
            }

            return path;
        }

        private static void CreateINIFile()
        {
            var sb = new StringBuilder();
            sb.Append("# TESUnity Configuration File\r\n");
            sb.Append("\r\n");

            sb.Append("[Global]\r\n");
            sb.Append("PlayMusic = True\r\n");
            sb.Append("MorrowindPath = \r\n");
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