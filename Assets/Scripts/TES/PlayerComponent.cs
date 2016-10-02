using UnityEngine;
using UnityEngine.VR;

namespace TESUnity
{
	public class PlayerComponent : MonoBehaviour
	{
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

				if(!_isFlying)
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

		private CapsuleCollider capsuleCollider;
		private new Rigidbody rigidbody;

		private bool isGrounded;
		private bool _isFlying = false;
		
		private void Start()
		{
			capsuleCollider = GetComponent<CapsuleCollider>();
			rigidbody = GetComponent<Rigidbody>();

            if (VRSettings.enabled)
            {
                InputTracking.Recenter();

                // Put the Canvas in WorldSpace and Attach it to the camera.
                var canvas = FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    canvas.renderMode = RenderMode.WorldSpace;
                    var canvasTransform = canvas.GetComponent<Transform>();
                    canvasTransform.parent = Camera.main.transform.parent;
                    canvasTransform.localPosition = new Vector3(0.0f, 0.0f, 1.0f);
                    canvasTransform.localRotation = Quaternion.identity;
                    canvasTransform.localScale = new Vector3(0.003f, 0.003f, 0.003f);
                }
            }
        }

		private void Update()
		{
			Rotate();

			if(Input.GetKeyDown(KeyCode.Tab))
			{
				isFlying = !isFlying;
			}

			if(isGrounded && !isFlying && Input.GetButtonDown("Jump"))
			{
				var newVelocity = rigidbody.velocity;
				newVelocity.y = 5;

				rigidbody.velocity = newVelocity;
			}

			if(Input.GetKeyDown(KeyCode.L))
			{
				var light = lantern.GetComponent<Light>();
				light.enabled = !light.enabled;
			}
		}
		private void FixedUpdate()
		{
			isGrounded = CalculateIsGrounded();

			if(isGrounded || isFlying)
			{
				SetVelocity();
			}
			else if(!isGrounded || !isFlying)
			{
				ApplyAirborneForce();
			}
		}

		private void Rotate()
		{
			if ( Cursor.lockState != CursorLockMode.Locked )
			{
				if ( Input.GetMouseButtonDown( 0 ) )
					Cursor.lockState = CursorLockMode.Locked;
				else
					return;
			}
			else
			{
				if ( Input.GetKeyDown( KeyCode.BackQuote ) )
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				}
			}

			var eulerAngles = new Vector3(camera.transform.localEulerAngles.x, transform.localEulerAngles.y, 0);

			// Make eulerAngles.x range from -180 to 180 so we can clamp it between a negative and positive angle.
			if(eulerAngles.x > 180)
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

			if(!isFlying)
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

			if(Input.GetKey(KeyCode.LeftShift))
			{
				speed = fastSpeed;
			}
			else if(Input.GetKey(KeyCode.LeftControl))
			{
				speed = slowSpeed;
			}
			else
			{
				speed = normalSpeed;
			}

			if(isFlying)
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
	}
}