using BattleRoyale.EventModule;
using BattleRoyale.MainModule;
using BattleRoyale.NetworkModule;
using BattleRoyale.SceneModule;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.NetworkModule
{
    public class GameOverManager : NetworkBehaviour
    {
        public static GameOverManager Instance { get; private set; }

        private float _waitDurationBeforeScoreBoard = 3f;

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
            if (IsServer)
            {
                StartCoroutine(GameOverWaitBeforeScoreCardCoroutine(_waitDurationBeforeScoreBoard));
            }
        }

        private IEnumerator GameOverWaitBeforeScoreCardCoroutine(float duration)
        {
            if (IsServer)
            {
                yield return new WaitForSeconds(duration);

                RaiseGameOverScoreCardLocally();
                RaiseGameOverScoreCardClientRpc();
                StartCountdown(GameManager.Instance.ui_SO.gameOverCountdownDuration);
            }
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
            LoadStartScene();
        }

        private void ResetPlayerSessionData()
        {
            if (IsServer)
            {
                PlayerSessionManager.Instance.ResetAllSessions();
            }
        }

        private void LoadStartScene()
        {
            SceneLoader.Instance.LoadScene(SceneName.StartScene, true);
        }

        [ClientRpc]
        private void UpdateGameOverCountdownClientRpc(int secondsRemaining)
        {
            EventBusManager.Instance.Raise(EventName.GameOverCountdownTick, secondsRemaining);
        }

        private void RaiseGameOverScoreCardLocally()
        {
            EventBusManager.Instance.RaiseNoParams(EventName.GameOverScoreCard);
        }

        [ClientRpc]
        private void RaiseGameOverScoreCardClientRpc()
        {
            EventBusManager.Instance.RaiseNoParams(EventName.GameOverScoreCard);
        }
    }
}
