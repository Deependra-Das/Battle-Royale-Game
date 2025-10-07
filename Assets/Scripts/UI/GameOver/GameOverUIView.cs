using BattleRoyale.EventModule;
using BattleRoyale.MainModule;
using BattleRoyale.SceneModule;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UIModule
{
    public class GameOverUIView : MonoBehaviour
    {
        [SerializeField] private GameObject _scoreboardUI;
        [SerializeField] private Transform _scoreboardContentTransform;
        [SerializeField] private TMP_Text _gameOverCountdownText;

        [Header("GameOver PopUp")]
        [SerializeField] private GameObject _gameOverPopUp;

        [Header("Disconnected PopUp")]
        [SerializeField] private GameObject _disconnectedGameOverUIPopUp;
        [SerializeField] private TMP_Text _disconnectedCountdownGameOverUIText;

        private int _currentCountdownValue = -1;
        public float _disconnectedCountdownTime = 5f;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            EventBusManager.Instance.Subscribe(EventName.GameOverCountdownTick, HandleCountdownTick);
            EventBusManager.Instance.Subscribe(EventName.GameOverScoreCard, HandleGameOverScoreCard);
            NetworkManager.Singleton.OnClientDisconnectCallback += ShowDisconnectionGameOverUI;
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventName.GameOverCountdownTick, HandleCountdownTick);
            EventBusManager.Instance.Unsubscribe(EventName.GameOverScoreCard, HandleGameOverScoreCard);
            NetworkManager.Singleton.OnClientDisconnectCallback -= ShowDisconnectionGameOverUI;
        }

        private void Start()
        {
            _disconnectedGameOverUIPopUp.SetActive(false);
            HideScoreboard();
            ShowGameOverPopUp();
        }

        public void EnableView()
        {
            gameObject.SetActive(true);
        }

        public void DisableView()
        {
            gameObject.SetActive(false);
        }

        public void ShowScoreboard()
        {
            _scoreboardUI.SetActive(true);
        }

        public void HideScoreboard()
        {
            _scoreboardUI.SetActive(false);
        }

        private void HandleCountdownTick(object[] parameters)
        {
            int secondsRemaining = (int)parameters[0];

            if (secondsRemaining > 0)
            {
                _gameOverCountdownText.text = "Returning To Main Menu In... " +secondsRemaining.ToString() +"s";
                _currentCountdownValue = secondsRemaining;
            }
        }

        public Transform GetScoreboardContentTransform()
        {
            return _scoreboardContentTransform;
        }

        private void ShowDisconnectionGameOverUI(ulong clientID)
        {
            if ((NetworkManager.Singleton.IsServer && clientID == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.ConnectedClients.Count <= 1) || !NetworkManager.Singleton.IsServer)
            {
                _disconnectedGameOverUIPopUp.SetActive(true);
                StartCoroutine(DisconnectedCountdownSequence());
            }
        }

        private IEnumerator DisconnectedCountdownSequence()
        {
            float currentTime = _disconnectedCountdownTime;

            while (currentTime > 0)
            {
                _disconnectedCountdownGameOverUIText.text = "Returning To Main Menu In... " + Mathf.Ceil(currentTime).ToString() + "s";
                currentTime -= 1f;
                yield return new WaitForSeconds(1f);
            }

            SceneLoader.Instance.LoadScene(SceneName.StartScene, false);
        }

        private void HandleGameOverScoreCard(object[] parameters)
        {
            HideGameOverPopUp();
            ShowScoreboard();
        }

        public void ShowGameOverPopUp()
        {
            _gameOverPopUp.SetActive(true);
        }

        public void HideGameOverPopUp()
        {
            _gameOverPopUp.SetActive(false);
        }

    }
}
