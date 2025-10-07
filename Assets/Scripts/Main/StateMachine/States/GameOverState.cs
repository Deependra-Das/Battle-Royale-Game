using BattleRoyale.EventModule;
using BattleRoyale.SceneModule;
using BattleRoyale.NetworkModule;
using BattleRoyale.UIModule;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.MainModule
{
    public class GameOverState : IGameState
    {
        private GameOverUIService _gameOverUIObj;
        private NetworkObject _gameOverManagerNetworkObj;

        public void Enter()
        {
            RegisterGameOverServices();
            _gameOverUIObj = GameManager.Instance.Get<GameOverUIService>();
            _gameOverUIObj.ShowUI();

            if (NetworkManager.Singleton.IsServer)
            {
                SpawnGameOverManager();
            }
        }

        private void SpawnGameOverManager()
        {
            if (GameplayManager.Instance != null) return;

            GameObject _gameOverMngrObj = UnityEngine.Object.Instantiate(GameManager.Instance.network_SO.gameOverManagerPrefab.gameObject);
            _gameOverMngrObj.name = "GameOverManager";
            _gameOverManagerNetworkObj = _gameOverMngrObj.GetComponent<NetworkObject>();
            _gameOverManagerNetworkObj.Spawn(true);
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

            if (_gameOverManagerNetworkObj!=null && _gameOverManagerNetworkObj.IsSpawned)
            {
                _gameOverManagerNetworkObj.Despawn();
                UnityEngine.Object.Destroy(_gameOverManagerNetworkObj.gameObject);
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
