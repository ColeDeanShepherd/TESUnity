﻿using TESUnity.Inputs;
using TESUnity.UI;
using UnityEngine;

namespace TESUnity
{
    public class PlayerComponent : MonoBehaviour
    {
        private Transform _camTransform;
        private Transform _transform;
        private CapsuleCollider _capsuleCollider;
        private Rigidbody _rigidbody;
        private UICrosshair _crosshair;
        private bool _paused = false;
        private bool _isGrounded = false;
        private bool _isFlying = false;

        #region Editor Fields

        [Header("Movement Settings")]
        public float slowSpeed = 3;
        public float normalSpeed = 5;
        public float fastSpeed = 10;
        public float flightSpeedMultiplier = 3;
        public float airborneForceMultiplier = 5;
        public float mouseSensitivity = 3;
        public float minVerticalAngle = -90;
        public float maxVerticalAngle = 90;

        [Header("Misc")]
        public Light lantern;
        public Transform leftHand;
        public Transform rightHand;

        #endregion

        #region Public Fields

        public bool isFlying
        {
            get { return _isFlying; }
            set
            {
                _isFlying = value;

                if (!_isFlying)
                    _rigidbody.useGravity = true;
                else
                    _rigidbody.useGravity = false;
            }
        }

        public bool Paused
        {
            get { return _paused; }
        }

        #endregion

        private void Start()
        {
            _transform = GetComponent<Transform>();
            _camTransform = Camera.main.GetComponent<Transform>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _rigidbody = GetComponent<Rigidbody>();

            // Setup the camera
            var tes = TESUnity.instance;
            var camera = Camera.main;
            camera.renderingPath = tes.renderPath;
            camera.farClipPlane = tes.cameraFarClip;

            _crosshair = FindObjectOfType<UICrosshair>();
        }

        private void Update()
        {
            if (_paused)
                return;

            Rotate();

            if (Input.GetKeyDown(KeyCode.Tab))
                isFlying = !isFlying;

            if (_isGrounded && !isFlying && InputManager.GetButtonDown("Jump"))
            {
                var newVelocity = _rigidbody.velocity;
                newVelocity.y = 5;

                _rigidbody.velocity = newVelocity;
            }

            if (InputManager.GetButtonDown("Light"))
                lantern.enabled = !lantern.enabled;
        }

        private void FixedUpdate()
        {
            _isGrounded = CalculateIsGrounded();

            if (_isGrounded || isFlying)
                SetVelocity();
            else if (!_isGrounded || !isFlying)
                ApplyAirborneForce();
        }

        private void Rotate()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                if (Input.GetMouseButtonDown(0))
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    return;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            var eulerAngles = new Vector3(_camTransform.localEulerAngles.x, _transform.localEulerAngles.y, 0);

            // Make eulerAngles.x range from -180 to 180 so we can clamp it between a negative and positive angle.
            if (eulerAngles.x > 180)
                eulerAngles.x = eulerAngles.x - 360;

            var deltaMouse = mouseSensitivity * (new Vector2(InputManager.GetAxis("Mouse X"), InputManager.GetAxis("Mouse Y")));

            eulerAngles.x = Mathf.Clamp(eulerAngles.x - deltaMouse.y, minVerticalAngle, maxVerticalAngle);
            eulerAngles.y = Mathf.Repeat(eulerAngles.y + deltaMouse.x, 360);

            _camTransform.localEulerAngles = new Vector3(eulerAngles.x, 0, 0);
            _transform.localEulerAngles = new Vector3(0, eulerAngles.y, 0);
        }

        private void SetVelocity()
        {
            Vector3 velocity;

            if (!isFlying)
            {
                velocity = _transform.TransformVector(CalculateLocalVelocity());
                velocity.y = _rigidbody.velocity.y;
            }
            else
                velocity = _camTransform.TransformVector(CalculateLocalVelocity());

            _rigidbody.velocity = velocity;
        }

        private void ApplyAirborneForce()
        {
            var forceDirection = _transform.TransformVector(CalculateLocalMovementDirection());
            forceDirection.y = 0;
            forceDirection.Normalize();

            var force = airborneForceMultiplier * _rigidbody.mass * forceDirection;

            _rigidbody.AddForce(force);
        }

        private Vector3 CalculateLocalMovementDirection()
        {
            // Calculate the local movement direction.
            var direction = new Vector3(InputManager.GetAxis("Horizontal"), 0.0f, InputManager.GetAxis("Vertical"));

            // A small hack for French Keyboard...
            if (Application.systemLanguage == SystemLanguage.French)
            {
                // Cancel Qwerty
                if (Input.GetKeyDown(KeyCode.W))
                    direction.z = 0;
                else if (Input.GetKeyDown(KeyCode.A))
                    direction.x = 0;

                // Use Azerty
                if (Input.GetKey(KeyCode.Z))
                    direction.z = 1;
                else if (Input.GetKey(KeyCode.S))
                    direction.z = -1;

                if (Input.GetKey(KeyCode.Q))
                    direction.x = -1;
                else if (Input.GetKey(KeyCode.D))
                    direction.x = 1;
            }

            return direction.normalized;
        }

        private float CalculateSpeed()
        {
            var speed = normalSpeed;

            if (InputManager.GetButton("Run"))
                speed = fastSpeed;

            else if (InputManager.GetButton("Slow"))
                speed = slowSpeed;

            if (isFlying)
                speed *= flightSpeedMultiplier;

            return speed;
        }

        private Vector3 CalculateLocalVelocity()
        {
            return CalculateSpeed() * CalculateLocalMovementDirection();
        }

        private bool CalculateIsGrounded()
        {
            var playerCenter = _transform.position + _capsuleCollider.center;
            var castedSphereRadius = 0.8f * _capsuleCollider.radius;
            var sphereCastDistance = (_capsuleCollider.height / 2);

            return Physics.SphereCast(new Ray(playerCenter, -_transform.up), castedSphereRadius, sphereCastDistance);
        }

        public void Pause(bool pause)
        {
            _paused = pause;
            _crosshair.SetActive(!_paused);

            Time.timeScale = pause ? 0.0f : 1.0f;
            Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = pause;

            // Used by the VR Component to enable/disable some features.
            SendMessage("OnPlayerPause", pause, SendMessageOptions.DontRequireReceiver);
        }
    }
}