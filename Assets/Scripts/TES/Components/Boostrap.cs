using System;
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

#if UNITY_EDITOR
        [Header("Editor Only")]
        [SerializeField]
        private bool _bypassConfig = false;
#endif
        void Awake()
        {
            var tes = GetComponent<TESUnity>();

#if UNITY_EDITOR
            // We need to do that in the menu and use it only in the editor.
            var path = tes.dataPath;

            if (_bypassConfig)
                path = GameSettings.CheckSettings(tes);
#else
            var path = GameSettings.CheckSettings(tes);
#endif

            if (!GameSettings.IsValidPath(path))
            {
                GameSettings.SetDataPath(string.Empty);
                SceneManager.LoadScene("AskScene");
            }
            else
            {
                tes.LoadingCompleted += OnEngineLoaded;
                tes.dataPath = path;
                tes.enabled = true;
            }
        }

        private void OnEngineLoaded(object sender, EventArgs e)
        {
            ((TESUnity)sender).LoadingCompleted -= OnEngineLoaded;
            _mainUI.SetActive(true);
            Destroy(this);
        }
    }
}