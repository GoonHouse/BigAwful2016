using UnityEngine;
using System.Collections;

public class FogController : MonoBehaviour {
	public float closeFogStart = 0;
	public float endFogStart = 0;
	public float closeFogTarget = 8;
	public float endFogTarget = 16;
	public float fogSpeed = 1;

	private float diff = 0;

	// Use this for initialization
	void Start () {
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
	}
}
