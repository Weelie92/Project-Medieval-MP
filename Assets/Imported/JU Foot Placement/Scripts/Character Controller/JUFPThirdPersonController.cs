using UnityEngine;

[AddComponentMenu("JU Foot Placement/Player Controller/Third Person Controller")]
public class JUFPThirdPersonController : MonoBehaviour
{
    Rigidbody rb;
    Animator mAnimator;
    ThirdPersonCameraController mCamera;
    Transform DesiredDirectionTransform;

    [JUSubHeader("Movement")]
    [JUHeader("MOVEMENT SETTINGS")]
    public bool UseRigidbodyController;
    [JUReadOnly("UseRigidbodyController", true)]public CharacterController CharacterController;
    private Vector3 playerVelocity;
    public float Speed = 1.8f;
    private float SpeedMultiplier = 0f;
    [JUReadOnly("UseRigidbodyController", true)] public float Gravity = -10;
    private float GravityForce;
    public bool BodyInclination = true;
    public bool PreventHighSlopWalking = true;
    private bool Walkable = true;
    [Range(0,60)]
    public float MaxSlopeAngle = 40;
    [JUSubHeader("Jumping")]
    [JUReadOnly("UseRigidbodyController")] public LayerMask GroundLayers;
    public float JumpForce = 3;
    [JUSubHeader("Ground Checker")]
    [JUReadOnly("UseRigidbodyController")] public float GroundCheckerHeightOffset = 0.45f;
    [JUReadOnly("UseRigidbodyController")] public float GroundCheckerHeightSize = 1f;
    [JUReadOnly("UseRigidbodyController")] public float GroundCheckerRadius = 0.15f;

    public bool IKTransitionOnGroundCheck = true;
    public JUFootPlacement FootPlacementScript;

    float InputX;
    float InputZ;

    Vector3 DesiredDirection;
    Quaternion DesiredCameraRotation;

    Vector3 EulerRotation;
    float BodyRotation;

    [JUHeader("STATES")]
    public bool CanMove = true;
    public bool IsWalking;
    public bool IsRunning;

    public bool IsGrounded;
    public bool IsJumping;

    private bool Stop = false;
    void Start()
    {
        if (UseRigidbodyController)
        {
            rb = GetComponent<Rigidbody>();
        }
        else
        {
            CharacterController = GetComponent<CharacterController>();
        }
        mAnimator = GetComponent<Animator>();
        mCamera = Camera.main.GetComponent<ThirdPersonCameraController>();
        DesiredDirectionTransform = new GameObject("DesiredDirectionTransform").transform;
        if (FootPlacementScript == null)
            FootPlacementScript = GetComponent<JUFootPlacement>();
        if (GroundLayers.value == 0)
            GroundLayers = LayerMask.GetMask("Default");
    }

    void Update()
    {
        Move();

        SetInputs();
        SetAnimation();
    }

