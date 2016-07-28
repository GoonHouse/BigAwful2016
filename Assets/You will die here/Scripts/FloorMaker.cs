using UnityEngine;
using System.Collections;

public class FloorMaker : MonoBehaviour {
    public GameObject moreOfMe;

    public Vector2 writeHead = new Vector2();
    public Vector2 writeDirection = new Vector2();

    public float tilesLeft = 110;

    public float operationRate = 1.0f;
    public float operationTimer = 0.0f;

    public float chanceToTurnLeft = 0.33f;
    public float chanceToTurnAround = 0.34f;
    public float chanceToTurnRight = 0.33f;

    public float chanceToClone = 0.05f;
    public float chanceToCloneDecay = 0.50f;

    public float chanceOfBlackRoom = 0.10f;

    private RoomGenergreater rg;

    void Clone(Vector2 loc) {
        // Create a clone. No frills.
        loc = loc.Round(0);
        var pos = new Vector3(loc.x * rg.roomSize, 0, loc.y * rg.roomSize);
        var go = Instantiate(moreOfMe, pos, Quaternion.identity) as GameObject;
        go.transform.SetParent(GameObject.Find("Rooms").transform, true);
        go.transform.localPosition = pos;
        go.transform.rotation = GameObject.Find("Rooms").transform.rotation;

        // Absorb stats.
        var fm = go.GetComponent<FloorMaker>();

        // Tiles
        var passedTiles = tilesLeft / 2;
        fm.tilesLeft = passedTiles;
        tilesLeft -= passedTiles;

        // Additional clone chances.
        chanceToClone *= chanceToCloneDecay;
        fm.chanceToClone = chanceToClone;

        // Pick a random direction.
        fm.writeHead = writeHead;
        fm.writeDirection = God.RandomDirection();

        // Reset rate to set apart from current writer.
        fm.operationRate = operationRate;
        fm.operationTimer = operationRate;

        // Other chances.
        fm.chanceToTurnLeft = chanceToTurnLeft;
        fm.chanceToTurnAround = chanceToTurnAround;
        fm.chanceToTurnRight = chanceToTurnRight;

        fm.chanceOfBlackRoom = chanceOfBlackRoom;
    }

    // Use this for initialization
    void Start () {
        writeDirection = God.RandomDirection();
        rg = God.main.GetComponent<RoomGenergreater>();
        rg.NewTileRunner();
    }
	
	// Update is called once per frame
	void Update () {
        operationTimer -= Time.deltaTime;
        if( operationTimer <= 0.0f ) {
            operationTimer += operationRate;

            // Draw a tile OR draw a huge room.
            if( rg.IsFree(writeHead) ){
                DrawTile();
            }

            // Move.
            Move();

            // Consider cloning?
            if (Random.value <= chanceToClone) {
                Clone(writeHead);
            }

            // Consider turning?
            ConsiderTurning();
        }
	}

    public void ConsiderTurning() {
        var turning = false;
        if (Random.value <= chanceToTurnLeft && !turning) {
            turning = true;
            writeDirection = writeDirection.Rotate(90);
        }
        if (Random.value <= chanceToTurnAround && !turning) {
            OnTurnAround();
            turning = true;
            writeDirection = writeDirection.Rotate(180);
        }
        if (Random.value <= chanceToTurnRight && !turning) {
            turning = true;
            writeDirection = writeDirection.Rotate(90);
        }
    }

    public void OnTurnAround() {
        if( Random.value <= chanceOfBlackRoom ){
            rg.SpawnPart("BlackRoom", writeHead);
        } else {
            rg.SpawnPart("VoidRoom", writeHead);
        }
    }
    
    public void Move() {
        writeHead += writeDirection;
        transform.localPosition = new Vector3(writeHead.x * rg.roomSize, 0.0f, writeHead.y * rg.roomSize);
    }

    public void DrawTile() {
        if (tilesLeft > 0) {
            tilesLeft--;
            rg.SpawnPart("Floor", writeHead);
        } else {
            Destroy(gameObject);
        }
    }

    public void OnDestroy() {
        if( rg ) {
            rg.LessTileRunner();
        }
    }
}
