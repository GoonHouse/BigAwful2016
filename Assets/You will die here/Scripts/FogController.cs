using UnityEngine;
using System.Collections;

[System.Serializable]
public class FogSnapshot : System.Object {
    public FogSnapshot(float startDistance, float endDistance, Color color) {
        this.startDistance = startDistance;
        this.endDistance = endDistance;
        this.color = color;
    }

    public float startDistance;
    public float endDistance;
    public Color color;
}

public class FogController : MonoBehaviour {
    public FogSnapshot thisSnapshot;
    public FogSnapshot targetSnapshot;

    public float timeToStopFog;
    public float timeToChangeFog = 1.0f;

    public float timeToStopColor;
    public float timeToChangeColor = 1.0f;

    // Use this for initialization
    void Start () {
        Change(targetSnapshot, timeToChangeFog, timeToChangeColor);
	}

    public void Change(FogSnapshot snap, float timeFog, float timeColor) {
        thisSnapshot = GetFogSnapshot();
        targetSnapshot = snap;

        timeToChangeFog = timeFog;
        timeToStopFog = Time.time + timeToChangeFog;

        timeToChangeColor = timeColor;
        timeToStopColor = Time.time + timeToChangeColor;
    }
	
	// Update is called once per frame
	void Update () {
        if( Time.time <= timeToStopFog ) {
            RenderSettings.fogStartDistance = Mathf.Lerp(thisSnapshot.startDistance, targetSnapshot.startDistance, Time.time / timeToStopFog );
            RenderSettings.fogEndDistance = Mathf.Lerp(thisSnapshot.endDistance, targetSnapshot.endDistance, Time.time / timeToStopFog );
        }

        if( Time.time <= timeToStopColor) {
            RenderSettings.fogColor = Color.Lerp(thisSnapshot.color, targetSnapshot.color, Time.time / timeToStopFog);
            Camera.main.backgroundColor = RenderSettings.fogColor;
        }
	}

    public FogSnapshot GetFogSnapshot() {
        return new FogSnapshot(RenderSettings.fogStartDistance, RenderSettings.fogEndDistance, RenderSettings.fogColor);
    }
}
