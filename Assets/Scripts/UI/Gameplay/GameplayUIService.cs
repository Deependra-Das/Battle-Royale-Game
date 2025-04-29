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