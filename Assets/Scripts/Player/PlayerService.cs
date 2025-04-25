using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace BattleRoyale.Player
{
    [DefaultExecutionOrder(-1)]
    public class PlayerService : MonoBehaviour
    {
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private PlayerScriptableObject _player_SO;
        [SerializeField] private CinemachineCamera _playerCamera;
        [SerializeField] private List<Vector2> playerSpawnPositionList;

        private PlayerController _acivePlayerController;

        void Start()
        {
            SpawnPlayer();
            SetCameraTarget();
        }

        public void SpawnPlayer()
        {
            _acivePlayerController = new PlayerController(_playerView, _player_SO, transform, playerSpawnPositionList[0]);
        }

        public void SetCameraTarget()
        {
            _playerCamera.Follow = _acivePlayerController.PlayerCameraRoot.transform;
        }
    }
}
