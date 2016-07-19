﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LookGrandpa : MonoBehaviour {
	public float turnSpeed = 1f;
	public Transform thingToLookAt;
	public Transform eyeTarget;
	public Transform grampsHead;
    public UnityEngine.UI.Text thinkText;
	public float tooFar = 10f;
	public float maxAngle = 0.75f;

    public float timeOnMind = 0.0f;
    public float timeToCommit = 0.33f;
    public bool didCommitYet = false;

    private EmotionProcessor epu;

    void OnLevelWasLoaded() {
        thinkText = GameObject.Find("Canvas/GrandpaThoughts").GetComponent<UnityEngine.UI.Text>();
        // you can't forget yourself, grandpa!
        // like hell I can't
        Forget(gameObject);
    }

        // Use this for initialization
    void Awake () {
        thingToLookAt = null;
        if( thinkText != null) {
            thinkText.text = "";
        } else {
            thinkText = GameObject.Find("Canvas/GrandpaThoughts").GetComponent<UnityEngine.UI.Text>();
            thinkText.text = "";
        }
        epu = GetComponent<EmotionProcessor>();
	}
		
	// Update is called once per frame
	void Update () {
        if( thingToLookAt != null) {
            float dist = Vector3.Distance(thingToLookAt.transform.position, grampsHead.transform.position); //this makes grandpa forget when things get too far away
            if (dist > tooFar) {
                Forget( thingToLookAt.gameObject );
            }
        }
	}

    void Think(LookTarget lt, Thought t) {
        if (lt != null && lt.thoughts.Count > 0) {
            var thought = lt.thoughts[Random.Range(0, lt.thoughts.Count)];
            thinkText.text = thought.text;
        }
    }

    void Notice(GameObject go) {
        thingToLookAt = go.transform;
        timeOnMind = 0.0f;
        didCommitYet = false;
    }

    void Remember(GameObject go) {
        didCommitYet = true;
        var lt = go.GetComponentInChildren<LookTarget>();
        var knob = go.GetComponentInChildren<Knob>();
        if( knob != null && !epu.targets.Contains(lt)) {
            var grampsH = GameObject.Find("GrampsHolder");
            var grandpa = grampsH.GetComponent<Grandpa>();
            grandpa.FocusOnKnob(knob);
        }
        if( lt != null && epu.Commit(lt) ) {
            if (lt.thoughts.Count > 0) {
                var thought = lt.thoughts[Random.Range(0, lt.thoughts.Count)];
                Think(lt, thought);
            }
        }
    }

    void Forget(GameObject go) {
        thingToLookAt = null;
        timeOnMind = 0.0f;
        didCommitYet = false;
    }

    void Ponder() {
        if( timeOnMind < timeToCommit && !didCommitYet && thingToLookAt != null ){
            timeOnMind += Time.deltaTime;
            if ( timeOnMind >= timeToCommit && !didCommitYet && thingToLookAt != null ) {
                Remember(thingToLookAt.gameObject);
            }
        }
    }

	void LateUpdate () {
        if( thingToLookAt != null) {
            rotateTowards(thingToLookAt.transform.position, eyeTarget.transform.position);
        } else {
            rotateTowards(eyeTarget.transform.position, eyeTarget.transform.position);
        }
		//transform.LookAt(thingToLookAt);
	}

	void OnCollisionEnter(Collision collision){
        if( collision.gameObject.GetComponentInChildren<LookTarget>() != null && thingToLookAt != collision.gameObject) {
            Notice(collision.gameObject);
        }
    }

	protected void rotateTowards(Vector3 to, Vector3 eyes) {
		Quaternion lookRotation = Quaternion.LookRotation ((to - grampsHead.transform.position).normalized);
		Quaternion ahead = Quaternion.LookRotation ((eyes - grampsHead.transform.position).normalized);
		float diff = Quaternion.Dot(lookRotation, ahead);
		diff = Mathf.Abs (diff);
		if (diff > maxAngle) {
			grampsHead.transform.rotation = Quaternion.Slerp (grampsHead.transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
            Ponder();
		} else {
			grampsHead.transform.rotation = Quaternion.Slerp (grampsHead.transform.rotation, ahead, Time.deltaTime * turnSpeed);
		}
	}
}
