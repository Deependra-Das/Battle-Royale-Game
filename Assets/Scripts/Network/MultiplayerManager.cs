using BattleRoyale.CharacterSelection;
using BattleRoyale.Event;
using BattleRoyale.Main;
using BattleRoyale.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleRoyale.Network
{
    public class MultiplayerManager : NetworkBehaviour
    {
        public static MultiplayerManager Instance { get; private set; }
        public string PlayerUsername { get; private set; }

        public const int MAX_LOBBY_SIZE = 8;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            PlayerUsername = PlayerPrefs.GetString(GameManager.UsernameKey).ToString();
        }

        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.StartHost();
            PlayerSessionManager.Instance.RegisterPlayer(NetworkManager.Singleton.LocalClientId, PlayerUsername);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientHostDisconnected;
        }

        private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
        {
            connectionApprovalResponse.Approved = true;
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientHostDisconnected;
        }

        private void OnClientConnected(ulong clientId)
        {
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            {
                RequestPlayerRegistrationServerRpc(clientId, PlayerUsername);
            }
        }

        private void OnClientHostDisconnected(ulong clientId)
        {
            string activeSceneName = SceneManager.GetActiveScene().name.ToString();
            Enum.TryParse<SceneName>(activeSceneName, out var sceneEnumValue);

            if (NetworkManager.Singleton.IsServer && sceneEnumValue == SceneName.CharacterSelectionScene)
            {
                CharacterManager.Instance.DespawnCharacterForDisconnectedClient(clientId);
                RequestPlayerDeregistrationServerRpc(clientId);
            }
        }

        private IEnumerator DelayedReturnToStartScreen(float duration)
        {
            yield return new WaitForSeconds(duration);
            SceneLoader.Instance.LoadScene(SceneName.LobbyScene, true);
        }
    
        [ServerRpc(RequireOwnership = false)]
        public void RequestPlayerRegistrationServerRpc(ulong clientId, string username)
        {
            PlayerSessionManager.Instance.RegisterPlayer(clientId, username);

            PlayerSessionManager.Instance.GetAllPlayerSessionData();
            ConfirmPlayerRegistrationClientRpc(clientId);
        }

        [ClientRpc]
        private void ConfirmPlayerRegistrationClientRpc(ulong clientId)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                CharacterManager.Instance.HandleLateJoin(clientId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestPlayerDeregistrationServerRpc(ulong clientId)
        {
            PlayerSessionManager.Instance.DeregisterPlayer(clientId);
        }
    }
}
