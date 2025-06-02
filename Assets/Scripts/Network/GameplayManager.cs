using BattleRoyale.Event;
using BattleRoyale.Level;
using BattleRoyale.Main;
using BattleRoyale.Player;
using BattleRoyale.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameplayManager : NetworkBehaviour
{
    public static GameplayManager Instance { get; private set; }

    private NetworkVariable<GameplayState> _state = new NetworkVariable<GameplayState>(GameplayState.WaitingToStart);

    private LevelService _levelObj;
    private PlayerService _playerObj;

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

        UpdateCountdownClientRpc(0);
        _state.Value = GameplayState.GamePlaying;
        EventBusManager.Instance.Raise(EventName.ActivateTilesForGameplay, true);
    }

    [ClientRpc]
    private void UpdateCountdownClientRpc(int secondsRemaining)
    {
        EventBusManager.Instance.Raise(EventName.CountdownTick, secondsRemaining);
    }

    public override void OnNetworkSpawn()
    {
        _levelObj = GameManager.Instance.Get<LevelService>();
        _playerObj = GameManager.Instance.Get<PlayerService>();

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            _levelObj.StartLevel(NetworkManager.Singleton.ConnectedClientsIds.Count);
            HandlePlayerSpawn();
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

            NotifyClientsToShowGameplayUIClientRpc();
            StartCountdown(GameManager.Instance.ui_SO.countDownDuration);
        }
    }
}
