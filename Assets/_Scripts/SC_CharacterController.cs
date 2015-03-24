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
/// #DESCRIPTION OF CLASS#
/// </summary>
public class SC_CharacterController : MonoBehaviour
{
    #region Variables (private)
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SC_FootContact feet;
    [SerializeField]
    private float DirectionalDampTime = .25f;
    [SerializeField]
    private float speedDampTime = .05f;
    [SerializeField]
    private SC_ThirdPersonCamera gamecam;
    [SerializeField]
    private float directionSpeed = 3.0f;
    [SerializeField]
    private float moveSpeed = 5.0f;
    [SerializeField]
    private float targetMoveSpeed = 5.0f;
    [SerializeField]
    private float minJumpHeight = 0.3f;
    [SerializeField]
    private float maxJumpHeight = 5f;
    [SerializeField]
    private float maxJumpTime = 3f;
    [SerializeField]
    private float doubleJumpStrength = 8f;
    [SerializeField]
    private float airMoveSpeed = .25f;
    [SerializeField]
    private float rotationDegreePerSecond = 120f;
    [SerializeField]
    private float aimSensitivity = 120f;
    [SerializeField]
    private bool invertAim = false;
    [SerializeField]
    private float fovDampTime = 3f;
    [SerializeField]
    private float jumpMultiplier = 1f;
    [SerializeField]
    private CapsuleCollider capCollider;
    [SerializeField]
    private SC_RibLook ribLook;
    [SerializeField]
    private Transform FireRangedSpot;
    [SerializeField]
    private GameObject Projectile;
    [SerializeField]
    private LineRenderer LaserPointer;
    [SerializeField]
    private Transform Reticule;
    [SerializeField]
    private AttackArea AttackArea;
    [SerializeField]
    private PlayerAttack lightAttack1;
    [SerializeField]
    private PlayerAttack lightAttack2;
    [SerializeField]
    private PlayerAttack lightAttack3;
    [SerializeField]
    private PlayerAttack heavyAttack;
    [SerializeField]
    private ParticleSystem heavyAttackParticlesCharge;
    [SerializeField]
    private ParticleSystem heavyAttackParticlesExplode;
    [SerializeField]
    private AttackArea DiveAttackArea;
    [SerializeField]
    private PlayerAttack _DiveAttack;
    [SerializeField]
    private ParticleSystem DiveExplosion;

    [SerializeField]
    AudioClip la1;
    [SerializeField]
    AudioClip la2;
    [SerializeField]
    AudioClip la3;

    [SerializeField]
    AudioClip haCharge;
    [SerializeField]
    AudioClip haRelease;

    [SerializeField]
    AudioClip Jump;
    [SerializeField]
    AudioClip DoubleJump;

    [SerializeField]
    AudioClip Dive;
    [SerializeField]
    AudioClip DiveImpact;

    //private global only
    private AnimatorStateInfo stateInfo;
    private AnimatorTransitionInfo transInfo;
    private const float SPRINT_SPEED = 2f;
    private const float SPRINT_FOV = 75f;
    private const float NORMAL_FOV = 60f;
    private float capsuleHeight;
    private bool isTargeting = false;
    private bool isShooting = false;
    private bool isJumping = false;
    private bool leftGround = false;
    private bool landed = false;
    private bool inAir = false;
    private bool hasDoubleJumped = false;
    private bool isAttacking = false;
    private bool isDiveAttack = false;
    private bool isBlocking = false;
    private AudioSource audio;

    //Input
    private float horizontal = 0.0f;
    private float vertical = 0.0f;
    private float rightHorizontal = 0.0f;
    private float rightVertical = 0.0f;

    private bool jumpKeyDown = false;
    private bool jumpKeyUp = false;
    private bool jumpKeyPressed = false;

    private bool fireKeyDown = false;
    private bool fireKeyUp = false;
    private bool fireKeyPressed = false;

