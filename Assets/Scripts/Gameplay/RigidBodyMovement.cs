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

    // Rotation and look
    private float xRotation;
    private float sensitivity = 50f;
    public float sensMultiplier = 1f;
    
    [Header("Movement")]
    public float moveSpeed = 1750;
    public float maxSpeed = 13;
    public bool grounded;
    public bool sliding;
    public bool bouncing;
    public bool inWindArea;

    public LayerMask whatIsGround;
    public LayerMask whatIsSlides;
    public LayerMask whatIsBouncy;
    public LayerMask whatIsWindArea;
    
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    // Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 grappleGunScale = new Vector3(1, 2f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    [Header("Aerial Movement")]
    public float jumpForce = 550f;
    public float gravity = 375f;
    public float airStrafeForward = 0.55f;
    public float airStrafeSideways = 0.55f;
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;

    // Dashing
    private bool canDash = true;
    public float dashForce = 40f;

    // Input
    float x, y;
    bool jumping, sprinting, crouching, dashing;
    
    // Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    // Wind Information
    private float windMagnitude;
    private Vector3 windDirection;

    // Ground Check
    private float groundDistance = 0.3f;

    [Header("Stair Climbing")]
    public bool allowStairClimb = false;
    public float stepHeight = 0.95f;
    public float stepSmooth = 3f;

    [Header("Particle Systems")]
    public ParticleSystem speedlinesSlow;
    public ParticleSystem speedlinesFast;

    [Header("Extra Variables")]
    public bool isPaused;
    public bool gameStart;
    public bool gameWon;

    GameOver gameOver;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        stepRayUpper.transform.position = new Vector3(stepRayUpper.position.x, stepHeight, stepRayUpper.position.z);
    }
    
    void Start() {
        gameOver = FindObjectOfType<GameOver>();
        playerScale =  transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    private void FixedUpdate() {
        Movement();
        if(allowStairClimb)
        {
            StepCheck();
        }
    }

    private void Update() {
        
        // Get player input
        MyInput();
        Look();

        // Checking for what type of object the player is currently on for physics
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);
        sliding = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsSlides);
        bouncing = Physics.CheckSphere(groundCheck.position, groundDistance + 0.2f, whatIsBouncy);
        inWindArea = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsWindArea);

        // Decrease dashBarAmount while holding left shift, or increase it if grounded
        if(grounded)
        {
            canDash = true;
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

        if (other.gameObject.tag == "Funnel")
        {
            if (other.gameObject.name == "Teleporter")
                transform.position = new Vector3(-1.82f, 172f, 6.9f);
            else if (other.gameObject.name == "Teleporter (1)")
                transform.position = new Vector3(-169.7f, 177f, -62.4f);
            else if (other.gameObject.name == "Teleporter (2)")
                transform.position = new Vector3(-220f, 105f, -186.5f);
            else if (other.gameObject.name == "Teleporter (3)")
                transform.position = new Vector3(-148.2f, 204f, -371.1f);
        }
    }

    /// <summary>
    /// Find user input.
    /// </summary>
    private void MyInput() {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);
        dashing = Input.GetKey(KeyCode.LeftShift);
      
        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();

        if (Input.GetKey(KeyCode.LeftShift))
            Dash();
    }

    private void StartCrouch() {
        transform.localScale = crouchScale;
        grapplePosition.localScale = grappleGunScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f) {
            if (grounded) {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    private void StopCrouch() {
        transform.localScale = playerScale;
        grapplePosition.localScale = Vector3.one;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement() {
        if(gameOver.isGameOver)
        {
            // Mommy
        }
        else
        {
            // Extra gravity
            rb.AddForce(Vector3.down * Time.deltaTime * gravity);
        }
        
        // Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        // Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);
        
        // If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        // Set max speed
        float maxSpeed = this.maxSpeed;
        
        // If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump) {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }
        
        // If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        // Apply SpeedLines UI depending on velocity
        float velo = Mathf.Abs(xMag) + Mathf.Abs(yMag);
        if(velo > 65)
        {
            speedlinesFast.Play();
        }
        else if (velo > 50)
        {
            if(speedlinesFast.isPlaying)
            {
                speedlinesFast.Stop();
            }
            speedlinesSlow.Play();
        }
        else
        {
            if(speedlinesSlow.isPlaying)
            {
                speedlinesSlow.Stop();
            }
            if(speedlinesFast.isPlaying)
            {
                speedlinesFast.Stop();
            }
        }

        // Some multipliers
        float multiplier = 1f, multiplierV = 1f;
        
        // Movement in air
        if (!grounded) {
            multiplier = airStrafeForward;
            multiplierV = airStrafeSideways;
        }
        
        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;

        // Movement while being on a Slide
        if (sliding) multiplierV = 0f;

        // Movement while hitting a Bouncy obstacle
        if (bouncing)
        {
            canDash = true;
        }

        // Movement while in wind area
        if (inWindArea)
        {
            rb.AddForce(windDirection * windMagnitude);
        }

        // Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
    }

    private void Jump() {
        if (grounded && readyToJump) {
            readyToJump = false;

            // Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
            
            // If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0) 
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    
    private void ResetJump() {
        readyToJump = true;
    }

    private void Dash()
    {
        if (!grounded && canDash)
        {
            rb.AddForce(orientation.transform.forward * dashForce, ForceMode.Impulse);
            canDash = false;
        }
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

    private void CounterMovement(float x, float y, Vector2 mag) {
        if (!grounded || jumping) return;

        // Slow down sliding
        if (crouching) {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        // Counter movement
        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }
        
        // Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook() {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);
        
        return new Vector2(xMag, yMag);
    }

    public bool isGrounded()
    {
        return grounded;
    }

    public bool isBouncing()
    {
        return bouncing;
    }

    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private void StepCheck()
    {
        RaycastHit lower;
        if(Physics.Raycast(stepRayLower.position, orientation.forward, out lower, 0.4f, whatIsGround))
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

    private void StopGrounded() {
        grounded = false;
    }
    
}
