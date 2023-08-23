using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// My apologies for this script, physics is always a mess
// How this character controller works, is it uses a rigidbody to move the player around
// with instant acceleration because it feels more responsive that way, and non-instant
// deceleration because it feels smoother that way.
public class PlayerMovement : MonoBehaviour {

    [Header("Basic Movement")]
    // Speed limits in each direction, in m/s
    public float maxSpeed = 5f;
    public float forwardSpeed = 5f;
    public float sidewaysSpeed = 5f;
    public float backwardsSpeed = 5f;
    
    // How much of the player's velocity is retained each frame, 0 = no retention, 1 = full retention
    // Don't worry, it's timestep independent
    public float floatiness = 0.99f; 

    [Header("Sprinting")]
    // Toggle to enable/disable sprinting
    public bool sprintAllowed = true;
    // Speed limit while sprinting
    public float sprintVelocity;

    [Header("Jumping")]
    // Toggle to enable/disable jumping
    public bool jumpAllowed = true;
    // Time in seconds after touching the ground until the player can jump again
    public float jumpDelay; 
    // Player's jump height in metres
    public float jumpHeight;
    // The length of the raycast that tests whether or not the player is grounded
    public float groundedRaycastDistance = 1.1f;

    private Rigidbody _rigidbody;
    private float _originalFOV; // The FOV of the camera when not sprinting
    
    private bool _isSprinting;
    
    private bool _canJump = true;
    private bool _jumpTimerTicking = false;
    private float _jumpForce;
    
    private float _forwardInput;
    private float _sideInput;
    private bool _jumpInput;
    
    // Whether or not the player is moving
    public bool IsMoving { get; private set; }
    
	void Start ()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _originalFOV = Camera.main.fieldOfView;

        // v_f^2 - v_i^2 = 2ah
        // -v_i^2 = 2ah - v_f^2
        // v_i^2 = v_f^2 - 2ah
        // v_i^2 = 0 - 2ah
        // v_i^2 = -2ah
        // v_i = sqrt(-2ah)
        _jumpForce = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);

	}

    // Note that Update is called once per frame while FixedUpdate is called once per physics step (which is 
    // typically below 60FPS), so polling input and non-physics related calculations should be done in Update
    // so as to be more responsive
    void Update()
    {
        _forwardInput = Input.GetAxisRaw("Vertical");
        _sideInput = Input.GetAxisRaw("Horizontal");
        _jumpInput = Input.GetKey(KeyCode.Space);
        
        if (!sprintAllowed) return;

        var sprintKeyHeld = Input.GetKey(KeyCode.LeftShift);
        _isSprinting = sprintKeyHeld && IsGrounded() && (Input.GetAxisRaw("Vertical") > 0);
        
        // Make the camera zoom out while sprinting to give a sense of speed
        if (sprintKeyHeld)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, _originalFOV * 1.1f, 0.2f);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, _originalFOV, 0.1f);
        }
    }

	// Physics-related calculations should be done in FixedUpdate to ensure consistency and match the framerate
    // of the physics engine which controls things like rigidbodies and collisions
	void FixedUpdate ()
    {
        if (IsGrounded())
        {
            Jumping();
        }
        
        Movement();
        LimitSpeed();
	}

    // Checks if the player is grounded by casting a ray downwards from the player's position
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -transform.up, groundedRaycastDistance);
    }

    // Globally limits the player's speed
    void LimitSpeed()
    {
        var vel = _rigidbody.velocity;
        vel.y = 0;
        if (vel.magnitude > maxSpeed)
        {
            vel = vel.normalized * maxSpeed;
        }
        vel.y = _rigidbody.velocity.y;
        _rigidbody.velocity = vel;
    }

    // Handles jumping
    void Jumping()
    {
        if (jumpAllowed && IsGrounded())
        {
            // If jumping is allowed, the player is grounded, the player presses the jump key, and is not on cooldown,
            // let them jump!
            if (_jumpInput && _canJump)
            {
                var vel = _rigidbody.velocity;
                vel.y = _jumpForce;
                _rigidbody.velocity = vel;

                _canJump = false;
            }
            // If the player is not allowed to jump yet, start the timer to let them jump again!
            else if(!_canJump && !_jumpTimerTicking)
            {
                _jumpTimerTicking = true;
                StartCoroutine(JumpTimer());
            }
        }
    }

    // Waits jumpDelay seconds then lets the player jump again
    IEnumerator JumpTimer()
    {
        yield return new WaitForSeconds(jumpDelay);
        _canJump = true;
        _jumpTimerTicking = false;
    }
    
    void Movement()
    {
        var fwd = _forwardInput;
        
        // Select between backwards speed, forwards speed, or sprint speed for the Z axis
        fwd = fwd < 0 ? fwd * backwardsSpeed : (_isSprinting ? fwd * sprintVelocity : fwd * forwardSpeed);

        var side = _sideInput * sidewaysSpeed;

        var vel = _rigidbody.velocity;

        // If the player is moving forwards or sideways, reset their speed in that direction to zero
        // because later on we will be adding the new speed to the current speed
        
        var localVel_0 = transform.InverseTransformDirection(vel);

        if(fwd != 0)
        {
            localVel_0.z = 0;
        }

        if(side != 0)
        {
            localVel_0.x = 0;
        }

        IsMoving = fwd != 0 || side != 0;

        vel = transform.TransformDirection(localVel_0);
        
        // Add the new speed to the current speed (after resetting to zero, it ends up
        // being the same as just setting the speed to the new value)

        var camFwd = transform.forward;
        camFwd.y = 0;

        vel += camFwd * fwd;
        vel += Camera.main.transform.right * side;

        // If the player is not moving forwards or sideways, damp their speed in that direction
        // to simulate friction
        var localVel = transform.InverseTransformDirection(vel);

        if(fwd == 0)
        {
            localVel.z *= Mathf.Pow(floatiness, Time.deltaTime * 50f);
        }

        if(side == 0)
        {
            localVel.x *= Mathf.Pow(floatiness, Time.deltaTime * 50f);
        }

        vel = transform.TransformDirection(localVel);
        
        _rigidbody.velocity = vel;
    }

}
