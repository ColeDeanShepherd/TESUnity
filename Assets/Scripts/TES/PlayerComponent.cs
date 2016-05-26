using UnityEngine;

namespace TESUnity
{
	public class PlayerComponent : MonoBehaviour
	{
		public float slowSpeed = 3;
		public float normalSpeed = 5;
		public float fastSpeed = 10;

		public float mouseSensitivity = 3;
		public float minVerticalAngle = -90;
		public float maxVerticalAngle = 90;

		public new GameObject camera;
		public CharacterController characterController;
		
		private void Start()
		{
			characterController = GetComponent<CharacterController>();
		}
		private void Update()
		{
			Rotate();
			Translate();
		}

		private void Rotate()
		{
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
		private void Translate()
		{
			var velocity = transform.TransformVector(CalculateLocalVelocity());

			characterController.SimpleMove(velocity);
		}

		private Vector3 CalculateLocalMovementDirection()
		{
			// Calculate the local movement direction.
			var direction = Vector3.zero;

			if(Input.GetKey(KeyCode.W))
			{
				direction += Vector3.forward;
			}

			if(Input.GetKey(KeyCode.A))
			{
				direction += Vector3.left;
			}

			if(Input.GetKey(KeyCode.S))
			{
				direction += Vector3.back;
			}

			if(Input.GetKey(KeyCode.D))
			{
				direction += Vector3.right;
			}

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

			return speed;
		}
		private Vector3 CalculateLocalVelocity()
		{
			return CalculateSpeed() * CalculateLocalMovementDirection();
		}
	}
}