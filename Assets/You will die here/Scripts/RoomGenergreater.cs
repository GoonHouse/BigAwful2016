using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGenergreater : MonoBehaviour {
    public Dictionary<string, RoomObject> rooms = new Dictionary<string, RoomObject>();
    public Dictionary<string, WallObject> walls = new Dictionary<string, WallObject>();

    public Vector2 worldMin = new Vector2();
    public Vector2 worldMax = new Vector2();

    public float roomSize = 10.0f;
    public int playerGenerationRadius = 2;

    public bool IsFree(Vector2 loc) {
        var coord = God.Key(loc);

        return !rooms.ContainsKey(coord);
    }

    public GameObject SpawnPart(string partName, Vector2 loc) {
        var coord = God.Key(loc);

        if (rooms.ContainsKey(coord) || partName == null) {
            Debug.LogWarning("EITHER " + coord + " ALREADY EXISTS OR " + partName + " ISN'T A PART!");
            return null;
        }

        var pos = new Vector3(loc.x * roomSize, 0, loc.y * roomSize);
        var part = Resources.Load("RoomParts/" + partName);
        var go = Instantiate(part, pos, Quaternion.identity) as GameObject;
        go.transform.SetParent(GameObject.Find("Rooms").transform, true);
        go.transform.localPosition = pos;
        go.transform.rotation = GameObject.Find("Rooms").transform.rotation;

        go.name = coord + " " + partName;

        rooms.Add(coord, go.GetComponent<RoomObject>());

        if (loc.x < worldMin.x) {
            worldMin.x = loc.x;
        } else if (loc.x > worldMax.x) {
            worldMax.x = loc.x;
        }

        if (loc.y < worldMin.y) {
            worldMin.y = loc.y;
        } else if (loc.y > worldMax.y) {
            worldMax.y = loc.y;
        }

        return go;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
