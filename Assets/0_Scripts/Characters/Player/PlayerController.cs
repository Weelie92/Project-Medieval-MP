using RPGCharacterAnims.Extensions;
using RPGCharacterAnims.Lookups;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Cinemachine;
using System;
using RPGCharacterAnims.Actions;
using System.Collections;
using PsychoticLab;

public class PlayerController : NetworkBehaviour, IKillable
{
    public static PlayerController LocalInstance { get; private set; }


    public static event EventHandler OnAnyPlayerSpawned;

    public List<AudioClip> footstepSounds;
    public List<AudioClip> jumpSounds;
    public List<AudioClip> landSounds;
    public List<AudioClip> damageSounds;
    public List<AudioClip> attackSounds;
    public List<AudioClip> victorySounds;
    public List<AudioClip> lossSounds;


    [SerializeField] private AudioSource audioSource;

    // Components
    [Header("Components")]
    private Camera _camera;
    private GameObject _cameraFollowTarget;
    private PlayerControls _playerControls;
    //private CharacterController _playerController;
    [SerializeField] public Rigidbody _playerRigidbody;
    private JUFootPlacement _footPlacementScript;
    public Animator _playerAnimator;

    // UI
    [Header("UI")]

    // Scriptable Objects
    [Header("Scriptable Objects")]
    private InventoryItemList inventoryItemList;
    private ItemPrefabList itemPrefabList;

    // Variables
    [Header("Variables")]
    public bool allowMovement = true;
    public float nextAttackTime;
    public float playerStaminaRegen;
    public bool isMoving = false;
    public bool isRunning = false;
    public bool isJumping = false;
    public bool isGrounded = true;
    public bool isOffhanding = false;
    public bool isMainhanding = false;
    public bool isCustomizing = false;
    public bool isTakingSurvey = false;
    public bool isPaused = false;

    public bool isInventoryOpen = false;
    //public bool isChangingScene = false;
    public bool isAlive = true;
    public string playerWeaponString = "Unarmed";


    [Header("UI/HUD")]
    //[HideInInspector] public PauseMenuUI pauseMenuUI;
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

    


    private void Awake()
    {

        _camera = Camera.main;

        //DontDestroyOnLoad(gameObject);


        _scoreBoardUI = GameObject.FindGameObjectWithTag("UI_ScoreBoard");

        _playerRigidbody = GetComponent<Rigidbody>();

        _ragdollRigidbodies = transform.Find("Root").GetComponentsInChildren<Rigidbody>();
        _ragdollBoxColliders = transform.Find("Root").GetComponentsInChildren<BoxCollider>();
        _ragdollCapsuleColliders = transform.Find("Root").GetComponentsInChildren<CapsuleCollider>();
        _ragdollSphereColliders = transform.Find("Root").GetComponentsInChildren<SphereCollider>();
       
        DisableRagdoll();

        _cameraFollowTarget = transform.Find("Camera Follow Target").gameObject;
        anim = GetComponent<Animator>();


        SceneManager.sceneLoaded += OnSceneLoaded;
    }



    private IEnumerator Start()
    {
        if (IsOwner)
        {
            GetComponent<PlayerAiming>().enabled = false;
            GetComponent<PlayerAiming>().enabled = true;

            CustomizeUI.Instance.InitializePlayerUI(GetComponent<NetworkObject>());
            PauseMenuUI.Instance.InitializePlayerUI(GetComponent<NetworkObject>());

            BuildUI();
            InitializePlayerControls();
            InitializeComponents();
            InitializePlayerEvents();

            MinigameScoreTracker.Instance.AddPlayerServerRpc(NetworkObject.OwnerClientId);

            yield return new WaitForSeconds(1f);
            CustomizeUI.Instance.ActivateEnabledObjects();
            LoadingHUD.Instance.ToggleFade(false);

            transform.position = new Vector3(22, 0.7f, 7);

            GetComponent<CharacterRandomizer>().Randomize();
        }
        _playerAnimator = GetComponent<Animator>();

    }


    void FixedUpdate()
    {

        if (!IsOwner) return;

        if (isCustomizing) return;

        HandleMovement();
        HandleAnimations();
        HandleAttack();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (isCustomizing) return;



        CheckIfPlayFallAnimation();

        CheckPlayerInteraction();

        if (Input.GetKeyDown(KeyCode.I)) ReviveServerRpc(NetworkObject.OwnerClientId);
        if (Input.GetKeyDown(KeyCode.K)) KillServerRpc(NetworkObject.OwnerClientId);
    }

    public Transform _ragdollPelvisTransform;