    public void SetInputs()
    {
        // >>> Get Controller Input
        InputX = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;
        InputZ = Input.GetAxis("Vertical") * Speed * Time.deltaTime;

        // >>> Get Rotation Direction
        DesiredDirection = new Vector3(InputX, 0, InputZ);


        // >>> Camera Direction
        DesiredCameraRotation = mCamera.transform.rotation;
        DesiredCameraRotation.z = 0;
        DesiredCameraRotation.x = 0;

        //>>> Set Walking State
        if (Walkable)
        {
            if (Mathf.Abs(InputX) > 0 || Mathf.Abs(InputZ) > 0)
            {
                IsWalking = true;
            }
            else
            {
                IsWalking = false;
            }
        }
        else
        {
            IsWalking = false;
        }
        // >>> Set Running State
        if (Input.GetKey(KeyCode.LeftShift) && IsWalking == true)
        {
            IsRunning = true;
        }
        else
        {
            IsRunning = false;
        }
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            Jump();
        }
    }
    Vector3 move;
    public void Move()
    {
        if (UseRigidbodyController == false)
        {
            //Ground Check
            RaycastHit hit;
            if (IsJumping == false)
                IsGrounded = Physics.SphereCast(transform.position + transform.up * 1f, 0.2f, -transform.up, out hit, 1.5f, GroundLayers);
            else
                IsGrounded = false;

            //Reset gravity velocity
            if (CharacterController.isGrounded && playerVelocity.y < 0)
            {
                GravityForce = -1;
            }

            if (Input.GetButtonDown("Jump") && IsGrounded)
            {
                Jump();
            }

            //Apply gravity

            move = transform.forward * SpeedMultiplier * Speed * Time.deltaTime + Vector3.up * GravityForce * Time.deltaTime;
            GravityForce += Gravity * Time.deltaTime;
            CharacterController.Move(playerVelocity);
            playerVelocity = move;
        }
        if (IsGrounded && Walkable)
        {
            // >>> Transform Movement
            if (UseRigidbodyController)
            {
                rb.velocity = transform.forward * SpeedMultiplier * Speed + transform.up * rb.velocity.y;
            }


            // >>> Smooths Speed Multiplier
            if (IsWalking)
            {
                Stop = true;
                if (IsRunning)
                {
                    SpeedMultiplier = Mathf.Lerp(SpeedMultiplier, 2f, 3f * Time.deltaTime);
                }
                else
                {
                    SpeedMultiplier = Mathf.Lerp(SpeedMultiplier, 1, 3f * Time.deltaTime);
                }
            }
            else
            {
                SpeedMultiplier = Mathf.Lerp(SpeedMultiplier, 0, 6f * Time.deltaTime);
                
                //Lock movement for a short period
                if (Stop == true)
                {
                    LockMovementForSeconds(0.2f);
                    Stop = false;
                }
            }
        }

        

        // >> ROTATION
        Vector3 DesiredEulerAngles = transform.eulerAngles;

        if (InputX != 0 || InputZ != 0)
        {
            // >>> Set Desired Direction
            DesiredDirectionTransform.rotation = Quaternion.LookRotation(DesiredDirection) * DesiredCameraRotation;
            DesiredDirectionTransform.rotation = Quaternion.FromToRotation(DesiredDirectionTransform.up, transform.up) * DesiredDirectionTransform.rotation;
            DesiredDirectionTransform.position = transform.position;

            DesiredEulerAngles.y = Mathf.MoveTowardsAngle(DesiredEulerAngles.y, DesiredDirectionTransform.eulerAngles.y, 200 * Time.deltaTime);
        }
        // >>> Set Transform Rotation
        transform.eulerAngles = DesiredEulerAngles;

        // >>> Calculate Body Inclination
        float AngleBetweenDesiredDirectionAndCurrentDirection = Vector3.SignedAngle(transform.forward, DesiredDirectionTransform.forward, transform.up);

        if (BodyInclination)
        {
            if (IsGrounded)
            {
                mAnimator.SetFloat("Angle", Mathf.Lerp(mAnimator.GetFloat("Angle"), AngleBetweenDesiredDirectionAndCurrentDirection, 3 * Time.deltaTime));
                BodyRotation = Mathf.LerpAngle(BodyRotation, -(AngleBetweenDesiredDirectionAndCurrentDirection/6) * SpeedMultiplier, 4 * Time.deltaTime);
                Debug.DrawLine(transform.position, transform.position + transform.forward * 0.6f);
                Debug.DrawLine(transform.position, DesiredDirectionTransform.position + DesiredDirectionTransform.forward * 0.6f);
            }
            else
            {
                BodyRotation = Mathf.Lerp(BodyRotation, 0f, 8 * Time.deltaTime);
            }
        }


        if (IsJumping == false)
        {
            if (UseRigidbodyController)
            {
                // >>> Ground Checker
                Vector3 GroundCheckerSize = new Vector3(GroundCheckerRadius, GroundCheckerHeightSize, GroundCheckerRadius);
                var GroundCheker = Physics.OverlapBox(transform.position + transform.up * GroundCheckerHeightOffset, GroundCheckerSize, transform.rotation, GroundLayers);
                if (GroundCheker.Length != 0)
                {
                    IsGrounded = true;
                }
                else
                {
                    IsGrounded = false;
                }

                if (PreventHighSlopWalking)
                {
                    // >>> Slope Detection and Slide
                    RaycastHit SlopeDetectionHitGround;
                    if (Physics.Raycast(transform.position + transform.up * 1 + transform.forward * 0.2f, -Vector3.up, out SlopeDetectionHitGround, 3, GroundLayers))
                    {
                        float SlopeAngle = Vector3.Angle(Vector3.up, SlopeDetectionHitGround.normal);

                        if (SlopeAngle > MaxSlopeAngle)
                        {
                            // Movement Lock
                            if (IsWalking)
                            {
                                SpeedMultiplier = -1f;
                            }

                            //Slide slope
                            Vector3 SlopeNormal = SlopeDetectionHitGround.normal;
                            Vector3 GroundParallel = Vector3.Cross(transform.up, SlopeNormal);
                            Vector3 SlopeParallel = Vector3.Cross(GroundParallel, SlopeNormal);
                            //transform.position += SlopeParallel.normalized / 8;

                            rb.AddForce(SlopeParallel.normalized / 8, ForceMode.VelocityChange);
                            transform.Translate(0, -3f * Time.deltaTime, 0);
                            SpeedMultiplier = 0;
                            Walkable = false;
                        }
                        else
                        {
                            Walkable = true;
                        }
                    }
                    else
                    {
                        Walkable = true;
                    }
                }
                //Walk Up Stairs
                RaycastHit StairDetection;
                if (Physics.Raycast(transform.position + transform.up * 1 + transform.forward * 0.4f, -Vector3.up, out StairDetection, 0.9f, GroundLayers))
                {
                    Debug.DrawLine(transform.position + transform.up * 1 + transform.forward * 0.4f, StairDetection.point);
                    if (StairDetection.point.y > transform.position.y && IsWalking)
                    {
                        Vector3 StairHeight = transform.position;
                        StairHeight.y = StairDetection.point.y + 0.5f;
                        var distancee = Vector3.Distance(transform.position, StairHeight);

                        Vector3 rbvelocity = rb.velocity;
                        rbvelocity.y = distancee * 3;
                        rb.velocity = rbvelocity;
                    }
                }
            }
        }


        // >>> Body Placement is controlled by Ground Checker, this will detach the character from the ground.
        if (FootPlacementScript != null)
        {
            //Enable / Disable character's ground fixer 
            FootPlacementScript.SmoothBodyPlacementTransition = IsGrounded;

            //Enable / Disable foot placement smoothly 
            FootPlacementScript.SmoothIKTransition = IsGrounded;
        }
    }
    public void Jump()
    {
        IsJumping = true;
        IsGrounded = false;

        if (FootPlacementScript != null)
        {
            FootPlacementScript.KeepCharacterOnGround = false;
            //FootPlacementScript.EnableDynamicBodyPlacing = false;
        }
        if (UseRigidbodyController)
        {
            rb.AddRelativeForce(0, JumpForce * 100, 0, ForceMode.Impulse);
        }
        else
        {
            //cc.Move( new Vector3 (0,Mathf.Sqrt(JumpForce * -3.0f * Gravity),0));
            //playerVelocity.y += Mathf.Sqrt(JumpForce * -3.0f * Gravity);
            GravityForce += Mathf.Sqrt(JumpForce * -3.0f * Gravity);
        }
        mAnimator.SetTrigger("Jump");

        Invoke("DisableIsJumpingState", 0.5f);
    }
    public void DisableIsJumpingState()
    {
        IsJumping = false;
    }
    public void SetAnimation()
    {
        // >>> Set Animator States
        if (CanMove)
        {
            mAnimator.SetBool("IsWalking", IsWalking);
            mAnimator.SetBool("IsRunning", IsRunning);
        }
        else
        {
            mAnimator.SetBool("IsWalking", false);
            mAnimator.SetBool("IsRunning", false);
            IsRunning = false;
            IsWalking = false;
        }
        mAnimator.SetBool("IsJumping", IsJumping);
        mAnimator.SetBool("IsGrounded", IsGrounded);
    }

    public void LockMovementForSeconds(float Time)
    {
        CanMove = false;
        Invoke("EnableMovement", Time);
    }

    private void EnableMovement()
    {
        CanMove = true;
        Stop = false;
        if (IsInvoking("EnableMovement"))
        {
            CancelInvoke("EnableMovement");
        }
    }

    private void OnDrawGizmos()
    {
        if (UseRigidbodyController == false) return;
        Vector3 GroundCheckerSize = new Vector3(GroundCheckerRadius, GroundCheckerHeightSize, GroundCheckerRadius);
        if (IsGrounded)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + transform.up * GroundCheckerHeightOffset, GroundCheckerSize);
        }
        else
        {
            Gizmos.color = new Color(0,1,0, 0.3f);
            Gizmos.DrawWireCube(transform.position + transform.up * GroundCheckerHeightOffset, GroundCheckerSize);
        }
    }
}
