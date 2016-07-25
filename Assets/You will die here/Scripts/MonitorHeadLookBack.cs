using UnityEngine;
using System.Collections;

public class MonitorHeadLookBack : MonoBehaviour {
    public float turnSpeed = 1f;
    public GameObject thingToLookAt;
    public Transform myHead;
    public GameObject eyeTarget;
    public float tooFar = 10f;
    public float maxAngle = 0.75f;
    public bool isTooFar = true;

    // Use this for initialization
    void Start () {
        thingToLookAt = GameObject.Find("GrampsHolder/GrandFatherContainmentUnit/EyeTarget");
	}
	
	// Update is called once per frame
	void Update () {
        if (thingToLookAt != null) {
            float dist = Vector3.Distance(thingToLookAt.transform.position, myHead.transform.position);
            isTooFar = (dist > tooFar);
        }
    }

    void LateUpdate() {
        if (!isTooFar) {
            rotateTowards(thingToLookAt.transform.position, eyeTarget.transform.position);
        } else {
            rotateTowards(myHead.transform.position, myHead.transform.position);
        }
    }

    protected void rotateTowards(Vector3 to, Vector3 eyes) {
        var doink = (to - myHead.transform.position).normalized;
        Quaternion lookRotation = Quaternion.identity;
        if( doink != Vector3.zero) {
            lookRotation = Quaternion.LookRotation(doink);
        }
        var dink = (eyes - myHead.transform.position).normalized;
        Quaternion ahead = Quaternion.identity;
        if( dink != Vector3.zero) {
            ahead = Quaternion.LookRotation(dink);
        }
        float diff = Quaternion.Dot(lookRotation, ahead);
        diff = Mathf.Abs(diff);
        if (diff > maxAngle) {
            myHead.transform.rotation = Quaternion.Slerp(myHead.transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        } else {
            myHead.transform.rotation = Quaternion.Slerp(myHead.transform.rotation, ahead, Time.deltaTime * turnSpeed);
        }
    }
}
