using BattleRoyale.EventModule;
using BattleRoyale.LevelModule;
using BattleRoyale.LobbyModule;
using BattleRoyale.NetworkModule;
using BattleRoyale.PlayerModule;
using BattleRoyale.SceneModule;
using BattleRoyale.UIModule;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.MainModule
{
    public class StartState : IGameState
    {
        private StartMenuUIService _startMenuUIObj;

        public void Enter()
        {
            MainMenuCleanup();
            RegisterGameplayServices();

            _startMenuUIObj = GameManager.Instance.Get<StartMenuUIService>();
            _startMenuUIObj.ShowUI();
        }

        public void Exit()
        {
            Cleanup();
            UnegisterGameplayServices();
        }

        public void Cleanup()
        {
            _startMenuUIObj.Dispose();
        }

        private void RegisterGameplayServices()
        {
            StartMenuUIView startMenuUIPrefab = GameManager.Instance.ui_SO.startMenuUIPrefab;
            ServiceLocator.Register(new StartMenuUIService(startMenuUIPrefab));
        }

        private void UnegisterGameplayServices()
        {
            ServiceLocator.Unregister<StartMenuUIService>();
        }

        private void MainMenuCleanup()
        {

            if (LobbyManager.Instance != null)
            {
                UnityEngine.Object.Destroy(LobbyManager.Instance.gameObject);
            }

            if (PlayerSessionManager.Instance != null)
            {
                if (PlayerSessionManager.Instance.NetworkObject.IsSpawned && NetworkManager.Singleton.IsServer)
                {
                    PlayerSessionManager.Instance.NetworkObject.Despawn();
                }
                    UnityEngine.Object.Destroy(PlayerSessionManager.Instance.gameObject);
            }

            if (MultiplayerManager.Instance != null)
            {
                if (MultiplayerManager.Instance.NetworkObject.IsSpawned && NetworkManager.Singleton.IsServer)
                {
                    MultiplayerManager.Instance.NetworkObject.Despawn();
                }
                    UnityEngine.Object.Destroy(MultiplayerManager.Instance.gameObject);
            }

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
                UnityEngine.Object.Destroy(NetworkManager.Singleton.gameObject);
            }
        }
    }
}
