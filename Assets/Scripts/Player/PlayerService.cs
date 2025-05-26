using BattleRoyale.Event;
using BattleRoyale.Level;
using BattleRoyale.Main;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.Player
{
    public class PlayerService
    {
        private PlayerScriptableObject _player_SO;
        private Dictionary<ulong, PlayerView> players = new();
        private CinemachineCamera _playerCamera;

        public PlayerService(PlayerScriptableObject player_SO)
        {
            _player_SO = player_SO;
        }

        public void SpawnPlayer(ulong clientId, Vector3 spawnPosition)
        {
            PlayerView playerView = null;
            playerView = Object.Instantiate(_player_SO.playerPrefab, spawnPosition, Quaternion.identity);
            playerView.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

            players[clientId] = playerView;
        }

        public void SetupPlayerCam(Transform playerCameraRoot)
        {
            _playerCamera = Object.Instantiate(_player_SO.playerCameraPrefab);
            _playerCamera.Follow = playerCameraRoot;
        }

        public void Dispose()
        {
            players.Clear();
            Object.Destroy(_playerCamera.gameObject);
        }
    }
}
