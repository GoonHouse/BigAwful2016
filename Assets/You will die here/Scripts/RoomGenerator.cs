using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour {

    public Dictionary<string, RoomObject> rooms = new Dictionary<string, RoomObject>();
    public Vector2 worldMin = new Vector2();
    public Vector2 worldMax = new Vector2();

    public float generateFrequency = 3.0f;
    public float generateTimer = 0.0f;

    // Use this for initialization
    void Start () {
        SpawnPart("NESW", 0, 0);
        SpawnPart("N___", 1, 0);
    }
	
	// Update is called once per frame
	void Update () {
        generateTimer -= Time.deltaTime;
        if( generateTimer <= 0.0f) {
            generateTimer += generateFrequency;
            var rx = Random.Range((int)(worldMin.x - 1.0f), (int)(worldMax.x + 2.0f));
            var ry = Random.Range((int)(worldMin.y - 1.0f), (int)(worldMax.y + 2.0f));
            var coord = rx.ToString() + "_" + ry.ToString();
            if (!rooms.ContainsKey(coord)) {
                SpawnPart("NESW", rx, ry);
            }

            /*
            var spawned = false;
            for ( float x = worldMin.x - 1.0f; x <= worldMax.x; x++) {
                for (float y = worldMin.y - 1.0f; y <= worldMax.y; y++) {
                    var coord = x.ToString() + "_" + y.ToString();
                    if ( ! rooms.ContainsKey( coord ) ){
                        SpawnPart("NESW", x, y);
                        spawned = true;
                        break;
                    }
                }
                if( spawned ) {
                    break;
                }
            }
            */
        }
    }

    public GameObject SpawnPart(string partName, float x, float y) {
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
