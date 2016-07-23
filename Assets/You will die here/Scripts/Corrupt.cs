using UnityEngine;
using System.Collections;

public class Corrupt : MonoBehaviour {
    public float corruptFactor = 0.01F;
    public float corruptTime = 1.0F;
    public float corruption;

    void Update() {
		corruption = Mathf.PingPong( Time.time * corruptFactor, corruptTime);
        Shader.SetGlobalFloat("_AltValue", corruption);
        Shader.SetGlobalColor("_FloorColor", Color.Lerp(new Color32(189, 189, 189, 255), Color.black, corruption));
	}
}
