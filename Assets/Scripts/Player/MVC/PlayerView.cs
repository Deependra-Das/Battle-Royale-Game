using BattleRoyale.Event;
using BattleRoyale.Level;
using BattleRoyale.Main;
using BattleRoyale.Tile;
using TMPro;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleRoyale.Player
{
    public class PlayerView : NetworkBehaviour
    {
        [SerializeField] private GameObject _cinemachineCameraTarget;
        [SerializeField] private CharacterController _charController;
        [SerializeField] private Animator _animator;
        [SerializeField] private TMP_Text _usernameText;
        private PlayerInput _playerInput;

        [SerializeField] private float _moveSpeed = 10.0f;
        [SerializeField] private float _speedChangeRate = 10.0f;
        [Range(0.0f, 0.3f)]
        [SerializeField] private float _rotationSmoothTime = 0.12f;
        [SerializeField] private float _jumpHeight = 1.2f;
        [SerializeField] private float _gravity = -15.0f;
        [SerializeField] private float _terminalVelocity = 53.0f;
        [SerializeField] private float _jumpTimeout = 0.50f;
        [SerializeField] private float _fallTimeout = 0.15f;
        [SerializeField] private float _groundedOffset = -0.14f;
        [SerializeField] private float _groundedRadius  = 0.28f;
        private bool _grounded = true;
        [SerializeField] private LayerMask _groundLayers;
        [Range(0.5f, 5f)] [SerializeField] private float strength = 1.1f;

        [Header("Cinemachine")]
        [SerializeField] private float TopClamp = 70.0f;
        [SerializeField] private float BottomClamp = -30.0f;
        [SerializeField] private float CameraAngleOverride = 0.0f;

        [SerializeField] private bool LockCameraPosition = false;

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
        private bool _canMove = false;

        private void SubscribeToEvents()
        {
            EventBusManager.Instance.Subscribe(EventName.ActivatePlayerForGameplay, HandlePlayerActivationForGameplay);
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventName.ActivatePlayerForGameplay, HandlePlayerActivationForGameplay);
        }

        public void Awake()
        {
            _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;
            AssignAnimationIDs();

            _jumpTimeoutDelta = _jumpTimeout;
            _fallTimeoutDelta = _fallTimeout;
        }

        public override void OnDestroy()
        {
            UnsubscribeToEvents();
            Destroy(gameObject);    
        }

        public override void OnNetworkSpawn()
        {
            SubscribeToEvents();
            _playerInput = GetComponent<PlayerInput>();
            
            if (IsOwner)
            {
                _usernameText.text = PlayerPrefs.GetString(GameManager.UsernameKey).ToString();
                _playerInput.enabled = true;
                _playerInput.SwitchCurrentControlScheme(Keyboard.current, Mouse.current);
                GameManager.Instance.Get<PlayerService>().SetupPlayerCam(PlayerCameraRoot.transform);
            }
            else
            {
                _playerInput.enabled = false;
            }

            if (IsServer)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            }
        }

        private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
        {
            if(clientId == OwnerClientId)
            {
                UnsubscribeToEvents();
                Destroy(gameObject);
            }
        }

        private void HandlePlayerActivationForGameplay(object[] parameters)
        {
            if (IsOwner)
            {
                if ((bool)parameters[0])
                {
                    _canMove = true;
                }
                else
                {
                    _canMove = false;
                }
            }
        }

        private void Update()
        {
            if (!IsOwner) return;

            UsernameTextFaceToCam();
            GroundedCheck();

            if (_canMove)
            {
                HandleJumpAndGravity();
                HandleMovement();
            }
            else
            {
                _verticalVelocity = 0f;
                _animator.SetFloat(_animIDSpeed, 0f);
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }
        }

        private void LateUpdate()
        {
            if (!IsOwner) return;

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
            move = _playerInput.actions["Move"].ReadValue<Vector2>();
            float targetSpeed = _moveSpeed;
            if (move == Vector2.zero) targetSpeed = 0.0f;

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
            Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;

            if (move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  Camera.main.gameObject.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    _rotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _charController.Move(targetDirection.normalized * (targetSpeed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            _animator.SetFloat(_animIDSpeed, _animationBlend);
        }

        private void HandleJumpAndGravity()
        {
            if (_grounded)
            {
                _fallTimeoutDelta = _fallTimeout;
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
                if (jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                    _animator.SetBool(_animIDJump, true);
                }
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = _jumpTimeout;

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

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += _gravity * Time.deltaTime;
            }
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset,
                transform.position.z);
            _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers,
                QueryTriggerInteraction.Ignore);

            _animator.SetBool(_animIDGrounded, _grounded);
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

        //public void OnMove(InputValue value)
        //{
        //    move = value.Get<Vector2>();
        //}

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
                    ulong tileNetId = groundHexTile.NetworkObject.NetworkObjectId;
                    NotifyTileTouchedServerRpc(tileNetId);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void NotifyTileTouchedServerRpc(ulong tileNetworkObjectId)
        {
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(tileNetworkObjectId, out NetworkObject tileObj))
            {
                HexTileView tile = tileObj.GetComponent<HexTileView>();
                if (tile != null)
                {
                    tile.PlayerOnTheTileDetected();
                }
            }
        }

        public GameObject PlayerCameraRoot { get { return _cinemachineCameraTarget; } }

        [ClientRpc]
        public void SetupInitialPostionClientRpc(Vector3 targetPosition)
        {
            if (IsOwner)
            {
                GetComponent<CharacterController>().enabled = false;
                transform.position = targetPosition;
                GetComponent<CharacterController>().enabled = true;
            }
        }

        void UsernameTextFaceToCam()
        {
            Vector3 directionToCamera = Camera.main.transform.position - _usernameText.transform.position;
            directionToCamera.y = 0; 

            if (directionToCamera != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);
                _usernameText.transform.rotation = lookRotation;
            }
        }
    }
}