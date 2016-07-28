using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleSceneCinematicExperiencePreserver : MonoBehaviour {
    public float mitigationStart = 0.0f;
    public float mitigationEnd = 1.0f;

    public bool  isChanging = true;
    public float timeToChange = 16.0f;
    public float currentTimeToChange = 0.0f;
    public float timeToChangeBonusWait = 3.0f;

    private List<NavLine> navs;
    private Grandpa grandpa;
    private Corrupt corrupt;

    void OnLevelWasLoaded() {
        corrupt.doCorrupt = true;
    }

        // Use this for initialization
    void Awake () {
        navs = new List<NavLine>( GameObject.Find("Rooms").GetComponentsInChildren<NavLine>() );
        grandpa = GameObject.Find("GrampsHolder").GetComponentInChildren<Grandpa>();
        grandpa.mitigation = mitigationStart;
        foreach (NavLine nav in navs) {
            nav.mitigation = mitigationStart;
        }
        corrupt = Camera.main.GetComponent<Corrupt>();
        corrupt.doCorrupt = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (isChanging) {
            currentTimeToChange += Time.deltaTime;
            foreach( NavLine nav in navs) {
                nav.mitigation = Mathf.Lerp(mitigationStart, mitigationEnd, currentTimeToChange / timeToChange);
            }
            grandpa.mitigation = BeeCoExtensions.LerpOutExpo(mitigationStart, mitigationEnd, currentTimeToChange / timeToChange);
            if (currentTimeToChange >= timeToChange) {
                currentTimeToChange = 0.0f;
                isChanging = false;
                foreach (NavLine nav in navs) {
                    nav.mitigation = mitigationEnd;
                }
                grandpa.mitigation = mitigationEnd;
            }
        }
	}
}