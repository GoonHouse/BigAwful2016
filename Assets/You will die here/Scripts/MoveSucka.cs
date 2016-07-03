using UnityEngine;
using System.Collections;

public class MoveSucka : MonoBehaviour {
	
	public float moveSpeed = 10f;
	public float turnAmount = 90.0f;
	public Transform Gramps;

	public void Start()
	{

	}

	void Update ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		if(Input.GetButtonDown("Fire2"))
		{
			transform.RotateAround (Gramps.transform.position, Vector3.up, turnAmount);
			Gramps.transform.Rotate (new Vector3(0, turnAmount, 0));
		}

		Vector3 moveDirection = new Vector3 (-moveHorizontal, 0.0f, -moveVertical);

		transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
	}
}