using UnityEngine;
using System.Collections;

public class Grandpa : MonoBehaviour {
    public float turnSpeed = 0.1F;
    public float moveSpeed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 lookDirection = Vector3.zero;

    private Animator m_Animator;
    private CharacterController controller;
    private GameObject character;

    void Start() {
        m_Animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        character = GameObject.Find("Gramps");
    }

    void Update() {

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Are we moving?
        m_Animator.SetBool("Walking", (moveVertical == 0 && moveHorizontal == 0));

        // 
        if (controller.isGrounded) {
            lookDirection = new Vector3(-moveHorizontal, 0.0f, -moveVertical);
            moveDirection = new Vector3(moveHorizontal, 0, moveVertical);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= moveSpeed;
            if (Input.GetButton("Jump")) {
                moveDirection.y = jumpSpeed;
            }
        }

        // Cancel gravity, move to position.
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        // Look to correct direction.
        if (moveHorizontal != 0 && moveVertical != 0) {
            float step = turnSpeed * Time.deltaTime;
            character.transform.rotation = Quaternion.RotateTowards(character.transform.rotation, Quaternion.LookRotation(lookDirection), step);
        }

        // Report location to fog shader.
        Shader.SetGlobalVector("_Origin", transform.position);
    }
}
