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
    //public float gravity = 20.0f;
    //public Vector3 cameraOffset = new Vector3(0.0f, 16.0f, -28.0f);
    public float cameraTurnAmount = 90.0f;
    public int cameraTurnDirection = 1;
    private Vector3 moveDirection = Vector3.zero;
    public Vector3 lookDirection = Vector3.zero;
	public float cameraTargetDirection = 0;

    public float cameraTurnRate = 2.0f;

    private float cameraTurnStopTime;
    private float targetCameraDirection;

    public bool isWalking = false;

    public bool isAlive = true;
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

    private Vector3 lastGoodPos;
    public int timesToSnapY = 100;
    public int timesSnappedY = 0;

    public Vector3 spawnPos = new Vector3(0, 50.0f, 0);
    public bool isFrozen = false;

    public bool skipFreeze = false;
    public bool shouldDie = false;

    private DeathClock dc;

    public void Freeze() {
        isFrozen = true;
        controller.enabled = false;
        //var rb = GetComponent<Rigidbody>();
        //rb.useGravity = false;
        //rb.isKinematic = false;
        inControl = false;
        doneMove = true;
        moveTarget = null;
    }

    public void UnFreeze() {
        isFrozen = false;
        controller.enabled = true;
        //var rb = GetComponent<Rigidbody>();
        //rb.useGravity = true;
        //rb.isKinematic = true;
        inControl = true;
        doneMove = true;
        moveTarget = null;
    }

    void OnLevelWasLoaded() {
        Debug.Log("I'M IN A NEW SCENE YEHAW");
        
        if( skipFreeze ){
            Debug.Log("I was told to skip freezing so I did.");
        } else {
            moveTarget = null;
            Freeze();
            transform.position = spawnPos;

            var fc = Camera.main.GetComponent<FogController>();
            fc.SetNow(fc.GetDarkness());

            var snap = fc.GetFogSnapshot();
            snap.color = (Color)(new Color32(189, 189, 189, 255));
            snap.startDistance = 8.0f;
            snap.endDistance = 16.0f;
            snap.fov = 30.0f;
            fc.Change(snap, 3.0f, 3.0f);
            lastGoodPos = spawnPos;
        }
        //Camera.main.backgroundColor = (Color)(new Color32(189, 189, 189, 255));
    }

    public void ActuallyDie() {
        Debug.LogWarning("WE FUCKING DIED");
        whenDoneDo = DeathStage_Start;
        moveTime = 0.1f;

        startPos = transform.position;
        startRot = character.transform.rotation;
        inControl = false;
        doneMove = false;
        moveTarget = null;
        moveTimeSpent = 0.0f;
    }

    public void Die() {
        shouldDie = true;
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
        snap.fov = 17.0f;
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
        grandpa.focusKnob.Unlock();

        grandpa.moveTime = 1.0f;
        grandpa.moveTimeSpent = 0.0f;
        grandpa.doneMove = false;

        grandpa.whenDoneDo = KnobStage_OpenDoorAndWait;
    }

    public static void KnobStage_OpenDoorAndWait(Grandpa grandpa) {
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
        grandpa.controller.enabled = false;
        grandpa.isWalking = true;

        var timeToShit = 2.0f;
        var fuckMult = 1.0f;
        var fc = Camera.main.GetComponent<FogController>();
        var snap = fc.GetFogSnapshot();
        snap.color = Color.black;
        snap.startDistance = 0.1f;
        snap.endDistance = 0.4f;
        fc.Change(snap, timeToShit * fuckMult, timeToShit * fuckMult);

        grandpa.moveTime = timeToShit;

        grandpa.SetTarget(grandpa.focusKnob.walkToTarget);
        grandpa.whenDoneDo = KnobStage_InsideDoor;
    }

    public static void KnobStage_InsideDoor(Grandpa grandpa) {

        // reset the movetime to get a wait out of this
        grandpa.controller.enabled = true;
        grandpa.isWalking = false;
        grandpa.moveTime = 0.1f;
        grandpa.SetTarget(grandpa.focusKnob.walkToTarget);

        var w = GameObject.FindObjectOfType<RoomGenergreater>();

        SceneManager.LoadScene(w.nextSceneName);

        grandpa.whenDoneDo = null;
    }

    public static void DeathStage_Start(Grandpa grandpa) {
        Debug.LogWarning("DEATH STARTED");
        //AkSoundEngine.PostEvent("grandpaDied", grandpa.gameObject);
        grandpa.m_Animator.SetBool("Dead", true);

        var lg = grandpa.gameObject.GetComponentInChildren<LookGrandpa>();
        lg.enabled = false;

        var timeToShit = 5.0f;

        grandpa.whenDoneDo = DeathStage_Out;
        grandpa.moveTime = timeToShit;
        grandpa.moveTimeSpent = 0.0f;
        grandpa.doneMove = false;
        
        var fc = Camera.main.GetComponent<FogController>();
        var snap = fc.GetFogSnapshot();
        snap.color = Color.black;
        snap.startDistance = 1.0f;
        snap.endDistance = 4.0f;
        snap.fov = 17.0f;
        fc.Change(snap, timeToShit, timeToShit);

        grandpa.isWalking = false;
        grandpa.inControl = false;
    }

    public static void DeathStage_Out(Grandpa grandpa) {
        Debug.LogWarning("WE OUTIE");
        var timeToShit = 2.0f;

        grandpa.whenDoneDo = DeathStage_Gone;
        grandpa.moveTime = timeToShit;
        grandpa.moveTimeSpent = 0.0f;
        grandpa.doneMove = false;

        var fc = Camera.main.GetComponent<FogController>();
        var snap = fc.GetFogSnapshot();
        snap.color = Color.black;
        snap.startDistance = 0.0f;
        snap.endDistance = 0.0f;
        fc.Change(snap, timeToShit, timeToShit);

        grandpa.isWalking = false;
        grandpa.inControl = false;
    }

    public static void DeathStage_Gone(Grandpa grandpa) {
        Debug.LogWarning("YEEHAW WE HERE");
        grandpa.controller.enabled = true;
        grandpa.isWalking = false;
        grandpa.moveTime = 0.1f;
        grandpa.moveTimeSpent = 0.0f;
        grandpa.doneMove = false;

        SceneManager.LoadScene("TheEnd");

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
        dc = GetComponent<DeathClock>();
        character = GameObject.Find("GrandFatherContainmentUnit");
		cameraHolder = GameObject.Find ("CameraHolder");
        cameraTurnStopTime = Time.fixedTime-1.0f;
        lastGoodPos = transform.position;
        // If for any reason the player is not at the world origin or the camera isn't facing it, this will break. \o/
        //cameraOffset = Camera.main.transform.position;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            var dc = GetComponent<DeathClock>();
            dc.timeToDie = 0.1f;
        }
        if ( inControl ){
            float moveHorizontal = Input.GetAxis("Horizontal") * mitigation;
            float moveVertical = Input.GetAxis("Vertical") * mitigation;

            // Are we moving?
            m_Animator.SetBool("Walking", (moveVertical == 0 && moveHorizontal == 0));

            if( Time.fixedTime <= cameraTurnStopTime ){
                cameraHolder.transform.RotateAround(transform.position, Vector3.up, (cameraTurnDirection * cameraTurnAmount * Time.deltaTime) / cameraTurnRate);
            } else {
                if (Input.GetButtonDown("Fire2")) {
                    cameraTurnStopTime = Time.fixedTime + cameraTurnRate;
                    cameraTargetDirection += cameraTurnAmount;
                    cameraTurnDirection = 1;
                    if (cameraTargetDirection > 359f) {
                        cameraTargetDirection = 0f;
                    }
                } else if ( Input.GetButtonDown("Fire1")) {
                    cameraTurnStopTime = Time.fixedTime + cameraTurnRate;
                    cameraTargetDirection -= cameraTurnAmount;
                    cameraTurnDirection = -1;
                    if (cameraTargetDirection < 0.0f) {
                        cameraTargetDirection = 360.0f - cameraTurnAmount;
                    }
                }
            }

            if (controller.isGrounded) {
                moveDirection = new Vector3(moveHorizontal, 0, moveVertical);
                moveDirection = Quaternion.AngleAxis(cameraTargetDirection, Vector3.up) * moveDirection;
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= moveSpeed;

                // Turn if we need to turn.
                if (moveVertical != 0 || moveHorizontal != 0) {
                    lookDirection = new Vector3(-moveHorizontal, 0, -moveVertical);
                    lookDirection = Quaternion.AngleAxis(cameraTargetDirection, Vector3.up) * lookDirection;
                    float step = turnSpeed * Time.deltaTime;
                    var lookRot = Quaternion.LookRotation(lookDirection.normalized);
                    character.transform.rotation = Quaternion.RotateTowards(character.transform.rotation, lookRot, step);
                }
            } else {
                moveDirection = Vector3.zero;
            }

            // Cancel gravity, move to position.
            //moveDirection.y -= Physics.gravity.y * Time.deltaTime;
            controller.SimpleMove(moveDirection * Time.deltaTime);

            // Consider our own mortality.
            dc.SecretUpdate();
            if ( shouldDie ){
                ActuallyDie();
            }
        } else {
            m_Animator.SetBool("Walking", !isWalking);

            // finish translating the camera if TomR tries to break it when opening a door
            if (Time.fixedTime <= cameraTurnStopTime) {
                cameraHolder.transform.RotateAround(transform.position, Vector3.up, (cameraTurnDirection * cameraTurnAmount * Time.deltaTime) / cameraTurnRate);
            }

            // player not in control, lerp to position
            if ( moveTimeSpent <= moveTime && !doneMove ) {
                moveTimeSpent += Time.deltaTime;
                if( moveTarget) {
                    transform.position = Vector3.Lerp(startPos, moveTarget.position, moveTimeSpent / moveTime);
                    character.transform.rotation = Quaternion.Lerp(startRot, moveTarget.rotation, moveTimeSpent / moveTime);
                }
                if( moveTimeSpent >= moveTime) {
                    doneMove = true;
                    if( whenDoneDo != null) {
                        whenDoneDo(this);
                    }
                }
            } else if( !isFrozen && moveTarget ){
                // enforce our local position because now that our character controller is disabled
                // we will go straight through the gat dang floor
                transform.position = moveTarget.position;
                character.transform.rotation = moveTarget.rotation;
            } else if( isFrozen ){
                transform.position = spawnPos;
            }

            if( controller.enabled) {
                moveDirection = Vector3.zero;
                //moveDirection.y -= Physics.gravity.y * Time.deltaTime;
                controller.SimpleMove(moveDirection * Time.deltaTime);
            }
        }

        if( isAlive) {
            // Report location to fog shader.
            Shader.SetGlobalVector("_Origin", transform.position + (Vector3.up * 2f));

            // Set the camera's position.
            cameraHolder.transform.position = transform.position;
        }

        // No more floor grandpas.
        var pos =  transform.position;
        if( pos.y < 0.0f) {
            Debug.LogWarning("FLOORCLIP: " + pos.y + " AT " + pos);
            pos.y += Mathf.Abs(pos.y);
            transform.position = pos;
            timesSnappedY++;
            if( timesSnappedY >= timesToSnapY) {
                transform.position = lastGoodPos;
            }
        } else if( controller.isGrounded ) {
            timesSnappedY = 0;
            lastGoodPos = pos;
        }
    }

    public void SetTarget(Transform target) {
        //controller.enabled = false;
        //var rb = GetComponent<Rigidbody>();
        //rb.useGravity = false;
        //rb.isKinematic = false;
        startPos = transform.position;
        startRot = character.transform.rotation;
        inControl = false;
        doneMove = false;
        moveTarget = target;
        moveTimeSpent = 0.0f;
    }
}
