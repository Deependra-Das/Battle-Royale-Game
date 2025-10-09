using BattleRoyale.LevelModule;
using BattleRoyale.NetworkModule;
using BattleRoyale.PlayerModule;
using BattleRoyale.UIModule;
using Unity.Netcode;

namespace BattleRoyale.MainModule
{
    public class GameplayState : IGameState
    {
        private LevelService _levelObj;
        private PlayerService _playerObj;
        private GameplayUIService _gameplayUIObj;

        public void Enter()
        {
            RegisterGameplayServices();
            _gameplayUIObj = GameManager.Instance.Get<GameplayUIService>();
            _levelObj = GameManager.Instance.Get<LevelService>();
            _playerObj = GameManager.Instance.Get<PlayerService>();

            GameplayManager.Instance.Initialize();
        }

        public void Exit()
        {
            Cleanup();
            UnegisterGameplayServices();
        }

        public void Cleanup()
        {
            _playerObj.Dispose();
            _levelObj.Dispose();
            _gameplayUIObj.Dispose();

            if (GameplayManager.Instance != null)
            {
                if (GameplayManager.Instance.NetworkObject.IsSpawned && NetworkManager.Singleton.IsServer)
                {
                    GameplayManager.Instance.NetworkObject.Despawn();
                }
                UnityEngine.Object.Destroy(GameplayManager.Instance.gameObject);
            }
        }

        private void RegisterGameplayServices()
        {
            GameplayUIView gameplayUIPrefab = GameManager.Instance.ui_SO.gameplayUIPrefab;

            ServiceLocator.Register(new LevelService(GameManager.Instance.level_SO));
            ServiceLocator.Register(new PlayerService(GameManager.Instance.player_SO));
            ServiceLocator.Register(new GameplayUIService(gameplayUIPrefab));
        }

        private void UnegisterGameplayServices()
        {
            ServiceLocator.Unregister<LevelService>();
            ServiceLocator.Unregister<PlayerService>();
            ServiceLocator.Unregister<GameplayUIService>();
        }
    }
}
