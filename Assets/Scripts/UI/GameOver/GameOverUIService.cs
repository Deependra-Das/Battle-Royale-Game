using UnityEngine;

namespace BattleRoyale.UI
{
    public class GameOverUIService
    {
        private GameOverUIView _gameOverUIView;

        public GameOverUIService(GameOverUIView GameOverUIPrefab)
        {
            Transform canvasTransform = CanvasUIManager.Instance.canvasTransform;
            _gameOverUIView = Object.Instantiate(GameOverUIPrefab, canvasTransform);
            HideUI();
        }

        public void ShowUI()
        {
            _gameOverUIView.EnableView();
        }

        public void HideUI()
        {
            _gameOverUIView.DisableView();
        }

        public void Dispose()
        {
            Object.Destroy(_gameOverUIView.gameObject);
            _gameOverUIView = null;
        }
    }
}
