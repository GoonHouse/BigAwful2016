using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleSceneCinematicExperiencePreserver : MonoBehaviour {

    public float mitigationStart = 0.5f;
    public float mitigationEnd = 1.0f;

    public bool  isChanging = true;
    public float timeToChange = 16.0f;
    public float currentTimeToChange = 0.0f;

    private List<NavLine> navs;

	// Use this for initialization
	void Start () {
        navs = new List<NavLine>( GameObject.Find("Rooms").GetComponentsInChildren<NavLine>() );
    }
	
	// Update is called once per frame
	void Update () {
        if (isChanging) {
            currentTimeToChange += Time.deltaTime;
            foreach( NavLine nav in navs) {
                nav.mitigation = Mathf.LerpUnclamped(mitigationStart, mitigationEnd, currentTimeToChange / timeToChange);
            }
            if (currentTimeToChange >= timeToChange) {
                currentTimeToChange = 0.0f;
                isChanging = false;
                foreach (NavLine nav in navs) {
                    nav.mitigation = mitigationEnd;
                }
            }
        }
	}
}