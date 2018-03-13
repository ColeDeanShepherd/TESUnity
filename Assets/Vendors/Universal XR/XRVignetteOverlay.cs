using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.XR;

namespace Demonixis.Toolbox.XR
{
    public sealed class XRVignetteOverlay : MonoBehaviour
    {
        private Vignette m_Vignette = null;
        private Rigidbody m_Rigidbody = null;

        [SerializeField]
        private float m_Threshold = 0.5f;

        private void Start()
        {
            if (!XRSettings.enabled)
                Destroy(this);

            var volume = FindObjectOfType<PostProcessVolume>();
            var profile = volume.profile;
            profile.TryGetSettings(out m_Vignette);

            if (m_Vignette == null)
                Destroy(this);

            m_Vignette.enabled.value = false;
            m_Vignette.intensity.value = 1.5f;

            m_Rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            var show = m_Rigidbody.velocity.sqrMagnitude > m_Threshold || m_Rigidbody.angularVelocity.sqrMagnitude > m_Threshold;

            if (show && !m_Vignette.enabled.value)
                m_Vignette.enabled.value = true;
            else if (!show && m_Vignette.enabled.value)
                m_Vignette.enabled.value = false;
        }
    }
}