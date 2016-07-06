using UnityEngine;
using System.Collections;

public class Grandpa : MonoBehaviour {
    public float turnSpeed = 180.0f;
    public float moveSpeed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Vector3 cameraOffset = new Vector3(0.0f, 16.0f, -28.0f);
    private Vector3 moveDirection = Vector3.zero;
    public Vector3 lookDirection = Vector3.zero;

    private Animator m_Animator;
    private CharacterController controller;
    private GameObject character;

    void Start() {
        m_Animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        character = GameObject.Find("Gramps");
        // If for any reason the player is not at the world origin or the camera isn't facing it, this will break. \o/
        cameraOffset = Camera.main.transform.position;
    }

    void Update() {

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Are we moving?
        m_Animator.SetBool("Walking", (moveVertical == 0 && moveHorizontal == 0));

        // 
        if (controller.isGrounded) {
            moveDirection = new Vector3(moveHorizontal, 0, moveVertical);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= moveSpeed;
            if (Input.GetButton("Jump")) {
                moveDirection.y = jumpSpeed;
            }

            if (moveVertical != 0 || moveHorizontal != 0) {
                lookDirection = new Vector3(-moveHorizontal, 0, -moveVertical);
                float step = turnSpeed * Time.deltaTime;
                var lookRot = Quaternion.LookRotation(lookDirection.normalized);
                character.transform.rotation = Quaternion.RotateTowards(character.transform.rotation, lookRot, step);
            }
        }

        // Cancel gravity, move to position.
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        // Look to correct direction.
        /*
        if( moveVertical != 0 || moveHorizontal != 0 ) {
            lookDirection = character.transform.position;

            lookDirection.x -= moveHorizontal;
            lookDirection.y = 0.0f;
            lookDirection.z -= moveVertical;

            float step = turnSpeed * Time.deltaTime;
            character.transform.rotation = Quaternion.RotateTowards(character.transform.rotation, Quaternion.LookRotation(lookDirection), step);
        }
        */

        /*
        Vector3 direction = new Vector3(-moveHorizontal, 0.0f, -moveVertical);

        if (moveVertical == 0 && moveHorizontal == 0) {
            //direction = new Vector3 (-1, 0, 1);
        } else {
            float step = 0.1F * Time.deltaTime;
            character.transform.rotation = Quaternion.RotateTowards(character.transform.rotation, Quaternion.LookRotation(direction), step);
        }
        */

        // Report location to fog shader.
        Shader.SetGlobalVector("_Origin", transform.position);

        // Set the camera's position.
        var cpos = transform.position;
        cpos += cameraOffset;
        Camera.main.transform.position = cpos;
    }
}
