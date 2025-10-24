using BattleRoyale.EventModule;
using BattleRoyale.MainModule;
using BattleRoyale.XPModule;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.NetworkModule
{
    public class GameOverManager : NetworkBehaviour
    {
        public static GameOverManager Instance { get; private set; }

        private Dictionary<ulong, (string playerName, bool isFlagged)> _flagDictionary;
        private float _waitDurationBeforeScoreBoard = 3f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _flagDictionary = new Dictionary<ulong, (string, bool)>();
        }

        private void OnEnable()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void OnDisable()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (_flagDictionary.ContainsKey(clientId))
            {
                _flagDictionary.Remove(clientId);
            }
        }

        public void Initialize()
        {
            SetClientFlagServerRpc();
        }

        private IEnumerator GameOverWaitBeforeScoreCardCoroutine(float duration)
        {
            if (IsServer)
            {
                yield return new WaitForSeconds(duration);
                AddXpForWinnersClientRpc();
                RaiseGameOverScoreCardClientRpc();
            }
        }
        private void AddXpBasedOnRank(int rank)
        {
            if (rank <= 3)
            {
                GameManager.Instance.Get<XPService>().AddXPOnGameOver(rank);
            }
        }

        [ClientRpc]
        private void RaiseGameOverScoreCardClientRpc()
        {
            EventBusManager.Instance.RaiseNoParams(EventName.GameOverScoreCard);
        }

        [ClientRpc]
        private void AddXpForWinnersClientRpc()
        {
            var allPlayerData = PlayerSessionManager.Instance.GetAllPlayerSessionData();
            List<PlayerSessionData> sortedPlayers = new List<PlayerSessionData>(allPlayerData.Values);
            sortedPlayers.Sort((a, b) => a.Rank.CompareTo(b.Rank));
            int myrank = sortedPlayers.Find(x => x.ClientId == NetworkManager.Singleton.LocalClientId).Rank;

            if (sortedPlayers.Count > 1)
            {
                AddXpBasedOnRank(myrank);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetClientFlagServerRpc(ServerRpcParams serverRpcParams = default)
        {
            ulong clientId = serverRpcParams.Receive.SenderClientId;

            if (!_flagDictionary.ContainsKey(clientId))
            {
                _flagDictionary.Add(clientId, ("Player" + clientId, true));
            }
            else
            {
                _flagDictionary[clientId] = (_flagDictionary[clientId].playerName, true);
            }

            bool allClientsFlagged = true;

            foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!_flagDictionary.ContainsKey(clientID) || !_flagDictionary[clientID].isFlagged)
                {
                    allClientsFlagged = false;
                    break;
                }
            }

            if (allClientsFlagged)
            {
                StartCoroutine(GameOverWaitBeforeScoreCardCoroutine(_waitDurationBeforeScoreBoard));
            }
        }
    }
}
