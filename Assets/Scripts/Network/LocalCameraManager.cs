using Fusion;
using Unity.Cinemachine;
using UnityEngine;
using static Unity.Collections.Unicode;

public class LocalCameraManager : MonoBehaviour
{
    [Header("Cinemachine")]
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;
    public bool LockCameraPosition = false;
    private const float _threshold = 0.01f;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private Transform _cinemachineCameraTarget;
    private CharacterMovementController _characterMovementController = null;
    private CharacterInputManager _characterInputManager = null;
    float deltaTimeMultiplier = 1.0f;

    private void Awake()
    {
        NetworkGameManager.OnAddPlayerCameraEvent += AddPlayerCamera;
    }

    void OnDestroy()
    {
        NetworkGameManager.OnAddPlayerCameraEvent -= AddPlayerCamera;
    }

    private void LateUpdate()
    {
        if (_characterMovementController == null || _characterInputManager == null ) return;
        CameraRotation();
    }


    private void CameraRotation()
    {
        Debug.Log(_characterInputManager.lookInputVector);
        if (_characterInputManager.lookInputVector.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            _cinemachineTargetYaw += _characterInputManager.lookInputVector.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _characterInputManager.lookInputVector.y * deltaTimeMultiplier;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f)
            lfAngle += 360f;

        if (lfAngle > 360f)
            lfAngle -= 360f;

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void AddPlayerCamera(NetworkRunner runner, NetworkPlayer playerObject)
    {
        if (playerObject.Object.HasStateAuthority)
        {   
             _characterMovementController = playerObject.gameObject.GetComponent<CharacterMovementController>();
            _cinemachineCameraTarget = _characterMovementController.cinemachineCameraTarget.transform;
            _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _characterInputManager = playerObject.gameObject.GetComponent<CharacterInputManager>();
            deltaTimeMultiplier = _characterMovementController.IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
        }
    }
}
