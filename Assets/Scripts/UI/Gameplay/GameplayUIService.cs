using BattleRoyale.EventModule;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.UIModule
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
            EventBusManager.Instance.Subscribe(EventName.UpdateEliminationCount, HandleEliminationCountUpdate);
            EventBusManager.Instance.Subscribe(EventName.PlayerEliminated, HandleEliminatedPopup);
            EventBusManager.Instance.Subscribe(EventName.PlayerAssignedRank, HandlePlayerRankAssigned);
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventName.PlayerSpawnCompleted, HandleGameplayUI);
            EventBusManager.Instance.Unsubscribe(EventName.UpdateEliminationCount, HandleEliminationCountUpdate);
            EventBusManager.Instance.Unsubscribe(EventName.PlayerEliminated, HandleEliminatedPopup);
            EventBusManager.Instance.Unsubscribe(EventName.PlayerAssignedRank, HandlePlayerRankAssigned);
        }

        private void HandleGameplayUI(object[] parameters)
        {
            ShowUI();
            int totalPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count;
            SetTotalPlayers(totalPlayers);
        }

        private void HandleEliminationCountUpdate(object[] parameters)
        {
            if (parameters.Length > 0 && parameters[0] is int count)
            {
                _gameplayUIView.UpdateEliminatedCount(count);
            }
        }

        private void HandleEliminatedPopup(object[] parameters)
        {
            _gameplayUIView.ShowEliminatedPopup();
        }

        private void HandlePlayerRankAssigned(object[] parameters)
        {
            int rank = (int)parameters[0];
            _gameplayUIView.UpdatePlayerRank(rank);
        }

        public void ShowUI()
        {
            _gameplayUIView.EnableView();
        }

        public void HideUI()
        {
            _gameplayUIView.DisableView();
        }

        public void SetTotalPlayers(int total) => _gameplayUIView.SetTotalPlayers(total);

        public void Dispose()
        {
            UnsubscribeToEvents();
            Object.Destroy(_gameplayUIView.gameObject);
            _gameplayUIView = null;
        }
    }
}