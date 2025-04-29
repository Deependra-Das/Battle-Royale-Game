using BattleRoyale.Event;
using BattleRoyale.Level;
using BattleRoyale.Player;
using BattleRoyale.Scene;
using BattleRoyale.UI;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class GameplayState : IGameState
    {
        private LevelService _levelObj;
        private PlayerService _playerObj;
        private GameplayUIService _gameplayUIObj;

        public void Enter()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneName.GameScene);
            EventBusManager.Instance.Subscribe(EventName.GameplaySceneLoadedEvent, HandleGameplayState);
        }

        private void HandleGameplayState(object[] parameters)
        {
            RegisterGameplayServices();

            _levelObj = GameManager.Instance.Get<LevelService>();
            _playerObj = GameManager.Instance.Get<PlayerService>();
            _gameplayUIObj = GameManager.Instance.Get<GameplayUIService>();

            _levelObj.StartLevel();
            List<Vector3> spawnPoints = _levelObj.GetPlayerSpawnPoints();
            _playerObj.SpawnPlayer(spawnPoints);
            _gameplayUIObj.ShowUI();
        }

        public void Exit()
        {
            EventBusManager.Instance.Unsubscribe(EventName.GameplaySceneLoadedEvent, HandleGameplayState);
            Cleanup();
            UnegisterGameplayServices();
        }

        public void Cleanup()
        {
            _playerObj.Dispose();
            _levelObj.Dispose();
            _gameplayUIObj.Dispose();
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
