using UnityEngine;
using System.Collections;

public class DeathClock : MonoBehaviour {
    public float totalTimeToDie = 600.0f;
    public float timeToDie = 600.0f; // Ten Minutes
    public float remainingDeathLerp = 120.0f; // At one minute left, do weird lerp shit.
    public bool didNotTrigger = true;
    public bool didDie = false;
    private UnityEngine.UI.Text timeText;
    private Corrupt c;
    public AudioSource aus;

    void GetUI() {
        timeText = GameObject.Find("Canvas/TimeLeft").GetComponent<UnityEngine.UI.Text>();
        timeText.text = "";
        c = Camera.main.GetComponent<Corrupt>();
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

        c.minCorrupt = Mathf.Lerp(1.0f, 0.0f, ( timeToDie / totalTimeToDie ));

        if( timeToDie <= remainingDeathLerp) {
            // Do some interesting effects when this happens.
            if( didNotTrigger ){
                // Do one-time things here. What a shitty bool name.
                aus.Play();
                didNotTrigger = false;
                //AkSoundEngine.PostEvent("grandpaDeathWarning", gameObject);
            }
            if( timeToDie <= (remainingDeathLerp / 2.0f)){
                aus.volume = Mathf.Lerp(1.0f, 0.0f, timeToDie / (remainingDeathLerp / 2.0f));
            } else {
                aus.volume = 1.0f;
            }
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
