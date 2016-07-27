using UnityEngine;
using System.Collections;

public class DeathClock : MonoBehaviour {
    public float timeToDie = 600.0f; // Ten Minutes
    public float remainingDeathLerp = 60.0f; // At one minute left, do weird lerp shit.
    public bool didNotTrigger = true;
    public bool didDie = false;
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
	public void SecretUpdate () {
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

        if( !didDie ){
            timeText.text = timeToDie.FormatTime();
            if (timeToDie <= 0.0f) {
                // We're fucking dead, yo.
                var grandpa = GetComponent<Grandpa>();
                grandpa.Die();
                didDie = true;
                timeText.text = "";
            }
        }
	}
}
