using UnityEngine;

public class FlyingCameraComponent : MonoBehaviour
{
	public float slowSpeed = 10;
	public float normalSpeed = 20;
	public float fastSpeed = 80;

	public float mouseSensitivity = 3;
	public float minVerticalAngle = -90;
	public float maxVerticalAngle = 90;

	private Vector3 eulerAngles;

	private void Start()
	{
		eulerAngles = transform.eulerAngles;
		eulerAngles.z = 0;
	}
	private void Update()
	{
		Rotate();
		Translate();
	}

	private void Rotate()
	{
		var deltaMouse = mouseSensitivity * (new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

		eulerAngles.x = Mathf.Clamp(eulerAngles.x - deltaMouse.y, minVerticalAngle, maxVerticalAngle);
		eulerAngles.y = Mathf.Repeat(eulerAngles.y + deltaMouse.x, 360);

		transform.eulerAngles = eulerAngles;
	}
	private void Translate()
	{
		// Calculate the movement direction.
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

		if(Input.GetKey(KeyCode.Q))
		{
			direction += Vector3.down;
		}

		if(Input.GetKey(KeyCode.E))
		{
			direction += Vector3.up;
		}

		direction.Normalize();

		// Calculate the speed.
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

		// translate
		var velocity = speed * direction;
		transform.Translate(Time.deltaTime * velocity);
	}
}