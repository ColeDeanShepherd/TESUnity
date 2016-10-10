using TESUnity.Components;
using UnityEngine;
using UnityEngine.VR;

namespace TESUnity
{
    public class PlayerComponent : MonoBehaviour
    {
        private bool _paused = false;

        public float slowSpeed = 3;
        public float normalSpeed = 5;
        public float fastSpeed = 10;
        public float flightSpeedMultiplier = 3;
        public float airborneForceMultiplier = 5;

        public float mouseSensitivity = 3;
        public float minVerticalAngle = -90;
        public float maxVerticalAngle = 90;

        public bool isFlying
        {
            get
            {
                return _isFlying;
            }
            set
            {
                _isFlying = value;

                if (!_isFlying)
                {
                    rigidbody.useGravity = true;
                }
                else
                {
                    rigidbody.useGravity = false;
                }
            }
        }

        public new GameObject camera;
        public GameObject lantern;
        public PlayerInventory inventory;
        public Transform leftHand;
        public Transform rightHand; 

        private CapsuleCollider capsuleCollider;
        private new Rigidbody rigidbody;

        private bool isGrounded;
        private bool _isFlying = false;
        private GameObject _crosshair;

        private void Start()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            rigidbody = GetComponent<Rigidbody>();

            // Add the crosshair
            var textureManager = TESUnity.instance.Engine.textureManager;
            var crosshairTexture = textureManager.LoadTexture("target");
            _crosshair = GUIUtils.CreateImage(GUIUtils.CreateSprite(crosshairTexture), GUIUtils.MainCanvas, 35, 35);

            // The crosshair needs an X and Y flip
            //var cursor = textureManager.LoadTexture("tx_cursor");
            //Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);

            leftHand = CreateHand(true);
            rightHand = CreateHand(false);

            if (VRSettings.enabled)
            {
                InputTracking.Recenter();
                // Put the Canvas in WorldSpace and Attach it to the camera.
                GUIUtils.SetupCanvasToVR(FindObjectOfType<Canvas>(), Camera.main.transform.parent);
                Camera.main.nearClipPlane = 0.1f;

                _crosshair.GetComponent<UnityEngine.UI.Image>().enabled = false;
            }
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

            if (Input.GetButtonDown("Recenter"))
                InputTracking.Recenter();
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