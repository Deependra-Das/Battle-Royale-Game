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
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                PlayerView playerView = Object.Instantiate(_player_SO.playerPrefab, spawnPosition, Quaternion.identity);
                Vector3 directionToCenter = Vector3.zero - spawnPosition;
                directionToCenter.y = 0f;
                if (directionToCenter != Vector3.zero)
                {
                    playerView.transform.rotation = Quaternion.LookRotation(directionToCenter);
                }
                playerView.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                playerView.SetupInitialPostionClientRpc(spawnPosition);

                players[clientId] = playerView;
            }     
        }

        public void SetupPlayerCam(Transform playerCameraRoot)
        {
            _playerCamera = Object.Instantiate(_player_SO.playerCameraPrefab);
            _playerCamera.Follow = playerCameraRoot;
            _playerCamera.transform.rotation = playerCameraRoot.rotation;
        }

        public void Dispose()
        {
            players.Clear();
            if(_playerCamera!=null) Object.Destroy(_playerCamera.gameObject);
        }
    }
}
