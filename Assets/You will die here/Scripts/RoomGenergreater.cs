﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomGenergreater : MonoBehaviour {
    public Dictionary<string, RoomObject> rooms = new Dictionary<string, RoomObject>();
    public Dictionary<string, WallObject> walls = new Dictionary<string, WallObject>();

    public Vector2 worldMin = new Vector2();
    public Vector2 worldMax = new Vector2();

    public float roomSize = 10.0f;
    public int playerGenerationRadius = 2;

    public int tileRunners = 0;

    public void NewTileRunner() {
        tileRunners++;
        Debug.Log("NEW TILE RUNNER, COUNT: " + tileRunners);
    }

    public void LessTileRunner() {
        if( tileRunners > 0) {
            tileRunners--;
            Debug.Log("LESS TILE RUNNER, COUNT: " + tileRunners);
        }
        if ( tileRunners <= 0 ) {
            Debug.Log("NO MORE TILE RUNNERS, COUNT: " + tileRunners);
            tileRunners = 0;
            TimeForWalls();
        }
    }

    public void TimeForWalls() {
        Debug.Log("YEAH BITCH I FUCKIN LOVE WALLS");
        foreach (KeyValuePair<string, RoomObject> item in rooms) {
            var room = item.Value;
            if( IsFree(room.pos + God.WEST) ){
                SpawnWall("WallsHolder", room.pos, "W");
            } else if(IsFree(room.pos + God.EAST)){
                SpawnWall("WallsHolder", room.pos + God.EAST, "W");
            }

            if (IsFree(room.pos + God.SOUTH)) {
                SpawnWall("WallsHolder", room.pos, "S");
            } else if (IsFree(room.pos + God.NORTH)) {
                SpawnWall("WallsHolder", room.pos + God.NORTH, "S");
            }
            //if( !rooms.ContainsKey( God.Key(room.pos) )) {
            //
            //}
        }
    }

    public bool IsFree(Vector2 loc) {
        var coord = God.Key(loc);

        return !rooms.ContainsKey(coord);
    }

    public GameObject SpawnWall(string partName, Vector2 loc, string dir) {
        if( partName == null) {
            partName = "WallsHolder";
        }

        loc = loc.Round(0);
        var coord = God.Key(loc) + "_" + dir;

        Debug.LogWarning(partName + " YEAH " + loc + " Z " + dir);

        if (walls.ContainsKey(coord) || partName == null) {
            Debug.LogWarning("EITHER " + coord + " ALREADY EXISTS OR " + partName + " ISN'T A PART!");
            return null;
        }

        var pos = new Vector3(loc.x * roomSize, 0, loc.y * roomSize);
        var rot = GameObject.Find("Rooms").transform.rotation;

        if (dir == "W") {
            var d = God.WEST * (roomSize / 2.0f);
            pos += new Vector3(d.x, 0, d.y);
        }

        if (dir == "S") {
            var d = God.SOUTH * (roomSize / 2.0f);
            pos += new Vector3(d.x, 0, d.y);
            rot.y = 135.0f;
        }

        var part = Resources.Load("RoomParts/" + partName);
        var go = Instantiate(part, pos, Quaternion.identity) as GameObject;
        go.transform.SetParent(GameObject.Find("Rooms").transform, true);
        go.transform.localPosition = pos;
        go.transform.rotation = GameObject.Find("Rooms").transform.rotation;

        if (dir == "S") {
            var r = go.transform.rotation;
            r.y += 45.0f;
            go.transform.rotation = r;
        }

        go.name = coord + " " + partName;

        var wo = go.GetComponent<WallObject>();

        wo.pos = loc;
        wo.dir = dir;

        walls.Add(coord, wo);

        return go;
    }

    public GameObject SpawnPart(string partName, Vector2 loc) {
        loc = loc.Round(0);
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

        var ro = go.GetComponent<RoomObject>();

        ro.pos = loc;

        rooms.Add(coord, ro);

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
