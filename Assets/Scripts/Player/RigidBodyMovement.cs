using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RigidBodyMovement : MonoBehaviour
{
    [Header("Transforms")]
    public Transform playerCam;
    public Transform grapplePosition;
    public Transform orientation;
    public Transform groundCheck;
    public Transform stepRayUpper;
    public Transform stepRayLower;
    
    // Other
    private Rigidbody rb;
    private GameOver gameOver;

    private AudioManager am;

    // Rotation and look
    private float xRotation;
    private float sensitivity = 50f;
    public float sensMultiplier = 1f;
    
    [Header("Movement")]
    public float moveSpeed = 50;
    public float maxSpeed = 17;
    public bool grounded;
    public bool moving;
    public bool sloped;
    public bool sliding;
    public bool bouncing;
    public bool inWindArea;

    public LayerMask whatIsGround;
    public LayerMask whatIsSlides;
    public LayerMask whatIsBouncy;
    public LayerMask whatIsWindArea;
    
    public float counterMovement = 0.25f;
    private float threshold = 0.01f;

    // Slope Movement
    public float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;
    private bool steepSlope = false;

    // Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 grappleGunScale = new Vector3(1, 2f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.25f;
    public float slideDashCooldown = 0.5f;
    private bool readyToSlideDash = true;
    private RaycastHit crouchHit;

    [Header("Aerial Movement")]
    public float jumpForce = 450f;
    public float gravity = 10f;
    public float airStrafeForward = 0.85f;
    public float airStrafeSideways = 0.75f;
    private bool readyToJump = true;
    public float jumpCooldown = 0.25f;

    // Dashing
    private bool canDash = true;
    public float dashForce = 30f;
    public int dashCooldown = 5;
    private DashRings dashR;

    // Input
    float x, y;
    bool jumping, sprinting, crouching, dashing, isCrouched;
    private InputController _input;
    
    // Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    // Wind Information
    private float windMagnitude;
    private Vector3 windDirection;

    // Ground Check
    private float groundDistance = 0.2f;

    [Header("Stair Climbing")]
    public bool allowStairClimb = false;
    public float stepHeight = 1f;
    public float stepSmooth = 3f;

    [Header("Particle Systems")]
    public ParticleSystem speedlinesSlow;
    public ParticleSystem speedlinesFast;
    private CameraFOV cameraFOV;


    [Header("Extra Variables")]
    public bool isPaused;
    public bool gameStart;
    public bool gameWon;
    private float stepTimer = 0f; // used to play step sound
    private float fallTimer = 0f; // used to play landing sound
    private float jumpTimer = 0f; // used to allow jump buffering

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        am = FindObjectOfType<AudioManager>();
        stepRayUpper.transform.localPosition = new Vector3(stepRayUpper.localPosition.x, stepHeight, stepRayUpper.localPosition.z);
    }
    
    void Start()
    {
        gameOver = FindObjectOfType<GameOver>();
        playerScale = transform.localScale;
        crouchScale = new Vector3(playerScale.x, playerScale.y / 2, playerScale.z);
        _input = GetComponent<InputController>();
        cameraFOV = GetComponentInChildren<CameraFOV>();
        dashR = GetComponentInChildren<DashRings>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    private void FixedUpdate() {
        Movement();
        if(allowStairClimb)
        {
            StairCheck();
        }
    }

    private void Update()
    {
        // Get player input
        GetInput();
        Look();

        // Checking for what type of object the player is currently on for physics
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);
        sliding = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsSlides);
        bouncing = Physics.CheckSphere(groundCheck.position, groundDistance + 0.2f, whatIsBouncy);
        inWindArea = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsWindArea);
        sloped = OnSlope();

        FallCheck();
        if(grounded)
        {
            fallTimer = 0f;
            //canDash = true;
        }

        if (gameOver.isGameOver)
        {
            float speed = 0.1f;
            if (Vector3.Distance(transform.position, gameOver.transform.position) > 300f)
                speed = 0.3f;
            else if (Vector3.Distance(transform.position, gameOver.transform.position) > 200f)
                speed = 0.2f;

            transform.position = Vector3.MoveTowards(transform.position, gameOver.transform.position, speed);
            rb.useGravity = false;
            gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "WindArea")
        {
            GameObject windArea = other.gameObject;
            windDirection = windArea.GetComponent<WindArea>().direction;
            windMagnitude = windArea.GetComponent<WindArea>().magnitude;
        }

        if (other.gameObject.tag == "GameStartTrigger")
        {
            gameStart = true;
        }

        if (other.gameObject.tag == "Crystal")
        {
            gameWon = true;
        }
    }

    /// <summary>
    /// Find user input.
    /// </summary>
    private void GetInput()
    {
        x = _input.move.x;
        y = _input.move.y;
        jumping = _input.jump;
        crouching = _input.crouch;
        dashing = _input.dash;
      
        //Crouching
        if (crouching)
            StartCrouch();
        if (!crouching)
            StopCrouch();

        if (x != 0 || y != 0)
            StepCheck();

        if (dashing)
            Dash();
    }

    private void StartCrouch()
    {
        if(!isCrouched)
        {
            isCrouched = true;
            playerScale = transform.localScale;
            transform.localScale = new Vector3(playerScale.x, playerScale.y / 2, playerScale.z);
            grapplePosition.localScale = grappleGunScale;
            if (rb.velocity.magnitude > 0.5f) {
                if (grounded) {
                    rb.AddForce(orientation.transform.forward * slideForce);
                }
            }

            // If we're very close to the ground, give extra downward and forward force
            if (Physics.CheckSphere(groundCheck.position, groundDistance + 5f, whatIsGround))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
                rb.AddForce(-orientation.transform.up * slideForce);

                if (readyToSlideDash)
                {
                    readyToSlideDash = false;
                    rb.AddForce(slideForce * y * orientation.transform.forward * 1.35f);
                    rb.AddForce(slideForce * x * orientation.transform.right);
                    Invoke(nameof(ResetSlideDash), slideDashCooldown);
                }
            }
        }
    }

    private void StopCrouch()
    {
        if(isCrouched)
        {
            // Don't uncrouch if there's an object directly above player
            // NOTE: This still allows them to move as if they were uncrouched, but prevents them from getting stuck in objects
            if (!Physics.Raycast(transform.position, Vector3.up, 2.69f))
            {
                transform.localScale = playerScale;
                grapplePosition.localScale = Vector3.one;
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                isCrouched = false;
            }
        }
    }

    private void Movement()
    {
        if(gameOver.isGameOver) return;

        // Extra gravity
        rb.AddForce(Vector3.down * gravity);
        
        // Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;
        float velo = Mathf.Abs(xMag) + Mathf.Abs(yMag);

        // Counteract gliding and sloppy movement
        CounterMovement(x, y, mag);
        
        // If we can jump, then jump
        // otherwise, begin jump buffer timer
        if (readyToJump && jumping) 
        {
            if (grounded && !steepSlope) Jump();
            else JumpBuffer();
        }

        // Apply SpeedLines UI and change camera FOV depending on velocity
        ApplySpeedEffects(mag);
        
        // If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        // Movement multipliers
        float multiplier = 1f, multiplierV = 1f;
        
        // Movement in air
        if (!grounded) {
            multiplier = airStrafeForward;
            multiplierV = airStrafeSideways;
        }
        
        // Movement while sliding
        if (grounded && crouching) 
        {
            rb.AddForce(60f * Vector3.down);
            multiplier = 0.1f;
            multiplierV = 0.1f;
        }

        // Movement while on a moving platform
        if (moving)
        {
            RaycastHit moveHit;
            if (Physics.Raycast(groundCheck.position, Vector3.down, out moveHit, groundDistance))
            {
                Vector3 hitVelo = moveHit.rigidbody.GetPointVelocity(moveHit.point);
                hitVelo.y = 0f;
                //Debug.Log(hitVelo);
                rb.AddForce(hitVelo * hitVelo.magnitude);
            }
        }

        // Movement while hitting a Bouncy obstacle
        if (bouncing)
        {
            ResetDashImmediate();
        }

        // Movement while in wind area
        if (inWindArea)
        {
            rb.AddForce(windDirection * windMagnitude);
        }

        // Movement while being on a Slide
        if (sliding && !grounded)
        {
            // Give player less control while on slides
            multiplier = 0.15f;
            multiplierV = 0.15f;

            // If going too slowly on a Slide, give player some speed so they don't get stuck
            if (velo < 18f)
            {
                // Get the down direction of the slide's slope
                Vector3 slopeCross = Vector3.Cross(Vector3.up, slopeHit.normal);
                Vector3 slopeDown = Vector3.Cross(slopeCross, slopeHit.normal);

                // Apply force both globally down, as well as on the slope's down
                rb.AddForce(30f * Vector3.down);
                rb.AddForce(60f * slopeDown);
            }
        }

        // Movement while on a steep slope
        if (steepSlope)
        {
            rb.AddForce(35f * slopeHit.normal);
            rb.AddForce(60f * Vector3.down);
            return;
        }
    
        // Movement while on a slope
        if (sloped && !sliding)
        {
            rb.AddForce(2f * moveSpeed * multiplier * multiplierV * y * GetSlopeMoveDirection());
            rb.AddForce(2f * moveSpeed * multiplier * x * GetSlopeMoveDirectionRight());

            // Prevent the player from sliding downwards when there's no movement
            if (rb.velocity.y < 1f)
            {
                rb.AddForce(-rb.velocity.normalized);
            }
            else // Otherwise, give a bit of extra gravity so there's no bounciness when walking up slopes
            {
                rb.AddForce(50f * Vector3.down);
            }

            return;
        }

        // Apply forces to move player regularly
        rb.AddForce(orientation.transform.forward * y * moveSpeed * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * multiplier);
    }

    private void Jump()
    {
        // Reset jump controls
        readyToJump = false;
        jumpTimer = 0f;

        // Add jump forces
        rb.AddForce(1.5f * jumpForce * Vector2.up);
        rb.AddForce(0.5f * jumpForce * normalVector);

        // If holding forward, give a bit of forward velocity when jumping
        //if (y > 0) rb.AddForce(0.5f * jumpForce * orientation.transform.forward);
            
        // If jumping while falling, reset y velocity.
        Vector3 vel = rb.velocity;
        if (rb.velocity.y < 0.5f)
            rb.velocity = new Vector3(vel.x, 0, vel.z);
        else if (rb.velocity.y > 0) 
            rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    // Allows Jump Buffering, leading to more responsive inputs
    private void JumpBuffer()
    {
        jumpTimer += 0.02f; // 0.02f = Time.deltaTime (usually)
        if (jumpTimer > 0.09f) // roughly 4-5 frames
        {
            // Resets jump input, forcing the player to let go of jump key to jump again
            _input.jump = false;
            jumpTimer = 0f;
            return;
        }
    }
    
    private void ResetJump() {
        readyToJump = true;
    }

    private void ResetSlideDash() {
        readyToSlideDash = true;
    }

    private void Dash()
    {
        if (!grounded && canDash)
        {
            canDash = false;
            dashR.Lock();

            if(y == 0 && x == 0) rb.AddForce(orientation.transform.forward * dashForce, ForceMode.Impulse);
            rb.AddForce(dashForce * y * orientation.transform.forward, ForceMode.Impulse);
            rb.AddForce(dashForce * x * orientation.transform.right, ForceMode.Impulse);

            cameraFOV.GoDashing();

            dashR.SetRings(0);
            am.Play("dash_impact");
            dashRoutine = StartCoroutine(ResetDashRoutine());
            //Invoke(nameof(ResetDashImmediate), dashCooldown);
        }
    }

    public void ResetDashImmediate() {
        if (!canDash)
        {
            Debug.Log("ResetDashImmediate: canDash = true");
            if (dashRoutine != null)
                StopCoroutine(dashRoutine);
            canDash = true;
            dashR.SetRings(3);
            dashR.Unlock();
            am.Play("dash_recharge");
        }
        else {return;}
    }

    private Coroutine dashRoutine;
    IEnumerator ResetDashRoutine() {
        if (!canDash)
        {
            for(int i = 0; i < dashCooldown; i++)
            {
                yield return new WaitForSeconds(1);
                dashR.SetRings(i);
                Debug.Log("ResetDashRoutine: " + i);
            }
            Debug.Log("ResetDashRoutine: canDash = true");
            canDash = true;
            //dashR.SetRings(3);
            dashR.Unlock();
            am.Play("dash_recharge");
            dashRoutine = null;
        }
        else
        {
            yield return null;
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(groundCheck.position, Vector3.down, out slopeHit, groundDistance))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            
            // Disable jumping & moving on steep slopes
            if (angle >= maxSlopeAngle) steepSlope = true;
            else steepSlope = false;

            return angle < maxSlopeAngle && angle != 0;
        }

        steepSlope = false;
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(orientation.transform.forward, slopeHit.normal).normalized;
    }

    private Vector3 GetSlopeMoveDirectionRight()
    {
        return Vector3.ProjectOnPlane(orientation.transform.right, slopeHit.normal).normalized;
    }
    
    private float desiredX;
    private void Look() 
    {
        if (!isPaused)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

            // Find current look rotation
            Vector3 rot = playerCam.transform.localRotation.eulerAngles;
            desiredX = rot.y + mouseX;

            // Rotate, and also make sure we dont over- or under-rotate.
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Perform the rotations
            playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
            orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
        }
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping || steepSlope) return;

        // Slow down sliding
        if (crouching) {
            rb.AddForce(moveSpeed * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        // Counter movement
        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) {
            rb.AddForce(-mag.x * counterMovement * moveSpeed * orientation.transform.right);
        }
        if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(-mag.y * counterMovement * moveSpeed * orientation.transform.forward);
        }
        
        // Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        // TODO: Fix this
        if (Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2)) > maxSpeed) {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    // Apply SpeedLines UI and change camera FOV depending on velocity
    private void ApplySpeedEffects(Vector2 mag)
    {
        float velo = Mathf.Abs(mag.x) + Mathf.Abs(mag.y);
        if (velo > 200)
        {
            cameraFOV.GoingWayTooFast();
        }
        if (velo > 150 && velo < 185)
        {
            cameraFOV.GoingTooFast();
        }
        else if (velo > 125 && velo < 145)
        {
            cameraFOV.GoingVeryFast();
        }
        else if(velo > 80 && velo < 120)
        {
            speedlinesFast.Play();
            cameraFOV.GoingFaster();
        }
        else if (velo > 50 && velo < 70)
        {
            if(speedlinesFast.isPlaying)
            {
                speedlinesFast.Stop();
            }
            speedlinesSlow.Play();
            cameraFOV.GoingFast();
        }
        else if (velo > 25)
        {
            // Buffer
        }
        else
        {
            speedlinesSlow.Stop();
            speedlinesFast.Stop();
            cameraFOV.GoingSlow();
        }
    }

    // Find the velocity relative to where the player is looking
    // Useful for vectors calculations regarding movement and limiting movement
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);
        
        return new Vector2(xMag, yMag);
    }

    public bool isBouncing()
    {
        return bouncing;
    }

    // Only called if 'allowStairClimb' is enabled
    private void StairCheck()
    {
        RaycastHit lower;
        if(Physics.SphereCast(stepRayLower.position, 0.4f, orientation.forward, out lower, whatIsGround))
        {
            Debug.Log("Lower Raycast Hit");
            RaycastHit upper;
            if(!Physics.Raycast(stepRayUpper.position, orientation.forward, out upper, 0.3f) && y > 0.1f)
            {
                Debug.Log("Upper Raycast No Hit");
                rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
            }
            else{
                Debug.Log("Upper Raycast Hit");
            }
        }
    }

    // Used to play walking sounds depending on material below
    private void StepCheck()
    {
        stepTimer += Time.deltaTime;
        if (grounded && stepTimer > 0.5f)
        {
            //Debug.Log("making sound");
            RaycastHit lower;
            if(Physics.SphereCast(stepRayLower.position, 0.01f, orientation.up * -1, out lower, whatIsGround))
            {
                //Debug.Log("getting mat");
                //Debug.Log(lower.collider.gameObject.tag);
                int variation = Random.Range(0, 2);
                if (lower.collider.gameObject.CompareTag("Leaf"))
                {
                    if (variation == 0)
                        am.Play("walk1_leaf");
                    else
                        am.Play("walk2_leaf");
                }
                else if (lower.collider.gameObject.CompareTag("Wood"))
                {
                    if (variation == 0)
                        am.Play("walk1_wood");
                    else
                        am.Play("walk2_wood");
                }
                else
                {
                    if (variation == 0)
                        am.Play("walk1");
                    else
                        am.Play("walk2");
                }
            }
            stepTimer = 0f;
        }
    }

    // Used to play falling sound
    private void FallCheck()
    {
        fallTimer += Time.deltaTime;
        if (grounded && fallTimer > 1.5f)
        {
            Debug.Log("Fall time: " + fallTimer);
            am.Play("landing_heavy");
            fallTimer = 0f;
        }
    }

    private void StopGrounded() {
        grounded = false;
    }
}
