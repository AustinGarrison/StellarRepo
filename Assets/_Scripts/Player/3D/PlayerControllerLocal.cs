using CallSOS.Player.Interaction;
using CallSOS.Player.SFX;
using CallSOS.Utilities;
using KinematicCharacterController;
using Michsky.UI.Heat;
using UnityEngine;

namespace CallSOS.Player
{
    public enum PlayerState
    {
        Walking,
        Crouching,
        Sprinting
    }

    public class PlayerControllerLocal : MonoBehaviour, ICharacterController
    {
        [Header("Stable Movement")]
        public float StableMovementSharpness = 15f;
        public float OrientationSharpness = 10f;
        public OrientationMethod OrientationMethod = OrientationMethod.TowardsCamera;

        [Header("Walking")]
        public float WalkingSpeed = 5f;

        [Header("Crouching")]
        public float CrouchingSpeed = 3f;
        public float CrouchedCapsuleHeight = 1f;

        [Header("Air Movement")]
        public float MaxAirMoveSpeed = 10f;
        public float AirAccelerationSpeed = 15f;
        public float Drag = 0.1f;

        [Header("Jumping")]
        public float JumpUpSpeed = 10.0f;
        public float JumpScalableForwardSpeed = 10.0f;
        public float JumpPreGroundingGraceTime = 0f;
        public float JumpPostGroundingGraceTime = 0f;

        [Header("Sprinting")]
        public float SprintSpeed = 10f;
        public float MaxSprintDuration = 1.5f;
        public float ExhaustedCooldown = 1f;

        [Header("Misc")]
        public AdditionalOrientationMethod AdditionalOrientationMethod = AdditionalOrientationMethod.None;
        public float AdditionalOrientationSharpness = 10f;
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        public Transform MeshRoot;
        public Transform cameraFollowPoint;

        public PlayerState CurrentPlayerState { get; private set; }

        public KinematicCharacterMotor motor { get {  return Motor; } }

        //Private
        private KinematicCharacterMotor Motor;
        private PlayerSoundManager playerSound;
        private CameraControllerLocal cameraController;
        private InventoryController inventoryController;
        [SerializeField] private PauseMenuManager pauseMenuManager;


        private float currentMoveSpeed;
        private Camera playerCamera;
        private Collider[] _probedColliders = new Collider[8];
        private RaycastHit[] _probedHits = new RaycastHit[8];
        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;
        private Vector3 _footstepRate = Vector3.zero;
        private bool isInitialized = false;
        private bool isPaused = false;

        // Jumping
        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0;
        private Vector3 _internalVelocityAdd = Vector3.zero;

        // Crouching
        private bool _shouldBeCrouching = false;
        private bool _isCrouching = false;

        // Sprinting
        private bool _shouldBeSprinting = false;
        private bool _isSprinting = false;
        internal float _timeSinceSprintStarted = 0f;
        private float _timeSinceSprintEnded = 0f;

        private void Awake()
        {
            Motor = GetComponent<KinematicCharacterMotor>();

            TransitionToState(PlayerState.Walking);

            Motor.CharacterController = this;
        }

        private void Start()
        {
            Motor.enabled = false;
            Init();
            Motor.enabled = true;
            return;
        }

