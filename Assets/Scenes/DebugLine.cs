using UnityEngine;
using System.Collections;

public class DebugLine : MonoBehaviour {
	public GameObject target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 drawTo = target.transform.position - gameObject.transform.position;
		Debug.DrawRay(gameObject.transform.position, drawTo, Color.green);
	}
}
