using UnityEngine;
using System.Collections;

public class LookGrandpa : MonoBehaviour {
	public float turnSpeed = 1f;
	public Transform thingToLookAt;
	public Transform eyeTarget;
	public Transform grampsHead;
	public float tooFar = 10f;
	public float maxAngle = 0.75f;

	// Use this for initialization
	void Start () {
	
	}
		
	// Update is called once per frame
	void Update () {
		float dist = Vector3.Distance (thingToLookAt.transform.position, grampsHead.transform.position); //this makes grandpa forget when things get too far away
		if (dist > tooFar) {
			thingToLookAt = eyeTarget;
			//print ("Too Far!");
		}
	}

	void LateUpdate () {
		rotateTowards (thingToLookAt.transform.position, eyeTarget.transform.position);
		//transform.LookAt(thingToLookAt);
	}

	void OnCollisionEnter(Collision collision){
		thingToLookAt = collision.gameObject.transform;
	}

	protected void rotateTowards(Vector3 to, Vector3 eyes) {

		Quaternion lookRotation = Quaternion.LookRotation ((to - grampsHead.transform.position).normalized);
		Quaternion ahead = Quaternion.LookRotation ((eyes - grampsHead.transform.position).normalized);
		float diff = Quaternion.Dot(lookRotation, ahead);
		diff = Mathf.Abs (diff);
		if (diff > maxAngle) {
			grampsHead.transform.rotation = Quaternion.Slerp (grampsHead.transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
		} else {
			grampsHead.transform.rotation = Quaternion.Slerp (grampsHead.transform.rotation, ahead, Time.deltaTime * turnSpeed);
		}
	}
}
