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

    public int minDarkRooms = 1;

    public float minDarkRoomRadius = 10.0f;
    public float maxDarkRoomRadius = 80.0f;

    public float chanceOfPhoto = 0.25f;
    public float chanceOfWallFloor = 0.33f;
    public float chanceOfFloor = 0.07f;
    public float chanceOfOversize = 0.03f;

    public List<GameObject> decorationPhotos;
    public List<GameObject> decorationFloor;
    public List<GameObject> decorationOversize;

    public List<GameObject> crazyDecorationPhotos;
    public List<GameObject> crazyDecorationFloor;
    public List<GameObject> crazyDecorationOversize;

    public void NewTileRunner() {
        tileRunners++;
    }

    public void LessTileRunner() {
        if( tileRunners > 0) {
            tileRunners--;
        }
        if ( tileRunners <= 0 ) {
            tileRunners = 0;
            EnforceDarkRooms();
            TimeForDoors();
            TimeForWalls();
            TimeForDecoration();
        }
    }

    public void OnDone() {

    }

    public void DestroyRoomAt(Vector2 pos) {
        var coord = God.Key(pos);

        Destroy(rooms[coord].gameObject);
        rooms.Remove(coord);
    }

    public void EnforceDarkRooms() {
        int numDarkRooms = 0;
        List<Vector2> badPoints = new List<Vector2>();
        foreach (KeyValuePair<string, RoomObject> item in rooms) {
            var room = item.Value;

            if (room.isDarkRoom) {
                var dist = Vector2.Distance(room.pos, Vector2.zero);
                if ( dist > maxDarkRoomRadius || dist < minDarkRoomRadius ){
                    Debug.LogWarning("ROOM OUT OF RANGE, " + room.pos);
                    badPoints.Add(room.pos);
                } else {
                    numDarkRooms++;
                }
            }
        }
        foreach( Vector2 pos in badPoints) {
            DestroyRoomAt( pos );
        }
        while( numDarkRooms < minDarkRooms ) {
            var mag = Random.Range(minDarkRoomRadius, maxDarkRoomRadius);
            var rad = Vector2.zero.RandomCircle( mag );
            var dist = Vector2.Distance(rad, Vector2.zero);

            if (dist > maxDarkRoomRadius || dist < minDarkRoomRadius) {
                Debug.LogWarning("WOW I'M RETARDED! " + mag + ", " + rad + ", " + dist);
            }

            if (!rooms.ContainsKey(God.Key(rad)) && HasNeighbor(rad) ) {
                SpawnPart("BlackRoom", rad);
                numDarkRooms++;
            } else {
                Debug.LogWarning("ATTEMPTED " + rad.Round(0) + " FAILED");
            }
        }
    }

    public void TimeForDecoration() {
        foreach (KeyValuePair<string, RoomObject> item in rooms) {
            var room = item.Value;

            if (room.isWalkable) {
                foreach (GameObject fa in room.floorAnchors) {
                    if (Random.value <= chanceOfFloor) {
                        var i = Random.Range(0, decorationFloor.Count);
                        SpawnFurniture(decorationFloor[i], fa);
                    }
                }
                foreach (GameObject oa in room.oversizeAnchors) {
                    if (Random.value <= chanceOfOversize) {
                        var i = Random.Range(0, decorationOversize.Count);
                        SpawnFurniture(decorationOversize[i], oa);
                    }
                }
            }
        }
        foreach (KeyValuePair<string, WallObject> item in walls) {
            var wall = item.Value;
            var loc = wall.pos.Round(0);

            var inside = false;
            var outside = false;

            if( wall.dir == "W") {
                if( !IsFree( loc + God.WEST ) ){
                    inside = true;
                }
                if( !IsFree( loc ) ){
                    outside = true;
                }
            }

            if (wall.dir == "S") {
                if (!IsFree(loc + God.SOUTH)) {
                    inside = true;
                }
                if (!IsFree(loc)) {
                    outside = true;
                }
            }

            if (inside) {
                foreach (GameObject ipa in wall.insidePhotoAnchors) {
                    if (Random.value <= chanceOfPhoto) {
                        var i = Random.Range(0, decorationPhotos.Count);
                        SpawnFurniture(decorationPhotos[i], ipa);
                    }
                }
                foreach (GameObject ifa in wall.insideFloorAnchors) {
                    if (Random.value <= chanceOfWallFloor) {
                        var i = Random.Range(0, decorationFloor.Count);
                        SpawnFurniture(decorationFloor[i], ifa);
                    }
                }
            }

            if (outside) {
                foreach (GameObject opa in wall.outsidePhotoAnchors) {
                    if (Random.value <= chanceOfPhoto) {
                        var i = Random.Range(0, decorationPhotos.Count);
                        SpawnFurniture(decorationPhotos[i], opa);
                    }
                }
                foreach (GameObject ofa in wall.outsideFloorAnchors) {
                    if (Random.value <= chanceOfWallFloor) {
                        var i = Random.Range(0, decorationFloor.Count);
                        SpawnFurniture(decorationFloor[i], ofa);
                    }
                }
            }
        }
    }

    public void EmergencyDoor(RoomObject room) {
        Debug.LogWarning("EMERGENCY STATE: ATTEMPTING TO DETECT");
        var doors = 0;
        if (!IsFree(room.pos + God.WEST)) {
            SpawnThing("DoorFrame", room.pos + God.WEST, "E");
            doors++;
        }
        if (!IsFree(room.pos + God.EAST)) {
            SpawnThing("DoorFrame", room.pos + God.EAST, "W");
            doors++;
        }
        if (!IsFree(room.pos + God.SOUTH)) {
            SpawnThing("DoorFrame", room.pos + God.SOUTH, "N");
            doors++;
        }
        if (!IsFree(room.pos + God.NORTH)) {
            SpawnThing("DoorFrame", room.pos + God.NORTH, "S");
            doors++;
        }
        if( doors <= 0 ){
            Debug.LogWarning("EMERGENCY FAILED I'M SORRY");
        }
    }

    public void TimeForDoors() {
        foreach (KeyValuePair<string, RoomObject> item in rooms) {
            var room = item.Value;

            if (room.isDarkRoom) {
                Vector2 i = God.RandomDirection();
                var numAttempts = 16;
                while ( IsFree(room.pos + i) ) {
                    i = God.RandomDirection();
                    numAttempts--;
                    if( numAttempts <= 0) {
                        Debug.LogWarning("GAVE UP PLACING DOOR ON BLACKCUBE AT " + room.pos);
                        break;
                    }
                }
                
                if( i == God.WEST && !IsFree(room.pos + God.WEST) ) {
                    SpawnThing("DoorFrame", room.pos + God.WEST, "E");
                } else if ( i == God.EAST && !IsFree(room.pos + God.EAST) ) {
                    SpawnThing("DoorFrame", room.pos + God.EAST, "W");
                } else if ( i == God.SOUTH && !IsFree(room.pos + God.SOUTH) ) {
                    SpawnThing("DoorFrame", room.pos + God.SOUTH, "N");
                } else if ( i == God.NORTH && !IsFree(room.pos + God.NORTH)) {
                    SpawnThing("DoorFrame", room.pos + God.NORTH, "S");
                } else {
                    EmergencyDoor(room);
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

    public bool HasNeighbor(Vector2 loc) {
        return !IsFree(loc + God.NORTH) || !IsFree(loc + God.EAST) || !IsFree(loc + God.SOUTH) || !IsFree(loc + God.WEST);
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

    public GameObject SpawnFurniture(GameObject prefab, GameObject parent) {
        // Actually spawn the object.
        var go = Instantiate(prefab, parent.transform.position, parent.transform.rotation) as GameObject;
        go.transform.SetParent(parent.transform, true);
        //go.transform.localPosition = pos;
        //go.transform.rotation = GameObject.Find("World/Rooms").transform.rotation;

        //go.name = coord + " " + partPath;

        return go;
    }

    public GameObject SpawnThing(string partPath, Vector2 loc, string dir = null) {
        loc = loc.Round(0);
        var coord = God.Key(loc);

        var isWall = false;
        var isFloor = false;

        var pos = new Vector3(loc.x * roomSize, 0, loc.y * roomSize);

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
                go.transform.RotateAround(go.transform.position, Vector3.up, 90);
            } else if (dir == "S") {
                go.transform.RotateAround(go.transform.position, Vector3.up, 270);
            } else if (dir == "E") {
                go.transform.RotateAround(go.transform.position, Vector3.up, 180);
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
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
