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

        ~GameplayUIService() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            EventBusManager.Instance.Subscribe(EventName.PlayerSpawnCompleted, OnPlayerSpawnCompleted);
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventName.PlayerSpawnCompleted, OnPlayerSpawnCompleted);
        }

        private void OnPlayerSpawnCompleted(object[] parameters)
        {
            ShowUI();
            _gameplayUIView.StartCoundown();
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
            Object.Destroy(_gameplayUIView.gameObject);
            _gameplayUIView = null;
        }
    }
}