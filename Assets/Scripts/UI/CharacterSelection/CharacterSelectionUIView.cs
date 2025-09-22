using BattleRoyale.Event;
using BattleRoyale.Main;
using BattleRoyale.Network;
using BattleRoyale.Scene;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class CharacterSelectionUIView : MonoBehaviour
    {
        [SerializeField] private Button _readyButtonPrefab;
        [SerializeField] private Button _notReadyButtonPrefab;
        [SerializeField] private Button _backToStartMenuButtonPrefab;
        [SerializeField] private GameObject _hostDisconnectedPanel;
        [SerializeField] private GameObject _clientDisconnectedPanel;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _readyButtonPrefab.onClick.AddListener(OnReadyButtonClicked);
            _notReadyButtonPrefab.onClick.AddListener(OnNotReadyButtonClicked);
            _backToStartMenuButtonPrefab.onClick.AddListener(OnBackToStartMenuButtonClicked);
            EventBusManager.Instance.Subscribe(EventName.HostDisconnected, HandleHostDisconnectCharSelectionUI);
        }

        private void UnsubscribeToEvents()
        {
            _readyButtonPrefab.onClick.RemoveListener(OnReadyButtonClicked);
            _notReadyButtonPrefab.onClick.RemoveListener(OnNotReadyButtonClicked);
            _backToStartMenuButtonPrefab.onClick.RemoveListener(OnBackToStartMenuButtonClicked);
            EventBusManager.Instance.Unsubscribe(EventName.HostDisconnected, HandleHostDisconnectCharSelectionUI);
        }

        private void Start()
        {
            _hostDisconnectedPanel.SetActive(false);
            _notReadyButtonPrefab.gameObject.SetActive(false);
            _readyButtonPrefab.gameObject.SetActive(true);
        }

        private void OnReadyButtonClicked()
        {
            PlayerLobbyStateManager.Instance.SetPlayerReady();
            _readyButtonPrefab.gameObject.SetActive(false);
            _notReadyButtonPrefab.gameObject.SetActive(true);
        }

        private void OnNotReadyButtonClicked()
        {
            PlayerLobbyStateManager.Instance.SetPlayerNotReady();
            _notReadyButtonPrefab.gameObject.SetActive(false);
            _readyButtonPrefab.gameObject.SetActive(true);
        }

        private void OnBackToStartMenuButtonClicked()
        {
            NetworkManager.Singleton.Shutdown();
            SceneLoader.Instance.LoadScene(SceneName.StartScene, false);
        }

        public void EnableView()
        {
            gameObject.SetActive(true);
        }

        public void DisableView()
        {
            gameObject.SetActive(false);
        }

        private void HandleHostDisconnectCharSelectionUI(object[] parameters)
        {
            _hostDisconnectedPanel.SetActive(true);
        }
    }
}
