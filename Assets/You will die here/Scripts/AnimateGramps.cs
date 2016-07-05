using UnityEngine;
using System.Collections;

public class AnimateGramps : MonoBehaviour {

	Animator m_Animator;

	// Use this for initialization
	void Start () {
		m_Animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		if (moveVertical == 0 && moveHorizontal == 0) {
			m_Animator.SetBool("Walking", false);
		} else {
			m_Animator.SetBool("Walking", true);
		}

	}
}

