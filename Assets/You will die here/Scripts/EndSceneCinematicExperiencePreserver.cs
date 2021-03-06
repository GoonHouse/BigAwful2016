using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndSceneCinematicExperiencePreserver : MonoBehaviour {

    public Material toApplyToPlayer;
    public AudioClip deathEnvironment;
    public float timeToFadeIn = 15.0f;
    public float timeFadingIn = 0.0f;
    private GameObject deadGrandpa;
    private GameObject cameraHolder;
    private Grandpa grandpa;
    private GameObject title;
    private GameObject titleAnchor;
    private AudioSource mus;

    void ThinkYeah() {
        deadGrandpa = GameObject.Find("DeadGrandpa");
        grandpa = GameObject.Find("GrampsHolder").GetComponent<Grandpa>();
        cameraHolder = GameObject.Find("CameraHolder");
        title = GameObject.Find("Title");
        titleAnchor = GameObject.Find("TitleAnchor");
        grandpa.isAlive = false;
        //var cap = grandpa.GetComponent<CapsuleCollider>();
        //cap.enabled = false;
        //var rigid = grandpa.GetComponent<Rigidbody>();
        //rigid.constraints = RigidbodyConstraints.FreezePositionY;
        grandpa.skipFreeze = true;
        grandpa.UnFreeze();
        grandpa.transform.position = new Vector3(12.0f, 1.0f, 9.0f);
        var animator = grandpa.GetComponentInChildren<Animator>();
        animator.SetBool("Dead", false);

        var meshRenderers = grandpa.gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in meshRenderers) {
            var b = mr.materials;
            for (int i = 0; i < b.Length; i++) {
                b[i] = toApplyToPlayer;
            }
            mr.materials = b;
        }

        var fc = Camera.main.GetComponent<FogController>();
        var snap = fc.GetFogSnapshot();
        snap.color = Color.black;
        snap.color.a = 0.0f;
        snap.startDistance = 0.0f;
        snap.endDistance = 0.0f;
        fc.SetNow(snap);

        var snap2 = fc.GetFogSnapshot();
        snap2.color = Color.black;
        snap2.color.a = 0.5f;
        snap2.startDistance = 1.0f;
        snap2.endDistance = 8.0f;
        fc.Change(snap2, 3.0f, 3.0f);

        // reset the camera & grandpa's movement direction
        var rot = cameraHolder.transform.localRotation;
        rot.y = 0.0f;
        cameraHolder.transform.localRotation = rot;

        grandpa.cameraTargetDirection = 0.0f;
        var lg = grandpa.gameObject.GetComponentInChildren<LookGrandpa>();
        lg.enabled = true;

        var c = Camera.main.GetComponent<Corrupt>();
        c.corruption = 0.0f;
        c.doCorrupt = false;
        Shader.SetGlobalFloat("_AltValue", 0.0f);
        Shader.SetGlobalColor("_FloorColor", Color.Lerp(new Color32(189, 189, 189, 255), Color.black, 0.0f));

        mus = Camera.main.GetComponent<AudioSource>();
        mus.Stop();
        mus.clip = deathEnvironment;
        mus.volume = 0.0f;
        mus.Play();
    }
        // Use this for initialization
    void Awake () {
        ThinkYeah();
    }

    void Start() {
        ThinkYeah();
    }
	
	// Update is called once per frame
	void Update () {
        // Report location to fog shader.
        Shader.SetGlobalVector("_Origin", deadGrandpa.transform.position + (Vector3.up * 2f));
        title.transform.position = Vector3.MoveTowards(title.transform.position, titleAnchor.transform.position, Time.deltaTime * 0.5f);

        // Set the camera's position.
        var cpos = deadGrandpa.transform.position;
        cameraHolder.transform.position = cpos;

        if( timeFadingIn < timeToFadeIn) {
            timeFadingIn += Time.deltaTime;
            mus.volume = Mathf.Lerp(0.0f, 1.0f, timeFadingIn / timeToFadeIn);
        } else {
            mus.volume = 1.0f;
        }
    }
}
