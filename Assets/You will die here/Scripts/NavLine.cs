using UnityEngine;
using System.Collections;

public class NavLine : MonoBehaviour {
	private GameObject gramps;
    private Vector3 grampsHeight = new Vector3(0.0f, 4.5f, 0.0f);
	// Use this for initialization
	void Start () {
		gramps = GameObject.Find("GrampsHolder");
	}
	
	// Update is called once per frame
	void Update () {
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetPosition (0, transform.position);
		lineRenderer.SetPosition (1, gramps.transform.position-Vector3.up+grampsHeight);
	}
}
