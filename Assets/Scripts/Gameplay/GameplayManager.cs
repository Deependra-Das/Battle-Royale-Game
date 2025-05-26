using BattleRoyale.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameplayManager : NetworkBehaviour
{
    public static GameplayManager Instance { get; private set; }

    private NetworkVariable<GameplayState> _state = new NetworkVariable<GameplayState>(GameplayState.WaitingToStart);

    private enum GameplayState
    {
        WaitingToStart,
        Countdown,
        GamePlaying,
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

        UpdateCountdownClientRpc(0);
        _state.Value = GameplayState.GamePlaying;
    }

    [ClientRpc]
    private void UpdateCountdownClientRpc(int secondsRemaining)
    {
        EventBusManager.Instance.Raise(EventName.CountdownTick, secondsRemaining);
    }
}
