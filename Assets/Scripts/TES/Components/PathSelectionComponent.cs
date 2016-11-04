using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TESUnity.Components
{
    public class PathSelectionComponent : MonoBehaviour
    {
        [SerializeField]
        private InputField _path = null;
        [SerializeField]
        private Button _button = null;
        [SerializeField]
        private Text _infoMessage = null;

        private void Awake()
        {
            var savedPath = GameSettings.GetDataPath();
            if (savedPath != string.Empty)
                StartCoroutine(LoadWorld(savedPath));
        }

        public void LoadWorld()
        {
            var path = _path.text;

            if (GameSettings.IsValidPath(path))
            {
                GameSettings.CreateConfigFile();
                GameSettings.SetDataPath(path);

                StartCoroutine(LoadWorld(_path.text, false));
            }
            else
                StartCoroutine(ShowErrorMessage("This path is not valid."));

        }

        private IEnumerator LoadWorld(string path, bool checkPath = true)
        {
            if (!checkPath || GameSettings.IsValidPath(path))
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
    }
}