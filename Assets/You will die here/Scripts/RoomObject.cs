using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomObject : MonoBehaviour {

    public bool isNorthOpen = false;
    public bool isEastOpen = false;
    public bool isSouthOpen = false;
    public bool isWestOpen = false;

    public bool isWalkable = true;
    public Vector2 pos;
    public bool isDarkRoom = false;

    public List<GameObject> floorAnchors;
    public List<GameObject> oversizeAnchors;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
