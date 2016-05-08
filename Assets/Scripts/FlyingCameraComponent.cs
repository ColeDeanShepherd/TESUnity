using UnityEngine;

public class FlyingCameraComponent : MonoBehaviour
{
	public float normalSpeed = 20;
	public float fastSpeed = 80;

	public float mouseSensitivity = 3;

	private Vector3 eulerAngles;

	private void Update()
	{
		// rotate
		var deltaMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;

		eulerAngles.x = Mathf.Clamp(eulerAngles.x - deltaMouse.y, -90, 90);
		eulerAngles.y = Mathf.Repeat(eulerAngles.y + deltaMouse.x, 360);

		transform.eulerAngles = eulerAngles;

		// move
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

		float speed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : normalSpeed;
		var velocity = speed * direction;

		transform.Translate(Time.deltaTime * velocity);
	}
}