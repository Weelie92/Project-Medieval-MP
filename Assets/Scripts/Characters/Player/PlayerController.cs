using System.Collections;
using System.Collections.Generic;
using RPGCharacterAnims;
using RPGCharacterAnims.Extensions;
using RPGCharacterAnims.Lookups;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : PlayerStats
{



    // Components
    private Transform _mainCamera;
    private Rigidbody _playerRb;
    private PlayerInput _playerInput;
    private PlayerControls _playerControls;
    private CharacterController _controller;
    private JUFootPlacement _footPlacementScript;
    private Animator _playerAnimator;
    private CheckInteraction _checkInteraction;



    // Variables
    private Vector3 _gravityVector;
    private float _gravityValue = -9.81f;
    private float _fallThreshold = 0.5f;
    private float _fallTimer = 0f;
    public bool _isCustomizing = false;





    


    void Awake()
    {
        // Components
        _playerRb = GetComponent<Rigidbody>();
        _playerControls = new PlayerControls();
        _mainCamera = Camera.main.transform;
        _controller = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<Animator>();
        _footPlacementScript = GetComponent<JUFootPlacement>();
        PlayerRunSpeed = PlayerWalkSpeed * 2f;
        _checkInteraction = GetComponent<CheckInteraction>();
        


        // Other

        _playerControls.Enable();

        _playerControls.Land.Move.started += Move;
        _playerControls.Land.Move.canceled += Move;

        _playerControls.Land.Jump.started += Jump;
        _playerControls.Land.Jump.canceled += Jump;

        _playerControls.Land.Use.started += Use;
        _playerControls.Land.Use.canceled += Use;

        _playerControls.Land.Crouch.started += Crouch;
        _playerControls.Land.Crouch.canceled += Crouch;

        _playerControls.Land.Prone.started += Prone;
        _playerControls.Land.Prone.canceled += Prone;

        _playerControls.Land.MainHand.started += MainHand;
        _playerControls.Land.MainHand.canceled += MainHand;

        _playerControls.Land.OffHand.started += OffHand;
        _playerControls.Land.OffHand.canceled += OffHand;

        _playerControls.Land.Run.started += Run;
        _playerControls.Land.Run.canceled += Run;

        _playerControls.Land.Pause.started += Pause;
        _playerControls.Land.Pause.canceled += Pause;

    }

    void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;

       

    }

    void Update()
    {
        
        PlayerGrounded = _controller.isGrounded;
        _playerAnimator.SetBool("isRunning", PlayerRunning);
        _playerAnimator.SetBool("isMoving", PlayerMoving);
        //_playerAnimator.SetBool("isFalling", !_controller.isGrounded);


        Movement();
        _checkInteraction.CheckInteractable();




        // Check if player can attack
        if (PlayerAttacking && Time.time > NextAttackTime)
        {
            //TODO: Make attack dynamic based on weapon
            NextAttackTime = Time.time + 2f;

            Attack();
        }

        // Regen health over time
        //TODO: Add health regen

    }

    void Movement()
    {
        // Movement
        Vector2 movement = _playerControls.Land.Move.ReadValue<Vector2>();
        Vector3 moveNew = _mainCamera.forward * movement.y + _mainCamera.right * movement.x;
        moveNew.y = 0f;
        moveNew = moveNew.normalized;

        

        _controller.Move((PlayerRunning ? PlayerRunSpeed : PlayerWalkSpeed) * Time.deltaTime * moveNew);

        if (PlayerGrounded && _gravityVector.y < 0)
        {
            _footPlacementScript.EnableDynamicBodyPlacing = true;
            _footPlacementScript.SmoothIKTransition = true;
            _gravityVector.y = 0f;
        }

        // Gravity 
        _fallTimer += Time.deltaTime;
        _playerAnimator.SetBool("isFalling", _fallTimer > _fallThreshold && !PlayerGrounded);
        _fallTimer = PlayerGrounded ? 0f : _fallTimer;

        _gravityVector.y += _gravityValue * Time.deltaTime;
        _controller.Move(_gravityVector * Time.deltaTime);

        

        // Rotate player
        if (PlayerOffhanding || PlayerAttacking)
        {
            // Rotate player towards offhanding
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, _mainCamera.eulerAngles.y, 0), 10f * Time.deltaTime);
            _playerAnimator.SetFloat("VelocityX", movement.x);
            _playerAnimator.SetFloat("VelocityZ", movement.y);
        }
        else if (PlayerMoving)
        {

            // Make character rotate direction going
            Quaternion targetRotation = Quaternion.LookRotation(moveNew);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

       
        
    }

    void Jump()
    {
        if (PlayerGrounded && !_isCustomizing)
        {
            // Jump
            _footPlacementScript.EnableDynamicBodyPlacing = false;
            _footPlacementScript.SmoothIKTransition = false;
            _playerAnimator.SetTrigger("Jump");
            _gravityVector.y = PlayerJumpForce;
            

        }
    }

    




    private void Attack()
    {
        //TODO: PLAY ATTACK ANIMATION


    }

    

    public void TakeDamage(float damageTaken)
    {
        PlayerHealth -= damageTaken;

        if (PlayerHealth > 0f)
        {
            // Update health bar
            // TODO: Update health bar
        }
        else
        {
            // Player dies
            // TODO: Player dies

        }
    }

    public void Pause()
    {
        if (_isCustomizing)
        {
            _isCustomizing = true;
            enabled = false;
        }
    }

    

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {

            case "Walkable":

                break;

            case "Rock":

                break;

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Walkable":


                break;

            case "Rock":

                break;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                PlayerMoving = true;
                

                //TODO: SET PLAYER MOVING 
                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                PlayerMoving = false;

                //TODO: SET PLAYER MOVING

                break;
        }
    }


    public void Jump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Jump();
                

                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                break;
        }

    }

    public void Use(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                if (_checkInteraction.interactableObject != null)
                {
                    _checkInteraction.interactableObject.GetComponent<Interactable>().Interact();
                }
                
                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                break;
        }
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                break;
        }
    }

    public void Prone(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                break;
        }
    }

    public void MainHand(InputAction.CallbackContext context)
    {


        switch (context.phase)
        {
            case InputActionPhase.Started:
                //TODO: CHANGE ATTACKING ANIMATION
                
                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                //TODO: CHANGE ATTACK ANIMaTION
                break;
        }
    }

    public void OffHand(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:

                //TODO: Change offhanding animation
                PlayerOffhanding = true;
                _playerAnimator.SetBool("isOffhanding", true);
                break;

            case InputActionPhase.Performed:

                break;

            case InputActionPhase.Canceled:
                PlayerOffhanding = false;
                _playerAnimator.SetBool("isOffhanding", false);
                //TODO: Change offhanding animation
                break;
        }

    }
    public void Run(InputAction.CallbackContext context)
    {

        switch (context.phase)
        {
            case InputActionPhase.Started:
                PlayerRunning = true;
                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                PlayerRunning = false;
                break;
        }

    }

    public void Pause(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Pause();

                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                break;
            }
    }

}




