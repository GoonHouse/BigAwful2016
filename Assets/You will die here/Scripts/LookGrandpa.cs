using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LookGrandpa : MonoBehaviour {
	public float turnSpeed = 1f;
	public Transform thingToLookAt;
	public Transform eyeTarget;
	public Transform grampsHead;
    public UnityEngine.UI.Text thinkText;
	public float tooFar = 10f;
	public float maxAngle = 0.75f;
    public AudioSource aus;
    public AudioClip ac;

    public float timeOnMind = 0.0f;
    public float timeToCommit = 0.33f;
    public bool didCommitYet = false;

    public EmotionProcessor epu;
    public bool hasThought = false;
    public bool fadeDone = false;
    public float timeToFadeIn = 0.5f;
    public float currentFadeTime = 0.0f;
    public float timeToExpose = 4.0f;
    public float currentExposeTime = 0.0f;
    public Color initColor;
    public Color noAlpha;
    public Color withAlpha;

    void Think(LookTarget lt, Thought t) {
        if( lt != null && lt.thoughts.Count > 0 && !hasThought ) {
            hasThought = true;
            fadeDone = false;
            currentFadeTime = 0.0f;
            var dex = Random.Range(0, lt.thoughts.Count);
            var thought = lt.thoughts[dex];
            thinkText.text = thought.text;

            var snd = ac;
            if( thought.sound != null ){
                snd = thought.sound;
            }
            if( !aus.isPlaying ){
                Debug.LogWarning("guhyuk");
                aus.clip = ac;
                aus.Play();
            }
            //AkSoundEngine.PostEvent("think_" + lt.propName + "_" + dex, gameObject);
        } else {
            God.main.LogError("YOU GOT ME ALL FUCKED UP. HOLD ON.");
        }
    }

    // Update is called once per frame
    void Update() {
        if (thingToLookAt != null) {
            float dist = Vector3.Distance(thingToLookAt.transform.position, grampsHead.transform.position); //this makes grandpa forget when things get too far away
            if (dist > tooFar) {
                Forget(thingToLookAt.gameObject);
            }
        }

        if (hasThought) {
            if (!fadeDone) {
                currentFadeTime += Time.deltaTime;
                if (currentFadeTime <= timeToFadeIn) {
                    thinkText.color = Color.Lerp(noAlpha, withAlpha, currentFadeTime / timeToFadeIn);
                } else {
                    thinkText.color = withAlpha;
                    currentExposeTime += Time.deltaTime;
                    if (currentExposeTime >= timeToExpose) {
                        fadeDone = true;
                        currentFadeTime = 0.0f;
                        currentExposeTime = 0.0f;
                    }
                }
            } else {
                currentFadeTime += Time.deltaTime;
                if (currentFadeTime <= timeToFadeIn) {
                    thinkText.color = Color.Lerp(withAlpha, noAlpha, currentFadeTime / timeToFadeIn);
                } else {
                    thinkText.color = noAlpha;
                    thinkText.text = "";
                    hasThought = false;
                    fadeDone = false;
                }
            }
        }
    }

    void OnLevelWasLoaded() {
        thinkText = GameObject.Find("Canvas/GrandpaThoughts").GetComponent<UnityEngine.UI.Text>();
        thinkText.text = "";
        initColor = thinkText.color;
        hasThought = false;
        fadeDone = false;
        currentFadeTime = 0.0f;
        currentExposeTime = 0.0f;
        noAlpha = initColor;
        noAlpha.a = 0.0f;
        withAlpha = initColor;
        withAlpha.a = 1.0f;
        thinkText.color = noAlpha;
        // you can't forget yourself, grandpa!
        // like hell I can't
        Forget(gameObject);
    }

    // Use this for initialization
    void Awake () {
        epu = GetComponent<EmotionProcessor>();
        thingToLookAt = null;
        thinkText = GameObject.Find("Canvas/GrandpaThoughts").GetComponent<UnityEngine.UI.Text>();
        thinkText.text = "";
        initColor = thinkText.color;
        hasThought = false;
        fadeDone = false;
        currentFadeTime = 0.0f;
        currentExposeTime = 0.0f;
        noAlpha = initColor;
        noAlpha.a = 0.0f;
        withAlpha = initColor;
        withAlpha.a = 1.0f;
        thinkText.color = noAlpha;
        // you can't forget yourself, grandpa!
        // like hell I can't
        Forget(gameObject);
	}

    void Notice(GameObject go) {
        thingToLookAt = go.transform;
        timeOnMind = 0.0f;
        didCommitYet = false;
    }

    void Remember(GameObject go) {
        didCommitYet = true;
        var lt = go.GetComponentInChildren<LookTarget>();
        var knob = go.GetComponentInChildren<Knob>();
        if( knob != null && !epu.targets.Contains(lt)) {
            var grampsH = GameObject.Find("GrampsHolder");
            var grandpa = grampsH.GetComponent<Grandpa>();
            grandpa.FocusOnKnob(knob);
        }
        if( lt != null && epu.Commit(lt) ) {
            if (lt.thoughts.Count > 0) {
                var thought = lt.thoughts[Random.Range(0, lt.thoughts.Count)];
                Think(lt, thought);
            }
        }
    }

    void Forget(GameObject go) {
        thingToLookAt = null;
        timeOnMind = 0.0f;
        didCommitYet = false;
    }

    void Ponder() {
        if( timeOnMind < timeToCommit && !didCommitYet && thingToLookAt != null && !hasThought ){
            timeOnMind += Time.deltaTime;
            if ( timeOnMind >= timeToCommit && !didCommitYet && thingToLookAt != null && !hasThought ) {
                Remember(thingToLookAt.gameObject);
            }
        }
    }

	void LateUpdate () {
        if( thingToLookAt != null) {
            rotateTowards(thingToLookAt.transform.position, eyeTarget.transform.position);
        } else {
            rotateTowards(eyeTarget.transform.position, eyeTarget.transform.position);
        }
	}

	void OnCollisionEnter(Collision collision){
        if( collision.gameObject.GetComponentInChildren<LookTarget>() != null && thingToLookAt != collision.gameObject) {
            Notice(collision.gameObject);
        }
    }

	protected void rotateTowards(Vector3 to, Vector3 eyes) {
		Quaternion lookRotation = Quaternion.LookRotation ((to - grampsHead.transform.position).normalized);
		Quaternion ahead = Quaternion.LookRotation ((eyes - grampsHead.transform.position).normalized);
		float diff = Quaternion.Dot(lookRotation, ahead);
		diff = Mathf.Abs (diff);
		if (diff > maxAngle) {
			grampsHead.transform.rotation = Quaternion.Slerp (grampsHead.transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
            Ponder();
		} else {
			grampsHead.transform.rotation = Quaternion.Slerp (grampsHead.transform.rotation, ahead, Time.deltaTime * turnSpeed);
		}
	}
}
