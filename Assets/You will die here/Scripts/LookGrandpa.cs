using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LookGrandpa : MonoBehaviour {
	public float turnSpeed = 1f;
	public Transform thingToLookAt;
	public Transform eyeTarget;
	public Transform grampsHead;
	public float tooFar = 10f;
	public float maxAngle = 0.75f;

    public float timeOnMind = 0.0f;
    public float timeToCommit = 3.0f;
    public bool didCommitYet = false;

    public List<LookTarget> targets = new List<LookTarget>();
    public List<EmotionFragment> emotions = new List<EmotionFragment>();
    public List<NeedFragment> needs = new List<NeedFragment>();

    // Use this for initialization
    void Start () {
	
	}
		
	// Update is called once per frame
	void Update () {
		float dist = Vector3.Distance (thingToLookAt.transform.position, grampsHead.transform.position); //this makes grandpa forget when things get too far away
		if (dist > tooFar) {
			thingToLookAt = eyeTarget;
            //print ("Too Far!");
            timeOnMind = 0.0f;
            didCommitYet = false;
        }
        if( thingToLookAt != eyeTarget && timeOnMind > timeToCommit && !didCommitYet){
            didCommitYet = true;
            targets.Add(thingToLookAt.gameObject.GetComponent<LookTarget>());
        }
	}

	void LateUpdate () {
		rotateTowards (thingToLookAt.transform.position, eyeTarget.transform.position);
		//transform.LookAt(thingToLookAt);
	}

	void OnCollisionEnter(Collision collision){
		thingToLookAt = collision.gameObject.transform;
        timeOnMind = 0.0f;
        didCommitYet = false;
    }

	protected void rotateTowards(Vector3 to, Vector3 eyes) {
		Quaternion lookRotation = Quaternion.LookRotation ((to - grampsHead.transform.position).normalized);
		Quaternion ahead = Quaternion.LookRotation ((eyes - grampsHead.transform.position).normalized);
		float diff = Quaternion.Dot(lookRotation, ahead);
		diff = Mathf.Abs (diff);
		if (diff > maxAngle) {
			grampsHead.transform.rotation = Quaternion.Slerp (grampsHead.transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
            if( !didCommitYet) {
                timeOnMind += Time.deltaTime;
            }
		} else {
			grampsHead.transform.rotation = Quaternion.Slerp (grampsHead.transform.rotation, ahead, Time.deltaTime * turnSpeed);
		}
	}
}
