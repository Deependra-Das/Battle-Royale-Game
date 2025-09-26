using BattleRoyale.CharacterSelection;
using BattleRoyale.Event;
using BattleRoyale.Network;
using BattleRoyale.Scene;
using BattleRoyale.UI;
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleRoyale.Main
{
    public class CharacterSelectionState : IGameState
    {
        private CharacterSelectionUIService _characterSelectionUIObj;

        public void Enter()
        {    
            RegisterCharacterSelectionServices();
            _characterSelectionUIObj = GameManager.Instance.Get<CharacterSelectionUIService>();
            _characterSelectionUIObj.ShowUI();

            if (NetworkManager.Singleton.IsServer)
            {
                SpawnCharacterManager();
            }
        }

        private void SpawnCharacterManager()
        {
            if (CharacterManager.Instance != null) return;

            GameObject managerObj = UnityEngine.Object.Instantiate(GameManager.Instance.network_SO.characterManagerPrefab.gameObject);
            managerObj.name = "CharacterManager";
            managerObj.GetComponent<NetworkObject>().Spawn(true);

            var totalPlayers = PlayerSessionManager.Instance.GetAllPlayerSessionData();
            foreach (var entry in totalPlayers)
            {
                Debug.Log($"Client ID: {entry.Key}, Username: {entry.Value.ConnectionStatus}");
            }
        }

        public void Exit()
        {
            _characterSelectionUIObj.HideUI();
            Cleanup();
            UnegisterCharacterSelectionServices();
        }

        public void Cleanup()
        {
            CharacterManager.Instance.DespawnAllSpawnedCharacters();
            _characterSelectionUIObj.Dispose();

            string activeSceneName = SceneManager.GetActiveScene().name.ToString();
            Enum.TryParse<SceneName>(activeSceneName, out var sceneEnumValue);

            Debug.Log(activeSceneName);
            if (sceneEnumValue == SceneName.StartScene)
            {
                CleanupNetworkResources();
            }
        }

        private void RegisterCharacterSelectionServices()
        {
            CharacterSelectionUIView characterSelectionUIPrefab = GameManager.Instance.ui_SO.characterSelectionUIPrefab;            
            ServiceLocator.Register(new CharacterSelectionUIService(characterSelectionUIPrefab));
        }

        private void UnegisterCharacterSelectionServices()
        {
            ServiceLocator.Unregister<CharacterSelectionUIService>();
        }

        private void CleanupNetworkResources()
        {
            if (PlayerSessionManager.Instance != null)
            {
                PlayerSessionManager.Instance.NetworkObject.Despawn();
                UnityEngine.Object.Destroy(PlayerSessionManager.Instance.gameObject);
            }

            if (MultiplayerManager.Instance != null)
            {
                MultiplayerManager.Instance.NetworkObject.Despawn();
                UnityEngine.Object.Destroy(MultiplayerManager.Instance.gameObject);
            }

            if (NetworkManager.Singleton.IsListening)
            {
                PrintNetworkObjects();
                NetworkManager.Singleton.Shutdown();
                UnityEngine.Object.Destroy(NetworkManager.Singleton.gameObject);
            }

            Debug.Log("Lobby resources cleaned up.");
        }

        public void PrintNetworkObjects()
        {
            if (NetworkManager.Singleton != null)
            {
                foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
                {
                    if (networkObject.Value != null)
                    {
                        Debug.Log($"NetworkObject: {networkObject.Value.name} (ID: {networkObject.Key})");
                    }
                }
            }
            else
            {
                Debug.LogWarning("NetworkManager is not initialized.");
            }
        }
    }
}
