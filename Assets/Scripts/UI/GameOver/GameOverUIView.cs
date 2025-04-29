using BattleRoyale.Main;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class GameOverUIView : MonoBehaviour
    {
        [SerializeField] private Button _backToStartMenuButtonPrefab;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _backToStartMenuButtonPrefab.onClick.AddListener(OnBackToStartMenuButtonClicked);
        }

        private void UnsubscribeToEvents()
        {
            _backToStartMenuButtonPrefab.onClick.RemoveListener(OnBackToStartMenuButtonClicked);
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
