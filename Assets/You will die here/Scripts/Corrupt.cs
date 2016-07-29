using UnityEngine;
using System.Collections;

public class Corrupt : MonoBehaviour {
    public bool  doCorrupt = false;
    public float corruptFactor = 0.01F;
    public float corruptTime = 1.0F;
    public float minCorrupt = 0.01f;
    public float maxCorrupt = 0.33f;
    public float corruption;

    void Awake() {
        corruption = 0.0f;
        Shader.SetGlobalFloat("_AltValue", corruption);
        Shader.SetGlobalColor("_FloorColor", Color.Lerp(new Color32(189, 189, 189, 255), Color.black, corruption));
    }

    void Update() {
        if( doCorrupt ){
            corruption = Mathf.Lerp( minCorrupt, maxCorrupt, Mathf.PingPong(Time.time * corruptFactor, corruptTime));
            Shader.SetGlobalFloat("_AltValue", corruption);
            Shader.SetGlobalColor("_FloorColor", Color.Lerp(new Color32(189, 189, 189, 255), Color.black, corruption));
        }
	}
}
