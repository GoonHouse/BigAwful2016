using UnityEngine;
using System.Collections;

public class Knob : MonoBehaviour {


    public Transform walkToTarget;
    public Transform walkFromTarget;

    public AudioClip unlockSound;
    public AudioClip buildupSound;

    private Animator anim;

    // Use this for initialization
    void Start () {
        anim = GetComponentInParent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Buildup() {
        AkSoundEngine.PostEvent("buildupSound", gameObject);
    }

    public void Unlock() {
        AkSoundEngine.PostEvent("unlockSound", gameObject);
    }

    public void OpenDoor() {
        anim.SetBool("MakeMeTrueToCauseTheDoorToOpen", true);
    }
}
