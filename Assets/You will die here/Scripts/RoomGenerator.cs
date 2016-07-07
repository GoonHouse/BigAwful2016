using UnityEngine;
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

    public float roomSize = 10.0f;
    public int playerGenerationRadius = 2;
    public float chanceOfWall = 0.75f;
    public float chanceOfDudRoom = 0.10f;

    private Vector2 D_NORTH = new Vector2(-1,  0);
    private Vector2 D_EAST  = new Vector2( 0,  1);
    private Vector2 D_SOUTH = new Vector2( 1,  0);
    private Vector2 D_WEST  = new Vector2( 0, -1);

    // Use this for initialization
    void Start () {
        var h = new Vector2();
        SpawnPart("NESW", h);
        SpawnPart("NESW", h + D_NORTH);
        SpawnPart("NESW", h + D_NORTH + D_EAST);
        SpawnPart("NESW", h + D_NORTH + D_WEST);
        SpawnPart("NESW", h + D_EAST );
        SpawnPart("NESW", h + D_SOUTH);
        SpawnPart("NESW", h + D_SOUTH + D_EAST);
        SpawnPart("NESW", h + D_SOUTH + D_WEST);
        SpawnPart("NESW", h + D_WEST );

        /*
        SpawnPart("NE_W", 1, 0);  // north =  1,  0
        SpawnPart("NES_", 0, -1); // east  =  0, -1
        SpawnPart("_ESW", -1, 0); // south = -1,  0
        SpawnPart("N_SW", 0, 1);  // west  =  0,  1
        */

        for (int i = 0; i < 500; i++) {
            DoGenerate();
		}
    }
    public void DoGenerate() {
        var r = new Vector2();
        r.x = Random.Range((int)(worldMin.x - 2.0f), (int)(worldMax.x + 2.0f));
        r.y = Random.Range((int)(worldMin.y - 2.0f), (int)(worldMax.y + 2.0f));

        MaybeSpawnAt(r);
    }
	
    public void Generate() {
        generateTimer -= Time.deltaTime;
        if (generateTimer <= 0.0f) {
            generateTimer += generateFrequency;
            DoGenerate();
        }
    }

    public void PutFloorNearPlayer(Vector2 loc) {
        for (int i = -playerGenerationRadius; i <= playerGenerationRadius; i++) {
            for (int j = -playerGenerationRadius; j <= playerGenerationRadius; j++) {
                MaybeSpawnAt(loc + new Vector2(i, j));
            }
        }
    }

	// Update is called once per frame
	void Update () {
        Generate();

        var pos = GetGridFromWorldPosition(GameObject.Find("GrampsHolder").transform);
        // Spawn an area around the player.
        PutFloorNearPlayer(pos);

        if (Input.GetKeyDown(KeyCode.F)) {
            Debug.Log("PLAYER AT: " + Key(pos));
        }
        if (Input.GetKeyDown(KeyCode.G)) {
            PutFloorNearPlayer(pos);
        }
    }

    public void MaybeSpawnAt(Vector2 loc) {
        var probe = ProbeSpot(loc);
        if( probe != null ){
            SpawnPart(probe, loc);
        }
    }

    public void MaybeFuckOrbAt(Vector2 loc) {
        if( !CalcDudRoom() ) {
            return;
        }
        RoomObject roomN, roomE, roomS, roomW;
        rooms.TryGetValue(Key(loc + D_NORTH), out roomN);
        rooms.TryGetValue(Key(loc + D_EAST), out roomE);
        rooms.TryGetValue(Key(loc + D_SOUTH), out roomS);
        rooms.TryGetValue(Key(loc + D_WEST), out roomW);

        var numNeighbors = 0;
        var numWalls = 0;

        if( roomN ){
            numNeighbors++;
            if( !roomN.isSouthOpen ){
                numWalls++;
            }
        }

        if( roomE ) {
            numNeighbors++;
            if( !roomE.isWestOpen ){
                numWalls++;
            }
        }

        if (roomS) {
            numNeighbors++;
            if (!roomS.isNorthOpen) {
                numWalls++;
            }
        }

        if (roomW) {
            numNeighbors++;
            if (!roomW.isEastOpen) {
                numWalls++;
            }
        }

        if( numWalls == numNeighbors) {
            SpawnPart("____", loc);
        }
    }

    public bool CalcRandomFace() {
        return Random.value <= chanceOfWall;
    }

    public bool CalcDudRoom() {
        return Random.value <= chanceOfDudRoom;
    }

    public Vector2 GetGridFromWorldPosition(Transform pos) {
        return GetGridFromWorldPosition(pos.localPosition.x, pos.localPosition.y, pos.localPosition.z);
    }

    public Vector2 GetGridFromWorldPosition(float x, float y, float z) {
        var dist = roomSize / 2.0f;

        var oX = Mathf.CeilToInt((Mathf.Abs(x) - dist) / roomSize);
        var oY = Mathf.CeilToInt((Mathf.Abs(z) - dist) / roomSize);

        if( Mathf.Abs(x) <= dist) {
            oX = 0;
        }
        if (Mathf.Abs(z) <= dist) {
            oY = 0;
        }
        if( x < 0.0f ) {
            oX *= -1;
        }
        if (z < 0.0f) {
            oY *= -1;
        }
        return new Vector2(oX, oY);
    }

    public string Key(Vector2 loc) {
        return loc.x.ToString() + "_" + loc.y.ToString();
    }

    public bool CheckHasAnyNeighbors(Vector2 loc) {
        var coordN = Key(loc + D_NORTH);
        var coordE = Key(loc + D_EAST);
        var coordS = Key(loc + D_SOUTH);
        var coordW = Key(loc + D_WEST);

        return (
            rooms.ContainsKey(coordN) || rooms.ContainsKey(coordE) || 
            rooms.ContainsKey(coordS) || rooms.ContainsKey(coordW)
        );
    }

    public string ProbeSpot(Vector2 loc) {
        var str = "";

        var coord = Key(loc);

        RoomObject roomN, roomE, roomS, roomW;
        rooms.TryGetValue(Key(loc + D_NORTH), out roomN);
        rooms.TryGetValue(Key(loc + D_EAST ), out roomE);
        rooms.TryGetValue(Key(loc + D_SOUTH), out roomS);
        rooms.TryGetValue(Key(loc + D_WEST ), out roomW);

        if (rooms.ContainsKey(coord)) {
            return null;
        }
        if (!CheckHasAnyNeighbors(loc)) {
            return "____";
        }

        if (roomN && roomN.isSouthOpen) {
            str += "N";
        } else if (roomN == null) {
            if ( CalcRandomFace() ) {
                str += "N";
            } else {
                str += "_";
                MaybeFuckOrbAt(loc + D_NORTH + D_NORTH);
            }
        } else {
            str += "_";
            MaybeFuckOrbAt(loc + D_NORTH + D_NORTH);
        }

        if (roomE && roomE.isWestOpen) {
            str += "E";
        } else if (roomE == null) {
            if (CalcRandomFace()) {
                str += "E";
            } else {
                str += "_";
                MaybeFuckOrbAt(loc + D_EAST + D_EAST);
            }
        } else {
            str += "_";
            MaybeFuckOrbAt(loc + D_EAST + D_EAST);
        }

        if (roomS && roomS.isNorthOpen) {
            str += "S";
        } else if (roomS == null) {
            if (CalcRandomFace()) {
                str += "S";
            } else {
                str += "_";
                MaybeFuckOrbAt(loc + D_SOUTH + D_SOUTH);
            }
        } else {
            str += "_";
            MaybeFuckOrbAt(loc + D_SOUTH + D_SOUTH);
        }

        if (roomW && roomW.isEastOpen) {
            str += "W";
        } else if (roomW == null) {
            if (CalcRandomFace()) {
                str += "W";
            } else {
                str += "_";
                MaybeFuckOrbAt(loc + D_WEST + D_WEST);
            }
        } else {
            str += "_";
            MaybeFuckOrbAt(loc + D_WEST + D_WEST);
        }

        return str;
    }

    public GameObject SpawnPart(string partName, Vector2 loc) {
        var coord = Key(loc);

        if ( rooms.ContainsKey(coord) || partName == null ) {
            Debug.LogWarning("EITHER " + coord + " ALREADY EXISTS OR " + partName + " ISN'T A PART!");
            return null;
        }

        var pos = new Vector3(loc.x * roomSize, 0, loc.y * roomSize);
        var part = Resources.Load("Rooms/" + partName);
        var go = Instantiate(part, pos, Quaternion.identity) as GameObject;
        go.transform.SetParent(GameObject.Find("Rooms").transform, true);
        go.transform.localPosition = pos;
		go.transform.rotation = GameObject.Find ("Rooms").transform.rotation;

        

        go.name = coord + " " + partName;

        rooms.Add( coord, go.GetComponent<RoomObject>() );

        if( loc.x < worldMin.x ) {
            worldMin.x = loc.x;
        } else if( loc.x > worldMax.x) {
            worldMax.x = loc.x;
        }

        if( loc.y < worldMin.y ) {
            worldMin.y = loc.y;
        } else if( loc.y > worldMax.y ) {
            worldMax.y = loc.y;
        }

        return go;
    }
}
