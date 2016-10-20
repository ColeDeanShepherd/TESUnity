﻿using System.Collections;
using TESUnity.Components;
using TESUnity.UI;
using UnityEngine;

namespace TESUnity
{
    public class PlayerComponent : MonoBehaviour
    {
        private bool _paused = false;
        private CapsuleCollider capsuleCollider;
        private new Rigidbody rigidbody;
        private bool isGrounded;
        private bool _isFlying = false;
        private UICrosshair _crosshair;

        public float slowSpeed = 3;
        public float normalSpeed = 5;
        public float fastSpeed = 10;
        public float flightSpeedMultiplier = 3;
        public float airborneForceMultiplier = 5;
        public float mouseSensitivity = 3;
        public float minVerticalAngle = -90;
        public float maxVerticalAngle = 90;
        
        public new GameObject camera;
        public GameObject lantern;
        public PlayerInventory inventory;
        public Transform leftHand;
        public Transform rightHand;

        public bool isFlying
        {
            get { return _isFlying; }
            set
            {
                _isFlying = value;

                if (!_isFlying)
                    rigidbody.useGravity = true;
                else
                    rigidbody.useGravity = false;
            }
        }

        public bool Paused
        {
            get { return _paused; }
        }

        private void Start()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            rigidbody = GetComponent<Rigidbody>();

            // Add the crosshair and the cursor
            var textureManager = TESUnity.instance.TextureManager;
            var cursor = textureManager.LoadTexture("tx_cursor", true);
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
            _crosshair = UICrosshair.Create(GUIUtils.MainCanvas.transform);

            // The two hands nodes.
            leftHand = CreateHand(true);
            rightHand = CreateHand(false);
        }

        private void Update()
        {
            if (_paused)
                return;

            Rotate();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                isFlying = !isFlying;
            }

            if (isGrounded && !isFlying && Input.GetButtonDown("Jump"))
            {
                var newVelocity = rigidbody.velocity;
                newVelocity.y = 5;

                rigidbody.velocity = newVelocity;
            }

            if (Input.GetButtonDown("Light"))
            {
                var light = lantern.GetComponent<Light>();
                light.enabled = !light.enabled;
            }
        }

        private void FixedUpdate()
        {
            isGrounded = CalculateIsGrounded();

            if (isGrounded || isFlying)
            {
                SetVelocity();
            }
            else if (!isGrounded || !isFlying)
            {
                ApplyAirborneForce();
            }

            if (TESUnity.instance.followHeadDirection)
            {
                var centerEye = Camera.main.transform;
                var root = centerEye.parent;
                var prevPos = root.position;
                var prevRot = root.rotation;

                transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

                root.position = prevPos;
                root.rotation = prevRot;
            }
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

            var eulerAngles = new Vector3(camera.transform.localEulerAngles.x, transform.localEulerAngles.y, 0);

            // Make eulerAngles.x range from -180 to 180 so we can clamp it between a negative and positive angle.
            if (eulerAngles.x > 180)
            {
                eulerAngles.x = eulerAngles.x - 360;
            }

            var deltaMouse = mouseSensitivity * (new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

            eulerAngles.x = Mathf.Clamp(eulerAngles.x - deltaMouse.y, minVerticalAngle, maxVerticalAngle);
            eulerAngles.y = Mathf.Repeat(eulerAngles.y + deltaMouse.x, 360);

            camera.transform.localEulerAngles = new Vector3(eulerAngles.x, 0, 0);
            transform.localEulerAngles = new Vector3(0, eulerAngles.y, 0);
        }

        private void SetVelocity()
        {
            Vector3 velocity;

            if (!isFlying)
            {
                velocity = transform.TransformVector(CalculateLocalVelocity());
                velocity.y = rigidbody.velocity.y;
            }
            else
            {
                velocity = camera.transform.TransformVector(CalculateLocalVelocity());
            }

            rigidbody.velocity = velocity;
        }

        private void ApplyAirborneForce()
        {
            var forceDirection = transform.TransformVector(CalculateLocalMovementDirection());
            forceDirection.y = 0;

            forceDirection.Normalize();

            var force = airborneForceMultiplier * rigidbody.mass * forceDirection;

            rigidbody.AddForce(force);
        }

        private Vector3 CalculateLocalMovementDirection()
        {
            // Calculate the local movement direction.
            var direction = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

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
            float speed;

            if (Input.GetButton("Run"))
            {
                speed = fastSpeed;
            }
            else if (Input.GetButton("Slow"))
            {
                speed = slowSpeed;
            }
            else
            {
                speed = normalSpeed;
            }

            if (isFlying)
            {
                speed *= flightSpeedMultiplier;
            }

            return speed;
        }

        private Vector3 CalculateLocalVelocity()
        {
            return CalculateSpeed() * CalculateLocalMovementDirection();
        }

        private bool CalculateIsGrounded()
        {
            var playerCenter = transform.position;
            var castedSphereRadius = 0.8f * capsuleCollider.radius;
            var sphereCastDistance = (capsuleCollider.height / 2);

            return Physics.SphereCast(new Ray(playerCenter, -transform.up), castedSphereRadius, sphereCastDistance);
        }

        public void Pause(bool pause)
        {
            _paused = pause;
            _crosshair.SetActive(!_paused);

            Time.timeScale = pause ? 0.0f : 1.0f;
            Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = pause;

            SendMessage("OnPlayerPause", pause);
        }

        private Transform CreateHand(bool left)
        {
            var hand = new GameObject((left ? "Left" : "Right") + " Hand");
            var hTransform = hand.GetComponent<Transform>();
            hand.transform.parent = Camera.main.transform;
            hand.transform.localPosition = new Vector3(left ? -0.3f : 0.3f, -0.3f, 0.45f);
            hand.transform.localRotation = Quaternion.Euler(-50.0f, 0.0f, left ? -90.0f : 90.0f);
            return hTransform;
        }
    }
}