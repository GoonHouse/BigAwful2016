using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallObject : MonoBehaviour {

    public Vector2 pos;
    public string dir;

    public List<GameObject> insidePhotoAnchors;
    public List<GameObject> insideFloorAnchors;
    public List<GameObject> insideBlockedByDoor;
    public List<GameObject> outsidePhotoAnchors;
    public List<GameObject> outsideFloorAnchors;
    public List<GameObject> outsideBlockedByDoor;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
