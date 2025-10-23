using BattleRoyale.NetworkModule;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.PlayerModule
{
    public class PlayerService
    {
        private PlayerScriptableObject _player_SO;
        private Dictionary<ulong, PlayerController> _activePlayers = new();
        private CinemachineCamera _playerCamera;

        public PlayerService(PlayerScriptableObject player_SO)
        {
            _player_SO = player_SO;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }

        public void SpawnPlayer(ulong clientId, Vector3 spawnPosition)
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                Vector3 directionToCenter = Vector3.zero - spawnPosition;
                directionToCenter.y = 0f;

                PlayerController playerController = Object.Instantiate(_player_SO.playerPrefab, spawnPosition, Quaternion.LookRotation(directionToCenter));
                playerController.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                playerController.SetUsernameText(PlayerSessionManager.Instance.GetPlayerSessionData(clientId).Username);

                int charSkinColorIndex = PlayerSessionManager.Instance.GetPlayerSessionData(clientId).SkinColorIndex;
                playerController.SetCharacterSkinMaterial(charSkinColorIndex);

                _activePlayers[clientId] = playerController;
            }     
        }

        public void SetupPlayerCam(Transform playerCameraRoot)
        {
            _playerCamera = Object.Instantiate(_player_SO.playerCameraPrefab, playerCameraRoot.transform.position, Quaternion.identity);
            _playerCamera.Follow = playerCameraRoot;
        }

        private void OnClientDisconnect(ulong clientId)
        {
            RemovePlayer(clientId);
        }

        public void RemovePlayer(ulong clientId)
        {
            if (_activePlayers.ContainsKey(clientId))
            {
                PlayerController playerController = _activePlayers[clientId];

                if(playerController.NetworkObject.IsSpawned)
                {
                    playerController.NetworkObject.Despawn();
                }

                Object.Destroy(playerController.gameObject);
                _activePlayers.Remove(clientId);
            }
        }

        public void Dispose()
        {
            foreach (var player in _activePlayers.Values)
            {
                if (player != null)
                {
                    player.NetworkObject.Despawn();
                    Object.Destroy(player.gameObject);
                }
            }

            _activePlayers.Clear();

            if (_playerCamera != null)
            {
                Object.Destroy(_playerCamera.gameObject);
            }

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            }
        }
    }
}
