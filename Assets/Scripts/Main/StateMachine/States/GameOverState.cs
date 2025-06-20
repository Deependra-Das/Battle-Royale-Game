using BattleRoyale.Event;
using BattleRoyale.Scene;
using BattleRoyale.UI;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class GameOverState : IGameState
    {
        private GameOverUIService _gameOverUIObj;

        public void Enter()
        {
            RegisterGameOverServices();
            _gameOverUIObj = GameManager.Instance.Get<GameOverUIService>();

            _gameOverUIObj.ShowUI();
        }

        public void Exit()
        {
            _gameOverUIObj.HideUI();
            Cleanup();
            UnegisterGameOverServices();
        }

        public void Cleanup()
        {
            _gameOverUIObj.Dispose();
        }

        private void RegisterGameOverServices()
        {
            GameOverUIView gameOverUIPrefab = GameManager.Instance.ui_SO.gameOverUIPrefab;
            ScoreboardEntryUIView scoreboardEntryUIPrefab = GameManager.Instance.ui_SO.scoreboardEntryUIPrefab;
            ServiceLocator.Register(new GameOverUIService(gameOverUIPrefab, scoreboardEntryUIPrefab));
        }

        private void UnegisterGameOverServices()
        {
            ServiceLocator.Unregister<GameOverUIService>();
        }
    }
}
