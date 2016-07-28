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

    public float minDarkRoomRadius = 0.625f; // 0.625f
    public float maxDarkRoomRadius = 0.9375f; // 0.9375f

    public float chanceOfPhoto = 0.25f;
    public float chanceOfWallFloor = 0.33f;
    public float chanceOfFloor = 0.07f;
    public float chanceOfOversize = 0.03f;

    public string nextSceneName;

    public List<GameObject> decorationPhotos;
    public List<GameObject> decorationFloor;
    public List<GameObject> decorationOversize;

    public List<GameObject> crazyDecorationPhotos;
    public List<GameObject> crazyDecorationFloor;
    public List<GameObject> crazyDecorationOversize;

    private List<float> roomDistances = new List<float>();
    private float maxDistanceFromOrigin = 0.0f;
    private float sumOfDistancesFromOrigin = 0.0f;
    private int maxTileRunners = 0;

    public int totalSpawnedFurniture = 0;
    public int spawnedFurniture = 0;

    void OnLevelWasLoaded() {
        rooms = new Dictionary<string, RoomObject>();
        walls = new Dictionary<string, WallObject>();
        roomDistances = new List<float>();
        maxDistanceFromOrigin = 0.0f;
        worldMin = new Vector2();
        worldMax = new Vector2();
        tileRunners = 0;
        maxTileRunners = 0;
        totalSpawnedFurniture += spawnedFurniture;
        spawnedFurniture = 0;
        isDone = false;
    }

    public void NewTileRunner() {
        tileRunners++;
        maxTileRunners++;
    }

    public void AnalyzeRooms() {
        foreach (KeyValuePair<string, RoomObject> item in rooms) {
            var room = item.Value;

            if (room.isWalkable) {
                var dist = Vector2.Distance(Vector2.zero, room.pos);
                roomDistances.Add(dist);

                sumOfDistancesFromOrigin += dist;

                if( dist > maxDistanceFromOrigin ){
                    maxDistanceFromOrigin = dist;
                }
            }
        }
        var avg = sumOfDistancesFromOrigin / roomDistances.Count;
        Debug.Log("MAX DISTANCE: " + maxDistanceFromOrigin + "; AVG DISTANCE: " + avg);
        Debug.Log("MIN: " + (minDarkRoomRadius * maxDistanceFromOrigin));
        Debug.Log("MAX: " + (maxDarkRoomRadius * maxDistanceFromOrigin));
    }

    public void LessTileRunner() {
        if( tileRunners > 0) {
            tileRunners--;
        }
        if ( tileRunners <= 0 ) {
            Debug.Log("TOTAL RUNNERS THIS FLOOR: " + maxTileRunners);
            Debug.Log("FINAL NUMBER OF ROOMS: " + rooms.Count);
            tileRunners = 0;
            AnalyzeRooms();
            EnforceDarkRooms();
            //DestroyRoomAt(God.NORTH + God.NORTH);
            //SpawnPart("BlackRoom", God.NORTH + God.NORTH);
            TimeForWalls();
            TimeForDoors();
            TimeForDecoration();
            OnDone();
        }
    }

    public void OnDone() {
        var gramps = GameObject.Find("GrampsHolder").GetComponent<Grandpa>();
        gramps.UnFreeze();
        var fc = Camera.main.GetComponent<FogController>();
        var snap = fc.GetFogSnapshot();
        snap.color = (Color)(new Color32(189, 189, 189, 255));
        snap.startDistance = 8.0f;
        snap.endDistance = 16.0f;
        fc.Change(snap, 2.0f, 2.0f);
    }

    public void DestroyRoomAt(Vector2 pos) {
        var coord = God.Key(pos);

        if( rooms.ContainsKey(coord) ) {
            Destroy(rooms[coord].gameObject);
            rooms.Remove(coord);
        }
    }

    public void EnforceDarkRooms() {
        int numDarkRooms = 0;
        List<Vector2> badPoints = new List<Vector2>();
        foreach (KeyValuePair<string, RoomObject> item in rooms) {
            var room = item.Value;

            if (room.isDarkRoom) {
                var dist = Vector2.Distance(room.pos, Vector2.zero);
                if ( dist > (maxDarkRoomRadius * maxDistanceFromOrigin) || dist < (minDarkRoomRadius * maxDistanceFromOrigin)  ){
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
        Dictionary<string, Vector2> candidates = new Dictionary<string, Vector2>();
        if( numDarkRooms < minDarkRooms) {
            for( int d = 0; d <= 360; d++ ){
                for( float i = (minDarkRoomRadius * maxDistanceFromOrigin); i < (maxDarkRoomRadius * maxDistanceFromOrigin); i++) {
                    var loc = Vector2.zero.PointOnCircle(i, d);
                    var coord = God.Key(loc);
                    if (!candidates.ContainsKey(coord) && !rooms.ContainsKey(coord) && HasNeighbor(loc)) {
                        candidates[coord] = loc;
                    }
                }
            }
            List<Vector2> possible = new List<Vector2>(candidates.Values);
            possible.Shuffle();

            foreach( Vector2 pos in possible ) {
                SpawnPart("BlackRoom", pos);
                numDarkRooms++;
                if ( numDarkRooms >= minDarkRooms ) {
                    break;
                }
            }

            var roomsLeft = minDarkRooms - numDarkRooms;
            var emergencyRooms = roomsLeft - possible.Count;

            if( emergencyRooms > 0) {
                Debug.LogWarning("everybody knows shit's fucked");
            }

            /*
            while( emergencyRooms > 0) {
                Debug.LogWarning("CREATING EMERGENCY ROOMS");
                var mag = Random.Range((minDarkRoomRadius * maxDistanceFromOrigin), (maxDarkRoomRadius * maxDistanceFromOrigin));
                var rad = Vector2.zero.RandomCircle(mag);
                // var dist = Vector2.Distance(rad, Vector2.zero);

                var coord = God.Key(rad);

                if( rooms.ContainsKey(God.Key(rad)) && !rooms[coord].isDarkRoom ){
                    DestroyRoomAt(rad);
                    SpawnPart("BlackRoom", rad);
                    emergencyRooms--;
                }
            }
            */
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

                // No huge props at spawn coordinates.
                if( room.pos != Vector2.zero) {
                    foreach (GameObject oa in room.oversizeAnchors) {
                        if (Random.value <= chanceOfOversize) {
                            var i = Random.Range(0, decorationOversize.Count);
                            SpawnFurniture(decorationOversize[i], oa);
                        }
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
                var dirs = new List<Vector2>{ God.NORTH, God.EAST, God.SOUTH, God.WEST };
                dirs.Shuffle();
                var door = Resources.Load("RoomParts/" + "DoorFrame") as GameObject;
                var didSpawn = false;
                foreach ( Vector2 dir in dirs) {
                    if( !IsFree(room.pos + dir) ){
                        var inside = true;
                        var wallPos = room.pos;
                        var face = "W";

                        if( dir == God.NORTH || dir == God.EAST) {
                            wallPos += dir;
                            inside = false;
                        }

                        if( dir == God.NORTH || dir == God.SOUTH) {
                            face = "S";
                        }

                        var coord = God.Key(wallPos) + "_" + face;
                        GameObject goToSpawnOn = null;
                        if ( walls.ContainsKey(coord)) {
                            if( inside) {
                                goToSpawnOn = walls[coord].insideDoor;
                                foreach(GameObject go in walls[coord].insideBlockedByDoor) {
                                    Destroy(go);
                                }
                            } else {
                                goToSpawnOn = walls[coord].outsideDoor;
                                foreach (GameObject go in walls[coord].outsideBlockedByDoor) {
                                    Destroy(go);
                                }
                            }
                            SpawnFurniture(door, goToSpawnOn);
                            didSpawn = true;

                            // We spawned at least one door, so break.
                            break;
                        } else {
                            Debug.LogError("THERE WAS NO FUCKING WALL AT " + coord);
                        }
                    }
                }
                if( !didSpawn ) {
                    Debug.LogError("COULDN'T FUCKIN PUT A DOOR ON " + God.Key(room.pos));
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
        // Ensure we got an object.
        if (prefab == null) {
            return null;
        }

        // Actually spawn the object.
        var go = Instantiate(prefab, parent.transform.position, parent.transform.rotation) as GameObject;
        go.transform.SetParent(parent.transform, true);

        //go.transform.localPosition = pos;
        //go.transform.rotation = GameObject.Find("World/Rooms").transform.rotation;

        //go.name = coord + " " + partPath;

        spawnedFurniture++;

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
