﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Direction {
    North, East, South, West,
}

public class RoomGenerator : MonoBehaviour {

    public Dictionary<string, RoomObject> rooms = new Dictionary<string, RoomObject>();
    public Vector2 worldMin = new Vector2();
    public Vector2 worldMax = new Vector2();
	//public int randos = 10; // for making a bunch of random blank spots

    public float generateFrequency = 3.0f;
    public float generateTimer = 0.0f;

    // Use this for initialization
    void Start () {
        SpawnPart("NESW", 0, 0);
        SpawnPart("NESW", -1, 0);
        SpawnPart("NESW", 1, 0);
        SpawnPart("NESW", 0, 1);
        SpawnPart("NESW", -1, 1);
        SpawnPart("NESW", 1, 1);
        SpawnPart("NESW", 0, -1);
        SpawnPart("NESW", -1, -1);
        SpawnPart("NESW", 1, -1);

        /*
        SpawnPart("NE_W", 1, 0);  // north =  1,  0
        SpawnPart("NES_", 0, -1); // east  =  0, -1
        SpawnPart("_ESW", -1, 0); // south = -1,  0
        SpawnPart("N_SW", 0, 1);  // west  =  0,  1
        */

        /* //for making a bunch of random blanks spots
		for (int i = 0; i < randos; i++) {
			var rx = Random.Range ((int)(worldMin.x - 1.0f), (int)(worldMax.x + 2.0f));
			var ry = Random.Range ((int)(worldMin.y - 1.0f), (int)(worldMax.y + 2.0f));
			var part = ProbeSpot(rx, ry);
			if( part != "no") {
				SpawnPart("NESW", rx, ry);
			}

		}
		*/

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
                var part = ProbeSpot(rx, ry);
                if( part != "no") {
                    Debug.Log(coord + " -> " + part);
                    SpawnPart(part, rx, ry);
                }
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
        
        if( Input.GetKeyDown(KeyCode.G) ){
            Debug.Log("honka");
            //SpawnPart("____", 1, 1);
        }
    }

    public bool CalcRandomFace() {
        return Random.value <= 0.75f;
    }

    public string ProbeSpot(float x, float y) {
        var str = "";

        var coord = x.ToString() + "_" + y.ToString();

        var coordN = (x + 1).ToString() + "_" + y.ToString();
        var coordE = x.ToString() + "_" + (y - 1).ToString();
        var coordS = (x - 1).ToString() + "_" + y.ToString();
        var coordW = x.ToString() + "_" + (y + 1).ToString();

        if (rooms.ContainsKey(coordN) || rooms.ContainsKey(coordE) || rooms.ContainsKey(coordS) || rooms.ContainsKey(coordW)) {
            if (!rooms.ContainsKey(coord)) {
                if (rooms.ContainsKey(coordS) && rooms[coordS].isSouthOpen) {
                    str += "N";
                } else if (!rooms.ContainsKey(coordS)) {
                    if ( CalcRandomFace()) {
                        str += "N";
                    } else {
                        str += "_";
                    }
                } else {
                    str += "_";
                }

                if (rooms.ContainsKey(coordW) && rooms[coordW].isWestOpen) {
                    str += "E";
                } else if (!rooms.ContainsKey(coordW)) {
                    if (CalcRandomFace()) {
                        str += "E";
                    } else {
                        str += "_";
                    }
                } else {
                    str += "_";
                }

                if (rooms.ContainsKey(coordN) && rooms[coordN].isNorthOpen) {
                    str += "S";
                } else if (!rooms.ContainsKey(coordN)) {
                    if (CalcRandomFace()) {
                        str += "S";
                    } else {
                        str += "_";
                    }
                } else {
                    str += "_";
                }

                if (rooms.ContainsKey(coordE) && rooms[coordE].isEastOpen) {
                    str += "W";
                } else if(!rooms.ContainsKey(coordE)) {
                    if (CalcRandomFace()) {
                        str += "W";
                    } else {
                        str += "_";
                    }
                } else {
                    str += "_";
                }
                return str;
            } else {
                return "no";
            }
        } else {
            return "no";
        }
    }

    public GameObject SpawnPart(string partName, float x, float y) {
        var pos = new Vector3(x * 10.0f, 0, y * 10.0f);
        var part = Resources.Load("Rooms/" + partName);
        var go = Instantiate(part, pos, Quaternion.identity) as GameObject;
        go.transform.SetParent(GameObject.Find("Rooms").transform, true);
        go.transform.localPosition = pos;
		go.transform.rotation = GameObject.Find ("Rooms").transform.rotation;

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