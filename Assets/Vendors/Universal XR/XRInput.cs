using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace Demonixis.Toolbox.XR
{
    public enum XRButton
    {
        Menu,
        Button1,
        Button2,
        Button3,
        Button4,
        Thumbstick,
        ThumbstickTouch,
        SecondaryTouchpad,
        SecondaryTouchpadTouch,
        Trigger,
        Grip,
        ThumbstickUp,
        ThumbstickDown,
        ThumbstickLeft,
        ThumbstickRight
    }

    public enum XRAxis
    {
        Trigger,
        Grip,
        ThumbstickX,
        ThumbstickY,
        SecondaryTouchpadX,
        SecondaryTouchpadY
    }

    public enum XRAxis2D
    {
        Thumbstick,
        SecondaryTouchpad
    }

    public enum XRVendor
    {
        None = 0, Oculus, OpenVR, WindowsMR
    }

    public sealed class XRInput : MonoBehaviour
    {
        private static XRInput s_Instance = null;
        private Vector2 m_TmpVector = Vector2.zero;
        private List<XRNodeState> m_XRNodeStates = new List<XRNodeState>();
        private XRButton[] m_Buttons = null;
        private XRVendor m_InputVendor = XRVendor.None;
        private bool m_Running = true;
        private bool[] m_AxisStates = null;

        [SerializeField]
        private float m_DeadZone = 0.1f;

        public static XRInput Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    var gameObject = new GameObject("VRInput");
                    s_Instance = gameObject.AddComponent<XRInput>();
                }

                return s_Instance;
            }
        }

        public XRVendor Vendor
        {
            get { return m_InputVendor; }
        }

        public bool IsConnected
        {
            get
            {
                m_XRNodeStates.Clear();
                InputTracking.GetNodeStates(m_XRNodeStates);

                var left = false;
                var right = false;

                foreach (var state in m_XRNodeStates)
                {
                    if (state.nodeType == XRNode.LeftHand)
                        left = state.tracked;

                    else if (state.nodeType == XRNode.RightHand)
                        right = state.tracked;
                }

                return left && right;
            }
        }

        public float DeadZone
        {
            get { return m_DeadZone; }
            set
            {
                m_DeadZone = value;

                if (m_DeadZone < 0)
                    m_DeadZone = 0.0f;
                else if (m_DeadZone >= 1.0f)
                    m_DeadZone = 0.9f;
            }
        }

        private delegate float GetAxisFunction(string axis);

        public void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(this);
                return;
            }

            var vendor = XRSettings.loadedDeviceName;
            if (vendor == "Oculus")
                m_InputVendor = XRVendor.Oculus;
            else if (vendor == "Openvr")
                m_InputVendor = XRVendor.OpenVR;
            else if (vendor == "Windowsmr")
                m_InputVendor = XRVendor.WindowsMR;

            m_Buttons = new XRButton[]
            {
                XRButton.Grip, XRButton.Trigger,
                XRButton.ThumbstickUp, XRButton.ThumbstickDown,
                XRButton.ThumbstickLeft, XRButton.ThumbstickRight
            };

            m_AxisStates = new bool[m_Buttons.Length * 2];

            StartCoroutine(UpdateAxisToButton());
        }

        private void OnDestroy()
        {
            m_Running = false;
        }

        private IEnumerator UpdateAxisToButton()
        {
            var endOfFrame = new WaitForEndOfFrame();
            var index = 0;

            while (m_Running)
            {
                index = 0;

                for (var i = 0; i < m_Buttons.Length; i++)
                {
                    m_AxisStates[index] = GetButton(m_Buttons[i], true);
                    m_AxisStates[index + 1] = GetButton(m_Buttons[i], false);
                    index += 2;
                }

                yield return endOfFrame;
            }
        }

        /// <summary>
        /// Gets the position of a specific node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Vector3 GetLocalPosition(XRNode node)
        {
            return InputTracking.GetLocalPosition(node);
        }

        /// <summary>
        /// Gets the rotation of a specific node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Quaternion GetLocalRotation(XRNode node)
        {
            return InputTracking.GetLocalRotation(node);
        }


        #region Methods to get button states

        /// <summary>
        /// Indicates whether a button is pressed.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns true if pressed otherwise it returns false.</returns>
        public bool GetButton(XRButton button, bool left)
        {
            if (button == XRButton.Menu)
            {
                if (m_InputVendor == XRVendor.OpenVR)
                    return Input.GetButton(left ? "Button 2" : "Button 0");
                else if (m_InputVendor == XRVendor.WindowsMR)
                    return Input.GetButton(left ? "Button 6" : "Button 7");

                return Input.GetButton("Button 7");
            }

            else if (button == XRButton.Button1)
                return Input.GetButton("Button 0");

            else if (button == XRButton.Button2)
                return Input.GetButton("Button 1");

            else if (button == XRButton.Button3)
                return Input.GetButton("Button 2");

            else if (button == XRButton.Button4)
                return Input.GetButton("Button 3");

            else if (button == XRButton.Thumbstick)
                return Input.GetButton(left ? "Button 8" : "Button 9");

            else if (button == XRButton.ThumbstickTouch)
            {
                if (m_InputVendor == XRVendor.WindowsMR)
                    return Input.GetButton(left ? "Button 18" : "19");
                else
                    return Input.GetButton(left ? "Button 16" : "17");
            }

            else if (button == XRButton.SecondaryTouchpad)
                return Input.GetButton(left ? "Button 16" : "17");

            else if (button == XRButton.SecondaryTouchpad)
                return Input.GetButton(left ? "Button 18" : "19");

            else if (button == XRButton.Trigger)
                return GetRawAxis(XRAxis.Trigger, left) > m_DeadZone;

            else if (button == XRButton.Grip)
                return GetRawAxis(XRAxis.Grip, left) > m_DeadZone;

            else if (button == XRButton.ThumbstickUp)
                return GetRawAxis(XRAxis.ThumbstickY, left) > m_DeadZone;

            else if (button == XRButton.ThumbstickDown)
                return GetRawAxis(XRAxis.ThumbstickY, left) < m_DeadZone * -1.0f;

            else if (button == XRButton.ThumbstickLeft)
                return GetRawAxis(XRAxis.ThumbstickX, left) < m_DeadZone * -1.0f;

            else if (button == XRButton.ThumbstickRight)
                return GetRawAxis(XRAxis.ThumbstickX, left) > m_DeadZone;

            return false;
        }

        /// <summary>
        /// Indicates whether a button was pressed.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns true if pressed otherwise it returns false.</returns>
        public bool GetButtonDown(XRButton button, bool left)
        {
            if (button == XRButton.Menu)
            {
                if (m_InputVendor == XRVendor.OpenVR)
                    return Input.GetButtonDown(left ? "Button 2" : "Button 0");
                else if (m_InputVendor == XRVendor.WindowsMR)
                    return Input.GetButtonDown(left ? "Button 6" : "Button 7");

                return Input.GetButtonDown("Button 7");
            }

            else if (button == XRButton.Button1)
                return Input.GetButtonDown("Button 0");

            else if (button == XRButton.Button2)
                return Input.GetButtonDown("Button 1");

            else if (button == XRButton.Button3)
                return Input.GetButtonDown("Button 2");

            else if (button == XRButton.Button4)
                return Input.GetButtonDown("Button 3");

            else if (button == XRButton.Thumbstick)
                return Input.GetButtonDown(left ? "Button 8" : "Button 9");

            else if (button == XRButton.ThumbstickTouch)
            {
                if (m_InputVendor == XRVendor.WindowsMR)
                    return Input.GetButtonDown(left ? "Button 18" : "19");
                else
                    return Input.GetButtonDown(left ? "Button 16" : "17");
            }

            // Simulate other buttons using previous states.
            var index = 0;
            for (var i = 0; i < m_Buttons.Length; i++)
            {
                if (m_Buttons[i] != button)
                {
                    index += 2;
                    continue;
                }

                var prev = m_AxisStates[left ? index : index + 1];
                var now = GetButton(m_Buttons[i], left);

                return now && !prev;
            }

            return false;
        }

        /// <summary>
        /// Indicates whether a button was released.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns true if pressed otherwise it returns false.</returns>
        public bool GetButtonUp(XRButton button, bool left)
        {
            if (button == XRButton.Menu)
            {
                if (m_InputVendor == XRVendor.OpenVR)
                    return Input.GetButtonUp(left ? "Button 2" : "Button 0");
                else if (m_InputVendor == XRVendor.WindowsMR)
                    return Input.GetButtonUp(left ? "Button 6" : "Button 7");

                return Input.GetButtonUp("Button 7");
            }

            else if (button == XRButton.Button1)
                return Input.GetButtonUp("Button 0");

            else if (button == XRButton.Button2)
                return Input.GetButtonUp("Button 1");

            else if (button == XRButton.Button3)
                return Input.GetButtonUp("Button 2");

            else if (button == XRButton.Button4)
                return Input.GetButtonUp("Button 3");

            else if (button == XRButton.Thumbstick)
                return Input.GetButtonUp(left ? "Button 8" : "Button 9");

            else if (button == XRButton.ThumbstickTouch)
            {
                if (m_InputVendor == XRVendor.WindowsMR)
                    return Input.GetButtonUp(left ? "Button 18" : "19");
                else
                    return Input.GetButtonUp(left ? "Button 16" : "17");
            }

            // Simulate other buttons using previous states.
            var index = 0;
            for (var i = 0; i < m_Buttons.Length; i++)
            {
                if (m_Buttons[i] != button)
                {
                    index += 2;
                    continue;
                }

                var prev = m_AxisStates[left ? index : index + 1];
                var now = GetButton(m_Buttons[i], left);

                return !now && prev;
            }

            return false;
        }

        /// <summary>
        /// Indicates if the button is pressed on the left or right controller.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button is pressed on the left or right controller.</returns>
        public bool GetAnyButton(XRButton button)
        {
            return GetButton(button, false) || GetButton(button, true);
        }

        /// <summary>
        /// Indicates if the button is pressed on both controllers.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button is pressed on both left and right controllers.</returns>
        public bool GetBothButtons(XRButton button)
        {
            return GetButton(button, false) && GetButton(button, true);
        }

        /// <summary>
        /// Indicates if the button was pressed on the left or right controller.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button was pressed on the left or right controller.</returns>
        public bool GetAnyButtonDown(XRButton button)
        {
            return GetButtonDown(button, false) || GetButtonDown(button, true);
        }

        /// <summary>
        /// Indicates if the button was pressed on both controllers.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button was pressed on both left and right controllers.</returns>
        public bool GetBothButtonsDown(XRButton button)
        {
            return GetButtonDown(button, false) && GetButtonDown(button, true);
        }

        /// <summary>
        /// Indicates if the button was released on the left or right controllers.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button was released on the left or right controller.</returns>
        public bool GetAnyButtonUp(XRButton button)
        {
            return GetButtonUp(button, false) || GetButtonUp(button, true);
        }

        /// <summary>
        /// Indicates if the button was just released on both controllers.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button was just released on both controllers.</returns>
        public bool GetBothButtonsUp(XRButton button)
        {
            return GetButtonUp(button, false) && GetButtonUp(button, true);
        }

        #endregion

        #region Methods to get axis state

        private float GetAxis(GetAxisFunction axisFunction, XRAxis axis, bool left)
        {
            if (axis == XRAxis.Trigger)
                return axisFunction(left ? "Axis 9" : "Axis 10");

            else if (axis == XRAxis.Grip)
                return axisFunction(left ? "Axis 11" : "Axis 12");

            else if (axis == XRAxis.ThumbstickX)
                return axisFunction(left ? "Axis 1" : "Axis 4");

            else if (axis == XRAxis.ThumbstickY)
                return axisFunction(left ? "Axis 2" : "Axis 5");

            else if (axis == XRAxis.SecondaryTouchpadX)
                return axisFunction(left ? "Axis 17" : "Axis 20");

            else if (axis == XRAxis.SecondaryTouchpadY)
                return axisFunction(left ? "Axis 18" : "Axis 21");

            return 0.0f;
        }

        private Vector2 GetAxis2D(GetAxisFunction axisFunction, XRAxis2D axis, bool left)
        {
            m_TmpVector.x = 0;
            m_TmpVector.y = 0;

            if (axis == XRAxis2D.Thumbstick)
            {
                m_TmpVector.x = axisFunction(left ? "Axis 1" : "Axis 4");
                m_TmpVector.y = axisFunction(left ? "Axis 2" : "Axis 5");
            }
            else if (axis == XRAxis2D.SecondaryTouchpad)
            {
                m_TmpVector.x = axisFunction(left ? "Axis 17" : "Axis 20");
                m_TmpVector.y = axisFunction(left ? "Axis 18" : "Axis 21");
            }

            return m_TmpVector;
        }

        /// <summary>
        /// Gets an axis value.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns the axis value.</returns>
        public float GetAxis(XRAxis axis, bool left)
        {
            return GetAxis(Input.GetAxis, axis, left);
        }

        /// <summary>
        /// Gets a raw axis value.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns the axis value.</returns>
        public float GetRawAxis(XRAxis axis, bool left)
        {
            return GetAxis(Input.GetAxisRaw, axis, left);
        }

        /// <summary>
        /// Gets two axis values.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns two axis values.</returns>
        public Vector2 GetAxis2D(XRAxis2D axis, bool left)
        {
            return GetAxis2D(Input.GetAxis, axis, left);
        }

        /// <summary>
        /// Gets two raw axis values.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns two axis values.</returns>
        public Vector2 GetRawAxis2D(XRAxis2D axis, bool left)
        {
            return GetAxis2D(Input.GetAxis, axis, left);
        }

        #endregion
    }
}