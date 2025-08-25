using BattleRoyale.CharacterSelection;
using BattleRoyale.Event;
using BattleRoyale.Scene;
using BattleRoyale.UI;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class CharacterSelectionState : IGameState
    {
        private CharacterSelectionUIService _characterSelectionUIObj;
        private CharacterSpawnService _characterSpawnerObj;

        public void Enter()
        {    
            RegisterCharacterSelectionServices();
            _characterSelectionUIObj = GameManager.Instance.Get<CharacterSelectionUIService>();
            _characterSpawnerObj = GameManager.Instance.Get<CharacterSpawnService>();
            _characterSelectionUIObj.ShowUI();
        }

        public void Exit()
        {
            _characterSelectionUIObj.HideUI();
            _characterSpawnerObj.DespawnAllSpawnedCharacters();
            Cleanup();
            UnegisterCharacterSelectionServices();
        }

        public void Cleanup()
        {
            _characterSelectionUIObj.Dispose();
        }

        private void RegisterCharacterSelectionServices()
        {
            CharacterSelectionUIView characterSelectionUIPrefab = GameManager.Instance.ui_SO.characterSelectionUIPrefab;
            CharacterScriptableObject characterSpawnData = GameManager.Instance.character_SO;
            
            ServiceLocator.Register(new CharacterSelectionUIService(characterSelectionUIPrefab));
            ServiceLocator.Register(new CharacterSpawnService(characterSpawnData));
        }

        private void UnegisterCharacterSelectionServices()
        {
            ServiceLocator.Unregister<CharacterSelectionUIService>();
            ServiceLocator.Unregister<CharacterSpawnService>();
        }
    }
}
