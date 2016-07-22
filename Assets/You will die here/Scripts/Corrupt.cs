using UnityEngine;
using System.Collections;

public class Corrupt : MonoBehaviour {
	public Renderer rend;
	void Start() {
		rend = GetComponent<Renderer>();
		rend.sharedMaterials[0].shader = Shader.Find("Custom/LinearFogTransparent");
		rend.sharedMaterials[1].shader = Shader.Find("Custom/LinearFog");
	}
	void Update() {
		float corruption = Mathf.PingPong(Time.time*0.01f, 1.0F);
		rend.sharedMaterials[0].SetFloat("_AltValue", corruption);
		rend.sharedMaterials[1].SetColor ("_Color", Color.Lerp (new Color32 (189, 189, 189, 255), Color.black, corruption));
	}
}
