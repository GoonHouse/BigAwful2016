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

    public int tileRunners = 0;

    public bool isDone = false;

    public void NewTileRunner() {
        tileRunners++;
    }

    public void LessTileRunner() {
        if( tileRunners > 0) {
            tileRunners--;
        }
        if ( tileRunners <= 0 ) {
            tileRunners = 0;
            TimeForDoors();
            TimeForWalls();
        }
    }

    public void OnDone() {

    }

    public void TimeForDoors() {
        Debug.LogWarning("OKAY TOM I'M MAKING DOORS NOW");
        foreach (KeyValuePair<string, RoomObject> item in rooms) {
            var room = item.Value;

            if (room.isDarkRoom) {
                Debug.LogWarning("CONSIDERING PLANTING A DOOR NEAR " + room.pos);
                Vector2 i = God.RandomDirection();
                while ( IsFree(room.pos + i) ) {
                    i = God.RandomDirection();
                }
                
                if( i == God.WEST && !IsFree(room.pos + God.WEST) ) {
                    SpawnThing("DoorFrame", room.pos + God.WEST, "E");
                    Debug.LogWarning("I PICKED WEST!");
                } else if ( i == God.EAST && !IsFree(room.pos + God.EAST) ) {
                    SpawnThing("DoorFrame", room.pos + God.EAST, "W");
                    Debug.LogWarning("I PICKED EAST!");
                } else if ( i == God.SOUTH && !IsFree(room.pos + God.SOUTH) ) {
                    SpawnThing("DoorFrame", room.pos + God.SOUTH, "N");
                    Debug.LogWarning("I PICKED SOUTH!");
                } else if ( i == God.NORTH && !IsFree(room.pos + God.NORTH)) {
                    SpawnThing("DoorFrame", room.pos + God.NORTH, "S");
                    Debug.LogWarning("I PICKED NORTH!");
                } else {
                    Debug.LogWarning("JACK SHIT HAPPENED!");
                }
            }
        }
    }

    public void TimeForWalls() {
        foreach (KeyValuePair<string, RoomObject> item in rooms) {
            var room = item.Value;

            if( room.isWalkable ) {
                if (IsFree(room.pos + God.WEST)) {
                    SpawnWall("WallsHolder", room.pos, "W");
                }
                if (IsFree(room.pos + God.EAST)) {
                    SpawnWall("WallsHolder", room.pos + God.EAST, "W");
                }

                if (IsFree(room.pos + God.SOUTH)) {
                    SpawnWall("WallsHolder", room.pos, "S");
                }
                if (IsFree(room.pos + God.NORTH)) {
                    SpawnWall("WallsHolder", room.pos + God.NORTH, "S");
                }
            }
        }
    }

    public bool IsFree(Vector2 loc) {
        var coord = God.Key(loc);

        RoomObject room;
        rooms.TryGetValue(coord, out room);

        if( room != null) {
            return !room.isWalkable;
        } else {
            return true;
        }
    }

    public GameObject SpawnThing(string partPath, Vector2 loc, string dir = null) {
        loc = loc.Round(0);
        var coord = God.Key(loc);

        var isWall = false;
        var isFloor = false;

        var pos = new Vector3(loc.x * roomSize, 0, loc.y * roomSize);
        var rot = GameObject.Find("World/Rooms").transform.rotation;

        if ( dir != null) {
            coord += "_" + dir;
            isWall = true;
            isFloor = false;

            Vector2 d = Vector2.zero;
            if (dir == "N") {
                d = God.NORTH;
            } else if (dir == "E") {
                d = God.EAST;
            } else if (dir == "S") {
                d = God.SOUTH;
            } else if (dir == "W") {
                d = God.WEST;
            } else {
                isWall = false;
            }
            
            d = d * (roomSize / 2.0f);
            pos += new Vector3(d.x, 0, d.y);
        } else {
            isWall = false;
            isFloor = true;
        }

        if( ( isWall && walls.ContainsKey(coord) ) || ( isFloor && rooms.ContainsKey(coord) )){
            return null;
        }

        // Actually spawn the object.
        var part = Resources.Load("RoomParts/" + partPath);
        var go = Instantiate(part, pos, Quaternion.identity) as GameObject;
        go.transform.SetParent(GameObject.Find("World/Rooms").transform, true);
        go.transform.localPosition = pos;
        go.transform.rotation = GameObject.Find("World/Rooms").transform.rotation;

        go.name = coord + " " + partPath;
        if( dir != null) {
            if (dir == "N") {
                var r = go.transform.rotation;
                go.transform.RotateAround(go.transform.position, Vector3.up, 90);
            } else if (dir == "S") {
                var r = go.transform.rotation;
                go.transform.RotateAround(go.transform.position, Vector3.up, 270);
            } else if (dir == "E") {
                var r = go.transform.rotation;
                go.transform.RotateAround(go.transform.position, Vector3.up, 180);
            } else {
                Debug.Log("I hope i didn't mess up");
            }
        }

        if ( isWall ){
            var wo = go.GetComponent<WallObject>();

            if( wo != null ) {
                wo.pos = loc;
                wo.dir = dir;

                walls.Add(coord, wo);
            }
        } else if( isFloor ){
            var ro = go.GetComponent<RoomObject>();

            if (ro != null) {
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
            }
        }
        
        return go;
    }

    /*
    public GameObject SpawnItem(string partPath, GameObject childToSpawn, Vector2 loc, ) {
        if( childToSpawn == null) {
            childToSpawn = GameObject.Find("World/Rooms");
        }
        var rpos = new Vector3(loc.x * roomSize, 0, loc.y * roomSize);
        var pos = childToSpawn.transform.position + rpos;

        var part = Resources.Load("RoomParts/" + partPath);
        var go = Instantiate(part, pos, Quaternion.identity) as GameObject;

        go.transform.SetParent(childToSpawn.transform, true);
        go.transform.localPosition = pos;
        go.transform.rotation = childToSpawn.transform.rotation;
    }
    public GameObject SpawnItem(string partPath, Vector2 loc, string dir = null) {
        if (partPath == null) {
            partPath = "WallsHolder";
        }

        loc = loc.Round(0);
        var coord = God.Key(loc) + "_" + dir;

        //if (walls.ContainsKey(coord) || partName == null) {
        //Debug.LogWarning("EITHER " + coord + " ALREADY EXISTS OR " + partName + " ISN'T A PART!");
        //return null;
        //}

        var pos = new Vector3(loc.x * roomSize, 0, loc.y * roomSize);
        var rot = GameObject.Find("World/Rooms").transform.rotation;

        if (dir == "W") {
            var d = God.WEST * (roomSize / 2.0f);
            pos += new Vector3(d.x, 0, d.y);
        }

        if (dir == "S") {
            var d = God.SOUTH * (roomSize / 2.0f);
            pos += new Vector3(d.x, 0, d.y);
        }

        

        if (dir == "S") {
            var r = go.transform.rotation;
            go.transform.RotateAround(go.transform.position, Vector3.up, 270);
        }

        go.name = coord + " " + partName;

        //var wo = go.GetComponent<WallObject>();

        //wo.pos = loc;
        //wo.dir = dir;

        //walls.Add(coord, wo);

        return go;
    }
    */
    public GameObject SpawnWall(string partName, Vector2 loc, string dir) {
        if( partName == null) {
            partName = "WallsHolder";
        }

        loc = loc.Round(0);
        var coord = God.Key(loc) + "_" + dir;

        if (walls.ContainsKey(coord) || partName == null) {
            Debug.LogWarning("EITHER " + coord + " ALREADY EXISTS OR " + partName + " ISN'T A PART!");
            return null;
        }

        var pos = new Vector3(loc.x * roomSize, 0, loc.y * roomSize);
        var rot = GameObject.Find("World/Rooms").transform.rotation;

        if (dir == "W") {
            var d = God.WEST * (roomSize / 2.0f);
            pos += new Vector3(d.x, 0, d.y);
        }

        if (dir == "S") {
            var d = God.SOUTH * (roomSize / 2.0f);
            pos += new Vector3(d.x, 0, d.y);
        }

        var part = Resources.Load("RoomParts/" + partName);
        var go = Instantiate(part, pos, Quaternion.identity) as GameObject;
        go.transform.SetParent(GameObject.Find("World/Rooms").transform, true);
        go.transform.localPosition = pos;
        go.transform.rotation = GameObject.Find("World/Rooms").transform.rotation;

        if (dir == "S") {
            var r = go.transform.rotation;
            go.transform.RotateAround(go.transform.position, Vector3.up, 270);
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
        SpawnPart("BlackRoom", God.NORTH);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
