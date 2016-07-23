using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndSceneCinematicExperiencePreserver : MonoBehaviour {

    public Material toApplyToPlayer;
    private GameObject deadGrandpa;
    private GameObject cameraHolder;
    private Grandpa grandpa;
    private GameObject title;
    private GameObject titleAnchor;

    // Use this for initialization
    void Awake () {
        deadGrandpa = GameObject.Find("DeadGrandpa");
        grandpa = GameObject.Find("GrampsHolder").GetComponent<Grandpa>();
        cameraHolder = GameObject.Find("CameraHolder");
        title = GameObject.Find("Title");
        titleAnchor = GameObject.Find("TitleAnchor");
        grandpa.isAlive = false;

        var meshRenderers = grandpa.gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach( MeshRenderer mr in meshRenderers) {
            var b = mr.materials;
            for( int i = 0; i < b.Length; i++) {
                b[i] = toApplyToPlayer;
            }
            mr.materials = b;
        }
    }
	
	// Update is called once per frame
	void Update () {
        // Report location to fog shader.
        Shader.SetGlobalVector("_Origin", deadGrandpa.transform.position + (Vector3.up * 2f));
        title.transform.position = Vector3.MoveTowards(title.transform.position, titleAnchor.transform.position, Time.deltaTime * 0.5f);

        // Set the camera's position.
        var cpos = deadGrandpa.transform.position;
        //cpos += cameraOffset;
        //Camera.main.transform.position = cpos;
        cameraHolder.transform.position = cpos;
    }
}