    private void DisableRagdoll()
    {
        Vector3 ragdollPosition = _ragdollPelvisTransform.position;
        Quaternion ragdollRotation = _ragdollPelvisTransform.rotation;

        transform.position = _ragdollPelvisTransform.position;

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

        transform.position = ragdollPosition;
        transform.rotation = ragdollRotation;
    }


    public void EnableRagdoll()
    {
        Vector3 playerVelocity = _playerRigidbody.velocity;

        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = false;
            rb.velocity = playerVelocity;
        }


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

        if (scene.name == "GameScene") transform.position = new Vector3(23, .55f, 7);


    }


    public void PlayFootstepSound()
    {
        int randomIndex = UnityEngine.Random.Range(0, footstepSounds.Count);
        AudioClip selectedClip = footstepSounds[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void JumpSound()
    {
        int randomIndex = UnityEngine.Random.Range(0, jumpSounds.Count);
        AudioClip selectedClip = jumpSounds[randomIndex];
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(selectedClip);
        audioSource.volume = 1f;
    }

    private bool canPlayLandSound = true;
    private float landSoundCooldown = 1f;

    public void LandSound()
    {
        if (!canPlayLandSound)
            return;

        int randomIndex = UnityEngine.Random.Range(0, landSounds.Count);
        AudioClip selectedClip = landSounds[randomIndex];
        audioSource.PlayOneShot(selectedClip);

        canPlayLandSound = false;
        StartCoroutine(StartLandSoundCooldown());
    }

    private IEnumerator StartLandSoundCooldown()
    {
        yield return new WaitForSeconds(landSoundCooldown);
        canPlayLandSound = true;
    }

    public void DamageTakenSound()
    {
        int randomIndex = UnityEngine.Random.Range(0, damageSounds.Count);
        AudioClip selectedClip = damageSounds[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void AttackSound()
    {
        int randomIndex = UnityEngine.Random.Range(0, attackSounds.Count);
        AudioClip selectedClip = attackSounds[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void VictorySound()
    {
        Debug.Log("Victory");
        int randomIndex = UnityEngine.Random.Range(0, victorySounds.Count);
        AudioClip selectedClip = victorySounds[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }

    public void LossSound()
    {
        Debug.Log("Loss");
        int randomIndex = UnityEngine.Random.Range(0, lossSounds.Count);
        AudioClip selectedClip = lossSounds[randomIndex];
        audioSource.PlayOneShot(selectedClip);
    }


    public Camera GetCamera()
    {
        if (_camera == null) _camera = Camera.main;

        return _camera;
    }

    public void MovePlayerToScene(Scene targetScene)
    {
        SceneManager.MoveGameObjectToScene(gameObject, targetScene);
    }


    public override void OnNetworkSpawn()
    {

        if (IsOwner)
        {
            LocalInstance = this;
        }



        if (IsClient && IsOwner)
        {

            GetComponent<JUFootPlacement>().enabled = true;
            GetComponent<PlayerAiming>().enabled = true;
            GetComponent<CinemachineInputProvider>().enabled = true;

            var cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = _cameraFollowTarget.transform;

            


            Cursor.lockState = CursorLockMode.None;
            transform.position = new Vector3(23, .55f, 7);

        }
        else
        {
            //GetComponent<PlayerController>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = true;

            enabled = false;
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




    [ServerRpc]
    public void KillServerRpc(ulong clientId)
    {

        //EnableRagdoll();

        // Notify the targeted client that it has been killed
        KillClientRpc(clientId);
    }


    [ClientRpc]
    public void KillClientRpc(ulong clientId)
    {
        if (NetworkObject.OwnerClientId == clientId)
        {

            isAlive = false;
            GetComponent<Animator>().enabled = false;

            EnableRagdoll();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReviveServerRpc(ulong clientId)
    {
        ReviveClientRpc(clientId);
    }

    [ClientRpc]
    public void ReviveClientRpc(ulong clientId)
    {
        if (clientId == NetworkObject.OwnerClientId)
        {
            DisableRagdoll();

            GetComponent<Animator>().enabled = true;
            GetComponent<Animator>().SetTrigger("GetUp");

            StartCoroutine(EnableMovementAfterDelay(.9f));
        }
    }


    private void BuildUI()
    {
        GameObject mainUI = GameObject.FindGameObjectWithTag("MainUI");

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
        _playerAnimator = GetComponent<Animator>();
        _footPlacementScript = GetComponent<JUFootPlacement>();
        PlayerRunSpeed = PlayerWalkSpeed * 2f;

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
        if (!isAlive) return;

        _playerAnimator.SetBool("isAction", isMainhanding || isOffhanding);
        _playerAnimator.SetBool("isRunning", !isMainhanding && isRunning);
        _playerAnimator.SetBool("isMoving", isMoving);
    }

    private void HandleAttack()
    {
        if (!isAlive || isPaused) return;

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


    [ClientRpc]
    public void SetAllowMovementClientRpc(bool isEnabled)
    {
        allowMovement = isEnabled;
    }

    private void HandleMovement()
    {
        if (!isAlive || !allowMovement || _isBeingKnockedBack) return;

        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();
            _playerControls.Enable();
        }

        Vector2 movementInput = _playerControls.Land.Move.ReadValue<Vector2>().normalized;

        if (_camera == null)
        {
            
            _camera = Camera.main;
        
        }

        Vector3 moveDirection = _camera.transform.forward * movementInput.y + _camera.transform.right * movementInput.x;
        moveDirection.y = 0f;
        moveDirection = moveDirection.normalized;

        float targetSpeed = _movementSpeed;
        if (isRunning && !isMainhanding)
        {
            targetSpeed *= 2f;
        }

        Vector3 targetVelocity = moveDirection * targetSpeed;
        Vector3 velocityChange = targetVelocity - _playerRigidbody.velocity;
        velocityChange.y = 0; // Do not affect the Y velocity

        if (_playerAnimator == null)
        {
            _playerAnimator = GetComponent<Animator>();
        }

        _playerAnimator.SetFloat("VelocityX", movementInput.x);
        _playerAnimator.SetFloat("VelocityZ", movementInput.y);

        _playerRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        if (isOffhanding || isMainhanding)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0), 10f * Time.fixedDeltaTime);
        }
        else if (movementInput.magnitude > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }

        if (!isGrounded)
        {
            StartCoroutine(CheckIfGroundedCoroutine());
        }
    }





    private IEnumerator CheckIfGroundedCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 raycastOrigin = transform.position + Vector3.up * 0.1f; // Slightly above the player's position

        isGrounded = Physics.Raycast(raycastOrigin, Vector3.down, 0.2f);

        if (isGrounded)
        {
            _footPlacementScript.EnableDynamicBodyPlacing = true;
            _footPlacementScript.SmoothIKTransition = true;
            
            LandSound();
        }
    }

    private void CheckPlayerInteraction()
    {
        if (isCustomizing) return;

        RaycastHit hit;

        if (_camera == null) _camera = Camera.main;
        
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

        if (_camera == null) _camera = Camera.main;

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
        
        JumpSound();

        if (_playerAnimator == null)
        {
            _playerAnimator = GetComponent<Animator>();

            if (_playerAnimator == null)
            {
                Debug.LogWarning("PlayerController: Animator component not found");

                return;
            }
        }

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

                AttackSound();

                CheckIfHit();
                break;
            default:
                _playerAnimator.SetTrigger("Attack");
                break;
        }
    }

    // Add this field to your PlayerController class
    [SerializeField] public bool _isBeingKnockedBack = false;


    private IEnumerator EnableMovementAfterDelay(float delay)
    {
        _playerAnimator.SetTrigger("GetUp");
        yield return new WaitForSeconds(delay);

        if (IsOwner)
        {
            _footPlacementScript.EnableDynamicBodyPlacing = true;
            _footPlacementScript.SmoothIKTransition = true;
        }




        yield return new WaitForSeconds(.4f);
        isAlive = true;
        _isBeingKnockedBack = false;
    }



    private void CheckIfHit()
    {
        if (!IsOwner) return;

        float sphereRadius = 0.5f;
        float sphereDistance = 1.0f;

        Vector3 sphereCenter = transform.position + transform.forward * 0.5f + transform.up * sphereDistance;
        Collider[] colliders = Physics.OverlapSphere(sphereCenter, sphereRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.CompareTag("Player"))
            {
                var playerController = collider.GetComponent<PlayerController>();
                if (playerController && !playerController._isBeingKnockedBack)
                {

                    Debug.Log($"OwnerClientId/TargetClientId: {collider.GetComponent<NetworkObject>().OwnerClientId}");

                    collider.gameObject.GetComponent<KnockbackHandler>().KnockbackServerRpc();
                    collider.GetComponent<KnockbackHandler>().Knockback();

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
        PauseMenuUI.Instance.TogglePauseMenu(!isPaused);
    }

    public void Move(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:

                isMoving = true;
                
                break;

            case InputActionPhase.Performed:
                break;

            case InputActionPhase.Canceled:
                isMoving = false;

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