/// <summary>
/// Bacterium Interactive - Script for upcoming immune system game 2015
/// Programmers: Alex Gilbert, Darias Skiedra, Zhan Simeonov, Dave Elliott
/// Artists: Linda Marie Martinez, Steph Rivo
///
/// Bacterium Homepage: https://sites.google.com/site/bacteriuminteractive
/// </summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// Struct to hold data for aligning camera
/// </summary>
struct CameraPosition
{
    //Position to align camera to, probably somewhere behind the character
    private Vector3 position;
    //Transform used for any rotation
    private Transform xForm;

    public Vector3 Position { get { return position; } set { position = value; } }
    public Transform XForm { get { return xForm; } set { xForm = value; } }

    public CameraPosition(string camName, Vector3 pos, Transform transform, Transform parent)
    {
        position = pos;
        xForm = transform;
        xForm.name = camName;
        xForm.parent = parent;
        xForm.localPosition = Vector3.zero;
        xForm.localPosition = position;
    }
}

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class SC_ThirdPersonCamera : MonoBehaviour
{
    #region Variables (private)
    [SerializeField]
    private Transform parentRig;
    [SerializeField]
    private float distanceAway;
    [SerializeField]
    private float distanceAwayMultiplier = 1.5f;
    [SerializeField]
    private float distanceUp;
    [SerializeField]
    private float distanceUpMultiplier = 5f;
    [SerializeField]
    private float smooth;
    [SerializeField]
    private Transform followXForm;
    [SerializeField]
    private SC_CharacterController follow;
    [SerializeField]
    private float widescreen = 0.2f;
    [SerializeField]
    private float targetingTime = 0.5f;
    [SerializeField]
    private float firstPersonThreshold = 0.5f;
    [SerializeField]
    private float firstPersonLookSpeed = 1.5f;
    [SerializeField]
    private Vector2 firstPersonXAxisClamp = new Vector2(-70.0f, 90.0f);
    [SerializeField]
    private float fpsRotationDegreePerSecond = 120f;
    [SerializeField]
    private float freeThreshold = -0.1f;
    [SerializeField]
    private Vector2 camMinDistFromChar = new Vector2(1f, -0.5f);
    [SerializeField]
    private float rightStickThreshold = 0.1f;
    [SerializeField]
    private const float freeRotationDegreePerSecond = -5f;
    [SerializeField]
    private Transform rightShoulderCamPos;
    [SerializeField]
    private Transform leftShoulderCamPos;

    //Smoothing and damping
    private Vector3 velocityCamSmooth = Vector3.zero;
    [SerializeField]
    private float camSmoothDampTime = 0.1f;
    private Vector3 velocityLookDir = Vector3.zero;
    [SerializeField]
    private float lookDirDampTime = 0.1f;

    // Private global only
    private Vector3 lookDir;
    private Vector3 curLookDir;
    //private Vector3 targetPosition;
    private CamStates camState = CamStates.Behind;


    private float xAxisRot = 0.0f;
    private CameraPosition firstPersonCamPos;
    private float lookWeight;
    private const float TARGETING_THRESHOLD = .01f;
    private Vector3 savedRigToGoal;
    private float distanceAwayFree;
    private float distanceUpFree;
    private Vector2 rightStickPrevFrame = Vector2.zero;
    

    private bool useRightCam = true;
    #endregion


    #region Properties (public)
    public enum CamStates
    {
        Behind,
        FirstPerson,
        SnapCamera,
        Targeting,
        Free
    }

    public CamStates CamState
    {
        get { return camState; }
    }

    public Transform ParentRig { get { return parentRig; } }
    public Transform FirePoint;
    #endregion


    #region Unity event functions

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        //followXForm = GameObject.FindWithTag("Player").transform;
        follow = followXForm.parent.GetComponent<SC_CharacterController>();
        //parentRig = this.transform.root;

        curLookDir = followXForm.forward;
        lookDir = followXForm.forward;

        //Position and parent a GameObject where first person view should be
        firstPersonCamPos = new CameraPosition
            (
                "First Person Camera",
                new Vector3(0.0f, 1.6f, 0.2f),
                new GameObject().transform,
                follow.transform
            );
    }


    public void Update()
    {
        if (Input.GetButtonDown("SwitchShoulderCam"))
        {
            useRightCam = !useRightCam;
        }
    }

    public void FixedUpdate()
    {
        
            // Pull Values form controller/keyboard
            float rightX = Input.GetAxis("RightStickX");
            float rightY = Input.GetAxis("RightStickY");
            float leftX = Input.GetAxis("Horizontal");
            float leftY = Input.GetAxis("Vertical");
            float dpadX = Input.GetAxisRaw("DPadX");
            float dpadY = Input.GetAxisRaw("DPadY");

            Vector3 characterOffset = followXForm.position + new Vector3(0f, distanceUp, 0f);
            Vector3 lookAt = characterOffset;
            Vector3 targetPosition = Vector3.zero;
            float dampTime = camSmoothDampTime;

            //Determine camera state
            if (Input.GetAxis("SnapCamera") > TARGETING_THRESHOLD)
            {
                camState = CamStates.SnapCamera;
            }
            else
            {
                // * First Person *
                if (dpadY > firstPersonThreshold && camState != CamStates.Free && camState != CamStates.Targeting)
                {
                    //Reset look before entering the first person mode
                    xAxisRot = 0;
                    lookWeight = 0f;
                    camState = CamStates.FirstPerson;
                }

                // * Free Camera *
                if (dpadY < freeThreshold && System.Math.Round(follow.Speed, 2) == 0 && camState != CamStates.FirstPerson && camState != CamStates.Targeting)
                {
                    camState = CamStates.Free;
                    savedRigToGoal = Vector3.zero;
                }

                //* Behind The back *
                if ((camState == CamStates.FirstPerson && Input.GetButton("ExitFPV") ||
                    (camState == CamStates.SnapCamera && (Input.GetAxis("SnapCamera") <= TARGETING_THRESHOLD))))
                {
                    camState = CamStates.Behind;
                }

                //* Targeting
                if (follow.IsTargeting)
                {
                    camState = CamStates.Targeting;

                }
                else if (camState == CamStates.Targeting)
                {
                    camState = CamStates.SnapCamera;
                }

                
            }

            //Execute camera logic based on state

            Quaternion rotationShift;
            Vector3 rotationAmount;
            Quaternion deltaRotation;

            //print(camState.ToString());    

            switch (camState)
            {
                case CamStates.Behind:

                    ResetCamera();

                    // Only update camera look direction if moving
                    if (follow.Speed > follow.LocomotionThreshold && follow.IsMoving())
                    {
                        lookDir = Vector3.Lerp(followXForm.right * (leftX < 0 ? 1f : -1f), followXForm.forward * (leftY < 0 ? -1f : 1f), Mathf.Abs(Vector3.Dot(this.transform.forward, followXForm.forward)));
                        Debug.DrawRay(this.transform.position, lookDir, Color.white);

                        // Calculate direction from camera to player, kill Y, and normalize to give a valid direction with unit magnitude
                        curLookDir = Vector3.Normalize(characterOffset - this.transform.position);
                        curLookDir.y = 0;
                        Debug.DrawRay(this.transform.position, curLookDir, Color.green);

                        // Damping makes it so we dont update targetPosition while pivoting; camera shouldn't rotate around playe
                        curLookDir = Vector3.SmoothDamp(curLookDir, lookDir, ref velocityLookDir, lookDirDampTime);
                    }

                    //parentRig.RotateAround(characterOffset, followXForm.up, freeRotationDegreePerSecond * (Mathf.Abs(rightX) > rightStickThreshold ? rightX : 0f));

                    targetPosition = characterOffset + followXForm.up * distanceUp - Vector3.Normalize(curLookDir) * distanceAway;
                    Debug.DrawLine(followXForm.position, targetPosition, Color.magenta);
                    break;
                case CamStates.SnapCamera:
                    ResetCamera();

                    curLookDir = followXForm.forward;
                    lookDir = followXForm.forward;
                    targetPosition = characterOffset + followXForm.up * distanceUp - lookDir * distanceAway;
                    break;
                case CamStates.FirstPerson:
                    // Looking up and down
                    // Calculate the amount of rotation and apply to the firstPersonCamPos GameObject
                    xAxisRot += (-rightY * firstPersonLookSpeed);
                    xAxisRot = Mathf.Clamp(xAxisRot, firstPersonXAxisClamp.x, firstPersonXAxisClamp.y);
                    firstPersonCamPos.XForm.localRotation = Quaternion.Euler(xAxisRot, 0, 0);

                    //Superimpose firstPersonCamPos GameObject's rotation on camera
                    rotationShift = Quaternion.FromToRotation(this.transform.forward, firstPersonCamPos.XForm.forward);
                    this.transform.rotation = rotationShift * this.transform.rotation;

                    //Looking left and right
                    // Similarly to how character is rotated while in locomotion, use Quaternion * to add rotation to character
                    rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0, fpsRotationDegreePerSecond * (rightX < 0f ? -1f : 1f), 0f), Mathf.Abs(rightX));
                    deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
                    follow.transform.rotation = follow.transform.rotation * deltaRotation;

                    // Move Camera to firstPersonCamPos
                    targetPosition = firstPersonCamPos.XForm.position;

                    //Smoothly transition look direction towards firstPersonCamPos when entering first person mode
                    lookAt = Vector3.Lerp(targetPosition + followXForm.forward, this.transform.position + this.transform.forward, camSmoothDampTime * Time.deltaTime);

                    //Choose lookAt target based on distance
                    lookAt = Vector3.Lerp(this.transform.position + this.transform.forward, lookAt, Vector3.Distance(this.transform.position, firstPersonCamPos.XForm.position));
                    break;
                case CamStates.Targeting:
                    dampTime = 0.075f;

                    Transform camPos = useRightCam ? rightShoulderCamPos : leftShoulderCamPos;

                    //Superimpose rightShoulderCamPos GameObject's rotation on camera
                    rotationShift = Quaternion.FromToRotation(this.transform.forward, camPos.forward);
                    this.transform.rotation = rotationShift * this.transform.rotation;

                    // Move Camera to rightShoulderCamPos
                    targetPosition = camPos.position;

                    //Choose lookAt target based on distance
                    lookAt = Vector3.Lerp(this.transform.position + this.transform.forward, FirePoint.position, Vector3.Distance(this.transform.position, camPos.position));
                    break;
                case CamStates.Free:
                    lookWeight = Mathf.Lerp(lookWeight, 0.0f, Time.deltaTime * firstPersonLookSpeed);

                    // Move height and distance from character in serparate parentRig thansform since RotateAround has control of both position and rotation
                    Vector3 rigToGoalDirection = Vector3.Normalize(characterOffset - this.transform.position);
                    // Can't calculate distanceAway from a vecotr with Y axis rotation in it; zero it out
                    rigToGoalDirection.y = 0f;

                    Vector3 rigToGoal = characterOffset - parentRig.position;
                    rigToGoal.y = 0;
                    Debug.DrawRay(parentRig.transform.position, rigToGoal, Color.red);

                    // Moving camera in and out
                    // If statement works for positiv values; don't tween if stick not increasing in either directino; also don't tween if user is rotating
                    // Checked against RIGHT X THRESHOLD becasue very small values fo rrightY mess up the lerp function
                    if (rightY < -1f * rightStickThreshold && rightY <= rightStickPrevFrame.y && Mathf.Abs(rightX) < rightStickThreshold)
                    {
                        distanceUpFree = Mathf.Lerp(distanceUp, distanceUp * distanceUpMultiplier, Mathf.Abs(rightY));
                        distanceAwayFree = Mathf.Lerp(distanceAway, distanceAway * distanceAwayMultiplier, Mathf.Abs(rightY));
                        targetPosition = characterOffset + followXForm.up * distanceUpFree - rigToGoalDirection * distanceAwayFree;
                    }
                    else if (rightY > rightStickThreshold && rightY >= rightStickPrevFrame.y && Mathf.Abs(rightX) < rightStickThreshold)
                    {
                        // Subtract height of camera from height of player to find Y distance
                        distanceUpFree = Mathf.Lerp(Mathf.Abs(transform.position.y - characterOffset.y), camMinDistFromChar.y, rightY);
                        // Use madnitude function to find X distance
                        distanceAwayFree = Mathf.Lerp(rigToGoal.magnitude, camMinDistFromChar.x, rightY);
                    }

                    // Store direction only if right stick inactive
                    if (rightX != 0 || rightY != 0)
                    {
                        savedRigToGoal = rigToGoalDirection;
                    }

                    parentRig.RotateAround(characterOffset, followXForm.up, freeRotationDegreePerSecond * (Mathf.Abs(rightX) > rightStickThreshold ? rightX : 0f));

                    // Still need to track camera behind player even if they aren't using the right sick; achieve this by saving distanceAway every frame
                    if (targetPosition == Vector3.zero)
                    {
                        targetPosition = characterOffset + followXForm.up * distanceUpFree - savedRigToGoal * distanceAwayFree;
                    }

                    break;
            }

            follow.Animator.SetBool("Targeting", camState == CamStates.Targeting);

            CompensateForWalls(characterOffset, ref targetPosition);

            smoothPosition(parentRig.position, targetPosition, dampTime);

            transform.LookAt(lookAt);

            rightStickPrevFrame = new Vector2(rightX, rightY);
    }

    

    /// <summary>
    /// Debugging information should be placed here.
    /// </summary>
    void OnDrawGizmos()
    {

    }

    #endregion


    #region Methods

    private void smoothPosition(Vector3 fromPos, Vector3 toPos, float dampTime)
    {
        parentRig.position = Vector3.SmoothDamp(fromPos, toPos, ref velocityCamSmooth, dampTime);
    }

    private void CompensateForWalls(Vector3 fromObject, ref Vector3 toTarget)
    {
        Debug.DrawLine(fromObject, toTarget, Color.cyan);
        // Compensate for walls between camera
        RaycastHit wallHit = new RaycastHit();
        if (Physics.Linecast(fromObject, toTarget, out wallHit))
        {
            Debug.DrawRay(wallHit.point, Vector3.left, Color.red);
            toTarget = new Vector3(wallHit.point.x, toTarget.y, wallHit.point.z);
        }
    }

    private void ResetCamera()
    {
        lookWeight = Mathf.Lerp(lookWeight, 0.0f, Time.deltaTime * firstPersonLookSpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime);
    }

    #endregion
}
