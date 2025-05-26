using BattleRoyale.Event;
using BattleRoyale.Level;
using BattleRoyale.Player;
using BattleRoyale.Scene;
using BattleRoyale.UI;
using System.Collections.Generic;
using Unity.Netcode;
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
            RegisterGameplayServices();
            _gameplayUIObj = GameManager.Instance.Get<GameplayUIService>();
            _levelObj = GameManager.Instance.Get<LevelService>();
            _playerObj = GameManager.Instance.Get<PlayerService>();             

            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                _levelObj.StartLevel(NetworkManager.Singleton.ConnectedClientsIds.Count);
                HandlePlayerSpawn();
            }
        }

        private void HandlePlayerSpawn()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                List<Vector3> spawnPoints = _levelObj.GetPlayerSpawnPoints();

                int spawnIndex = 0;
                foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    _playerObj.SpawnPlayer(clientId, spawnPoints[spawnIndex]);
                    spawnIndex++;
                }

                GameplayManager.Instance.NotifyClientsToShowGameplayUIClientRpc();
                GameplayManager.Instance.StartCountdown(GameManager.Instance.ui_SO.countDownDuration);
            }
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
