using BattleRoyale.AudioModule;
using BattleRoyale.EventModule;
using BattleRoyale.MainModule;
using BattleRoyale.TileModule;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleRoyale.PlayerModule
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

        [SerializeField] private bool _lockCameraPosition = false;

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

        [Header("Player Character Skin")]

        [SerializeField] private PlayerCharMatSkinColorScriptableObject _charSkinMatInfo_SO;
        [SerializeField] private SkinnedMeshRenderer[] _skinnedMeshRenderersForBodyParts;

        private NetworkVariable<int> _selectedMaterialIndex = new NetworkVariable<int>(0);
        private NetworkVariable<FixedString128Bytes> _usernameNetworkText = new NetworkVariable<FixedString128Bytes>("Player");

        private void SubscribeToEvents()
        {
            EventBusManager.Instance.Subscribe(EventName.ActivatePlayerForGameplay, HandlePlayerActivationForGameplay);
            _selectedMaterialIndex.OnValueChanged += ApplySelectedMaterial;
            _usernameNetworkText.OnValueChanged += UpdateCharacterUsername;
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventName.ActivatePlayerForGameplay, HandlePlayerActivationForGameplay);
            _selectedMaterialIndex.OnValueChanged -= ApplySelectedMaterial;
            _usernameNetworkText.OnValueChanged -= UpdateCharacterUsername;
        }

        public void Awake()
        {
            AssignAnimationIDs();
            _lockCameraPosition = true;
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
                _cinemachineCameraTarget.transform.localRotation = Quaternion.identity;
                _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;
             
                _usernameText.text = _usernameNetworkText.Value.ToString();
                _playerInput.enabled = true;
                _playerInput.SwitchCurrentControlScheme(Keyboard.current, Mouse.current);
            
                GameManager.Instance.Get<PlayerService>().SetupPlayerCam(_cinemachineCameraTarget.transform);
                _lockCameraPosition = false;
            }
            else
            {
                _playerInput.enabled = false;
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
            if (IsOwner)
            {
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

            UsernameTextFaceToCam();
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
            if (look.sqrMagnitude >= _threshold && !_lockCameraPosition)
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

        public void SetCharacterSkinMaterial(int materialIndex)
        {
            SetMaterialIndexServerRpc(materialIndex);
        }

        private void ApplySelectedMaterial(int oldMaterialIndex, int newMaterialIndex)
        {
            if (_skinnedMeshRenderersForBodyParts != null && newMaterialIndex >= 0 && newMaterialIndex < _charSkinMatInfo_SO.charSkinInfoList.Length)
            {
                foreach (SkinnedMeshRenderer renderer in _skinnedMeshRenderersForBodyParts)
                {
                    Material[] materialsToRemap = renderer.materials;
                    materialsToRemap[0] = _charSkinMatInfo_SO.charSkinInfoList[newMaterialIndex].skinColorMaterial;
                    renderer.materials = materialsToRemap;
                }
            }
        }

        public void SetUsernameText(string usernameText)
        {
            SetUsernameServerRpc(usernameText);
        }

        private void UpdateCharacterUsername(FixedString128Bytes oldValue, FixedString128Bytes newValue)
        {
            _usernameText.text = _usernameNetworkText.Value.ToString();
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

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioManager.Instance.PlayFootStepsAudio(AudioModule.AudioType.PlayerFootstep, transform.TransformPoint(_charController.center));
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioManager.Instance.PlayFootStepsAudio(AudioModule.AudioType.PlayerJumpLand, transform.TransformPoint(_charController.center));
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

        [ServerRpc(RequireOwnership = false)]
        public void SetMaterialIndexServerRpc(int materialIndex)
        {
            if (materialIndex >= 0 && materialIndex < _charSkinMatInfo_SO.charSkinInfoList.Length)
            {
                _selectedMaterialIndex.Value = materialIndex;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetUsernameServerRpc(string newUsername)
        {
            _usernameNetworkText.Value = newUsername;
        }
    }
}