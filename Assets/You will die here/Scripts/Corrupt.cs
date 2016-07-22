using UnityEngine;
using System.Collections;

public class Corrupt : MonoBehaviour {
	public Renderer rend;
	void Start() {
		rend = GetComponent<Renderer>();
		rend.sharedMaterial.shader = Shader.Find("Custom/LinearFogTransparent");
	}
	void Update() {
		float corruption = Mathf.PingPong(Time.time, 1.0F);
		rend.sharedMaterial.SetFloat("_AltValue", corruption);
	}
}
