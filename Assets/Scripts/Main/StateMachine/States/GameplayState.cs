using BattleRoyale.Level;
using BattleRoyale.Player;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class GameplayState : IGameState
    {
        public void Enter()
        {
            SceneLoader.SceneLoader.Instance.LoadSceneAsync("GameScene");
            SceneLoader.SceneLoader.Instance.OnSceneLoaded += HandleGameplayState;
        }

        public void Exit()
        {
            SceneLoader.SceneLoader.Instance.OnSceneLoaded -= HandleGameplayState;
            UnegisterGameplayServices();
        }

        private void HandleGameplayState()
        {
            RegisterGameplayServices();

            LevelService levelObj = GameManager.Instance.Get<LevelService>();
            PlayerService playerObj = GameManager.Instance.Get<PlayerService>();

            levelObj.StartLevel();
            List<Vector3> spawnPoints = levelObj.GetPlayerSpawnPoints();
            playerObj.SpawnPlayer(spawnPoints);
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
