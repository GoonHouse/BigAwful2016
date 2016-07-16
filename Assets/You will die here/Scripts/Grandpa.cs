using UnityEngine;
using System.Collections;

public delegate void OnDoneTarget(Grandpa grandpa);

public class Grandpa : MonoBehaviour {
    public float turnSpeed = 180.0f;
    public float moveSpeed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    //public Vector3 cameraOffset = new Vector3(0.0f, 16.0f, -28.0f);
    public float cameraTurnAmount = 90.0f;
    public int cameraTurnDirection = 1;
    private Vector3 moveDirection = Vector3.zero;
    public Vector3 lookDirection = Vector3.zero;
	private float cameraTargetDirection = 0;

    public float cameraTurnRate = 2.0f;

    private float cameraTurnStopTime;
    private float targetCameraDirection;
    private float initCameraDirection;

    public bool isWalking = false;

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

    private Knob focusKnob;
    private OnDoneTarget whenDoneDo;
    private int knobStage = 0;
    //private IEnumerator currentCor;
    //private IEnumerator nextCor;

    // How rude
    public void FocusOnKnob(Knob knob) {
        focusKnob = knob;
        whenDoneDo = KnobStage1;
        moveTime = 2.0f;

        var timeToShit = 2.0f;
        var fc = Camera.main.GetComponent<FogController>();
        var snap = fc.GetFogSnapshot();
        snap.color = Color.black;
        snap.startDistance = 1.0f;
        snap.endDistance = 4.0f;
        fc.Change(snap, timeToShit, timeToShit);

        isWalking = true;

        SetTarget(knob.walkFromTarget);
    }

    /* KNOB STAGES AND YOU:
     * 0 = approaching door
     * 1 = anticipation
     * 2 = door opens, wait
     * 3 = enter door, fade
     * 4 = sexual backgammon; door close, world ends
     */

    public static void KnobStage1(Grandpa grandpa) {
        grandpa.knobStage = 1;
        Debug.LogWarning("ANTICIPATING!");

        grandpa.isWalking = false;

        grandpa.focusKnob.Buildup();

        // reset the movetime to get a wait out of this
        grandpa.moveTime = 3.0f;
        grandpa.moveTimeSpent = 0.0f;
        grandpa.doneMove = false;

        grandpa.SetTarget(grandpa.focusKnob.walkFromTarget);

        grandpa.whenDoneDo = KnobStage2;
    }

    public static void KnobStage2(Grandpa grandpa) {
        grandpa.knobStage = 2;
        Debug.LogWarning("DOOR OPENING, NOT MOVING");
        grandpa.focusKnob.Unlock();

        grandpa.moveTime = 1.0f;
        grandpa.moveTimeSpent = 0.0f;
        grandpa.doneMove = false;

        grandpa.whenDoneDo = KnobStage3;
    }

    public static void KnobStage3(Grandpa grandpa) {
        grandpa.focusKnob.OpenDoor();
        
        
        // reset the movetime to get a wait out of this
        grandpa.moveTime = 2.0f;
        grandpa.moveTimeSpent = 0.0f;
        grandpa.doneMove = false;
        //grandpa.SetTarget(grandpa.focusKnob.walkToTarget);
        //grandpa.SetTarget(grandpa.focusKnob.walkFromTarget);
        grandpa.whenDoneDo = KnobStage4;
    }

    public static void KnobStage4(Grandpa grandpa) {
        var timeToShit = 3.0f;
        grandpa.knobStage = 4;

        grandpa.isWalking = true;
        Debug.LogWarning("MOVING IN, FADING FOG!");
        
        grandpa.whenDoneDo = KnobStage3;

        grandpa.moveTime = timeToShit;

        grandpa.SetTarget(grandpa.focusKnob.walkToTarget);
        grandpa.whenDoneDo = KnobStage5;
    }

    public static void KnobStage5(Grandpa grandpa) {
        grandpa.knobStage = 5;
        Debug.LogWarning("JACK SHIT!");
        // reset the movetime to get a wait out of this
        grandpa.moveTime = 0.1f;
        grandpa.moveTimeSpent = 0.0f;
        grandpa.doneMove = false;

        grandpa.isWalking = false;

        grandpa.whenDoneDo = null;
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
            m_Animator.SetBool("Walking", (moveVertical == 0 && moveHorizontal == 0));

            if( Input.GetKeyDown(KeyCode.F)) {
                var fc = Camera.main.GetComponent<FogController>();
                var fs = new FogSnapshot(2.0f, 8.0f, new Color(1.0f, 0.0f, 1.0f, 1.0f));
                fc.Change(fs, 4.0f, 20.0f);
            }

            if( Time.fixedTime <= cameraTurnStopTime ){
                var amountToSpin = Mathf.Lerp(initCameraDirection, cameraTargetDirection, Time.fixedTime / cameraTurnStopTime);
                cameraHolder.transform.RotateAround(transform.position, Vector3.up, (cameraTurnDirection * cameraTurnAmount * Time.deltaTime) / cameraTurnRate);
            } else {
                if (Input.GetButtonDown("Fire2")) {
                    cameraTurnStopTime = Time.fixedTime + cameraTurnRate;
                    initCameraDirection = cameraTargetDirection;
                    cameraTargetDirection += cameraTurnAmount;
                    cameraTurnDirection = 1;
                    if (cameraTargetDirection > 359f) {
                        cameraTargetDirection = 0f;
                    }
                } else if ( Input.GetButtonDown("Fire1")) {
                    cameraTurnStopTime = Time.fixedTime + cameraTurnRate;
                    initCameraDirection = cameraTargetDirection;
                    cameraTargetDirection -= cameraTurnAmount;
                    cameraTurnDirection = -1;
                    if (cameraTargetDirection < 0.0f) {
                        cameraTargetDirection = 359f;
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
            m_Animator.SetBool("Walking", !isWalking);
            // player not in control, lerp to position
            if ( moveTimeSpent <= moveTime && !doneMove ) {
                moveTimeSpent += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, moveTarget.position, moveTimeSpent / moveTime);
                transform.localRotation = Quaternion.Lerp(startRot, moveTarget.localRotation, moveTimeSpent / moveTime);
                if( moveTimeSpent >= moveTime) {
                    Debug.Log("FUCK YOU I WONT DO WHAT YA TOLD ME");
                    doneMove = true;
                    if( whenDoneDo != null) {
                        whenDoneDo(this);
                    }
                }
            } else {
                // enforce our local position because now that our character controller is disabled
                // we will go straight through the gat dang floor
                transform.position = moveTarget.position;
                transform.localRotation = moveTarget.localRotation;
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

    public void SetTarget(Transform target) {
        controller.enabled = false;
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        startPos = transform.position;
        startRot = transform.localRotation;
        inControl = false;
        doneMove = false;
        moveTarget = target;
        moveTimeSpent = 0.0f;
    }
}
