using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmotionProcessor : MonoBehaviour {

    public Dictionary<string, float> emotionTags = new Dictionary<string, float>();
    public Dictionary<string, float> needTags = new Dictionary<string, float>();

    public Dictionary<MajorEmotions, float> emotionScore = new Dictionary<MajorEmotions, float>();
    public Dictionary<MaslowNeed, float> needScore = new Dictionary<MaslowNeed, float>();

    public List<LookTarget> targets = new List<LookTarget>();

    public List<EmotionFragment> emotions = new List<EmotionFragment>();
    public List<NeedFragment> needs = new List<NeedFragment>();

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool Commit(LookTarget lt) {
        if (!targets.Contains(lt)) {
            targets.Add(lt);
            foreach (EmotionFragment emotion in lt.emotions) {
                emotions.Add(emotion);

                // Boost the tags.
                if (emotionTags.ContainsKey(emotion.name)) {
                    emotionTags[emotion.name] += emotion.strength;
                } else {
                    emotionTags[emotion.name] = emotion.strength;
                }

                // Boost the root score.
                if (emotionScore.ContainsKey(emotion.major)) {
                    emotionScore[emotion.major] += emotion.strength;
                } else {
                    emotionScore[emotion.major] = emotion.strength;
                }
            }

            foreach (NeedFragment need in lt.needs) {
                needs.Add(need);

                // Boost the tags.
                if (needTags.ContainsKey(need.name)) {
                    needTags[need.name] += need.strength;
                } else {
                    needTags[need.name] = need.strength;
                }

                // Boost the root score.
                if (needScore.ContainsKey(need.need)) {
                    needScore[need.need] += need.strength;
                } else {
                    needScore[need.need] = need.strength;
                }
            }
            return true;
        }
        return false;
    }
}