    private bool targetKeyDown = false;
    private bool targetKeyUp = false;
    private bool targetKeyPressed = false;

    //Movement Variables
    private float speed = 0.0f;
    private float direction = 0f;
    private float charAngle = 0f;
    private float yVelocity = 0f;
    private Vector3 toMove = Vector3.zero; // Essentially the velocity
    private float jumpHeight = 0.0f; // The current height of our jump
    private float jumpStartHeight = 0.0f; // The yvalue of our position when we started our jump
    private Vector3 jumpVelocity = Vector3.zero;
    private Vector3 amountMovedLastFrame = Vector3.zero; // the toMove of last frame
    private Vector3 relStickDir = Vector3.zero;
    
    //checkpoint + point system
    private float distance; //used for the checkpoint system
    private Vector3 checkpoint_forward;
    private bool boolWait = false;
    private bool boolCheckpoint = false;
    public GameObject checkpoint;
    private Vector3 checkpoint_vector1;
    public int point_count = 0;
    private Vector3 vPosition;
    private bool boolJump = false;
    #endregion


    #region Properties (public)

    public Animator Animator { get { return animator; } }
    public float Speed { get { return speed; } }
    public float LocomotionThreshold { get { return 0.2f; } }
    public bool IsTargeting { get { return isTargeting; } }
    public int Health;

    #endregion



    #region Unity event functions

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        audio = GetComponent<AudioSource>();
        
        //animator = GetComponent<Animator>();
        capCollider = GetComponent<CapsuleCollider>();
        capsuleHeight = capCollider.height;
        heavyAttackParticlesCharge.enableEmission = false;

        if (animator.layerCount >= 2)
        {
            animator.SetLayerWeight(1, 1);
        }

        
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>	
    void Update()
    {
        if (boolWait == true)
        {
            return;
        }
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        transInfo = animator.GetAnimatorTransitionInfo(0);

        HandleInput();
        CheckAirState();

        

        if (landed)
        {
            hasDoubleJumped = false;
        }

        if(targetKeyDown)
        {
            StopCoroutine("TargetLogic");
            StartCoroutine("TargetLogic");
        }

        charAngle = 0f;
        direction = 0f;

        if(gamecam.CamState != SC_ThirdPersonCamera.CamStates.FirstPerson)
        {
            //translate controls stick coordinates into world/cam/character space
            StickToWorldspace(this.transform, gamecam.transform, ref direction, ref speed, ref charAngle);

            if (animator)
            {
                animator.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
                animator.SetFloat("Direction", direction, DirectionalDampTime, Time.deltaTime);

                if (speed < LocomotionThreshold && Mathf.Abs(horizontal) < 0.05f)  //Dead Zone
                {
                    animator.SetFloat("Direction", 0f);
                }
            }
        }

        if(!isTargeting)
        {
            if (jumpKeyDown && CanJump())
            {
                //rigidbody.velocity += Vector3.up * 5.0f;
                animator.SetBool("Jump", true);
                StopCoroutine("JumpLogic");
                StartCoroutine("JumpLogic");
                audio.PlayOneShot(Jump);
            }
        }

        if(!IsGrounded() && !isJumping && jumpKeyDown && !hasDoubleJumped)
        {
            GetComponent<Rigidbody>().velocity += new Vector3(0, doubleJumpStrength - GetComponent<Rigidbody>().velocity.y, 0);
            this.transform.LookAt(this.transform.position + (relStickDir * 1.5f));
            jumpVelocity = this.transform.forward * moveSpeed * speed;
            hasDoubleJumped = true;
            animator.SetBool("DoubleJump", true);
            audio.PlayOneShot(DoubleJump);
            Invoke("DoubleJumpFalse", .5f);
        }


        if (Input.GetButtonDown("LightAttack") && !isAttacking)
        {
            StartCoroutine("ComboAttack");
        }

        if (IsGrounded() && Input.GetButtonDown("HeavyAttack") && !isAttacking)
        {
            StartCoroutine("HeavyAttack");
        }

        if (IsGrounded() && Input.GetButtonDown("Block") && !isAttacking)
        {
            StartCoroutine("Block");
        }

        if (!IsGrounded() && !isJumping && Input.GetButtonDown("HeavyAttack") && hasDoubleJumped && !isDiveAttack)
        {
            StartCoroutine("DiveAttack");
        }

        animator.SetBool("Grounded", IsGrounded());

        LaserPointer.enabled = isTargeting;
        Reticule.gameObject.SetActive(isTargeting);

        RaycastHit laserHitPoint = new RaycastHit();

        if(Physics.Raycast(FireRangedSpot.position, FireRangedSpot.up, out laserHitPoint, 20))
        {
            LaserPointer.SetPosition(0, FireRangedSpot.position);
            LaserPointer.SetPosition(1, laserHitPoint.point);
            Reticule.position = laserHitPoint.point;
        }
        else
        {
            LaserPointer.SetPosition(0, FireRangedSpot.position);
            LaserPointer.SetPosition(1, FireRangedSpot.position + FireRangedSpot.up * 20);
            Reticule.position = FireRangedSpot.position + FireRangedSpot.up * 20;
        }

        //checkpoint code
        distance = Vector3.Distance(checkpoint.transform.position, gameObject.transform.position);
        if ((distance < 50) || (boolCheckpoint==false)){
            checkpoint_vector1 = checkpoint.transform.position;
            //checkpoint_forward = gameObject.transform.forward;
            boolCheckpoint = true;
        }
        if ((gameObject.transform.position.y < -5) && (boolCheckpoint==true))
        {
            //StartCoroutine(Wait());
            //gameObject.transform.LookAt(checkpoint_forward);
            gameObject.transform.position = checkpoint_vector1;
        }
        if (boolJump == true)
        {
            GetComponent<Rigidbody>().velocity += new Vector3 (0,20,0);
            boolJump = false;
        }
    }

