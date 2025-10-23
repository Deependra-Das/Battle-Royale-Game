using BattleRoyale.AudioModule;
using BattleRoyale.EnvironmentModule;
using BattleRoyale.LobbyModule;
using BattleRoyale.NetworkModule;
using BattleRoyale.UIModule;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.MainModule
{
    public class StartState : IGameState
    {
        private StartMenuUIService _startMenuUIObj;
        private SkyboxService _skyboxObj;

        public void Enter()
        {
            MainMenuCleanup();
            RegisterGameplayServices();

            _skyboxObj = GameManager.Instance.Get<SkyboxService>();
            _skyboxObj.ApplySkyboxMaterialByType(SkyboxType.Day);

            _startMenuUIObj = GameManager.Instance.Get<StartMenuUIService>();
            _startMenuUIObj.ShowUI();
            AudioManager.Instance.PlayBGM(AudioModule.AudioType.MainMenuBGM);
        }

        public void Exit()
        {
            Cleanup();
            UnegisterGameplayServices();
        }

        public void Cleanup()
        {
            _startMenuUIObj.Dispose();
            _skyboxObj.Dispose();
        }

        private void RegisterGameplayServices()
        {
            StartMenuUIView startMenuUIPrefab = GameManager.Instance.ui_SO.startMenuUIPrefab;
            List<Sprite> galleryImages = GameManager.Instance.ui_SO.galleryImages;
            ServiceLocator.Register(new StartMenuUIService(startMenuUIPrefab, galleryImages));
            ServiceLocator.Register(new SkyboxService(GameManager.Instance.environment_SO.skyboxTypeMaterialMappings));
        }

        private void UnegisterGameplayServices()
        {
            ServiceLocator.Unregister<StartMenuUIService>();
            ServiceLocator.Unregister<SkyboxService>();
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
