using FishNet.Component.Spawning;
using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using GameKit.Dependencies.Utilities.Types;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;

public enum CharacterState
{
    Default,
}

public enum OrientationMethod
{
    TowardsCamera,
    TowardsMovement,
}

public enum AdditionalOrientationMethod
{
    None,
    TowardsGravity,
    TowardsGroundSlopeAndGravity,
}

public struct PlayerInputs
{
    public float MoveAxisForward;
    public float MoveAxisRight;
    public Quaternion CameraRotation;
    public float JumpDown;
    public bool CrouchDown;
    public bool CrouchUp;

}

public class PlayerController : NetworkBehaviour, ICharacterController
{

    [Header("Stable Movement")]
    public float MaxStableMoveSpeed = 10f;
    public float StableMovementSharpness = 15f;
    public float OrientationSharpness = 10f;
    public OrientationMethod OrientationMethod = OrientationMethod.TowardsCamera;

    [Header("Air Movement")]
    public float MaxAirMoveSpeed = 15f;
    public float AirAccelerationSpeed = 15f;
    public float Drag = 0.1f;

    [Header("Jumping")]
    public float JumpUpSpeed = 10.0f;
    public float JumpScalableForwardSpeed = 10.0f;
    public float JumpPreGroundingGraceTime = 0f;
    public float JumpPostGroundingGraceTime = 0f;

    [Header("Misc")]
    public AdditionalOrientationMethod AdditionalOrientationMethod = AdditionalOrientationMethod.None;
    public float AdditionalOrientationSharpness = 10f;
    public Vector3 Gravity = new Vector3(0, -30f, 0);
    public Transform MeshRoot;
    public Transform cameraFollowPoint;
    public float CrouchedCapsuleHeight = 1f;

    public CharacterState CurrentCharacterState { get; private set; }

    //Private
    private KinematicCharacterMotor Motor;
    private CameraController cameraController;
    private InteractController inventoryController;
    private Camera playerCamera;
    private Collider[] _probedColliders = new Collider[8];
    private RaycastHit[] _probedHits = new RaycastHit[8];
    private Vector3 _moveInputVector;
    private Vector3 _lookInputVector;
    private bool _jumpRequested = false;
    private bool _jumpConsumed = false;
    private bool _jumpedThisFrame = false;
    private float _timeSinceJumpRequested = Mathf.Infinity;
    private float _timeSinceLastAbleToJump = 0;
    private Vector3 _internalVelocityAdd = Vector3.zero;
    private bool _shouldBeCrouching = false;
    private bool _isCrouching = false;
    private bool initialized = false;


    private void Awake()
    {
        Motor = GetComponent<KinematicCharacterMotor>();
        
        TransitionToState(CharacterState.Default);

        Motor.CharacterController = this;
    }

    private void BootstrapNetworkManager_OnGameStarted(object sender, System.EventArgs e)
    {
        Init();
        Motor.enabled = true;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if(!base.IsOwner)
        {
            this.enabled = false;
            Motor.enabled = false;
            return;
        }
        else
        {
            BootstrapNetworkManager.OnGameStarted += BootstrapNetworkManager_OnGameStarted;
        }
    }

    public void DebugInit()
    {
        Init();
        Motor.enabled = true;
    }

    internal void Init()
    {
        GameInput.Instance.OnJumpAction += GameInput_OnJumpAction;
        GameInput.Instance.OnCrouchAction += GameInput_OnCrouchAction;
        GameInput.Instance.OnStandAction += GameInput_OnStandAction;

        playerCamera = Camera.main;
        cameraController = playerCamera.GetComponent<CameraController>();
        cameraController.BaseAwake();

        inventoryController = GetComponent<InteractController>();
        inventoryController.Init();

        // Tell camera to follow transform
        cameraController.SetFollowTransform(cameraFollowPoint);

        // Ignore the character's collider(s) for camera obstruction checks
        cameraController.IgnoredColliders.Clear();
        cameraController.IgnoredColliders.AddRange(GetComponentsInChildren<Collider>());

        initialized = true;
    }



    private void Update()
    {
        if(!base.IsOwner || initialized == false)
        {
            return;
        }

        HandleMovement();
        HandleCharacterInput();

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //}
    }

