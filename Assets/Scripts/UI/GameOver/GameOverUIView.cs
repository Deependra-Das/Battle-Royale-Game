using BattleRoyale.Event;
using BattleRoyale.Main;
using BattleRoyale.Scene;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class GameOverUIView : MonoBehaviour
    {
        [SerializeField] private GameObject _scoreboardUI;
        [SerializeField] private Transform _scoreboardContentTransform;
        [SerializeField] private TMP_Text _countdownText;

        [Header("GameOver PopUp")]
        [SerializeField] private GameObject _gameOverPopUp;

        [Header("Disconnected PopUp")]
        [SerializeField] private GameObject _disconnectedPopUp;
        [SerializeField] private Button _disconnectedBackButtonPrefab;

        private int _currentCountdownValue = -1;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            EventBusManager.Instance.Subscribe(EventName.GameOverCountdownTick, HandleCountdownTick);
            NetworkManager.Singleton.OnClientDisconnectCallback += ShowDisconnectionGameOverUI;
            _disconnectedBackButtonPrefab.onClick.AddListener(OnDisconnectedBackButtonClicked);
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventName.GameOverCountdownTick, HandleCountdownTick);
            NetworkManager.Singleton.OnClientDisconnectCallback -= ShowDisconnectionGameOverUI;
            _disconnectedBackButtonPrefab.onClick.AddListener(OnDisconnectedBackButtonClicked);
        }

        private void Start()
        {
            _disconnectedPopUp.SetActive(false);
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
                _countdownText.text = "Returning To Lobby In... " +secondsRemaining.ToString() +"s";
                _currentCountdownValue = secondsRemaining;
            }
        }

        public Transform GetScoreboardContentTransform()
        {
            return _scoreboardContentTransform;
        }

        private void ShowDisconnectionGameOverUI(ulong clientID)
        {
            _disconnectedPopUp.SetActive(true);
        }

        private void OnDisconnectedBackButtonClicked()
        {
            _disconnectedPopUp.SetActive(false);
            SceneLoader.Instance.LoadScene(SceneName.StartScene, false);
        }
    }
}
