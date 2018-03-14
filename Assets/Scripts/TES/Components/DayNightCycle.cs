using UnityEngine;

namespace TESUnity.Components
{
    public class DayNightCycle : MonoBehaviour
    {
        private Transform m_Transform = null;
        private Quaternion m_OriginalOrientation;

        [SerializeField]
        private float m_RotationTime = 0.5f;

        private void Start()
        {
            m_Transform = transform;
            m_OriginalOrientation = m_Transform.rotation;
            RenderSettings.sun = GetComponent<Light>();
        }

        private void Update()
        {
            m_Transform.Rotate(m_RotationTime * Time.deltaTime, 0.0f, 0.0f);
        }
    }
}
