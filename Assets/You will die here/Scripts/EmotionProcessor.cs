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

    public int totalThingsSeen = 0;
    public int thingsSeen = 0;

    // Use this for initialization
    void Start () {
	
	}

    public void Analyze() {
        float masNoneScore, masPhysScore, masSafeScore, masLoveScore, masEsteemScore, masSelfScore;
        needScore.TryGetValue(MaslowNeed.None, out masNoneScore);
        needScore.TryGetValue(MaslowNeed.Physiological, out masPhysScore);
        needScore.TryGetValue(MaslowNeed.Safety, out masSafeScore);
        needScore.TryGetValue(MaslowNeed.Love_Belonging, out masLoveScore);
        needScore.TryGetValue(MaslowNeed.Esteem, out masEsteemScore);
        needScore.TryGetValue(MaslowNeed.Self_Actualization, out masSelfScore);

        float needNoneScore, needMadScore, needScareScore, needJoyScore, needPowerScore, needPeaceScore, needSadScore;
        emotionScore.TryGetValue(MajorEmotions.None, out needNoneScore);
        emotionScore.TryGetValue(MajorEmotions.Mad, out needMadScore);
        emotionScore.TryGetValue(MajorEmotions.Scared, out needScareScore);
        emotionScore.TryGetValue(MajorEmotions.Joyful, out needJoyScore);
        emotionScore.TryGetValue(MajorEmotions.Powerful, out needPowerScore);
        emotionScore.TryGetValue(MajorEmotions.Peaceful, out needPeaceScore);
        emotionScore.TryGetValue(MajorEmotions.Sad, out needSadScore);

        God.main.Log(
            "NEED INDEX: " +
            "\nSad: " + needSadScore +
            "\nPeace: " + needPeaceScore +
            "\nPower: " + needPowerScore +
            "\nJoy: " + needJoyScore +
            "\nScared: " + needScareScore +
            "\nMad: " + needMadScore +
            "\nNone: " + needNoneScore +
            "\nMASLOW INDEX: " +
            "\nSelf: " + masSelfScore +
            "\nEsteem: " + masEsteemScore +
            "\nLove: " + masLoveScore +
            "\nSafe: " + masSafeScore +
            "\nPhys: " + masPhysScore +
            "\nNone: " + masNoneScore
        );
    }

    public bool Commit(LookTarget lt) {
        if (!targets.Contains(lt)) {
            targets.Add(lt);

            thingsSeen++;

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
