using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public static string VectorToInvertedSignal(Vector2 pos) {
        if (pos == NORTH) {
            return "S";
        } else if (pos == EAST) {
            return "W";
        } else if (pos == SOUTH) {
            return "N";
        } else if (pos == WEST) {
            return "E";
        } else {
            return "?";
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


    public static float Scale(this float valueIn, float baseMin, float baseMax, float limitMin, float limitMax) {
        return ((limitMax - limitMin) * (valueIn - baseMin) / (baseMax - baseMin)) + limitMin;
    }

    public static void Shuffle<T>(this IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static float Round(this float value, int digits = 2) {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    public static Vector2 PointOnCircle(this Vector2 center, float radius, float ang) {
        Vector2 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    public static Vector2 RandomCircle(this Vector2 center, float radius) {
        float ang = Random.value * 360;
        Vector2 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    public static Vector2 Round(this Vector2 v, int digits) {
        return new Vector2(v.x.Round(digits), v.y.Round(digits));
    }

    public static Vector2 Rotate(this Vector2 v, float degrees) {
        return Quaternion.Euler(0, 0, degrees) * v;
    }
}