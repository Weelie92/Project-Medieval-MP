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
using RPGCharacterAnims.Actions;
using System.Collections;

public class PlayerController : NetworkBehaviour, IKillable, IKnockbackable
{
    // Components
    [Header("Components")]
    private Camera _camera;
    private GameObject _cameraFollowTarget;
    private PlayerControls _playerControls;
    //private CharacterController _playerController;
    [SerializeField] private Rigidbody _playerRigidbody;
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
    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public bool isRunning = false;
    public bool isJumping = false;
    public bool isGrounded = true;
    [HideInInspector] public bool isOffhanding = false;
    [HideInInspector] public bool isMainhanding = false;
    [HideInInspector] public bool isCustomizing = false;
    [HideInInspector] public bool isTakingSurvey = false;
    [HideInInspector] public bool isPaused = false;
    [HideInInspector] public bool isInventoryOpen = false;
    [HideInInspector] public bool isChangingScene = false;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public string playerWeaponString = "Unarmed";

    [Header("UI/HUD")]
    [HideInInspector] public PauseMenuUI pauseMenuUI;
    [SerializeField] private GameObject _scoreBoardUI;

    [HideInInspector] public Vector3 moveDirection;
    [HideInInspector] public float _ySpeed;
    private readonly float _gravityValue = -9.81f;
    private readonly float _fallThreshold = 0.5f;
    private float _fallTimer = 0f;
    private bool _attackLeft = true;

    // Stats
    [Header("Stats")]
    [HideInInspector] public float PlayerHealth = 10f;
    [HideInInspector] public float PlayerMaxHealth = 10f;
    [HideInInspector] public float PlayerStamina = 10f;
    [HideInInspector] public float PlayerMaxStamina = 10f;
    public float PlayerWalkSpeed = 7000f;
    public float PlayerJumpForce = 100f;
    [HideInInspector] public float PlayerRunSpeed = 0f;
    [HideInInspector] public int PlayerInventorySpace = 30;

    // Head look at direction
    [HideInInspector] public bool UseCameraDirection = true;
    [HideInInspector] public Transform DirectionReference;
    [Range(0, 1)]
    [HideInInspector] public float HeadWeight = 1;
    [Range(0, 1)]
    [HideInInspector] public float SpineHeight = 0.05f;
    Animator anim;

    // Check player interaction
    [SerializeField] private List<GameObject> interactables = new List<GameObject>();
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private float checkDistance = 2f;
    [SerializeField] private GameObject interactableObject;
    [SerializeField] private Transform head;
    [SerializeField] private RaycastHit hit;

    // Ragdoll
    [SerializeField] private Rigidbody[] _ragdollRigidbodies;
    [SerializeField] private Collider[] _ragdollBoxColliders;
    [SerializeField] private CapsuleCollider[] _ragdollCapsuleColliders;
    [SerializeField] private SphereCollider[] _ragdollSphereColliders;

    public bool isKnockbackable;

    public bool CanBeKnockedBack
    {
        get => isKnockbackable;
        set => isKnockbackable = value;
    }


    private void Awake()
    {
        _camera = Camera.main;

        _scoreBoardUI = GameObject.FindGameObjectWithTag("UI_ScoreBoard");

        _playerRigidbody = GetComponent<Rigidbody>();

        _ragdollRigidbodies = transform.Find("Root").GetComponentsInChildren<Rigidbody>();
        _ragdollBoxColliders = transform.Find("Root").GetComponentsInChildren<BoxCollider>();
        _ragdollCapsuleColliders = transform.Find("Root").GetComponentsInChildren<CapsuleCollider>();
        _ragdollSphereColliders = transform.Find("Root").GetComponentsInChildren<SphereCollider>();
        // FIX ME
        // DisableRagdoll();

        _cameraFollowTarget = transform.Find("Camera Follow Target").gameObject;
        anim = GetComponent<Animator>();

        isKnockbackable = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        GetComponent<PlayerAiming>().enabled = false;
        GetComponent<PlayerAiming>().enabled = true;

        MinigameScoreTracker.Instance.AddPlayerServerRpc(NetworkObject.OwnerClientId);


        BuildUI();
        InitializePlayerControls();
        InitializeComponents();
        InitializePlayerEvents();
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        if (isCustomizing || isTakingSurvey || isChangingScene) return;

        HandleMovement();

        HandleAnimations();
        HandleAttack();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (isCustomizing || isTakingSurvey || isChangingScene) return;

        

        CheckIfPlayFallAnimation();

        CheckPlayerInteraction();

        if (Input.GetKeyDown(KeyCode.I)) Revive();
        if (Input.GetKeyDown(KeyCode.K)) Kill();
    }

