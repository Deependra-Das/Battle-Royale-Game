using BattleRoyale.Event;
using UnityEngine;

namespace BattleRoyale.UI
{
    public class GameplayUIService
    {
        private GameplayUIView _gameplayUIView;

        public GameplayUIService(GameplayUIView GameplayUIPrefab)
        {
            Transform canvasTransform = CanvasUIManager.Instance.canvasTransform;
            _gameplayUIView = Object.Instantiate(GameplayUIPrefab, canvasTransform);
            HideUI();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            EventBusManager.Instance.Subscribe(EventName.PlayerSpawnCompleted, HandleGameplayUI);
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventName.PlayerSpawnCompleted, HandleGameplayUI);
        }

        private void HandleGameplayUI(object[] parameters)
        {
            ShowUI();
            EventBusManager.Instance.RaiseNoParams(EventName.StartGameplayCountdown);       
        }

        public void ShowUI()
        {
            _gameplayUIView.EnableView();
        }

        public void HideUI()
        {
            _gameplayUIView.DisableView();
        }

        public void Dispose()
        {
            UnsubscribeToEvents();
            Object.Destroy(_gameplayUIView.gameObject);
            _gameplayUIView = null;
        }
    }
}