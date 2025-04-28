using BattleRoyale.Level;
using BattleRoyale.Main;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace BattleRoyale.Player
{
    public class PlayerService
    {
        private PlayerScriptableObject _player_SO;
        private CinemachineCamera _playerCamera;
        private PlayerController _acivePlayerController;

        public PlayerService(PlayerScriptableObject player_SO)
        {
            _player_SO = player_SO;
        }

        public void SpawnPlayer(List<Vector3> playerSpawnPositionList)
        {
            _acivePlayerController = new PlayerController(_player_SO, playerSpawnPositionList[0]);
            SetPlayerCamera(playerSpawnPositionList[0]);
        }

        public void SetPlayerCamera(Vector3 camIntialPosition)
        {
            _playerCamera = Object.Instantiate(_player_SO.playerCameraPrefab , camIntialPosition , Quaternion.identity);
            _playerCamera.Follow = _acivePlayerController.PlayerCameraRoot.transform;
        }

        public void Dispose()
        {
            _acivePlayerController.DisposePlayerGameObject();
            _acivePlayerController = null;
            Object.Destroy(_playerCamera.gameObject);
        }
    }
}