    private void LateUpdate()
    {
        if (!base.IsOwner || initialized == false)
        {
            return;
        }

        if (cameraController.RotateWithPhysicsMover && Motor.AttachedRigidbody != null)
        {
            cameraController.PlanarDirection = Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * cameraController.PlanarDirection;
            cameraController.PlanarDirection = Vector3.ProjectOnPlane(cameraController.PlanarDirection, Motor.CharacterUp).normalized;
        }

        cameraController.HandleCameraInput();
    }


    private void HandleCharacterInput()
    {
        PlayerInputs playerInputs = new PlayerInputs();

        playerInputs.CameraRotation = cameraController.Transform.rotation;
    }

    public void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveInputVector = new Vector3(inputVector.x, 0, inputVector.y);

        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(cameraController.Transform.rotation * Vector3.forward, Motor.CharacterUp).normalized;
        if (cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(cameraController.Transform.rotation * Vector3.up, Motor.CharacterUp).normalized;
        }
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

        switch (CurrentCharacterState)
        {
            case CharacterState.Default:

                //Move and look
                _moveInputVector = cameraPlanarRotation * moveInputVector;

                switch (OrientationMethod)
                {
                    case OrientationMethod.TowardsCamera:
                        _lookInputVector = cameraPlanarDirection;
                        break;
                    case OrientationMethod.TowardsMovement:
                        _lookInputVector = _moveInputVector.normalized;
                        break;
                }
                break;
        }

    }

    private void GameInput_OnCrouchAction(object sender, System.EventArgs e)
    {
        _shouldBeCrouching = true;

        if (!_isCrouching)
        {
            _isCrouching = true;
            Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
            MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
        }
    }

    private void GameInput_OnStandAction(object sender, System.EventArgs e)
    {
        _shouldBeCrouching = false;
    }

    private void GameInput_OnJumpAction(object sender, System.EventArgs e)
    {
        _timeSinceJumpRequested = 0;
        _jumpRequested = true;
    }

    /// <summary>
    /// Handles movement state transitions and enter/exit callbacks
    /// </summary>
    public void TransitionToState(CharacterState newState)
    {
        CharacterState tempOldState = CurrentCharacterState;
        OnStateExit(tempOldState, newState);
        CurrentCharacterState = newState;
        OnStateEnter(newState, tempOldState);
    }

    /// <summary>
    /// Event when entering a state
    /// </summary>
    public void OnStateEnter(CharacterState toState, CharacterState fromState)
    {
        switch (toState)
        {
            case CharacterState.Default:
                break;
        }
    }

    /// <summary>
    /// Event when exiting a state
    /// </summary>
    public void OnStateExit(CharacterState fromState, CharacterState toState)
    {
        switch (fromState)
        {
            case CharacterState.Default:
                break;
        }
    }


    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called before the character begins its movement update
    /// </summary>
    public void BeforeCharacterUpdate(float deltaTime)
    {
    }


    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its rotation should be right now. 
    /// This is the ONLY place where you should set the character's rotation
    /// </summary>
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
                if (_lookInputVector.sqrMagnitude > 0 && OrientationSharpness > 0f)
                {
                    // Smoothly interpolate from current to target look direction
                    Vector3 smoothedLookDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;
                    currentRotation = Quaternion.LookRotation(smoothedLookDirection, Motor.CharacterUp);
                }

                Vector3 currentUp = (currentRotation * Vector3.up);
                if (AdditionalOrientationMethod == AdditionalOrientationMethod.TowardsGravity)
                {
                    // Rotate from current up to invert gravity
                    Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-AdditionalOrientationSharpness * deltaTime));
                    currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                }
                else if (AdditionalOrientationMethod == AdditionalOrientationMethod.TowardsGroundSlopeAndGravity)
                {
                    if (Motor.GroundingStatus.IsStableOnGround)
                    {
                        Vector3 initialCharacterBottomHemiCenter = Motor.TransientPosition + (currentUp * Motor.Capsule.radius);
                        Vector3 smoothedGroundNormal = Vector3.Slerp(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-AdditionalOrientationSharpness * deltaTime));
                        currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                        // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                        Motor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * Motor.Capsule.radius));

                    }
                    else
                    {
                        Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-AdditionalOrientationSharpness * deltaTime));
                        currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                    }
                }
                else
                {
                    Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-AdditionalOrientationSharpness * deltaTime));
                    currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                }
                break;
        }
    }


    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its velocity should be right now. 
    /// This is the ONLY place where you can set the character's velocity
    /// </summary>
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
                {
                    // Ground movement
                    if (Motor.GroundingStatus.IsStableOnGround)
                    {
                        UpdateGroundVelocity(ref currentVelocity, deltaTime);
                    }
                    // Air movement
                    else
                    {
                        UpdateAirVelocity(ref currentVelocity, deltaTime);
                    }

                    // Handle jumping
                    UpdateJumpingVelocity(ref currentVelocity, deltaTime);

                    // Take into account additive velocity
                    if (_internalVelocityAdd.sqrMagnitude > 0f)
                    {
                        currentVelocity += _internalVelocityAdd;
                        _internalVelocityAdd = Vector3.zero;
                    }
                    break;
                }
        }
    }


    private void UpdateGroundVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        float currentVelocityMagnitude = currentVelocity.magnitude;

        Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

        // Reorient velocity on slope
        currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

        // Calculate target velocity
        Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
        Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
        Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

        // Smooth movement Velocity
        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
    }


    private void UpdateAirVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        // Add move input
        if (_moveInputVector.sqrMagnitude > 0f)
        {
            Vector3 addedVelocity = _moveInputVector * AirAccelerationSpeed * deltaTime;

            Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

            // Limit air velocity from inputs
            if (currentVelocityOnInputsPlane.magnitude < MaxAirMoveSpeed)
            {
                // clamp addedVel to make total vel not exceed max vel on inputs plane
                Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, MaxAirMoveSpeed);
                addedVelocity = newTotal - currentVelocityOnInputsPlane;
            }
            else
            {
                // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                {
                    addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                }
            }

            // Prevent air-climbing sloped walls
            if (Motor.GroundingStatus.FoundAnyGround)
            {
                if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                {
                    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                    addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                }
            }

            // Apply added velocity
            currentVelocity += addedVelocity;
        }

        // Gravity
        currentVelocity += Gravity * deltaTime;

        // Drag
        currentVelocity *= (1f / (1f + (Drag * deltaTime)));
    }

    private void UpdateJumpingVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        _jumpedThisFrame = false;
        _timeSinceJumpRequested += deltaTime;
        if (_jumpRequested)
        {
            // See if we actually are allowed to jump
            if (!_jumpConsumed && (Motor.GroundingStatus.FoundAnyGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime)
            {
                // Calculate jump direction before ungrounding
                Vector3 jumpDirection = Motor.CharacterUp;
                if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                {
                    jumpDirection = Motor.GroundingStatus.GroundNormal;
                }

                // Makes the character skip ground probing/snapping on its next update. 
                // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                Motor.ForceUnground();

                // Add to the return velocity and reset jump state
                currentVelocity += (jumpDirection * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                currentVelocity += (_moveInputVector * JumpScalableForwardSpeed);
                _jumpRequested = false;
                _jumpConsumed = true;
                _jumpedThisFrame = true;
            }
        }
    }


    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called after the character has finished its movement update
    /// </summary>
    public void AfterCharacterUpdate(float deltaTime)
    {
        switch (CurrentCharacterState)
        {
            case CharacterState.Default:
                {
                    // Handle jump-related values
                    {
                        // Handle jumping pre-ground grace period
                        if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                        {
                            _jumpRequested = false;
                        }

                        if (Motor.GroundingStatus.IsStableOnGround)
                        {
                            // If we're on a ground surface, reset jumping values
                            if (!_jumpedThisFrame)
                            {
                                _jumpConsumed = false;
                            }
                            _timeSinceLastAbleToJump = 0f;
                        }
                        else
                        {
                            // Keep track of time since we were last able to jump (for grace period)
                            _timeSinceLastAbleToJump += deltaTime;
                        }

                        // Handle Uncrouching
                        {
                            if (_isCrouching && !_shouldBeCrouching)
                            {
                                // Do an overlap test with the character's standing height to see if there are any obstructions
                                Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                                if (Motor.CharacterOverlap(
                                    Motor.TransientPosition,
                                    Motor.TransientRotation,
                                    _probedColliders,
                                    Motor.CollidableLayers,
                                    QueryTriggerInteraction.Ignore) > 0)
                                {
                                    // If obstructions, just stick to crouching dimensions
                                    Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
                                }
                                else
                                {
                                    // If no obstructions, uncrouch
                                    MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                                    _isCrouching = false;
                                }
                            }
                        }
                    }

                    break;
                }
        }
    }


    public void PostGroundingUpdate(float deltaTime)
    {
        if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLanded();
        }
        else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLeaveStableGround();
        }
    }

    protected void OnLanded()
    {
    }

    protected void OnLeaveStableGround()
    {
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }
}
