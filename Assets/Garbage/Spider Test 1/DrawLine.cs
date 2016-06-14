using UnityEngine;
using System.Collections;

public class DrawLine : MonoBehaviour {
	public GameObject target;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		LineRenderer lineRenderer = GetComponent<LineRenderer> ();
		Vector3 drawTo = target.transform.position - gameObject.transform.position;
		lineRenderer.SetPosition (1, drawTo);
	}
}
