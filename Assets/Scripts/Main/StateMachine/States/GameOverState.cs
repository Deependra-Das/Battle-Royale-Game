using BattleRoyale.AudioModule;
using BattleRoyale.NetworkModule;
using BattleRoyale.UIModule;
using BattleRoyale.XPModule;
using Unity.Netcode;

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
            AudioManager.Instance.PlayBGM(AudioModule.AudioType.GameOverBGM);
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
            ServiceLocator.Register(new XPService(GameManager.Instance.xpMileStone_SO.xpMilestones));
            GameOverUIView gameOverUIPrefab = GameManager.Instance.ui_SO.gameOverUIPrefab;
            ScoreboardEntryUIView scoreboardEntryUIPrefab = GameManager.Instance.ui_SO.scoreboardEntryUIPrefab;
            ServiceLocator.Register(new GameOverUIService(gameOverUIPrefab, scoreboardEntryUIPrefab));
        }

        private void UnegisterGameOverServices()
        {
            ServiceLocator.Unregister<GameOverUIService>();
            ServiceLocator.Unregister<XPService>();
        }
    }
}
