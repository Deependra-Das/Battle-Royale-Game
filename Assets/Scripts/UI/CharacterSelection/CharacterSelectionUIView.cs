using BattleRoyale.Main;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class CharacterSelectionUIView : MonoBehaviour
    {
        [SerializeField] private Button _readyButtonPrefab;
        [SerializeField] private Button _backToStartMenuButtonPrefab;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _readyButtonPrefab.onClick.AddListener(OnReadyButtonClicked);
            _backToStartMenuButtonPrefab.onClick.AddListener(OnBackToStartMenuButtonClicked);
        }

        private void UnsubscribeToEvents()
        {
            _readyButtonPrefab.onClick.RemoveListener(OnReadyButtonClicked);
            _backToStartMenuButtonPrefab.onClick.RemoveListener(OnBackToStartMenuButtonClicked);
        }

        private void OnReadyButtonClicked()
        {
            PlayerStateManager.Instance.SetPlayerReady();
        }

        private void OnBackToStartMenuButtonClicked()
        {
         
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
