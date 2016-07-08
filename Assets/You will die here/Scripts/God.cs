using UnityEngine;
using System.Collections;

[System.Flags]
public enum Direction {
    North, East, South, West,
}

public class God : MonoBehaviour {
    public static God main;

    public static Vector2 NORTH = new Vector2(-1,  0);
    public static Vector2 EAST  = new Vector2( 0,  1);
    public static Vector2 SOUTH = new Vector2( 1,  0);
    public static Vector2 WEST  = new Vector2( 0, -1);

    public static string Key(Vector2 loc) {
        loc = loc.Round(0);
        return loc.x.ToString() + "_" + loc.y.ToString();
    }

    public static Vector2 RandomDirection() {
        var i = Random.value;
        if (i <= 0.25f) {
            return NORTH;
        } else if (i > 0.25f && i <= 0.50f) {
            return EAST;
        } else if (i > 0.50f && i <= 0.75f) {
            return SOUTH;
        } else {
            return WEST;
        }
    }

    public static Vector2 DirectionToVector(Direction dir) {
        switch (dir) {
            case Direction.North:
                return NORTH;
            case Direction.East:
                return EAST;
            case Direction.South:
                return SOUTH;
            case Direction.West:
                return WEST;
            default:
                return Vector2.zero;
        }
    }

    void Awake() {
        if (main == null) {
            DontDestroyOnLoad(gameObject);
            main = this;
        } else if (main != this) {
            DestroyImmediate(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}


public static class BeeCoExtensions {
    public static float Round(this float value, int digits = 2) {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    public static Vector2 Round(this Vector2 v, int digits) {
        return new Vector2(v.x.Round(digits), v.y.Round(digits));
    }

    public static Vector2 Rotate(this Vector2 v, float degrees) {
        return Quaternion.Euler(0, 0, degrees) * v;
    }
}