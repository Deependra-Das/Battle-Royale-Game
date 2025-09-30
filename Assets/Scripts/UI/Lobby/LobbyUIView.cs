using BattleRoyale.Main;
using BattleRoyale.Scene;
using BattleRoyale.Network;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class LobbyUIView : MonoBehaviour
    {
        [SerializeField] private Button _createLobbyButtonPrefab;
        [SerializeField] private Button _quickJoinButtonPrefab;
        [SerializeField] private Button _backToStartMenuButtonPrefab;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _createLobbyButtonPrefab.onClick.AddListener(OnCreateLobbyButtonClicked);
            _quickJoinButtonPrefab.onClick.AddListener(OnQuickJoinButtonClicked);
            _backToStartMenuButtonPrefab.onClick.AddListener(OnBackToStartMenuButtonClicked);
        }

        private void UnsubscribeToEvents()
        {
            _createLobbyButtonPrefab.onClick.RemoveListener(OnCreateLobbyButtonClicked);
            _quickJoinButtonPrefab.onClick.RemoveListener(OnQuickJoinButtonClicked);
            _backToStartMenuButtonPrefab.onClick.RemoveListener(OnBackToStartMenuButtonClicked);
        }

        private void OnCreateLobbyButtonClicked()
        {
            LobbyManager.Instance.CreateLobby("LobbyName", false);
        }

        private void OnQuickJoinButtonClicked()
        {
            LobbyManager.Instance.QuickJoin();
        }

        private void OnBackToStartMenuButtonClicked()
        {
            GameManager.Instance.ChangeGameState(GameState.Start);
        }

        public void EnableView()
        {
            gameObject.SetActive(true);
        }

        public void DisableView()
        {
            gameObject.SetActive(false);
        }
    }
}