    private void DisableRagdoll()
    {
        _playerRigidbody.isKinematic = false;

        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }



        GetComponent<CapsuleCollider>().enabled = true;

        foreach (var col in _ragdollBoxColliders)
        {
            col.enabled = false;
        }

        foreach (var col in _ragdollCapsuleColliders)
        {
            col.enabled = false;
        }

        foreach (var col in _ragdollSphereColliders)
        {
            col.enabled = false;
        }
    }

    private void EnableRagdoll()
    {

        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }

        _playerRigidbody.isKinematic = true;

        GetComponent<CapsuleCollider>().enabled = false;

        foreach (var col in _ragdollBoxColliders)
        {
            col.enabled = true;
        }

        foreach (var col in _ragdollCapsuleColliders)
        {
            col.enabled = true;
        }

        foreach (var col in _ragdollSphereColliders)
        {
            col.enabled = true;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby") transform.position = new Vector3(23, .55f, 7);


        InitializePlayerNewScene();
        Revive();
    }

    private void InitializePlayerNewScene()
    {
        isPaused = false;
        pauseMenuUI.TogglePauseMenu(isPaused);
    }

    public Camera GetCamera()
    {
        return _camera;
    }

    public void ChangingScene(bool isChanging)
    {
        isChangingScene = isChanging;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsClient && IsOwner)
        {
            //GetComponent<CharacterController>().enabled = true;
            GetComponent<JUFootPlacement>().enabled = true;
            GetComponent<PlayerAiming>().enabled = true;
            GetComponent<CinemachineInputProvider>().enabled = true;

            var cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = _cameraFollowTarget.transform;

            GameObject.FindGameObjectWithTag("UI_Customize").GetComponent<CustomizeUI>()._player = gameObject;
            GameObject.FindGameObjectWithTag("UI_Customize").GetComponent<CustomizeUI>().Initialize();

            Cursor.lockState = CursorLockMode.None;
            transform.position = new Vector3(23, .55f, 7);

        }
        else
        {
            //GetComponent<CharacterController>().enabled = false;
            //GetComponent<CapsuleCollider>().enabled = true;

            //enabled = false;
        }

    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        MinigameScoreTracker.Instance.RemovePlayerServerRpc(NetworkObject.OwnerClientId);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }




    [ClientRpc]
    public void KillClientRpc(ulong clientId)
    {
        if (clientId != NetworkObject.OwnerClientId) return;

        Kill();
    }

    public void Kill()
    {
        isAlive = false;

        GetComponent<Animator>().enabled = false;
        // FIX ME
        // EnableRagdoll();
    }

    public void Revive()
    {
        isAlive = true;
        GetComponent<Animator>().enabled = true;
        //GetComponent<CharacterController>().enabled = true;

        // Reset the rotation of the player
        transform.rotation = Quaternion.identity;

        // FIX ME
        // DisableRagdoll();
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
        //_playerController = GetComponent<CharacterController>();
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
        _playerControls.Land.ScoreBoard.started += ScoreBoard;
        _playerControls.Land.ScoreBoard.canceled += ScoreBoard;
    }

    

    private void HandleAnimations()
    {
        _playerAnimator.SetBool("isAction", isMainhanding || isOffhanding);
        _playerAnimator.SetBool("isRunning", !isMainhanding && isRunning);
        _playerAnimator.SetBool("isMoving", isMoving);
    }

    private void HandleAttack()
    {
        if (!isAlive) return;

        if (isMainhanding && Time.time > nextAttackTime)
        {
            nextAttackTime = Time.time + 1f;
            Attack();
        }
    }




    [SerializeField] private float _movementSpeed = 3f;
    [SerializeField] private float _accelerationTime = 0.1f;
    [SerializeField] private float _decelerationTime = 0.1f;
    [SerializeField] private float _stopThreshold = 0.2f;
    [SerializeField] private float _movementDrag = 10f;


    private float _currentSpeed = 0f;
    private Vector3 _moveDirection = Vector3.zero;

    private void HandleMovement()
    {
        if (!isAlive) return;

        Vector2 movement = _playerControls.Land.Move.ReadValue<Vector2>();
        Vector3 moveNew = _camera.transform.forward * movement.y + _camera.transform.right * movement.x;
        moveNew.y = 0f;
        moveNew = moveNew.normalized;

        float targetSpeed = _movementSpeed;
        if (isRunning && !isMainhanding)
        {
            targetSpeed *= 2f;
        }
        float acceleration = targetSpeed / _accelerationTime;
        float deceleration = _movementSpeed / _decelerationTime;

        if (movement.magnitude < _stopThreshold)
        {
            _playerRigidbody.drag = _movementDrag;
            _currentSpeed = Mathf.Max(_currentSpeed - deceleration * Time.fixedDeltaTime, 0f);
        }
        else
        {
            _playerRigidbody.drag = 0f;
            _currentSpeed = Mathf.Min(_currentSpeed + acceleration * Time.fixedDeltaTime, targetSpeed);
        }
        Vector3 moveVelocity = moveNew * _currentSpeed;
        _playerRigidbody.velocity = moveVelocity;

        if (isOffhanding || isMainhanding)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0), 10f * Time.fixedDeltaTime);
        }
        else if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveNew);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }

        if (movement.magnitude < _stopThreshold && _playerRigidbody.velocity.magnitude < 0.1f)
        {
            _playerRigidbody.velocity = Vector3.zero;
        }

        if (!isGrounded)
        {
            StartCoroutine(CheckIfGroundedCoroutine());
        }
    }



    private IEnumerator CheckIfGroundedCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.5f);
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

        DirectionReference = _camera.transform;

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

        float jumpSpeed = Mathf.Sqrt(2f * Physics.gravity.magnitude * PlayerJumpForce);

        _playerRigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);

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

                CheckIfHit();
                break;
            default:
                _playerAnimator.SetTrigger("Attack");
                break;
        }
    }

    // Add this field to your PlayerController class
    private bool _isBeingKnockedBack = false;

    // Call this method to knock the player back
    public void Knockback(Vector3 direction, float force)
    {
        if (_isBeingKnockedBack)
        {
            // Player is already being knocked back, do nothing
            return;
        }

        // Disable player movement while they are being knocked back
        _isBeingKnockedBack = true;

        // Calculate knockback vector
        Vector3 knockbackVector = direction * force;

        // Apply knockback
        _playerRigidbody.AddForce(knockbackVector, ForceMode.Impulse);

        // Wait for the knockback to finish before enabling player movement
        StartCoroutine(EnableMovementAfterDelay(.5f));
    }

    private IEnumerator EnableMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Enable player movement
        _isBeingKnockedBack = false;
    }

    private void CheckIfHit()
    {
        float sphereRadius = 0.5f;
        float sphereDistance = 1.0f;

        Vector3 sphereCenter = transform.position + transform.forward * 0.5f + transform.up * sphereDistance;

        Collider[] colliders = Physics.OverlapSphere(sphereCenter, sphereRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.CompareTag("Player"))
            {
                IKnockbackable knockbackable = collider.GetComponent<IKnockbackable>();
                if (knockbackable != null && knockbackable.CanBeKnockedBack)
                {
                    // Apply knockback
                    float knockbackUpwardForce = 2f;
                    Vector3 knockbackDirection = (collider.transform.position - transform.position).normalized + Vector3.up * knockbackUpwardForce;
                    knockbackable.Knockback(knockbackDirection, 585.75f);

                    return;
                }
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
                    GameObject.Find("BugReportManager").GetComponent<BugReportManager>().CloseBugReport();
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

    public void ScoreBoard(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                if (isTakingSurvey || isCustomizing) return;

                _scoreBoardUI.GetComponent<Canvas>().enabled = true;

                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                _scoreBoardUI.GetComponent<Canvas>().enabled = false;
                break;
        }
    }
}




