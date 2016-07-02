using UnityEngine;
using System.Collections;

public class MoveSucka : MonoBehaviour {
	
	public float moveSpeed = 10f;

	void Update ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 direction = new Vector3 (-moveHorizontal, 0.0f, -moveVertical);

			transform.Translate(direction * moveSpeed * Time.deltaTime);
	}
}