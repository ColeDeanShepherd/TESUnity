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

            // Setup the render path
            Camera.main.renderingPath = TESUnity.instance.renderPath;

            // Add the crosshair and the cursor. TODO: Move that.
            var textureManager = TESUnity.instance.TextureManager;
            var cursor = textureManager.LoadTexture("tx_cursor", true);
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
            _crosshair = UICrosshair.Create(GUIUtils.MainCanvas.transform);
        }

        private void Update()
        {
            if (_paused)
                return;

            Rotate();

            if (Input.GetKeyDown(KeyCode.Tab))
                isFlying = !isFlying;

            if (_isGrounded && !isFlying && Input.GetButtonDown("Jump"))
            {
                var newVelocity = _rigidbody.velocity;
                newVelocity.y = 5;

                _rigidbody.velocity = newVelocity;
            }

            if (Input.GetButtonDown("Light"))
                lantern.enabled = !lantern.enabled;
        }

        private void FixedUpdate()
        {
            _isGrounded = CalculateIsGrounded();

            if (_isGrounded || isFlying)
                SetVelocity();
            else if (!_isGrounded || !isFlying)
                ApplyAirborneForce();

            if (TESUnity.instance.followHeadDirection)
            {
                var centerEye = _camTransform;
                var root = centerEye.parent;
                var prevPos = root.position;
                var prevRot = root.rotation;

                // TODO: Do the same with position.
                _transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

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

            var eulerAngles = new Vector3(_camTransform.localEulerAngles.x, _transform.localEulerAngles.y, 0);

            // Make eulerAngles.x range from -180 to 180 so we can clamp it between a negative and positive angle.
            if (eulerAngles.x > 180)
                eulerAngles.x = eulerAngles.x - 360;

            var deltaMouse = mouseSensitivity * (new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

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
            var speed = normalSpeed;

            if (Input.GetButton("Run"))
                speed = fastSpeed;

            else if (Input.GetButton("Slow"))
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
            var playerCenter = _transform.position;
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
            SendMessage("OnPlayerPause", pause);
        }
    }
}