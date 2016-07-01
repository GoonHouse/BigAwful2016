using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransparencyToucher : MonoBehaviour {
    public List<GameObject> touched = new List<GameObject>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        foreach(GameObject touch in touched) {
            var cpos = touch.transform.position;
            var tpos = gameObject.transform.position;
            var dist = (cpos - tpos).magnitude;
            var ratio = dist / gameObject.transform.lossyScale.z;
            Debug.Log(touch + " " + ratio);
        }
	
	}

    void OnCollisionExit(Collision collision) {
        touched.Remove(collision.gameObject);
    }

    void OnCollisionEnter(Collision collision) {
        touched.Add(collision.gameObject);
    }
}
