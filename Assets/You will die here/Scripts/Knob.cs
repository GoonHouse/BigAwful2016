﻿using UnityEngine;
using System.Collections;

public class Knob : MonoBehaviour {


    public Transform walkToTarget;
    public Transform walkFromTarget;

    public AudioClip unlockSound;
    public AudioClip buildupSound;

    private Animator anim;
    private AudioSource aus;

    // Use this for initialization
    void Start () {
        anim = GetComponentInParent<Animator>();
        aus = GetComponent<AudioSource>(); 
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Buildup() {
        // aus.PlayOneShot(buildupSound);
        AkSoundEngine.PostEvent("buildupSound", gameObject);
    }

    public void Unlock() {
        //aus.PlayOneShot(unlockSound);
        AkSoundEngine.PostEvent("unlockSound", gameObject);
    }

    public void OpenDoor() {
        anim.SetBool("MakeMeTrueToCauseTheDoorToOpen", true);
    }
}
