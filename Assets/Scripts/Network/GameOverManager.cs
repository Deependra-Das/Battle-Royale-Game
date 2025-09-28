using BattleRoyale.Event;
using BattleRoyale.Network;
using BattleRoyale.Scene;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameOverManager : NetworkBehaviour
{
    public static GameOverManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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

    [ClientRpc]
    private void UpdateGameOverCountdownClientRpc(int secondsRemaining)
    {
        EventBusManager.Instance.Raise(EventName.GameOverCountdownTick, secondsRemaining);
    }
}
