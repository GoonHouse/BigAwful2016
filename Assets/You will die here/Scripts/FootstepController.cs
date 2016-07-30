using UnityEngine;
using System.Collections;

public class FootstepController : MonoBehaviour {

    public bool isStepping = false;
	// Use this for initialization
    public void stepNow()
    {
        isStepping = true;
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (isStepping == true)
        {
            isStepping = false;
            //AkSoundEngine.PostEvent("grandpaStep", gameObject);
        }
	}
}
