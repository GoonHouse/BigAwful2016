using UnityEngine;
using System.Collections;

public class Grandpa : MonoBehaviour {
    public float turnSpeed = 180.0f;
    public float moveSpeed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    //public Vector3 cameraOffset = new Vector3(0.0f, 16.0f, -28.0f);
    public float cameraTurnAmount = 90.0f;
    private Vector3 moveDirection = Vector3.zero;
    public Vector3 lookDirection = Vector3.zero;
	private float cameraTargetDirection = 0;

    public bool inControl = true;
    public Transform moveTarget;

    private Animator m_Animator;
    private CharacterController controller;
    private GameObject character;
	private GameObject cameraHolder;

    public void OnInMoveTarget() {

    }

    void Start() {
        m_Animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        character = GameObject.Find("GrandFatherContainmentUnit");
		cameraHolder = GameObject.Find ("CameraHolder");

        // If for any reason the player is not at the world origin or the camera isn't facing it, this will break. \o/
        //cameraOffset = Camera.main.transform.position;
    }

    void Update() {
        if( inControl ){
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            // Are we moving?
            m_Animator.SetBool("Walking", (moveVertical == 0 && moveHorizontal == 0));

            if( Input.GetKeyDown(KeyCode.F)) {
                var fc = Camera.main.GetComponent<FogController>();
                var fs = new FogSnapshot(2.0f, 8.0f, new Color(1.0f, 0.0f, 1.0f, 1.0f));
                fc.Change(fs, 4.0f, 20.0f);
            }

            if (Input.GetButtonDown("Fire2")) {
                //Camera.main.transform.RotateAround(transform.position, Vector3.up, cameraTurnAmount);
                //Camera.main.transform.Rotate(new Vector3(0, cameraTurnAmount, 0));
                cameraHolder.transform.RotateAround(transform.position, Vector3.up, cameraTurnAmount);
                cameraTargetDirection += cameraTurnAmount;
                if (cameraTargetDirection > 359f) {
                    cameraTargetDirection = 0f;
                }
            }

            // Handle movement.
            if (controller.isGrounded) {
                moveDirection = new Vector3(moveHorizontal, 0, moveVertical);
                moveDirection = Quaternion.AngleAxis(cameraTargetDirection, Vector3.up) * moveDirection;
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= moveSpeed;
                if (Input.GetButton("Jump")) {
                    moveDirection.y = jumpSpeed;
                }

                // Turn if we need to turn.
                if (moveVertical != 0 || moveHorizontal != 0) {
                    lookDirection = new Vector3(-moveHorizontal, 0, -moveVertical);
                    lookDirection = Quaternion.AngleAxis(cameraTargetDirection, Vector3.up) * lookDirection;
                    float step = turnSpeed * Time.deltaTime;
                    var lookRot = Quaternion.LookRotation(lookDirection.normalized);
                    character.transform.rotation = Quaternion.RotateTowards(character.transform.rotation, lookRot, step);
                }
            }

            // Cancel gravity, move to position.
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
        } else {
            // player not in control, lerp to position
            m_Animator.SetBool("Walking", !inControl);

        }

        // Report location to fog shader.
        Shader.SetGlobalVector("_Origin", transform.position);

        // Set the camera's position.
        var cpos = transform.position;
        //cpos += cameraOffset;
        //Camera.main.transform.position = cpos;
		cameraHolder.transform.position = cpos;
    }
}
