using Unity.Cinemachine;
using UnityEngine;

namespace BattleRoyale.Player
{
    public class PlayerService : MonoBehaviour
    {
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private PlayerScriptableObject _player_SO;
        [SerializeField] private CinemachineCamera _playerCamera;

        private PlayerController _acivePlayerController;

        void Start()
        {
            SpawnPlayer();
            SetCameraTarget();
        }

        public void SpawnPlayer()
        {
            _acivePlayerController = new PlayerController(_playerView, _player_SO);
        }

        public void SetCameraTarget()
        {
            _playerCamera.Follow = _acivePlayerController.PlayerCameraRoot.transform;
        }
    }
}
