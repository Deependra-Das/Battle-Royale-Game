using Unity.Android.Gradle.Manifest;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace BattleRoyale.Player
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private GameObject _cinemachineCameraTarget;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Animator _animator;
        [SerializeField] private PlayerInput _playerInput;

        [Header("Cinemachine")]
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;


        private const float _threshold = 0.01f;

        public Vector2 move;
        public Vector2 look;
        public bool jump;

        public bool analogMovement;
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        private bool IsCurrentDeviceMouse
        {
            get
            {
                return _playerInput.currentControlScheme == "KeyboardMouse";
            }
        }

        public float MoveSpeed = 10.0f;
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 20.0f;
        public float VerticalSpeedChangeRate = 0.5f;
        public float FallGravityClamp = -20.0f;
        public float MaxJumpTime = 1.5f;
        public float gravity = -9.8f;
        public float GroundedGravity = -0.05f;
        public float FallMultiplier = 2.0f;

        private float _targetRotation = 0.0f;
        private float _rotationVelocity = 0.0f;
        private float _appliedverticalVelocity = 0.0f;
        private float _currentVerticalVelocity;
        private float _initialJumpVelocity;


        private bool _isJumping = false;
        private bool _isJumpingAnimating = false;
        private float _animationBlend;
        private int _animIDSpeed;
        private int _animIDJump;

        private void Start()
        {
            _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;

            AssignAnimationIDs();
            SetupJumpVariables();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDJump = Animator.StringToHash("Jump");
        }

        private void SetupJumpVariables()
        {
            float timeToApex = MaxJumpTime / 2;
            gravity = (-2 * MaxJumpTime) / Mathf.Pow(timeToApex, 2);
            _initialJumpVelocity = (2 * MaxJumpTime) / timeToApex;
        }


        private void Update()
        {
            HandleMovement();
            HandleGravity();
            HandleJump();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void HandleMovement()
        {
            float targetSpeed = MoveSpeed;

            if (move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;


            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;

            if (move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  Camera.main.gameObject.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _controller.Move(targetDirection.normalized * (targetSpeed * Time.deltaTime) +
                             new Vector3(0.0f, _appliedverticalVelocity, 0.0f) * Time.deltaTime);

            _animator.SetFloat(_animIDSpeed, _animationBlend);

        }

        private void HandleGravity()
        {
            bool isFalling = _currentVerticalVelocity <= 0.0f || !jump;

            if (_controller.isGrounded)
            {
                if (_isJumpingAnimating)
                {
                    _animator.SetBool(_animIDJump, false);
                    _isJumpingAnimating = false;
                }

                _currentVerticalVelocity = GroundedGravity;
                _appliedverticalVelocity = GroundedGravity;
            }
            else if (isFalling)
            {
                float prevVelocity_Y = _currentVerticalVelocity;
                _currentVerticalVelocity = _currentVerticalVelocity + (gravity * FallMultiplier * Time.deltaTime);
                _appliedverticalVelocity = Mathf.Max((prevVelocity_Y + _currentVerticalVelocity) * VerticalSpeedChangeRate, FallGravityClamp);
            }
            else
            {
                float prevVelocity_Y = _currentVerticalVelocity;
                _currentVerticalVelocity = _currentVerticalVelocity + (gravity * Time.deltaTime);
                _appliedverticalVelocity = (prevVelocity_Y + _currentVerticalVelocity) * VerticalSpeedChangeRate;
            }
        }

        private void HandleJump()
        {
            if (!_isJumping && _controller.isGrounded && jump)
            {
                _animator.SetBool(_animIDJump, true);
                _isJumpingAnimating = true;
                _isJumping = true;
                jump = false;
                _currentVerticalVelocity = _initialJumpVelocity;
                _appliedverticalVelocity = _initialJumpVelocity;
            }
            else if (_isJumping && _controller.isGrounded && !jump)
            {
                _isJumping = false;
            }
        }

        private void CameraRotation()
        {
            if (look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                _cinemachineTargetYaw += look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += look.y * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

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
    }
}