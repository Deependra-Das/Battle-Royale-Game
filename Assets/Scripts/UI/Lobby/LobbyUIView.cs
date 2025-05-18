using BattleRoyale.Main;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class LobbyUIView : MonoBehaviour
    {
        [SerializeField] private Button _createGameButtonPrefab;
        [SerializeField] private Button _joinGameButtonPrefab;
        [SerializeField] private Button _backToStartMenuButtonPrefab;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _createGameButtonPrefab.onClick.AddListener(OnCreateGameButtonClicked);
            _joinGameButtonPrefab.onClick.AddListener(OnJoinGameButtonClicked);
            _backToStartMenuButtonPrefab.onClick.AddListener(OnBackToStartMenuButtonClicked);
        }

        private void UnsubscribeToEvents()
        {
            _createGameButtonPrefab.onClick.RemoveListener(OnCreateGameButtonClicked);
            _joinGameButtonPrefab.onClick.RemoveListener(OnJoinGameButtonClicked);
            _backToStartMenuButtonPrefab.onClick.RemoveListener(OnBackToStartMenuButtonClicked);
        }

        private void OnCreateGameButtonClicked()
        {
            //GameManager.Instance.ChangeGameState(GameState.Start);
        }

        private void OnJoinGameButtonClicked()
        {
            //GameManager.Instance.ChangeGameState(GameState.Start);
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
