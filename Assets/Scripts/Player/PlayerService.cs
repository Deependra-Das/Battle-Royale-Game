using BattleRoyale.Event;
using BattleRoyale.Level;
using BattleRoyale.Main;
using BattleRoyale.Network;
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
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
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
                playerView.SetUsernameText(PlayerSessionManager.Instance.GetPlayerSessionData(clientId).Username);

                int charSkinColorIndex = PlayerSessionManager.Instance.GetPlayerSessionData(clientId).SkinColorIndex;
                playerView.SetCharacterSkinMaterial(charSkinColorIndex);

                players[clientId] = playerView;
            }     
        }

        public void SetupPlayerCam(Transform playerCameraRoot)
        {
            _playerCamera = Object.Instantiate(_player_SO.playerCameraPrefab);
            _playerCamera.Follow = playerCameraRoot;
            _playerCamera.transform.rotation = playerCameraRoot.rotation;
        }

        private void OnClientDisconnect(ulong clientId)
        {
            RemovePlayer(clientId);
        }

        public void RemovePlayer(ulong clientId)
        {
            if (players.ContainsKey(clientId))
            {
                PlayerView playerView = players[clientId];

                if(playerView.NetworkObject.IsSpawned)
                {
                    playerView.NetworkObject.Despawn();
                }

                Object.Destroy(playerView.gameObject);
                players.Remove(clientId);
            }
        }

        public void Dispose()
        {
            foreach (var player in players.Values)
            {
                if (player != null)
                {
                    player.NetworkObject.Despawn();
                    Object.Destroy(player.gameObject);
                }
            }

            players.Clear();

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
