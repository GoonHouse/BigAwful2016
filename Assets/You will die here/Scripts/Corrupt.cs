using UnityEngine;
using System.Collections;

public class Corrupt : MonoBehaviour {
    public bool  doCorrupt = false;
    public float corruptFactor = 0.01F;
    public float corruptTime = 1.0F;
    public float minCorrupt = 0.01f;
    public float maxCorrupt = 1.0f;
    public float corruptTimer = 0.0f;
    public float corruptDirection = 1.0f;
    public float corruption;

    void Awake() {
        corruption = 0.0f;
        Shader.SetGlobalFloat("_AltValue", corruption);
        Shader.SetGlobalColor("_FloorColor", Color.Lerp(new Color32(189, 189, 189, 255), Color.black, corruption));
    }

    void Update() {
        if( doCorrupt ){
            corruptTimer += Time.deltaTime * corruptFactor * corruptDirection;
            if (corruptDirection == 1.0f && corruptTimer >= corruptTime) {
                corruptTimer = corruptTime;
                corruptDirection *= -1.0f;
            } else if (corruptDirection == -1.0f && corruptTimer <= 0.0f) {
                corruptTimer = 0.0f;
                corruptDirection *= -1.0f;
            }
            corruption = Mathf.Lerp( minCorrupt, maxCorrupt, (corruptTimer / corruptTime) );
            Shader.SetGlobalFloat("_AltValue", corruption);
            Shader.SetGlobalColor("_FloorColor", Color.Lerp(new Color32(189, 189, 189, 255), Color.black, corruption));
        }
	}
}
