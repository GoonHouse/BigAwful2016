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

    public float cameraTurnRate = 2.0f;

    private float cameraTurnStopTime;
    private float targetCameraDirection;
    private float initCameraDirection;

    public bool inControl = true;
    public bool doneMove = true;
    public Vector3 startPos;
    public Quaternion startRot;
    public Transform moveTarget;
    public float moveTime = 1.0f;
    public float moveTimeSpent = 0.0f;

    private Animator m_Animator;
    private CharacterController controller;
    private GameObject character;
	private GameObject cameraHolder;

    public void SetTarget(Transform target) {
        //controller.enabled = false;
        startPos = transform.position;
        startRot = transform.rotation;
        inControl = false;
        doneMove = false;
        moveTarget = target;
        moveTimeSpent = 0.0f;
    }

    public void OnInMoveTarget() {

    }

    void Start() {
        m_Animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        character = GameObject.Find("GrandFatherContainmentUnit");
		cameraHolder = GameObject.Find ("CameraHolder");
        cameraTurnStopTime = Time.fixedTime-1.0f;

        // If for any reason the player is not at the world origin or the camera isn't facing it, this will break. \o/
        //cameraOffset = Camera.main.transform.position;
    }

    void Update() {
        if( inControl ){
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            // Are we moving?
            Debug.Log("BOINGA");
            m_Animator.SetBool("Walking", (moveVertical == 0 && moveHorizontal == 0));

            if( Input.GetKeyDown(KeyCode.F)) {
                var fc = Camera.main.GetComponent<FogController>();
                var fs = new FogSnapshot(2.0f, 8.0f, new Color(1.0f, 0.0f, 1.0f, 1.0f));
                fc.Change(fs, 4.0f, 20.0f);
            }

            if( Time.fixedTime <= cameraTurnStopTime ){
                var amountToSpin = Mathf.Lerp(initCameraDirection, cameraTargetDirection, Time.fixedTime / cameraTurnStopTime);
                cameraHolder.transform.RotateAround(transform.position, Vector3.up, (cameraTurnAmount * Time.deltaTime) / cameraTurnRate);
            } else {
                if (Input.GetButtonDown("Fire2")) {
                    cameraTurnStopTime = Time.fixedTime + cameraTurnRate;
                    initCameraDirection = cameraTargetDirection;
                    cameraTargetDirection += cameraTurnAmount;
                    if (cameraTargetDirection > 359f) {
                        cameraTargetDirection = 0f;
                    }
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
            if ( moveTimeSpent <= moveTime && !doneMove ) {
                m_Animator.SetBool("Walking", false);
                moveTimeSpent += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, moveTarget.position, moveTimeSpent / moveTime);
                //transform.rotation = Quaternion.Lerp(transform.rotation, moveTarget.rotation, moveTimeSpent / moveTime);
                if( moveTimeSpent >= moveTime) {
                    Debug.Log("FUCK YOU I WONT DO WHAT YA TOLD ME");
                    doneMove = true;
                    transform.rotation = moveTarget.rotation;
                    transform.position = moveTarget.position;
                    m_Animator.SetBool("Walking", true);
                }
            } else {
                // jack and shit
            }
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
