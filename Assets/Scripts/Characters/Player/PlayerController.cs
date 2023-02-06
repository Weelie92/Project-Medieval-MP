using RPGCharacterAnims.Extensions;
using RPGCharacterAnims.Lookups;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Cinemachine;
using System;
using UnityEngine.Networking.Types;

public class PlayerController : NetworkBehaviour
{
    // Components
    [Header("Components")]
    private Camera _camera;
    private GameObject _cameraFollowTarget;
    private PlayerControls _playerControls;
    private CharacterController _controller;
    private JUFootPlacement _footPlacementScript;
    private Animator _playerAnimator;

    // UI
    [Header("UI")]

    // Scriptable Objects
    [Header("Scriptable Objects")]
    private InventoryItemList inventoryItemList;
    private ItemPrefabList itemPrefabList;

    // Variables
    [Header("Variables")]
    [HideInInspector] public float nextAttackTime;
    [HideInInspector] public float playerStaminaRegen;
    public bool isMoving = false;
    public bool isRunning = false;
    public bool isJumping = false;
    [HideInInspector] public bool isGrounded = true;
    public bool isOffhanding = false;
    public bool isMainhanding = false;
    public bool isCustomizing = false;
    public bool isTakingSurvey = false;
    public bool isPaused = false;
    public bool isInventoryOpen = false;
    [HideInInspector] public string playerWeaponString = "Unarmed";

    [Header("UI/HUD")]
    [HideInInspector] public PauseMenuUI pauseMenuUI;

    public Vector3 moveDirection;
    public float _ySpeed;
    private readonly float _gravityValue = -9.81f;
    private readonly float _fallThreshold = 0.5f;
    private float _fallTimer = 0f;
    private bool _attackLeft = true;

    // Stats
    [Header("Stats")]
    public float PlayerHealth = 10f;
    public float PlayerMaxHealth = 10f;
    public float PlayerStamina = 10f;
    public float PlayerMaxStamina = 10f;
    public float PlayerWalkSpeed = 2f;
    public float PlayerJumpForce = 5f;
    public float PlayerRunSpeed = 0f;
    public int PlayerInventorySpace = 30;

    // Head look at direction
    public bool UseCameraDirection = true;
    public Transform DirectionReference;
    [Range(0, 1)]
    public float HeadWeight = 1;
    [Range(0, 1)]
    public float SpineHeight = 0.05f;
    Animator anim;

    // Check player interaction
    [SerializeField] private List<GameObject> interactables = new List<GameObject>();
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private float checkDistance = 2f;
    [SerializeField] private GameObject interactableObject;
    [SerializeField] private Transform head;
    [SerializeField] private RaycastHit hit;

    private void Awake()
    {
        _camera = Camera.main;
        _cameraFollowTarget = transform.Find("Camera Follow Target").gameObject;
        anim = GetComponent<Animator>();
        GetComponent<CharacterController>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
    }

