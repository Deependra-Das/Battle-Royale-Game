using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleRoyale.Player
{
    public class PlayerView : MonoBehaviour
    {
        private PlayerInputActions _playerInputActions;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Animator _animator;

        [SerializeField] float rotationFactorPerFrame = 15.0f;
        [SerializeField] float runSpeedMultiplier = 3.0f;
        Vector2 currentMovementInput;
        Vector3 currentMovement;
        bool isMovementPressed;
        int isMovingHash;

        void Awake()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.PlayerInputControls.Move.started += OnMovementInput;
            _playerInputActions.PlayerInputControls.Move.canceled += OnMovementInput;
            _playerInputActions.PlayerInputControls.Move.performed += OnMovementInput;
            AssignAnimParamHash();
        }

        void OnMovementInput(InputAction.CallbackContext context)
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        }

        void OnEnable()
        {
            _playerInputActions.PlayerInputControls.Enable();
        }

        void OnDisable()
        {
            _playerInputActions.PlayerInputControls.Disable();
        }

        void Update()
        {
            HandleRotation();
            HandleAnimation();
            _characterController.Move(currentMovement * runSpeedMultiplier * Time.deltaTime);
        }

        private void AssignAnimParamHash()
        {
            isMovingHash = Animator.StringToHash("isMoving");
        }


        void HandleAnimation()
        {
            bool isMoving = _animator.GetBool(isMovingHash);

            if(isMovementPressed && !isMoving)
            {
                _animator.SetBool(isMovingHash, true);
            }
            else if (!isMovementPressed && isMoving)
            {
                _animator.SetBool(isMovingHash, false);
            }
        }

        void HandleRotation()
        {
            Vector3 positionToLookAt;
            positionToLookAt.x = currentMovement.x;
            positionToLookAt.y = 0.0f;
            positionToLookAt.z = currentMovement.z;
            Quaternion currentRotation = transform.rotation;

            if (isMovementPressed)
            {
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
            }            
        }

        void HandleGravity()
        {
            if(_characterController.isGrounded)
            {
                float groundedGravity = -0.05f;
                currentMovement.y = groundedGravity;
            }
            else
            {
                float gravity = -9.8f;
                currentMovement.y += gravity;
            }
        }
    }
}