        internal void Init()
        {
            GameInputPlayer.Instance.OnJumpAction += GameInput_OnJumpAction;
            GameInputPlayer.Instance.OnCrouchAction += GameInput_OnCrouchAction;
            GameInputPlayer.Instance.OnStandAction += GameInput_OnStandAction;
            GameInputPlayer.Instance.OnSprintStartAction += GameInput_OnSprintStartAction;
            GameInputPlayer.Instance.OnSprintEndAction += GameInput_OnSprintEndAction;

            pauseMenuManager.OnPauseToggle += PauseMenuManager_OnPauseToggle;

            playerCamera = Camera.main;
            cameraController = playerCamera.GetComponent<CameraControllerLocal>();
            cameraController.BaseAwake();

            inventoryController = GetComponent<InventoryController>();
            inventoryController.Initialize();

            playerSound = GetComponentInChildren<PlayerSoundManager>();
            if(playerSound != null) playerSound.Init();

            // Tell camera to follow transform
            cameraController.SetFollowTransform(cameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            cameraController.IgnoredColliders.Clear();
            cameraController.IgnoredColliders.AddRange(GetComponentsInChildren<Collider>());

            isInitialized = true;
        }

        #region Events

        private void GameInput_OnCrouchAction(object sender, System.EventArgs e)
        {
            _shouldBeCrouching = true;

            if (!_isCrouching)
            {
                _isCrouching = true;
                TransitionToState(PlayerState.Crouching);
                Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
                MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
            }
        }

        private void GameInput_OnStandAction(object sender, System.EventArgs e)
        {
            if (_isCrouching)
                _shouldBeCrouching = false;
        }

        private void GameInput_OnJumpAction(object sender, System.EventArgs e)
        {
            _timeSinceJumpRequested = 0;
            _jumpRequested = true;
        }
        private void GameInput_OnSprintStartAction(object sender, System.EventArgs e)
        {
            _shouldBeSprinting = true;

            if (!_isSprinting)
            {
                _isSprinting = true;
                TransitionToState(PlayerState.Sprinting);
            }
        }

        private void GameInput_OnSprintEndAction(object sender, System.EventArgs e)
        {
            if (_isSprinting)
            {
                _shouldBeSprinting = false;
                _isSprinting = false;
                TransitionToState(PlayerState.Walking);
            }

        }

        private void PauseMenuManager_OnPauseToggle(object sender, System.EventArgs e)
        {
            isPaused = !isPaused;
        }

        #endregion


        private void Update()
        {
            if (!isInitialized) return;
            if (isPaused)
            {
                _moveInputVector = Vector3.zero;
                return;
            }

            HandleMovement();
            GetCameraInput();
        }


        private void FixedUpdate()
        {
            // Calculate footsteps, _foodstepRate updated in UpdateGroundVelocity()
            if (Motor.GroundingStatus.IsStableOnGround && playerSound != null)
                playerSound.ProcessStepCycle(_footstepRate, CurrentPlayerState);
        }

        private void LateUpdate()
        {
            if (cameraController.RotateWithPhysicsMover && Motor.AttachedRigidbody != null)
            {
                cameraController.PlanarDirection = Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * cameraController.PlanarDirection;
                cameraController.PlanarDirection = Vector3.ProjectOnPlane(cameraController.PlanarDirection, Motor.CharacterUp).normalized;
            }

            cameraController.HandleCameraInput();
        }




        public void HandleMovement()
        {
            Vector2 inputVector = GameInputPlayer.Instance.GetMovementVectorNormalized();
            Vector3 moveInputVector = new Vector3(inputVector.x, 0, inputVector.y);

            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(cameraController.Transform.rotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(cameraController.Transform.rotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            switch (CurrentPlayerState)
            {
                case PlayerState.Walking:

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
                case PlayerState.Sprinting:

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
                case PlayerState.Crouching:

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

        private void GetCameraInput()
        {
            PlayerInputs playerInputs = new PlayerInputs();

            playerInputs.CameraRotation = cameraController.Transform.rotation;
        }


        /// <summary>
        /// Handles movement state transitions and enter/exit callbacks
        /// </summary>
        public void TransitionToState(PlayerState newState)
        {
            PlayerState tempOldState = CurrentPlayerState;
            OnStateExit(tempOldState, newState);
            CurrentPlayerState = newState;
            OnStateEnter(newState, tempOldState);
        }

        /// <summary>
        /// Event when entering a state
        /// </summary>
        public void OnStateEnter(PlayerState toState, PlayerState fromState)
        {
            switch (toState)
            {
                case PlayerState.Walking:
                    currentMoveSpeed = WalkingSpeed;
                    break;
                case PlayerState.Sprinting:
                    currentMoveSpeed = SprintSpeed;
                    break;
                case PlayerState.Crouching:
                    currentMoveSpeed = CrouchingSpeed;
                    break;
            }
        }

        /// <summary>
        /// Event when exiting a state
        /// </summary>
        public void OnStateExit(PlayerState fromState, PlayerState toState)
        {
            switch (fromState)
            {
                case PlayerState.Walking:
                    break;
                case PlayerState.Sprinting:
                    _isSprinting = false;
                    break;
                case PlayerState.Crouching:
                    _isCrouching = false;
                    break;
            }
        }


        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called before the character begins its movement update
        /// </summary>
        public void BeforeCharacterUpdate(float deltaTime)
        {
            switch (CurrentPlayerState)
            {
                // Sould Track how long since stopped sprinting
                case PlayerState.Walking:

                    break;

                // Sould Track how long since started sprinting
                case PlayerState.Sprinting:

                    break;

                // Sould Track how long since stopped sprinting
                case PlayerState.Crouching:

                    break;

            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its rotation should be right now. 
        /// This is the ONLY place where you should set the character's rotation
        /// </summary>
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            switch (CurrentPlayerState)
            {
                case PlayerState.Walking:
                    DefaultRotation(ref currentRotation, deltaTime);
                    break;
                case PlayerState.Sprinting:
                    DefaultRotation(ref currentRotation, deltaTime);
                    break;
                case PlayerState.Crouching:
                    DefaultRotation(ref currentRotation, deltaTime);
                    break;
            }
        }


        /// <summary>
        /// (Called by UpdateRotation)
        /// Used by Walking, Crouching and Sprinting PlayerStates
        /// </summary>
        private void DefaultRotation(ref Quaternion currentRotation, float deltaTime)
        {
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
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its velocity should be right now. 
        /// This is the ONLY place where you can set the character's velocity
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            switch (CurrentPlayerState)
            {
                case PlayerState.Walking:

                    DefaultVelocity(ref currentVelocity, deltaTime);
                    break;

                case PlayerState.Sprinting:

                    DefaultVelocity(ref currentVelocity, deltaTime);
                    break;

                // Same as default, but doesnt allow jumping
                case PlayerState.Crouching:

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

                    // Take into account additive velocity
                    if (_internalVelocityAdd.sqrMagnitude > 0f)
                    {
                        currentVelocity += _internalVelocityAdd;
                        _internalVelocityAdd = Vector3.zero;
                    }
                    break;

            }
        }

        /// <summary>
        /// (Called by UpdateVelocity)
        /// Default Velocity used by Sprinting and Walking
        /// </summary>
        private void DefaultVelocity(ref Vector3 currentVelocity, float deltaTime)
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
            Vector3 targetMovementVelocity = reorientedInput * currentMoveSpeed;

            // Smooth movement Velocity
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));

            _footstepRate = currentVelocity;
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
            switch (CurrentPlayerState)
            {
                case PlayerState.Walking:
                    
                    // Handle jump-related 
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
                    break;
                case PlayerState.Sprinting:
                    // Handle jump-related 
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

                    _timeSinceSprintStarted += Time.deltaTime;
                    
                    if(_timeSinceSprintStarted > MaxSprintDuration)
                    {
                        _shouldBeSprinting = false;
                        _isSprinting = false;
                        TransitionToState(PlayerState.Walking);
                    }

                    break;
                case PlayerState.Crouching:

                    // Cancel crouch if jump was requested. Dont jump though
                    if (_jumpRequested)
                    {
                        _jumpRequested = false;
                        _shouldBeCrouching = false;
                    }

                    // Handle Uncrouching
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
                            TransitionToState(PlayerState.Walking);
                        }
                    }
                    
                    break;
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
            if (playerSound != null) playerSound.PlayLandingSound();
        }

        protected void OnLeaveStableGround()
        {
            if (playerSound != null) playerSound.PlayJumpSound();
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
}