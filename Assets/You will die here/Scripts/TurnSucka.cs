using UnityEngine;
using System.Collections;

public class TurnSucka : MonoBehaviour {

	public float speed = 0.1F;

	private Vector3 currentAngle;

	public void Start()
	{
		
	}

	void Update ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 direction = new Vector3 (-moveHorizontal, 0.0f, -moveVertical);

		if (moveVertical == 0 && moveHorizontal == 0) {
			//direction = new Vector3 (-1, 0, 1);
		} else {
			float step = speed * Time.deltaTime;
			transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation (direction), step);
		}

	}
}
