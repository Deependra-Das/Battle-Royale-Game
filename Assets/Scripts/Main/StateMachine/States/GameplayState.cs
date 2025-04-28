using BattleRoyale.Level;
using BattleRoyale.Player;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class GameplayState : IGameState
    {
        LevelService levelObj;
        PlayerService playerObj;

        public void Enter()
        {
            SceneLoader.SceneLoader.Instance.LoadSceneAsync("GameScene");
            SceneLoader.SceneLoader.Instance.OnSceneLoaded += HandleGameplayState;
        }

        private void HandleGameplayState()
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
            SceneLoader.SceneLoader.Instance.OnSceneLoaded -= HandleGameplayState;
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
            ServiceLocator.Register(new LevelService(GameManager.Instance._level_SO));
            ServiceLocator.Register(new PlayerService(GameManager.Instance._player_SO));
        }

        private void UnegisterGameplayServices()
        {
            ServiceLocator.Unregister<LevelService>();
            ServiceLocator.Unregister<PlayerService>();
        }
    }
}
