using BattleRoyale.CharacterSelectionModule;
using BattleRoyale.EventModule;
using BattleRoyale.NetworkModule;
using BattleRoyale.SceneModule;
using BattleRoyale.UIModule;
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleRoyale.MainModule
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
        }

        public void Exit()
        {
            _characterSelectionUIObj.HideUI();
            Cleanup();
            UnegisterCharacterSelectionServices();
        }

        public void Cleanup()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                CharacterManager.Instance.DespawnAllSpawnedCharacters();
            }

            _characterSelectionUIObj.Dispose();
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
    }
}
