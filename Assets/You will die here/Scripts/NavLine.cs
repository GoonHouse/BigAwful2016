using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        var navlines = FindObjectsOfType<NavLine>();
        var minDist = Mathf.Infinity;
        var maxDist = Mathf.NegativeInfinity;
        foreach( NavLine nav in navlines ) {
            var dist = Vector3.Distance(gramps.transform.position, nav.transform.position);
            if( dist < minDist ){
                minDist = dist;
            }
            if( dist > maxDist) {
                maxDist = dist;
            }
        }
        
        var nang = Vector3.MoveTowards(gramps.transform.position, transform.position, maxDistDelta);
        lineRenderer.SetPosition (1, Vector3.Lerp(transform.position+grampsHeight, nang-Vector3.up+grampsHeight, mitigation * grampsLerp));
        var mxdist = 3.0f;
        if( rg != null ) {
            mxdist = rg.maxDistanceFromOrigin;
        }
        var bdist = Vector3.Distance(gramps.transform.position, transform.position);
        var gigy = bdist.Scale(minDist, maxDist, minTriDist, maxTriDist);

		nang = Vector3.MoveTowards(gramps.transform.position, transform.position, maxDistDelta + gigy + triOff);
		lineRenderer.SetPosition (0, Vector3.Lerp(transform.position+grampsHeight, nang-Vector3.up+grampsHeight, mitigation * grampsLerp));
	}
}
