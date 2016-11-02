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

                            if (temp.Length == 2 && temp[0].Trim() == "MorrowindDataPath")
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
            sb.Append("# TESUnity Configuration File\n");
            sb.Append("\n");

            sb.Append("[Global]\n");
            sb.Append("PlayMusic = True");
            sb.Append("MorrowindPath = \n");
            sb.Append("\n");

            sb.Append("[Rendering]\n");
            sb.Append("RenderPath = 1\n");
            sb.Append("Shader = Standard\n");
            sb.Append("\n");

            sb.Append("[Lighting]\n");
            sb.Append("AnimateLights = False\n");
            sb.Append("SunShadows = True\n");
            sb.Append("LightShadows = True\n");
            sb.Append("RenderExteriorCellLights = False\n");
            sb.Append("\n");

            sb.Append("[Effects]\n");
            sb.Append("AntiAliasing = False\n");
            sb.Append("AmbientOcclusion = False\n");
            sb.Append("Bloom = True\n");
            sb.Append("WaterBackSideTransparent = False\n");
            sb.Append("\n");

            sb.Append("[VR]\n");
            sb.Append("FollowHeadDirection = False\n");
            sb.Append("DirectModePreview = False\n");
            sb.Append("\n");

            sb.Append("[Debug]\n");
            sb.Append("CreaturesEnabled = True\n");

            File.WriteAllText(ConfigFile, sb.ToString());
        }
    }
}