using UnityEngine;
using System.Collections;

public class DeathClock : MonoBehaviour {
    public float timeToDie = 600.0f; // Ten Minutes
    public float remainingDeathLerp = 60.0f; // At one minute left, do weird lerp shit.
    public bool didNotTrigger = true;
    private UnityEngine.UI.Text timeText;

    void GetUI() {
        timeText = GameObject.Find("Canvas/TimeLeft").GetComponent<UnityEngine.UI.Text>();
        timeText.text = "";
    }

    void OnLevelWasLoaded() {
        GetUI();
    }

    // Use this for initialization
    void Awake () {
        GetUI();
	}
	
	// Update is called once per frame
	void Update () {
        timeToDie -= Time.deltaTime;

        if( timeToDie <= remainingDeathLerp) {
            // Do some interesting effects when this happens.
            if( didNotTrigger ){
                // Do one-time things here. What a shitty bool name.
                didNotTrigger = false;
                AkSoundEngine.PostEvent("grandpaDeathWarning", gameObject);
            }
            //Mathf.Lerp(0.0f, 0.0f, timeToDie / remainingDeathLerp);
        }

        if( timeToDie <= 0.0f) {
            AkSoundEngine.PostEvent("grandpaDied", gameObject);
            // We're fucking dead, yo.
        }

        timeText.text = timeToDie.FormatTime();
	}
}
