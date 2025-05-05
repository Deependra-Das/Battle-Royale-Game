using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;
using BattleRoyale.Player;

public class CharacterMovementController : NetworkBehaviour
{
    // Serialized fields
    [SerializeField] public GameObject cinemachineCameraTarget;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerInput _playerInput;

    private CharacterInputManager _characterInputManager;

    // Device check for mouse
    public bool IsCurrentDeviceMouse
    {
        get
        {
            return _playerInput.currentControlScheme == "KeyboardMouse";
        }
    }

    // Player movement settings
    public float MoveSpeed = 5.0f;
    [Range(0.0f, 0.3f)] public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;

    // Internal movement state
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;

    public float jumpHeight = 1.2f;
    public float gravity = -20.0f;
    public float terminalVelocity = 53.0f;
    public float jumpTimeout = 0.50f;
    public float fallTimeout = 0.15f;
    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;

    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private bool Grounded = true;
    public LayerMask GroundLayers;

    
    void Awake()
    {
    
    }

    private void Start()
    {
        _characterInputManager = GetComponent<CharacterInputManager>();
    }


    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData networkInputData))
        {
            HandleRotation(networkInputData);
            HandleMovement(networkInputData);
            GroundedCheck();
            HandleJumpAndGravity(networkInputData);
        }
    }

    private void HandleMovement(NetworkInputData networkInputData)
    {
        float targetSpeed = MoveSpeed;

        if (networkInputData.movementInput == Vector2.zero)
            targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Runner.DeltaTime * SpeedChangeRate);

        if (_animationBlend < 0.01f)
            _animationBlend = 0f;


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (targetSpeed * Runner.DeltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Runner.DeltaTime);
    }

    private void HandleRotation(NetworkInputData networkInputData)
    {
        Vector3 inputDirection = new Vector3(networkInputData.movementInput.x, 0.0f, networkInputData.movementInput.y).normalized;

        if (networkInputData.movementInput != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + Camera.main.gameObject.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }


    private void HandleJumpAndGravity(NetworkInputData networkInputData)
    {
        if (Grounded)
        {
            _fallTimeoutDelta = fallTimeout;
            //_animator.SetBool(_animIDJump, false);
            //_animator.SetBool(_animIDFreeFall, false);

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
            if (networkInputData.isJumpPressed && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                //_animator.SetBool(_animIDJump, true);
            }
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Runner.DeltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = jumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Runner.DeltaTime;
            }
            else
            {
                //_animator.SetBool(_animIDFreeFall, true);
            }

            networkInputData.isJumpPressed = false;
        }

        if (_verticalVelocity < terminalVelocity)
        {
            _verticalVelocity += gravity * Runner.DeltaTime;
        }
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, groundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        //_animator.SetBool(_animIDGrounded, Grounded);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f)
            lfAngle += 360f;

        if (lfAngle > 360f)
            lfAngle -= 360f;

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
