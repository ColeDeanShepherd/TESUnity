using System;
using System.IO;
using UnityEngine;

namespace TESUnity.Components
{
    public sealed class ScreenshotCapturer : MonoBehaviour
    {
        [SerializeField]
        private KeyCode m_ScreenshotKey = KeyCode.F1;
        [SerializeField]
        private int m_ScreenshotSuperSampling = 1;

        private void Update()
        {
            if (Input.GetKeyDown(m_ScreenshotKey))
                CaptureScreenshot();
        }

        private string GetSavePath(string folder)
        {
            var path = string.Format("{0}/..", Application.dataPath);

            if (folder != string.Empty)
                path = string.Format("{0}/{1}", path, folder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public void CaptureScreenshot()
        {
            var name = string.Format("{0}_{1}.png", Application.productName, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            var folder = GetSavePath("Screenshots");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            ScreenCapture.CaptureScreenshot(Path.Combine(folder, name), m_ScreenshotSuperSampling);
        }
    }
}