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
    public GameObject _cameraFollowTarget;

    private PlayerControls _playerControls;
    private CharacterController _controller;
    private JUFootPlacement _footPlacementScript;
    private Animator _playerAnimator;
    private ShowInventory _showInventory;

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
        enabled = false;
        _camera = Camera.main;
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

        foreach(PlayerData player in GameObject.FindGameObjectWithTag("GameManager").GetComponent<ConnectedPlayers>()._playerList)
        {
            
        }

        

        enabled = IsClient;

        if (!IsOwner)
        { 
            enabled = false;
            GetComponent<CharacterController>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = true;
            return;
        }

        enabled = true;
        GetComponent<CharacterController>().enabled = true;

        var cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = _cameraFollowTarget.transform;


        GameObject.FindGameObjectWithTag("UI_Customize").GetComponent<CustomizeUI>()._player = gameObject;
        GameObject.FindGameObjectWithTag("UI_Customize").GetComponent<CustomizeUI>().Initialize();


        transform.position = new Vector3(23, .55f, 7);


        // GetComponent<CustomizeUI>().Initialize();
    }

    

    void Start()
    {
        
        GetComponent<PlayerAiming>().enabled = false;
        GetComponent<PlayerAiming>().enabled = true;

        // Build UI
        GameObject mainUI = GameObject.Find("HUD/UI");
        
        //mainUI.transform.Find("UI_Customize").GetComponent<CustomizeUI>().Initialize();
        mainUI.transform.Find("UI_Inventory").GetComponent<InventoryUI>().Initialize();
        mainUI.transform.Find("UI_PauseMenu").GetComponent<PauseMenuUI>().Initialize();

        pauseMenuUI = mainUI.transform.Find("UI_PauseMenu").GetComponent<PauseMenuUI>();


        // Head look at direction
        if (UseCameraDirection) DirectionReference = _camera.transform;

        // Check player interaction
        interactableObject = null;

        // Components
        _playerControls = new PlayerControls();
        _controller = GetComponent<CharacterController>();
        _playerAnimator = GetComponent<Animator>();
        _footPlacementScript = GetComponent<JUFootPlacement>();
        PlayerRunSpeed = PlayerWalkSpeed * 2f;
        _showInventory = GetComponent<ShowInventory>();

        inventoryItemList = Resources.Load("InventoryItemList") as InventoryItemList;
        itemPrefabList = Resources.Load("ItemPrefabList") as ItemPrefabList;


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

        _playerControls.Land.Inventory.started += Inventory;
        _playerControls.Land.Inventory.canceled += Inventory;

        _playerControls.Land.ReloadScene.started += ReloadScene;
        _playerControls.Land.ReloadScene.canceled += ReloadScene;

        //Cursor.lockState = CursorLockMode.Locked;

    }
    

    void Update()
    {
        if (!IsOwner) return;
        
        if (isCustomizing || isTakingSurvey) return;
        
        _playerAnimator.SetBool("isAction", isMainhanding || isOffhanding);

        _playerAnimator.SetBool("isRunning", !isMainhanding && isRunning);
        
        _playerAnimator.SetBool("isMoving", isMoving);

        Movement();




        // Check if player can attack
        if (isMainhanding && Time.time > nextAttackTime)
        {
            //TODO: Make attack dynamic based on weapon
            nextAttackTime = Time.time + 1f;

            Attack();
        }

        // Regen health over time
        //TODO: Add health regen

    }

    void Movement()
    {
        if (isPaused) return;

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
        
        Ray ray = new Ray(head.position, gameObject.GetComponent<PlayerController>()._camera.transform.forward);

        if (Physics.Raycast(ray, out hit, checkDistance, _interactableLayer))
        {
            interactableObject = hit.collider.gameObject;

            interactableObject.GetComponent<Interactable>()._interactE.enabled = true;

        }
        else if (interactableObject != null && !interactableObject.GetComponent<Interactable>().showInteractE)
        {
            interactableObject.GetComponent<Interactable>()._interactE.enabled = false;
            interactableObject = null;
        }
        else
        {
            interactableObject = null;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (isCustomizing) return;
        
        if (anim == null)
            anim = GetComponent<Animator>();

        anim.SetLookAtWeight(1, SpineHeight, HeadWeight);
        anim.SetLookAtPosition(DirectionReference.position + DirectionReference.forward * 50);
    }

    void CheckIfPlayFallAnimation()
    {
        _fallTimer += Time.deltaTime;
        _playerAnimator.SetBool("isFalling", _fallTimer > _fallThreshold && !isGrounded);
        _fallTimer = isGrounded ? 0f : _fallTimer;
    }
    
    public void Jump()
    {
        if (isGrounded && !(isCustomizing || isTakingSurvey))
        {            
            _footPlacementScript.EnableDynamicBodyPlacing = false;
            _footPlacementScript.SmoothIKTransition = false;
            
            _playerAnimator.SetAnimatorTrigger(AnimatorTrigger.JumpTrigger);

            _ySpeed = PlayerJumpForce;

            isGrounded = false;
        }
    }






    private void Attack()
    {        
        switch (playerWeaponString)
        {
            case "OneHand":
                break;
            case "Unarmed":
                if (isMoving)
                {
                    _playerAnimator.SetActionTrigger(AnimatorTrigger.AttackTrigger, _attackLeft ? 1 : 2);
                    _attackLeft = !_attackLeft;
                }
                else
                {
                    _playerAnimator.SetActionTrigger(AnimatorTrigger.AttackTrigger, (int)Mathf.Round(UnityEngine.Random.Range(1, 6)));
                }
                break;
            default:
                _playerAnimator.SetTrigger("Attack");
                break;
        }
    }

    public void LootItem(Item item)
    {
        GameObject.Find("QuestInventory").GetComponent<PrototypeQuest>().QuestObjectiveUpdate(1, 100); // QUEST: Test Inventory - Loot something

        // Check if item already in inventory
        for (int i = 0; i < PlayerInventorySpace; i++)
        {
            InventoryItem tempInv = inventoryItemList.items[i].GetComponent<InventoryItem>();

            if (tempInv.itemName == item.itemName && tempInv.stackSize < tempInv.maxStackSize)
            {
                tempInv.stackSize++;
                tempInv.UpdateItem();
                return;
            } 
        }

        // Add item to inventory
        for (int i = 0; i < PlayerInventorySpace; i++)
        {
            InventoryItem tempInv = inventoryItemList.items[i].GetComponent<InventoryItem>();

            if (tempInv.itemName == "Empty")
            {
                tempInv.itemName = item.itemName;
                tempInv.SetStackSize(item);
                tempInv.stackSize = 1;
                tempInv.UpdateItem();

                return;
            }
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
        pauseMenuUI.TogglePauseMenu();
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
                if (isInventoryOpen)
                {
                    _showInventory.ToggleInventory();
                    break;
                }

                //if (isCustomizing)
                //{
                //    GetComponent<PlayerCustomize>().Close(context);
                //    isCustomizing = false;
                //    break;
                //}

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

    public void Inventory(InputAction.CallbackContext context)
    {
        if (isCustomizing || isTakingSurvey || isPaused) return;

        switch (context.phase)
        {
            case InputActionPhase.Started:
                
                _showInventory.ToggleInventory();
                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                break;
        }
    }

    public void ReloadScene(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                SceneManager.LoadScene("Lobby");
                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                break;
        }
    }
}




