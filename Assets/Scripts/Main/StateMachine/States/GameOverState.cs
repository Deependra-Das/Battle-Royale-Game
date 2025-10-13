using BattleRoyale.NetworkModule;
using BattleRoyale.UIModule;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.MainModule
{
    public class GameOverState : IGameState
    {
        private GameOverUIService _gameOverUIObj;

        public void Enter()
        {
            RegisterGameOverServices();
            _gameOverUIObj = GameManager.Instance.Get<GameOverUIService>();
            _gameOverUIObj.ShowUI();
            GameOverManager.Instance.Initialize();
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

            if (GameOverManager.Instance != null)
            {
                if (GameOverManager.Instance.NetworkObject.IsSpawned && NetworkManager.Singleton.IsServer)
                {
                    GameOverManager.Instance.NetworkObject.Despawn();
                }
                UnityEngine.Object.Destroy(GameOverManager.Instance.gameObject);
            }
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
