using BattleRoyale.Event;
using BattleRoyale.Main;
using BattleRoyale.Scene;
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

        private void Awake()
        {
            Instance = this;
            PlayerUsername = PlayerPrefs.GetString(GameManager.UsernameKey).ToString();
            DontDestroyOnLoad(gameObject);
        }

        public void StartHost()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.StartHost();
            PlayerSessionManager.Instance.RegisterPlayer(NetworkManager.Singleton.LocalClientId, PlayerUsername);
        }

        private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
        {
            connectionApprovalResponse.Approved = true;
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        private void OnClientConnected(ulong clientId)
        {
            RequestPlayerRegistrationServerRpc(clientId, PlayerUsername);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestPlayerRegistrationServerRpc(ulong clientId, string username)
        {
            PlayerSessionManager.Instance.RegisterPlayer(clientId, username);
        }

        public void StartCountdown(float duration)
        {
            if (IsServer)
            {
                StartCoroutine(GameOverCountdownCoroutine(duration));
            }
        }

        private IEnumerator GameOverCountdownCoroutine(float duration)
        {
            float timeRemaining = duration;

            while (timeRemaining > 0)
            {
                int displayValue = Mathf.CeilToInt(timeRemaining);
                UpdateGameOverCountdownClientRpc(displayValue);
                yield return new WaitForSeconds(1f);
                timeRemaining -= 1f;
            }

            ResetPlayerSessionData();
            LoadCharacterSelectionScene();
        }

        [ClientRpc]
        private void UpdateGameOverCountdownClientRpc(int secondsRemaining)
        {
            EventBusManager.Instance.Raise(EventName.GameOverCountdownTick, secondsRemaining);
        }

        private void ResetPlayerSessionData()
        {
            if (IsServer)
            {
                PlayerSessionManager.Instance.ResetAllSessions();
            }
        }

        private void LoadCharacterSelectionScene()
        {
            SceneLoader.Instance.LoadScene(SceneName.CharacterSelectionScene, true);
        }
    }
}
