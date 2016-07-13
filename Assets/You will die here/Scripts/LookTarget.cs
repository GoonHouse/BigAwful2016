using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MajorEmotions {
    None,
    Mad,
    Scared,
    Joyful,
    Powerful,
    Peaceful,
    Sad,
}

[System.Serializable]
public class EmotionFragment : System.Object {
    public string name;
    public MajorEmotions major;
    public float strength = 1.0f;
}

public enum MaslowNeed {
    None,
    Physiological,
    Safety,
    Love_Belonging,
    Esteem,
    Self_Actualization,
}

[System.Serializable]
public class NeedFragment : System.Object {
    public string name;
    public MaslowNeed need;
    public float strength = 1.0f;
}

public class LookTarget : MonoBehaviour {
    public string propName;
    public List<EmotionFragment> emotions = new List<EmotionFragment>();
    public List<NeedFragment> needs = new List<NeedFragment>();


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
