using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour {

    public Dictionary<string, RoomObject> rooms = new Dictionary<string, RoomObject>();
    public Vector2 worldMin = new Vector2();
    public Vector2 worldMax = new Vector2();

    // Use this for initialization
    void Start () {
        SpawnPart("NESW", 0, 0);
        SpawnPart("N___", 1, 0);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public GameObject SpawnPart(string partName, int x, int y) {
        var pos = new Vector3(x * 10.0f, 0, y * 10.0f);
        var part = Resources.Load("Rooms/" + partName);
        var go = Instantiate(part, pos, Quaternion.identity) as GameObject;
        go.transform.SetParent( GameObject.Find("World").transform );

        rooms.Add( x.ToString() + "_" + y.ToString(), go.GetComponent<RoomObject>() );

        if( x < worldMin.x ) {
            worldMin.x = x;
        } else if( x > worldMax.x) {
            worldMax.x = x;
        }

        if( y < worldMin.y ) {
            worldMin.y = y;
        } else if( y > worldMax.y ) {
            worldMax.y = y;
        }

        return go;
    }
}