    public void FixedUpdate()
    {
        if (gamecam.CamState != SC_ThirdPersonCamera.CamStates.FirstPerson)
        {
            toMove = Vector3.zero;
            if (IsGrounded() && !isJumping)
            {
                jumpVelocity = Vector3.zero;
                if (isTargeting)
                {
                    TargetMoveLogic();
                }
                else
                {
                    //reset the spine rotation
                    ribLook.curRotation = 0;

                    if(!isAttacking && !isBlocking)
                        MoveLogic();
                    else if (speed > .2f)
                        this.transform.LookAt(this.transform.position + (relStickDir * 1.5f));
                }

                this.GetComponent<Rigidbody>().position += toMove * Time.deltaTime;
            }
            else
            {
                if (jumpVelocity.x == 0 && jumpVelocity.y == 0 && jumpVelocity.z == 0)
                    jumpVelocity.Set(amountMovedLastFrame.x, amountMovedLastFrame.y, amountMovedLastFrame.z);

                AirMoveLogic();

                if (!isDiveAttack)
                    this.GetComponent<Rigidbody>().position += jumpVelocity * Time.deltaTime;
                else
                    this.GetComponent<Rigidbody>().AddForce(Vector3.down * 50, ForceMode.Acceleration);
            }

            float yToMove = 0;

            if (jumpHeight > 0)
                yToMove = jumpHeight + jumpStartHeight - this.transform.position.y;

            this.GetComponent<Rigidbody>().position += new Vector3(0, yToMove, 0);

            amountMovedLastFrame = toMove;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        /*if (collision.gameObject.tag != null)
        {
            if (collision.gameObject.tag == "Platform")
            {
                boolJump = true;
            }
            else if (collision.gameObject.tag == "Point")
            {
                point_count++;
                Destroy(collision.gameObject);
            }
        }*/

        jumpVelocity = Vector3.one * .00001f;
    }

    #endregion

    

    #region Methods
    IEnumerator Wait()
    {
        boolWait = true;
        yield return new WaitForSeconds(1);
        boolWait = false;
    }
    private void MoveLogic()
    {
        if (Mathf.Abs(charAngle) < 135)
        {
            if (speed > .2f)
            {
                //Rotate character model if stick is tilted right or left, but only if character is moving in that direction
                if ((direction >= 0 && horizontal >= 0) || (direction < 0 && horizontal < 0))
                {
                    Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (horizontal < 0f ? -1f : 1f), 0f), Mathf.Abs(horizontal));
                    Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
                    this.transform.rotation = (this.transform.rotation * deltaRotation);
                }
                else
                {
                    Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, charAngle, 0f), 1);//Mathf.Abs(horizontal));
                    Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime * directionSpeed);
                    this.transform.rotation = (this.transform.rotation * deltaRotation);
                }

                toMove = this.transform.forward * moveSpeed * speed;
            }
            else if (speed > .1f)
            {
                this.transform.LookAt(this.transform.position + (relStickDir * 1.5f));
            }
        }
        else
        {
            this.transform.LookAt(this.transform.position + (relStickDir * 1.5f));
        }
    }

    private void TargetMoveLogic()
    {
        //Looking left and right
        if (Mathf.Abs(rightHorizontal) > 0.1f)
        {
            Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0, aimSensitivity * (rightHorizontal < 0f ? -1f : 1f), 0f), Mathf.Abs(rightHorizontal));
            Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
            this.transform.rotation = this.transform.rotation * deltaRotation;
        }


        // Looking up and down
        if(Mathf.Abs(rightVertical) > 0.1f)
            ribLook.AddRotation(rightVertical * aimSensitivity * (invertAim ? 1f : -1f) * Time.deltaTime);

        //moving
        if (speed > 0.2f)
        {
            Vector3 stickDirection = new Vector3(horizontal, 0, vertical);

            Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, this.transform.forward);
            toMove = referentialShift * stickDirection * targetMoveSpeed;
        }
    }

    private void DoubleJumpFalse()
    {
        animator.SetBool("DoubleJump", false);
    }

    private void AirMoveLogic()
    {
        if (speed > 0.2f)
        {
            jumpVelocity += relStickDir * airMoveSpeed;
            if (jumpVelocity.sqrMagnitude > moveSpeed)
            {
                jumpVelocity.Normalize();
                jumpVelocity *= moveSpeed;
            }

            Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, charAngle, 0f), 1);//Mathf.Abs(horizontal));
            Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime * directionSpeed * .25f);
            this.transform.rotation = (this.transform.rotation * deltaRotation);
        }
    }

    private IEnumerator JumpLogic()
    {
        jumpStartHeight = this.transform.position.y;
        GetComponent<Rigidbody>().useGravity = false;
        isJumping = true;
        //rigidbody.velocity += new Vector3(0,10,0);
        float jumpHeightV = 0f;


        while ((jumpHeight < minJumpHeight || jumpKeyPressed) && jumpHeight <= maxJumpHeight)
        {
            jumpHeight = Mathf.SmoothDamp(jumpHeight, maxJumpHeight + .15f, ref jumpHeightV, maxJumpTime);

            yield return null;
        }

        animator.SetBool("Jump", false);
        jumpHeight = 0;
        GetComponent<Rigidbody>().useGravity = true;
        isJumping = false;
        yield return null;
    }

    private IEnumerator TargetLogic()
    {
        isTargeting = true;
        animator.SetBool("Targeting", true);

        while(targetKeyPressed)
        {
            if(fireKeyDown)
            {
                StopCoroutine("FireLogic");
                StartCoroutine("FireLogic");
            }
            yield return null;
        }

        isTargeting = false;
        animator.SetBool("Targeting", false);
        yield return null;
    }

    private IEnumerator FireLogic()
    {
        isShooting = true;

        //We will use this if we choose to let the player charge his shots
        /*
        while (fireKeyPressed)
        {

            yield return null;
        }
        */

        Rigidbody ballInstance;
        ballInstance = ((GameObject)Instantiate(Projectile, FireRangedSpot.position, Quaternion.identity)).GetComponent<Rigidbody>();

        ballInstance.velocity = FireRangedSpot.up * 30f;

        isShooting = false;
        yield return null;
    }

    private IEnumerator ComboAttack()
    {
        float attackTime = .46f;
        animator.SetBool("LAttackFinished", false);

        //First Attack
        isAttacking = true;
        animator.SetBool("LAttackOne", true);
        audio.PlayOneShot(la1);
        AttackArea.ActivateAttack(lightAttack1);
        
        if(IsGrounded())
            GetComponent<Rigidbody>().AddForce(transform.forward * 200, ForceMode.Impulse);
        else
        {
            jumpVelocity = Vector3.one * 0.0001f;
        }

        bool gotoNextAttack = false;

        float curTime = 0;
        while(curTime <= attackTime)
        {
            if(curTime >= attackTime *.5f)
            {
                AttackArea.EndAttack();
            }

            yield return null;
            if (Input.GetButtonDown("LightAttack") && curTime >= attackTime * .5f && IsGrounded())
            {
                gotoNextAttack = true;
            }
            curTime += Time.deltaTime;
        }

        //Second Attack
        if(gotoNextAttack)
        {
            animator.SetBool("LAttackTwo", true);
            AttackArea.ActivateAttack(lightAttack2);
            audio.PlayOneShot(la2);
            GetComponent<Rigidbody>().AddForce(transform.forward * 200, ForceMode.Impulse);
            gotoNextAttack = false;

            curTime = 0;
            while (curTime <= attackTime)
            {
                if (curTime >= attackTime * .5f)
                {
                    AttackArea.EndAttack();
                }

                yield return null;
                if (Input.GetButtonDown("LightAttack") && curTime >= attackTime * .5f)
                {
                    gotoNextAttack = true;
                }
                curTime += Time.deltaTime;
            }

            //Third Attack
            if (gotoNextAttack)
            {
                animator.SetBool("LAttackThree", true);
                AttackArea.ActivateAttack(lightAttack1);
                audio.PlayOneShot(la3);
                ribLook.curZRotation = -90;
                GetComponent<Rigidbody>().AddForce(transform.forward * 500, ForceMode.Impulse);

                yield return new WaitForSeconds(attackTime * .5f);
                AttackArea.EndAttack();

                yield return new WaitForSeconds(attackTime * .5f);

                EndComboAttack();
            }
            else
                EndComboAttack();
        }
        else
            EndComboAttack();

        yield return null;
    }

    private void EndComboAttack()
    {
        AttackArea.EndAttack();

        animator.SetBool("LAttackOne", false);
        animator.SetBool("LAttackTwo", false);
        animator.SetBool("LAttackThree", false);
        ribLook.curZRotation = 0;

        animator.SetBool("LAttackFinished", true);

        isAttacking = false;
    }

    private IEnumerator HeavyAttack()
    {
        isAttacking = true;
        
        animator.SetBool("HeavyAttack", true);

        audio.PlayOneShot(haCharge);

        yield return new WaitForSeconds(.25f);
        heavyAttackParticlesCharge.enableEmission = true;

        while(Input.GetButton("HeavyAttack"))
        {
            yield return null;
        }

        heavyAttackParticlesCharge.enableEmission = false;
        animator.SetBool("HeavyAttack", false);
        
        //yield return new WaitForSeconds(.25f);

        AttackArea.ActivateAttack(heavyAttack);
        heavyAttackParticlesExplode.Play();
        audio.PlayOneShot(haRelease);
        yield return new WaitForSeconds(.25f);

        AttackArea.EndAttack();
        isAttacking = false;
    }

    private IEnumerator Block()
    {
        isBlocking = true;
        animator.SetBool("Block", true);

        while (Input.GetButton("Block"))
        {
            yield return null;
        }

        isBlocking = false;
        animator.SetBool("Block", false);
        yield return null;
    }

    private IEnumerator DiveAttack()
    {
        isDiveAttack = true;
        animator.SetBool("Dive Attack", true);
        DiveAttackArea.ActivateAttack(_DiveAttack);
        audio.PlayOneShot(Dive);

        while(IsGrounded() == false)
        {
            yield return null;
        }


        DiveAttackArea.EndAttack();
        DiveExplosion.Play();
        audio.PlayOneShot(DiveImpact);
        isDiveAttack = false;
        animator.SetBool("Dive Attack", false);
        yield return null;
    }



    /// <summary>
    /// Takes the current direction of the analogue stick and translates it to world space based off of the character transform and the camera
    /// </summary>
    /// <param name="root"> The transform the anolgue stick is affecting </param>
    /// <param name="camera"> The transform of the camera looking currently looking at the object </param>
    /// <param name="directionOut"> The direction, -1 = left 1 = right, in which the stick is pointing relative to the player and camera </param>
    /// <param name="speedOut"> The speed at which the character should move </param>
    public void StickToWorldspace(Transform root, Transform camera, ref float directionOut, ref float speedOut, ref float angleOut)
    {
        Vector3 rootDirection = root.forward;
        Vector3 stickDirection = new Vector3(horizontal, 0, vertical);

        speedOut = stickDirection.sqrMagnitude;

        if (speedOut > 1)
        {
            speedOut = 1;
            stickDirection.Normalize();
        }

        //Get camera rotation
        Vector3 CameraDirection = camera.forward;
        CameraDirection.y = 0.0f;//kill y
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, CameraDirection);

        //Convert joystick input to Worldspace coordinates
        relStickDir = referentialShift * stickDirection;
        Vector3 axisSign = Vector3.Cross(relStickDir, rootDirection);

        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), relStickDir, Color.green);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), axisSign, Color.red);
        //Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);

        float angleRootToMove = Vector3.Angle(rootDirection, relStickDir) * (axisSign.y >= 0 ? -1f : 1);

        //if (!isPivoting)
        //{
        angleOut = angleRootToMove;
        //}

        angleRootToMove /= 180f;

        directionOut = angleRootToMove;// *directionSpeed;
    }

    private void HandleInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        rightHorizontal = Input.GetAxis("RightStickX");
        rightVertical = Input.GetAxis("RightStickY");

        bool jumpIsNowPressed = Input.GetButton("Jump");
        jumpKeyDown = !jumpKeyPressed && jumpIsNowPressed;
        jumpKeyUp = jumpKeyPressed && !jumpIsNowPressed;
        jumpKeyPressed = jumpIsNowPressed;

        bool FireisNowPressed = Input.GetAxis("FireRanged") >= 0.5f  || Input.GetMouseButton(0);
        fireKeyDown = !fireKeyPressed && FireisNowPressed;
        fireKeyUp = fireKeyPressed && !FireisNowPressed;
        fireKeyPressed = FireisNowPressed;

        bool TargetisNowPressed = Input.GetAxis("Target") >= 0.5f || Input.GetButton("TargetKey");
        targetKeyDown = !targetKeyPressed && TargetisNowPressed;
        targetKeyUp = targetKeyPressed && !TargetisNowPressed;
        targetKeyPressed = TargetisNowPressed;
    }

    private void CheckAirState()
    {
        bool grounded = IsGrounded();
        landed = grounded && inAir;
        leftGround = !grounded && !inAir;
        inAir = grounded;
    }

    public bool CanJump()
    {
        return IsGrounded();
    }

    public bool IsGrounded()
    {
        return feet.NumOfContacts > 0;
    }

    public bool IsMoving()
    {
        return toMove.x != 0 || toMove.y != 0 || toMove.z != 0;
    }
    #endregion
}
