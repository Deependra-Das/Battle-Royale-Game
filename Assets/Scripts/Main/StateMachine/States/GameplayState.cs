using BattleRoyale.Event;
using BattleRoyale.Level;
using BattleRoyale.Network;
using BattleRoyale.Player;
using BattleRoyale.Scene;
using BattleRoyale.UI;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleRoyale.Main
{
    public class GameplayState : IGameState
    {
        private LevelService _levelObj;
        private PlayerService _playerObj;
        private GameplayUIService _gameplayUIObj;
        private NetworkObject _gameplayManagerNetworkObj;

        public void Enter()
        {
            RegisterGameplayServices();
            _gameplayUIObj = GameManager.Instance.Get<GameplayUIService>();
            _levelObj = GameManager.Instance.Get<LevelService>();
            _playerObj = GameManager.Instance.Get<PlayerService>();

            if (NetworkManager.Singleton.IsServer)
            {
                SpawnGameplayManager();
            }
        }

        private void SpawnGameplayManager()
        {
            if (GameplayManager.Instance != null) return;

            GameObject _gameplayMngrObj = UnityEngine.Object.Instantiate(GameManager.Instance.network_SO.gameplayManagerPrefab.gameObject);
            _gameplayMngrObj.name = "GameplayManager";
            _gameplayManagerNetworkObj = _gameplayMngrObj.GetComponent<NetworkObject>();
            _gameplayManagerNetworkObj.Spawn(true);
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

            if(_gameplayManagerNetworkObj.IsSpawned)
            {
                _gameplayManagerNetworkObj.Despawn();
                UnityEngine.Object.Destroy(_gameplayManagerNetworkObj.gameObject);
            }     

            string activeSceneName = SceneManager.GetActiveScene().name.ToString();
            Enum.TryParse<SceneName>(activeSceneName, out var sceneEnumValue);

            if (sceneEnumValue == SceneName.StartScene)
            {
                CleanupNetworkResources();
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

        private void CleanupNetworkResources()
        {
            if (PlayerSessionManager.Instance != null)
            {
                if (PlayerSessionManager.Instance.NetworkObject.IsSpawned)
                {
                    PlayerSessionManager.Instance.NetworkObject.Despawn();
                }

                UnityEngine.Object.Destroy(PlayerSessionManager.Instance.gameObject);
            }

            if (MultiplayerManager.Instance != null)
            {
                if (MultiplayerManager.Instance.NetworkObject.IsSpawned)
                {
                    MultiplayerManager.Instance.NetworkObject.Despawn();
                }

                UnityEngine.Object.Destroy(MultiplayerManager.Instance.gameObject);
            }

            if (NetworkManager.Singleton!= null)
            {
                NetworkManager.Singleton.Shutdown();
                UnityEngine.Object.Destroy(NetworkManager.Singleton.gameObject);
            }
        }
    }
}
