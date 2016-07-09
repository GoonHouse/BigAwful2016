using UnityEngine;
using System.Collections;

public class FogController : MonoBehaviour {
	public float closeFogStart = 0;
	public float endFogStart = 0;
	public float closeFogTarget = 8;
	public float endFogTarget = 16;
	public float fogSpeed = 1;
	public float fogColorSpeed = 1;
	public Color fogStart;
	public Color fogEnd;

	private float diff = 0;

	//Camera camera;

	// Use this for initialization
	void Start () {
		//camera = GetComponent<Camera> ();
		diff = endFogTarget - closeFogTarget;
		closeFogStart -= diff;
	}
	
	// Update is called once per frame
	void Update () {
		if (endFogStart < endFogTarget) {
			RenderSettings.fogEndDistance = endFogStart += (fogSpeed * Time.deltaTime);
		}
		if (closeFogStart < closeFogTarget) {
			RenderSettings.fogStartDistance = closeFogStart += (fogSpeed * Time.deltaTime);
		}
		RenderSettings.fogColor = Color.Lerp (fogStart, fogEnd, fogColorSpeed * Time.time);
		Camera.main.backgroundColor = Color.Lerp (fogStart, fogEnd, fogColorSpeed * Time.time);
	}
}
