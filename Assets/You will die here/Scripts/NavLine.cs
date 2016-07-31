using UnityEngine;
using System.Collections;

public class NavLine : MonoBehaviour {
	private GameObject gramps;
    private Vector3 grampsHeight = new Vector3(0.0f, 4.5f, 0.0f);
    private RoomGenergreater rg;
    public float grampsLerp = 1.0f;
    public float mitigation = 1.0f;
    public float maxDistDelta = 8.0f;
    public float minTriDist = 2.0f;
    public float maxTriDist = 8.0f;
    public float triOff = 0.0f;

	// Use this for initialization
	void Start () {
        rg = FindObjectOfType<RoomGenergreater>();
		gramps = GameObject.Find("GrampsHolder");
	}
	
	// Update is called once per frame
	void Update () {
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		//lineRenderer.SetPosition (0, transform.position+grampsHeight);

        var nang = Vector3.MoveTowards(gramps.transform.position, transform.position, maxDistDelta);
        lineRenderer.SetPosition (1, Vector3.Lerp(transform.position+grampsHeight, nang-Vector3.up+grampsHeight, mitigation * grampsLerp));
        var mxdist = 3.0f;
        if( rg != null ) {
            mxdist = rg.maxDistanceFromOrigin;
        }
        var gigy = Mathf.Lerp(minTriDist, maxTriDist, Vector3.Distance(gramps.transform.position, transform.position) / mxdist);
		nang = Vector3.MoveTowards(gramps.transform.position, transform.position, maxDistDelta + gigy + triOff);
		lineRenderer.SetPosition (0, Vector3.Lerp(transform.position+grampsHeight, nang-Vector3.up+grampsHeight, mitigation * grampsLerp));
	}
}
