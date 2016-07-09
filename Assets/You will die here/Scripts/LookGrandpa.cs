using UnityEngine;
using System.Collections;

public class LookGrandpa : MonoBehaviour {
	public float turnSpeed = 1f;
	public Transform thingToLookAt;
	public Transform eyeTarget;
	public float maxAngle = 0.75f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		rotateTowards (thingToLookAt.transform.position, eyeTarget.transform.position);
		//transform.LookAt(thingToLookAt);
	}

	protected void rotateTowards(Vector3 to, Vector3 eyes) {

		Quaternion lookRotation = Quaternion.LookRotation ((to - transform.position).normalized);
		Quaternion ahead = Quaternion.LookRotation ((eyes - transform.position).normalized);
		float diff = Quaternion.Dot(lookRotation, ahead);
		diff = Mathf.Abs (diff);
		if (diff > maxAngle) {
			transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
		} else {
			transform.rotation = Quaternion.Slerp (transform.rotation, ahead, Time.deltaTime * turnSpeed);
		}
	}
}