    public Camera GetCamera()
    {
        return _camera;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient && IsOwner)
        {
            GetComponent<CharacterController>().enabled = true;

            var cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = _cameraFollowTarget.transform;

            GameObject.FindGameObjectWithTag("UI_Customize").GetComponent<CustomizeUI>()._player = gameObject;
            GameObject.FindGameObjectWithTag("UI_Customize").GetComponent<CustomizeUI>().Initialize();

            transform.position = new Vector3(23, .55f, 7);
        }
        else
        {
            GetComponent<CharacterController>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = true;
        }
    }


    void Start()
    {
        GetComponent<PlayerAiming>().enabled = false;
        GetComponent<PlayerAiming>().enabled = true;

        BuildUI();
        InitializePlayerControls();
        InitializeComponents();
        InitializePlayerEvents();
    }

    private void BuildUI()
    {
        GameObject mainUI = GameObject.Find("HUD/UI");
        pauseMenuUI = mainUI.transform.Find("UI_PauseMenu").GetComponent<PauseMenuUI>();
        pauseMenuUI.Initialize();
    }

    private void InitializePlayerControls()
    {
        if (UseCameraDirection) DirectionReference = _camera.transform;
        _playerControls = new PlayerControls();
        _playerControls.Enable();
    }

    private void InitializeComponents()
    {
        interactableObject = null;
        _controller = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<Animator>();
        _footPlacementScript = GetComponent<JUFootPlacement>();
        PlayerRunSpeed = PlayerWalkSpeed * 2f;

        inventoryItemList = Resources.Load("InventoryItemList") as InventoryItemList;
        itemPrefabList = Resources.Load("ItemPrefabList") as ItemPrefabList;
    }

    private void InitializePlayerEvents()
    {
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

    void Update()
    {
        if (!IsOwner) return;

        if (isPaused || isCustomizing || isTakingSurvey) return;
        
        HandleAnimations();
        HandleMovement();
        HandleAttack();
    }

    private void HandleAnimations()
    {
        _playerAnimator.SetBool("isAction", isMainhanding || isOffhanding);
        _playerAnimator.SetBool("isRunning", !isMainhanding && isRunning);
        _playerAnimator.SetBool("isMoving", isMoving);
    }

    private void HandleAttack()
    {
        if (isMainhanding && Time.time > nextAttackTime)
        {
            nextAttackTime = Time.time + 1f;
            Attack();
        }
    }

    private void HandleMovement()
    {
        Vector2 movement = _playerControls.Land.Move.ReadValue<Vector2>();
        Vector3 moveNew = _camera.transform.forward * movement.y + _camera.transform.right * movement.x;
        moveNew.y = 0f;
        moveNew = moveNew.normalized;

        moveDirection = (isRunning && !isMainhanding ? PlayerRunSpeed : PlayerWalkSpeed) * moveNew;

        if (isGrounded)
        {
            _footPlacementScript.EnableDynamicBodyPlacing = true;
            _footPlacementScript.SmoothIKTransition = true;
            _ySpeed = -0.5f;
        }
        else
        {
            _ySpeed += _gravityValue * Time.deltaTime;
        }

        moveDirection.y = _ySpeed;
        _controller.Move(moveDirection * Time.deltaTime);

        if (isOffhanding || isMainhanding)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0), 10f * Time.deltaTime);
            _playerAnimator.SetFloat("VelocityX", movement.x);
            _playerAnimator.SetFloat("VelocityZ", movement.y);
        }
        else if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveNew);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        CheckIfPlayFallAnimation();
        CheckPlayerInteraction();

        isGrounded = _controller.isGrounded;
    }



    private void CheckPlayerInteraction()
    {
        if (isCustomizing) return;

        RaycastHit hit;
        
        Ray ray = new Ray(head.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out hit, checkDistance, _interactableLayer))
        {
            Interactable interactable = hit.collider.gameObject.GetComponent<Interactable>();
            interactable._interactE.enabled = true;
            interactableObject = interactable.gameObject;
        }
        else
        {
            interactableObject = null;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!IsLocalPlayer || isCustomizing) return;

        anim.SetLookAtWeight(HeadWeight, SpineHeight, 1);
        anim.SetLookAtPosition(DirectionReference.position + DirectionReference.forward * 50);
    }

    private void CheckIfPlayFallAnimation()
    {
        _fallTimer += Time.deltaTime;
        _playerAnimator.SetBool("isFalling", _fallTimer > _fallThreshold && !isGrounded);
        _fallTimer = isGrounded ? 0f : _fallTimer;
    }

    public void Jump()
    {
        if (!isGrounded || isCustomizing || isTakingSurvey) return;

        _footPlacementScript.EnableDynamicBodyPlacing = false;
        _footPlacementScript.SmoothIKTransition = false;
        
        _playerAnimator.SetAnimatorTrigger(AnimatorTrigger.JumpTrigger);

        _ySpeed = PlayerJumpForce;

        isGrounded = false;
    }

    private void Attack()
    {
        switch (playerWeaponString)
        {
            case "OneHand":
                break;
            case "Unarmed":
                int attackIndex = isMoving ? (_attackLeft ? 1 : 2) : Mathf.RoundToInt(UnityEngine.Random.Range(1, 6));
                _playerAnimator.SetActionTrigger(AnimatorTrigger.AttackTrigger, attackIndex);
                _attackLeft = !_attackLeft;
                break;
            default:
                _playerAnimator.SetTrigger("Attack");
                break;
        }
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
        if (isCustomizing || isTakingSurvey) return;

        //TODO: Add pause menu
        pauseMenuUI.TogglePauseMenu(!isPaused);
    }

    public void Move(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:

                isMoving = true;
                

                //TODO: SET PLAYER MOVING 
                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                isMoving = false;

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
                if (isCustomizing || isTakingSurvey || isPaused) break;

                if (interactableObject != null)
                {
                    interactableObject.GetComponent<Interactable>().Interact(gameObject);
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
                isMainhanding = true;

                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                //TODO: CHANGE ATTACK ANIMaTION
                isMainhanding = false;
                break;
        }
    }

    public void OffHand(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:

                //TODO: Change offhanding animation
                isOffhanding = true;
                break;

            case InputActionPhase.Performed:

                break;

            case InputActionPhase.Canceled:
                isOffhanding = false;
                                
                //TODO: Change offhanding animation
                break;
        }

    }
    public void Run(InputAction.CallbackContext context)
    {
        if (isMainhanding) return;

        switch (context.phase)
        {
            case InputActionPhase.Started:
                
                isRunning = true;
                
                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                isRunning = false;
                break;
        }

    }

    public void Pause(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                if (isTakingSurvey)
                {
                    GameObject.FindGameObjectWithTag("UI_Survey").GetComponent<SurveyUI>().CloseSurvey();
                    break;
                }
        
        
                Pause();

                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                break;
            }
    }
}




