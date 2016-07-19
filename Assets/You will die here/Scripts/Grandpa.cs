using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public delegate void OnDoneTarget(Grandpa grandpa);

public class Grandpa : MonoBehaviour {
    public static Grandpa main;

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

    public float mitigation = 1.0f;

    private Animator m_Animator;
    private CharacterController controller;
    private GameObject character;
	private GameObject cameraHolder;

    private Knob focusKnob;
    private OnDoneTarget whenDoneDo;

    public Vector3 spawnPos = new Vector3(0, 50.0f, 0);
    public bool isFrozen = false;

    public void Freeze() {
        isFrozen = true;
        //controller.enabled = false;
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        inControl = false;
        doneMove = true;
        moveTarget = null;
    }

    public void UnFreeze() {
        isFrozen = false;
        //controller.enabled = true;
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = true;
        inControl = true;
        doneMove = true;
        moveTarget = null;
    }

    void OnLevelWasLoaded() {
        Debug.Log("I'M IN A NEW SCENE YEHAW");
        Freeze();
        transform.position = spawnPos;
        Camera.main.backgroundColor = (Color)(new Color32(189, 189, 189, 255));
        /*
        var fc = Camera.main.GetComponent<FogController>();
        var snap = fc.GetFogSnapshot();
        snap.color = Color.black;
        snap.startDistance = 0.0f;
        snap.endDistance = 0.0f;
        fc.Change(snap, 0.0f, 0.0f);
        */
    }

    // How rude
    public void FocusOnKnob(Knob knob) {
        focusKnob = knob;
        whenDoneDo = KnobStage_AnticipateDoor;
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
     * KnobStage_AnticipateDoor
     * KnobStage_UnlockDoor
     * KnobStage_OpenDoorAndWait
     * KnobStage_EnterDoor
     * KnobStage_InsideDoor
     */

    public static void KnobStage_AnticipateDoor(Grandpa grandpa) {
        Debug.LogWarning("ANTICIPATING DOOR!");

        grandpa.isWalking = false;

        grandpa.focusKnob.Buildup();

        // reset the movetime to get a wait out of this
        grandpa.moveTime = 3.0f;
        grandpa.moveTimeSpent = 0.0f;
        grandpa.doneMove = false;

        grandpa.SetTarget(grandpa.focusKnob.walkFromTarget);

        grandpa.whenDoneDo = KnobStage_UnlockDoor;
    }

    public static void KnobStage_UnlockDoor(Grandpa grandpa) {
        Debug.LogWarning("UNLOCKING DOOR.");
        grandpa.focusKnob.Unlock();

        grandpa.moveTime = 1.0f;
        grandpa.moveTimeSpent = 0.0f;
        grandpa.doneMove = false;

        grandpa.whenDoneDo = KnobStage_OpenDoorAndWait;
    }

    public static void KnobStage_OpenDoorAndWait(Grandpa grandpa) {
        Debug.LogWarning("OPENING DOOR. WAITING.");
        grandpa.focusKnob.OpenDoor();
        
        
        // reset the movetime to get a wait out of this
        grandpa.moveTime = 2.0f;
        grandpa.moveTimeSpent = 0.0f;
        grandpa.doneMove = false;
        //grandpa.SetTarget(grandpa.focusKnob.walkToTarget);
        //grandpa.SetTarget(grandpa.focusKnob.walkFromTarget);
        grandpa.whenDoneDo = KnobStage_EnterDoor;
    }

    public static void KnobStage_EnterDoor(Grandpa grandpa) {
        Debug.LogWarning("ENTERING DOOR.");

        grandpa.isWalking = true;

        var timeToShit = 2.0f;
        var fuckMult = 1.0f;
        var fc = Camera.main.GetComponent<FogController>();
        var snap = fc.GetFogSnapshot();
        Debug.Log(snap.startDistance + " _ " + snap.endDistance + " _ " + snap.color);
        snap.color = Color.black;
        snap.startDistance = 0.1f;
        snap.endDistance = 0.4f;
        fc.Change(snap, timeToShit * fuckMult, timeToShit * fuckMult);

        grandpa.moveTime = timeToShit;

        grandpa.SetTarget(grandpa.focusKnob.walkToTarget);
        grandpa.whenDoneDo = KnobStage_InsideDoor;
    }

    public static void KnobStage_InsideDoor(Grandpa grandpa) {
        Debug.LogWarning("I'M GOING INSIDE! WAIT. I'M ALREADY THERE.");
        
        // reset the movetime to get a wait out of this
        grandpa.isWalking = false;
        grandpa.moveTime = 0.1f;
        grandpa.SetTarget(grandpa.focusKnob.walkToTarget);

        var w = GameObject.FindObjectOfType<RoomGenergreater>();

        SceneManager.LoadScene(w.nextSceneName);

        grandpa.whenDoneDo = null;
    }

    public void OnInMoveTarget() {

    }

    void Awake() {
        if (main == null) {
            DontDestroyOnLoad(gameObject);
            main = this;
        } else if (main != this) {
            Debug.LogWarning("KILLED GRANDPA TO MAKE ROOM");
            DestroyImmediate(gameObject);
        }
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
            float moveHorizontal = Input.GetAxis("Horizontal") * mitigation;
            float moveVertical = Input.GetAxis("Vertical") * mitigation;

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
                        cameraTargetDirection = 360.0f - cameraTurnAmount;
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
                character.transform.rotation = Quaternion.Lerp(startRot, moveTarget.rotation, moveTimeSpent / moveTime);
                if( moveTimeSpent >= moveTime) {
                    Debug.Log("FUCK YOU I WONT DO WHAT YA TOLD ME");
                    doneMove = true;
                    if( whenDoneDo != null) {
                        whenDoneDo(this);
                    }
                }
            } else if( !isFrozen ){
                // enforce our local position because now that our character controller is disabled
                // we will go straight through the gat dang floor
                transform.position = moveTarget.position;
                character.transform.rotation = moveTarget.rotation;
            } else if( isFrozen ){
                transform.position = spawnPos;
            }
            moveDirection = Vector3.zero;
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
        }

        // Report location to fog shader.
		Shader.SetGlobalVector("_Origin", transform.position + (Vector3.up * 2f));

        // Set the camera's position.
        var cpos = transform.position;
        //cpos += cameraOffset;
        //Camera.main.transform.position = cpos;
		cameraHolder.transform.position = cpos;
    }

    public void SetTarget(Transform target) {
        //controller.enabled = false;
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        startPos = transform.position;
        startRot = character.transform.rotation;
        inControl = false;
        doneMove = false;
        moveTarget = target;
        moveTimeSpent = 0.0f;
    }
}
