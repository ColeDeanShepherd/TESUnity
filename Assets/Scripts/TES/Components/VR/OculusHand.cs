#if OCULUS
using UnityEngine;

namespace TESUnity.Components.VR
{
    public class OculusHand : MonoBehaviour
    {
        private Transform _transform = null;

        public OVRInput.Controller TouchID { get; set; }

        void Start()
        {
            _transform = GetComponent<Transform>();

            // Add Input Binding

            // TODO: Wait for Oculus Avatar to display hands.
        }

        void Update()
        {
            _transform.localPosition = OVRInput.GetLocalControllerPosition(TouchID);
            _transform.localRotation = OVRInput.GetLocalControllerRotation(TouchID);
        }
    }
}
#endif