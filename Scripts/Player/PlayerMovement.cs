using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour {
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 10.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 15f;

    //New Position tracking - Faris
    public Vector3 _previousPosition;
    public float _movementThreshold = 0.01f; // Adjust as needed to detect movement
    private Vector3 _lastMoveDirection = Vector3.zero;

    //New - Faris 
    [Tooltip("Acceleration and deceleration")]
    public float AccelerationRate = 1.0f;
    [Tooltip("Acceleration and deceleration")]
    public float DecelerationRate = 2.0f;

    //Might not be needed - Faris
    //[Tooltip("Acceleration and deceleration")]
    //public float SpeedChangeRate = 10.0f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.6f;
    //Removed No need for this - Faris
    //public bool useJumpEquation = true;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -25.0f;
    //New - Faris
    [Tooltip("Maximum downwards velocity")]
    public float TerminalVelocity = 80.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.00f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = 0.745f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.84f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;


    //New needed for SFX- Faris
    [Header("Audio/SFX")]
    public AudioClip[] walkingAudioClips;
    public AudioClip[] runningAudioClips;

    public AudioClip[] JumpingAudioClips;
    public AudioClip[] LandingAudioClips;

    //New needed for checking audio sounds
    //Jumping
    public bool LandedPlayed = false;
    //Walking
    public bool WalkingPlayed = false;
    public float delayBetweenWalks = 0.5f;
    //Running
    public bool RunningPlayed = false;
    public float delayBetweenRuns = 0.2f;

    // player
    private float _speed;
    private float _rotationVelocity;
    private float _verticalVelocity;

    //Removed no need for this - Faris
    //public float _terminalVelocity = 80.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    //Other
    private CharacterController _controller;
    private PlayerInput _input;


    private void Start() {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;

        //TEMPORARY - Faris
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        GroundedCheck();
        JumpAndGravity();
        Move();
    }

    //New - Faris
    private void GroundedCheck() {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + GroundedOffset,
                transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        if (LandedPlayed == false && Grounded == true) {
            LandedPlayed = true;

            //New, will be needed for sound effect - Faris
            //SFXManager.instance.PlayRandomSoundFXClip(LandingAudioClips, transform, 0.7f);
        }
    }

    //Old - Faris
    //private void GroundedCheck() {
    //    Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y+ GroundedOffset, transform.position.z);
    //    Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    //}

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + GroundedOffset,
                transform.position.z), GroundedRadius);
    }


    private void JumpAndGravity() {
        if (Grounded) {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f) {
                _verticalVelocity = -10f;
            }

            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f) {

                // Will be added later for SFX
                //SFXManager.instance.PlayRandomSoundFXClip(JumpingAudioClips, transform, 1f);

                LandedPlayed = false;
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                GetComponent<CameraControl>().CameraWobble(true, 10f);
                print("jumped");
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f) {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        } else {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f) {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity > -TerminalVelocity) {
            _verticalVelocity += Gravity * Time.deltaTime;
        } else {
        }
    }

    //Old - Faris
    //private void JumpAndGravity() {
    //    if (Grounded) {
    //        // reset the fall timeout timer
    //        _fallTimeoutDelta = FallTimeout;

    //        // stop our velocity dropping infinitely when grounded
    //        if (_verticalVelocity < 0.0f) 
    //        {
    //            _verticalVelocity = -10f;
    //        }

    //        // Jump
    //        if (_input.jump && _jumpTimeoutDelta <= 0.0f) {
    //            // the square root of H * -2 * G = how much velocity needed to reach desired height
    //            if(useJumpEquation) 
    //            {
    //                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
    //            } 
    //            else _verticalVelocity = JumpHeight;
    //        }

    //        // jump timeout
    //        if (_jumpTimeoutDelta >= 0.0f) {
    //            _jumpTimeoutDelta -= Time.deltaTime;
    //        }
    //    }
    //    else 
    //    {
    //        // reset the jump timeout timer
    //        _jumpTimeoutDelta = JumpTimeout;

    //        // fall timeout
    //        if (_fallTimeoutDelta >= 0.0f) {
    //            _fallTimeoutDelta -= Time.deltaTime;
    //        } 

    //        // if we are not grounded, do not jump
    //        _input.jump = false;
    //    }

    //    // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
    //    if (_verticalVelocity > -_terminalVelocity) 
    //    {
    //        _verticalVelocity += Gravity * Time.deltaTime;
    //    } 
    //}


    //TESTING
    public AudioClip[] grassFootstepSounds;
    public AudioClip[] dirtFootstepSounds;
    public AudioClip[] carpetFootstepSounds;
    public AudioClip[] tileFootstepSounds;
    public AudioClip[] woodFootstepSounds;

    private void Move() {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

        // a reference to the player's current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.move.magnitude; // Update input magnitude to reflect actual input

        if (_input.move == Vector2.zero) {
            targetSpeed = 0.0f;
        }

        // determine the rate of speed change
        float speedChangeRate = _input.move == Vector2.zero ? DecelerationRate : AccelerationRate;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset) {
            // creates curved result rather than a linear one giving a more organic speed change
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        } else {
            _speed = targetSpeed;
        }

        // normalize input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // if there is a move input, rotate player when the player is moving
        if (_input.move != Vector2.zero) {
            // move in the input direction
            inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            // store the last direction of movement
            _lastMoveDirection = inputDirection;

        } else if (_speed > 0) {
            // If there is no input, maintain the last known direction while decelerating
            inputDirection = _lastMoveDirection;
        }

        // move the player
        _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        if (inputDirection.magnitude > 0.0f) {
            GetComponent<CameraControl>().CameraWobble(true, 3f);
        }


        //TESTING

        // Check if the player has moved significantly since the last frame  FOR AUDIO
        if ((Vector3.Distance(transform.position, _previousPosition) > _movementThreshold) && Grounded) {
            //NEW Dynamic footsteps system
            Vector3 TestPosition = new Vector3(transform.position.x, transform.position.y + GroundedOffset, transform.position.z);
            Collider[] colliders = Physics.OverlapSphere(TestPosition, GroundedRadius);

            if (_input.sprint) {
                if (RunningPlayed == false) {

                    //Will be needed for Audio
                    //SFXManager.instance.PlayRandomSoundFXClip(runningAudioClips, transform, 1f);

                    //New 
                    foreach (var collider in colliders) {

                        if (collider.CompareTag("Carpet")) {
                            //SFXManager.instance.PlayRandomSoundFXClip(carpetFootstepSounds, transform, 1f);
                        } else if (collider.CompareTag("Tile")) {
                            //SFXManager.instance.PlayRandomSoundFXClip(tileFootstepSounds, transform, 1f);
                        } else if (collider.CompareTag("Wood")) {
                            //SFXManager.instance.PlayRandomSoundFXClip(woodFootstepSounds, transform, 1f);
                        }
                    }
                    RunningPlayed = true;
                    StartCoroutine(ResetRunningValue());
                }
            } else {
                if (WalkingPlayed == false) {
                    //Will be needed for Audio
                    //SFXManager.instance.PlayRandomSoundFXClip(walkingAudioClips, transform, 1f);

                    //New
                    foreach (var collider in colliders) {

                        if (collider.CompareTag("Carpet")) {
                           // SFXManager.instance.PlayRandomSoundFXClip(carpetFootstepSounds, transform, 1f);
                        } else if (collider.CompareTag("Tile")) {
                           // SFXManager.instance.PlayRandomSoundFXClip(tileFootstepSounds, transform, 1f);
                        } else if (collider.CompareTag("Wood")) {
                           // SFXManager.instance.PlayRandomSoundFXClip(woodFootstepSounds, transform, 1f);
                        }
                    }
                    WalkingPlayed = true;
                    StartCoroutine(ResetWalkingValue());
                }
            }

        }
        //         //-----------------------------------------------------------------steku code----------------------------------------------------------------------

        //  // adding audio to player movement test
        //     // Check if the player has moved significantly since the last frame FOR AUDIO
        //     if (Vector3.Distance(transform.position, _previousPosition) > _movementThreshold)
        //     {
        //         if (_input.sprint)
        //         {
        //             audioManager.instance.PlayRunningSound(); // Call running sound
        //         }
        //         else
        //         {
        //             audioManager.instance.PlayWalkingSound(); // Call walking sound
        //         }
        //     }

        //         //---------------------------------------------------------------------------end steku code----------------------------------------------------------------------



        // Update the previous position
        _previousPosition = transform.position;
    }

    //Old - Faris
    //private void Move() {
    //    // set target speed based on move speed, sprint speed and if sprint is pressed
    //    float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

    //    // if there is no input, set the target speed to 0
    //    if (_input.move == Vector2.zero) targetSpeed = 0.0f;

    //    // a reference to the players current horizontal velocity
    //    float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

    //    float speedOffset = 0.1f;
    //    float inputMagnitude = 1;

    //    // accelerate or decelerate to target speed
    //    if (currentHorizontalSpeed < targetSpeed - speedOffset ||
    //             currentHorizontalSpeed > targetSpeed + speedOffset) {
    //        // creates curved result rather than a linear one giving a more organic speed change
    //        // note T in Lerp is clamped, so we don't need to clamp our speed
    //        _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
    //            Time.deltaTime * SpeedChangeRate);

    //        // round speed to 3 decimal places
    //        _speed = Mathf.Round(_speed * 1000f) / 1000f;
    //    } else {
    //        _speed = targetSpeed;
    //    }

    //    // normalise input direction
    //    Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

    //    if (_input.move != Vector2.zero) 
    //    {
    //        inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
    //    }

    //    _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

    //}


    //New Reseting Walking - Faris
    IEnumerator ResetRunningValue() {
        yield return new WaitForSeconds(delayBetweenRuns);
        ResetRunningPlayer();
    }
    public void ResetWalkingPlayed() {
        WalkingPlayed = false;
    }


    //New Reseting Running - Faris
    IEnumerator ResetWalkingValue() {
        yield return new WaitForSeconds(delayBetweenWalks);
        ResetWalkingPlayed();
    }
    public void ResetRunningPlayer() {
        RunningPlayed = false;
    }






}
