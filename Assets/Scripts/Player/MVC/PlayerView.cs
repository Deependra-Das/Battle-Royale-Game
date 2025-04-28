using BattleRoyale.Tile;
using BattleRoyale.Level;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleRoyale.Player
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private GameObject _cinemachineCameraTarget;
        [SerializeField] private CharacterController _charController;
        [SerializeField] private Animator _animator;
        [SerializeField] private PlayerInput _playerInput;

      
        private PlayerModel _playerModel;
        private bool _isPlayerInitialized = false;

        private bool Grounded = true;
        public LayerMask GroundLayers;
        [Range(0.5f, 5f)] public float strength = 1.1f;

        [Header("Cinemachine")]
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;

        public bool LockCameraPosition = false;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;

        private const float _threshold = 0.01f;

        [Header("Player Input")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;

        public bool analogMovement;
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;
        private bool IsCurrentDeviceMouse { get { return _playerInput.currentControlScheme == "KeyboardMouse"; } }


        public void Initialize(PlayerModel playerModel)
        {
            _playerModel = playerModel;
            _isPlayerInitialized = true;
            _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;
            AssignAnimationIDs();

            _jumpTimeoutDelta = _playerModel.JumpTimeout;
            _fallTimeoutDelta = _playerModel.FallTimeout;
        }

        private void Update()
        {
            if (_isPlayerInitialized)
            {
                HandleJumpAndGravity();
                GroundedCheck();
                HandleMovement();
            }
        }

        private void LateUpdate()
        {
            HandleCameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
        }

        private void HandleMovement()
        {
            float targetSpeed = _playerModel.MoveSpeed;
            if (move == Vector2.zero) targetSpeed = 0.0f;

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _playerModel.SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
            Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;

            if (move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  Camera.main.gameObject.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    _playerModel.RotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _charController.Move(targetDirection.normalized * (targetSpeed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            _animator.SetFloat(_animIDSpeed, _animationBlend);
        }

        private void HandleJumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = _playerModel.FallTimeout;
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
                if (jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(_playerModel.JumpHeight * -2f * _playerModel.Gravity);
                    _animator.SetBool(_animIDJump, true);
                }
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = _playerModel.JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }

                jump = false;
            }

            if (_verticalVelocity < _playerModel.TerminalVelocity)
            {
                _verticalVelocity += _playerModel.Gravity * Time.deltaTime;
            }
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _playerModel.GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, _playerModel.GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            _animator.SetBool(_animIDGrounded, Grounded);
        }

        private void HandleCameraRotation()
        {
            if (look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += look.y * deltaTimeMultiplier;
            }
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        //private void OnDrawGizmosSelected()
        //{
        //    Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        //    Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        //    if (Grounded) Gizmos.color = transparentGreen;
        //    else Gizmos.color = transparentRed;
        //    Gizmos.DrawSphere(
        //        new Vector3(transform.position.x, transform.position.y - _playerModel.GroundedOffset, transform.position.z),
        //        _playerModel.GroundedRadius);
        //}

        public void OnMove(InputValue value)
        {
            move = value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                look = value.Get<Vector2>();
            }
        }

        public void OnJump(InputValue value)
        {
            jump = value.isPressed;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            if (body != null)
            {
                HexTileView groundHexTile = body.gameObject.GetComponent<HexTileView>();

                if (groundHexTile != null)
                {
                    groundHexTile.PlayerOnTheTileDetected();
                }
            }
        }

        public GameObject PlayerCameraRoot { get { return _cinemachineCameraTarget; } }
    }
}