using BattleRoyale.Event;
using BattleRoyale.Level;
using BattleRoyale.Player;
using BattleRoyale.Scene;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class GameplayState : IGameState
    {
        LevelService levelObj;
        PlayerService playerObj;

        public void Enter()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneName.GameScene);
            EventBusManager.Instance.Subscribe(EventName.GameplaySceneLoadedEvent, HandleGameplayState);
        }

        private void HandleGameplayState(object[] parameters)
        {
            RegisterGameplayServices();

            levelObj = GameManager.Instance.Get<LevelService>();
            playerObj = GameManager.Instance.Get<PlayerService>();

            levelObj.StartLevel();
            List<Vector3> spawnPoints = levelObj.GetPlayerSpawnPoints();
            playerObj.SpawnPlayer(spawnPoints);
        }

        public void Exit()
        {
            EventBusManager.Instance.Unsubscribe(EventName.GameplaySceneLoadedEvent, HandleGameplayState);
            Cleanup();
            UnegisterGameplayServices();
        }

        public void Cleanup()
        {
            playerObj.Dispose();
            levelObj.Dispose();
        }

        private void RegisterGameplayServices()
        {
            ServiceLocator.Register(new LevelService(GameManager.Instance.level_SO));
            ServiceLocator.Register(new PlayerService(GameManager.Instance.player_SO));
        }

        private void UnegisterGameplayServices()
        {
            ServiceLocator.Unregister<LevelService>();
            ServiceLocator.Unregister<PlayerService>();
        }
    }
}
