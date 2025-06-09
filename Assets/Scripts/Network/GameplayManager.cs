using BattleRoyale.Event;
using BattleRoyale.Level;
using BattleRoyale.Main;
using BattleRoyale.Player;
using BattleRoyale.Scene;
using BattleRoyale.Tile;
using BattleRoyale.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : NetworkBehaviour
{
    public static GameplayManager Instance { get; private set; }

    private NetworkVariable<GameplayState> _state = new NetworkVariable<GameplayState>(GameplayState.WaitingToStart);

    private LevelService _levelObj;
    private PlayerService _playerObj;

    private HashSet<ulong> _tileReadyClients = new();

    private Dictionary<ulong, PlayerSessionData> _playerResults = new();
    private bool _gameEnded = false;

    private enum GameplayState
    {
        WaitingToStart,
        Countdown,
        GamePlaying,
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

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            _levelObj = GameManager.Instance.Get<LevelService>();
            _playerObj = GameManager.Instance.Get<PlayerService>();

            SpawnTileRegistry();
            StartCoroutine(InitializeLevelCoroutine());

            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!_playerResults.ContainsKey(clientId))
                {
                    _playerResults[clientId] = new PlayerSessionData(clientId);
                }
            }

            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void SpawnTileRegistry()
    {
        if (TileRegistry.Instance != null) return;

        GameObject tileRegistryObj = Instantiate(GameManager.Instance.network_SO.tileRegistryPrefab.gameObject);
        tileRegistryObj.name = "TileRegistry";
        tileRegistryObj.GetComponent<NetworkObject>().Spawn(true);
    }

    private IEnumerator InitializeLevelCoroutine()
    {
        yield return GameManager.Instance.StartCoroutine(_levelObj.StartLevelCoroutine(NetworkManager.Singleton.ConnectedClientsIds.Count));

        yield return new WaitUntil(() => _levelObj.IsLevelReady);

        SendExpectedTileCountClientRpc(_levelObj.ServerSpawnedTileCount);        
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
        Debug.Log(clientId);

        if (_tileReadyClients.Add(clientId))
        {
            Debug.Log($"Client {clientId} finished tile loading. ({_tileReadyClients.Count}/{NetworkManager.Singleton.ConnectedClientsIds.Count})");

            if (_tileReadyClients.Count == NetworkManager.Singleton.ConnectedClientsIds.Count)
            {
                HandlePlayerSpawn();

                NotifyClientsToShowGameplayUIClientRpc();
                StartCountdown(GameManager.Instance.ui_SO.countDownDuration);
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
                _playerObj.SpawnPlayer(clientId, spawnPoints[spawnIndex]);
                spawnIndex++;
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
        if (IsServer)
        {
            _state.Value = GameplayState.Countdown;
            StartCoroutine(CountdownCoroutine(duration));
        }
    }

    private IEnumerator CountdownCoroutine(float duration)
    {
        float timeRemaining = duration;

        while (timeRemaining > 0)
        {
            int displayValue = Mathf.CeilToInt(timeRemaining);
            UpdateCountdownClientRpc(displayValue);
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
        }

        InitializeGameplay();
    }

    private void InitializeGameplay()
    {
        UpdateCountdownClientRpc(0);
        _state.Value = GameplayState.GamePlaying;
        ActivatePlayerForGameplayClientRpc();
        //EventBusManager.Instance.Raise(EventName.ActivateTilesForGameplay, true);
    }

    [ClientRpc]
    private void UpdateCountdownClientRpc(int secondsRemaining)
    {
        EventBusManager.Instance.Raise(EventName.CountdownTick, secondsRemaining);
    }

    [ClientRpc]
    private void ActivatePlayerForGameplayClientRpc()
    {
        EventBusManager.Instance.Raise(EventName.ActivatePlayerForGameplay, true);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (_playerResults.TryGetValue(clientId, out var player) && !player.IsEliminated)
        {
            player.MarkDisconnected();
            HandlePlayerGameOver(clientId);
        }
    }

    public void HandlePlayerGameOver(ulong clientId)
    {
        if (!_playerResults.TryGetValue(clientId, out var player) || player.IsEliminated)
            return;

        int assignedRank = GetNextRank();
        player.MarkEliminated(assignedRank);
        int eliminatedCount = _playerResults.Count(p => p.Value.IsEliminated);

        NotifyClientEliminatedClientRpc(clientId);
        NotifyClientOfRankClientRpc(clientId, assignedRank);
        UpdateEliminationCountClientRpc(eliminatedCount);

        CheckEndGameCondition();
    }

    private int GetNextRank()
    {
        int totalPlayers = _playerResults.Count;
        int eliminatedCount = _playerResults.Count(player => player.Value.IsEliminated);

        return totalPlayers - eliminatedCount;
    }

    private void CheckEndGameCondition()
    {
        if (_gameEnded) return;

        int remaining = _playerResults.Values.Count(player => !player.IsEliminated);
        if (remaining <= 1)
        {
            _gameEnded = true;
            EndGameForAll();
        }
    }

    private void EndGameForAll()
    {
        foreach (var player in _playerResults.Values.Where(player => !player.IsEliminated))
        {
            int assignedRank = GetNextRank();
            player.MarkEliminated(assignedRank);
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
