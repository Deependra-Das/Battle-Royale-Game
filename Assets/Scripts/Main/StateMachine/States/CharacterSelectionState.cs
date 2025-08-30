using BattleRoyale.CharacterSelection;
using BattleRoyale.Event;
using BattleRoyale.Network;
using BattleRoyale.Scene;
using BattleRoyale.UI;
using Unity.Netcode;
using UnityEngine;

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

            GameObject managerObj = Object.Instantiate(GameManager.Instance.network_SO.characterManagerPrefab.gameObject);
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
            CharacterManager.Instance.DespawnAllSpawnedCharacters();
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
