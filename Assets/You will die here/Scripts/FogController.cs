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

    public bool isChangeFog = false;
    public float currentTimeToChangeFog;
    public float timeToChangeFog = 1.0f;

    public bool isChangeColor = false;
    public float currentTimeToChangeColor;
    public float timeToChangeColor = 1.0f;

    // Use this for initialization
    void Start () {
        //Change(targetSnapshot, timeToChangeFog, timeToChangeColor);
	}

    public void Change(FogSnapshot snap, float timeFog, float timeColor) {
        thisSnapshot = GetFogSnapshot();
        targetSnapshot = snap;

        isChangeFog = true;
        isChangeColor = true;

        currentTimeToChangeFog = 0.0f;
        currentTimeToChangeColor = 0.0f;

        if( timeFog == 0.0f) {
            timeFog = timeToChangeFog;
        }
        if (timeColor == 0.0f) {
            timeColor = timeToChangeColor;
        }

        timeToChangeFog = timeFog;
        timeToChangeColor = timeColor;
    }
	
	// Update is called once per frame
	void Update () {
        if( isChangeFog ) {
            currentTimeToChangeFog += Time.deltaTime;
            RenderSettings.fogStartDistance = Mathf.LerpUnclamped(thisSnapshot.startDistance, targetSnapshot.startDistance, currentTimeToChangeFog / timeToChangeFog);
            RenderSettings.fogEndDistance = Mathf.LerpUnclamped(thisSnapshot.endDistance, targetSnapshot.endDistance, currentTimeToChangeFog / timeToChangeFog);
            if( currentTimeToChangeFog >= timeToChangeFog ){
                currentTimeToChangeFog = 0.0f;
                isChangeFog = false;
                RenderSettings.fogStartDistance = targetSnapshot.startDistance;
                RenderSettings.fogEndDistance = targetSnapshot.endDistance;
            }
        }

        if( isChangeColor ){
            currentTimeToChangeColor += Time.deltaTime;
            RenderSettings.fogColor = Color.LerpUnclamped(thisSnapshot.color, targetSnapshot.color, currentTimeToChangeFog / timeToChangeFog);
            Camera.main.backgroundColor = RenderSettings.fogColor;
            if( currentTimeToChangeColor >= timeToChangeColor ){
                currentTimeToChangeColor = 0.0f;
                isChangeColor = false;
                RenderSettings.fogColor = targetSnapshot.color;
                Camera.main.backgroundColor = targetSnapshot.color;
            }
        }
        
	}

    public FogSnapshot GetFogSnapshot() {
        return new FogSnapshot(
            RenderSettings.fogStartDistance,
            RenderSettings.fogEndDistance,
            RenderSettings.fogColor
        );
    }
}
