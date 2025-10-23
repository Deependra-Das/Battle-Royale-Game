using BattleRoyale.EventModule;
using BattleRoyale.LevelModule;
using BattleRoyale.MainModule;
using BattleRoyale.PlayerModule;
using BattleRoyale.SceneModule;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.NetworkModule
{
    public class GameplayManager : NetworkBehaviour
    {
        [SerializeField] private float _gameplayStartCountdownDuration;
        public static GameplayManager Instance { get; private set; }

        private NetworkVariable<GameplayState> _state = new NetworkVariable<GameplayState>(GameplayState.WaitingToStart);

        private LevelService _levelObj;
        private PlayerService _playerObj;

        private HashSet<ulong> _tileReadyClients = new();
        private bool _gameEnded = false;

        private enum GameplayState
        {
            WaitingToStart,
            Countdown,
            GamePlaying,
            GameOver
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Initialize()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                _levelObj = GameManager.Instance.Get<LevelService>();
                _playerObj = GameManager.Instance.Get<PlayerService>();

                PlayerSessionManager.Instance.GetAllPlayerSessionData();
                SpawnTileRegistry();
                StartCoroutine(InitializeLevelCoroutine());

                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            }
        }

        public override void OnNetworkDespawn()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        private void SpawnTileRegistry()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                if (TileRegistry.Instance != null) return;

                GameObject tileRegistryObj = Instantiate(GameManager.Instance.network_SO.tileRegistryPrefab.gameObject);
                tileRegistryObj.name = "TileRegistry";
                tileRegistryObj.GetComponent<NetworkObject>().Spawn(true);
            }
        }

        private IEnumerator InitializeLevelCoroutine()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                yield return GameManager.Instance.StartCoroutine(_levelObj.StartLevelCoroutine(NetworkManager.Singleton.ConnectedClientsIds.Count));

                yield return new WaitUntil(() => _levelObj.IsLevelReady);

                SendExpectedTileCountClientRpc(_levelObj.ServerSpawnedTileCount);
            }
        }

        [ClientRpc]
        private void SendExpectedTileCountClientRpc(int count)
        {
            if (!IsClient) return;
            TileRegistry.Instance?.InitializeTileRegistry(count);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RegisterClientTileReadyServerRpc(ulong clientId, ServerRpcParams rpcParams = default)
        {
            if (_tileReadyClients.Add(clientId))
            {
                Debug.Log($"Client {clientId} finished tile loading. ({_tileReadyClients.Count}/{NetworkManager.Singleton.ConnectedClientsIds.Count})");

                if (_tileReadyClients.Count == NetworkManager.Singleton.ConnectedClientsIds.Count)
                {
                    HandlePlayerSpawn();

                    NotifyClientsToShowGameplayUIClientRpc();
                    StartCountdown(_gameplayStartCountdownDuration);
                }
            }
        }

        private void HandlePlayerSpawn()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                List<Vector3> spawnPoints = _levelObj.GetPlayerSpawnPoints();

                int spawnIndex = 0;
                foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    PlayerSessionData playerSessionData = PlayerSessionManager.Instance.GetPlayerSessionData(clientId);

                    if (playerSessionData != null)
                    {
                        _playerObj.SpawnPlayer(clientId, spawnPoints[spawnIndex]);
                        PlayerSessionManager.Instance.SetPlayerStatusServerRpc(clientId, PlayerState.Playing);
                        spawnIndex++;
                    }
                    else
                    {
                        Debug.Log("Player does not have Session Data.");
                    }
                }
            }
        }

        [ClientRpc]
        public void NotifyClientsToShowGameplayUIClientRpc()
        {
            EventBusManager.Instance.RaiseNoParams(EventName.PlayerSpawnCompleted);
        }

        public void StartCountdown(float duration)
        {
            if (IsServer && _state.Value == GameplayState.WaitingToStart)
            {
                _state.Value = GameplayState.Countdown;
                StartCoroutine(GameplayCountdownCoroutine(duration));
            }
        }

        private IEnumerator GameplayCountdownCoroutine(float duration)
        {
            float timeRemaining = duration;

            while (timeRemaining > 0)
            {
                int displayValue = Mathf.CeilToInt(timeRemaining);
                UpdateGameplayCountdownClientRpc(displayValue);
                yield return new WaitForSeconds(1f);
                timeRemaining -= 1f;
            }

            InitializeGameplay();
        }

        private void InitializeGameplay()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                UpdateGameplayCountdownClientRpc(0);
                _state.Value = GameplayState.GamePlaying;
                ActivatePlayerForGameplayClientRpc();
                EventBusManager.Instance.Raise(EventName.ActivateTilesForGameplay, true);
            }
        }

        [ClientRpc]
        private void UpdateGameplayCountdownClientRpc(int secondsRemaining)
        {
            EventBusManager.Instance.Raise(EventName.GameplayStartCountdownTick, secondsRemaining);
        }

        [ClientRpc]
        private void ActivatePlayerForGameplayClientRpc()
        {
            EventBusManager.Instance.Raise(EventName.ActivatePlayerForGameplay, true);
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                PlayerSessionData playerSessionData = PlayerSessionManager.Instance.GetPlayerSessionData(clientId);

                if (playerSessionData != null)
                {
                    PlayerSessionManager.Instance.SetPlayerConnectionStatusServerRpc(clientId, PlayerConnectionState.Disconnected);
                    HandlePlayerGameOver(clientId);
                }
                else
                {
                    Debug.Log("Player does not have Session Data.");
                }
            }
        }

        public void HandlePlayerGameOver(ulong clientId)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                PlayerSessionData playerSessionData = PlayerSessionManager.Instance.GetPlayerSessionData(clientId);

                if (playerSessionData != null && playerSessionData.PlayerStatus != PlayerState.Eliminated)
                {
                    int assignedRank = GetLowestRank();
                    PlayerSessionManager.Instance.SetPlayerStatusServerRpc(clientId, PlayerState.Eliminated);
                    PlayerSessionManager.Instance.SetPlayerRankServerRpc(clientId, assignedRank);

                    int eliminatedCount = PlayerSessionManager.Instance.GetAllPlayerSessionData().Count(p => p.Value.PlayerStatus == PlayerState.Eliminated);

                    if(playerSessionData.ConnectionStatus != PlayerConnectionState.Disconnected)
                    {
                        NotifyClientEliminatedClientRpc(clientId);
                        NotifyClientOfRankClientRpc(clientId, assignedRank);
                        UpdateEliminationCountClientRpc(eliminatedCount);
                    }

                    CheckEndGameCondition();
                }
                else
                {
                    Debug.Log("Player does not have Session Data.");
                }
            }
        }

        private int GetLowestRank()
        {
            int totalPlayers = PlayerSessionManager.Instance.GetAllPlayerSessionData().Count();
            int eliminatedCount = PlayerSessionManager.Instance.GetAllPlayerSessionData().Count(p => p.Value.PlayerStatus == PlayerState.Eliminated);

            return totalPlayers - eliminatedCount;

        }

        private void CheckEndGameCondition()
        {
            if (_gameEnded) return;

            int remaining = PlayerSessionManager.Instance.GetAllPlayerSessionData().Count(p => p.Value.PlayerStatus != PlayerState.Eliminated);
            if (remaining <= 1)
            {
                _gameEnded = true;
                EndGameForAll();
            }
        }

        private void EndGameForAll()
        {
            _state.Value = GameplayState.GameOver;
            var playerResults = PlayerSessionManager.Instance.GetAllPlayerSessionData();
            foreach (var player in playerResults.Values.Where(player => player.PlayerStatus != PlayerState.Eliminated))
            {
                int assignedRank = GetLowestRank();
                PlayerSessionManager.Instance.SetPlayerStatusServerRpc(player.ClientId, PlayerState.Eliminated);
                PlayerSessionManager.Instance.SetPlayerRankServerRpc(player.ClientId, assignedRank);
                NotifyClientOfRankClientRpc(player.ClientId, assignedRank);
            }

            StartCoroutine(DelayedGameOverSceneChange());
        }

        private IEnumerator DelayedGameOverSceneChange()
        {
            yield return new WaitForSeconds(3f);
            SceneLoader.Instance.LoadScene(SceneName.GameOverScene, true);
        }

        [ClientRpc]
        private void NotifyClientEliminatedClientRpc(ulong clientId)
        {
            if (NetworkManager.Singleton.LocalClientId != clientId) return;

            EventBusManager.Instance.RaiseNoParams(EventName.PlayerEliminated);
        }

        [ClientRpc]
        private void NotifyClientOfRankClientRpc(ulong clientId, int rank)
        {
            if (NetworkManager.Singleton.LocalClientId != clientId) return;

            EventBusManager.Instance.Raise(EventName.PlayerAssignedRank, rank);
        }

        [ClientRpc]
        private void UpdateEliminationCountClientRpc(int count)
        {
            EventBusManager.Instance.Raise(EventName.UpdateEliminationCount, count);
        }
    }
}