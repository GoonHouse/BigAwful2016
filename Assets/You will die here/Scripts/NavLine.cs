using UnityEngine;
using System.Collections;

public class NavLine : MonoBehaviour {
	private GameObject gramps;
    private Vector3 grampsHeight = new Vector3(0.0f, 4.5f, 0.0f);
    public float grampsLerp = 1.0f;
    public float mitigation = 1.0f;
    public float maxDistDelta = 8.0f;

	// Use this for initialization
	void Start () {
		gramps = GameObject.Find("GrampsHolder");
	}
	
	// Update is called once per frame
	void Update () {
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetPosition (0, transform.position+grampsHeight);

        var nang = Vector3.MoveTowards(gramps.transform.position, transform.position, maxDistDelta);
        lineRenderer.SetPosition (1, Vector3.Lerp(transform.position+grampsHeight, nang-Vector3.up+grampsHeight, mitigation * grampsLerp));
	}
}
