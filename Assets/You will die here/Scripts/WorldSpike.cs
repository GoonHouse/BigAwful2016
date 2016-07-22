using UnityEngine;
using System.Collections;

public class WorldSpike : MonoBehaviour {
	private SkinnedMeshRenderer Spike;
	public float TimeToReshape = 1f;
	public bool Animate = false;
	private float timer = 0f;
		

	// Use this for initialization
	void Start () {
		Spike = GetComponent<SkinnedMeshRenderer> ();
		reshape ();
	}
		// Update is called once per frame
	void Update () {
		if (Animate) {
			timer -= Time.deltaTime;
			if (timer < 0) {
				timer = TimeToReshape;
				reshape ();
			}
		}
	}
		void reshape() {
		int targets = Spike.sharedMesh.blendShapeCount;
		for(int i = 0; i < targets; i++)
		{
			float foo = Random.Range (0, 100);
			Spike.SetBlendShapeWeight (i, foo);
			}
		}
}
