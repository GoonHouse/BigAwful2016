using UnityEngine;
using System.Collections;

public class MoveSucka : MonoBehaviour {
	
	public float moveSpeed = 10f;
	public float turnSpeed = 50f;


	void Update ()
	{
		if(Input.GetKey(KeyCode.UpArrow))
			transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

		if(Input.GetKey(KeyCode.DownArrow))
			transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);

		if(Input.GetKey(KeyCode.LeftArrow))
			//transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
			//transform.RotateAround(Vector3.up, Vector3.up, -turnSpeed * Time.deltaTime);
			transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

		if(Input.GetKey(KeyCode.RightArrow))
			//transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
			//transform.RotateAround(Vector3.up, Vector3.up, turnSpeed * Time.deltaTime);
			transform.Translate(-Vector3.right * moveSpeed * Time.deltaTime);
	}
}