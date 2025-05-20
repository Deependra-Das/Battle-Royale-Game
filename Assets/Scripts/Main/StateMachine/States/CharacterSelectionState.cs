using BattleRoyale.Event;
using BattleRoyale.Scene;
using BattleRoyale.UI;
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
        }

        public void Exit()
        {
            _characterSelectionUIObj.HideUI();
            Cleanup();
            UnegisterCharacterSelectionServices();
        }

        public void Cleanup()
        {
            _characterSelectionUIObj.Dispose();
        }

        private void RegisterCharacterSelectionServices()
        {
            CharacterSelectionUIView CharacterSelectionUIPrefab = GameManager.Instance.ui_SO.characterSelectionUIPrefab;
            ServiceLocator.Register(new CharacterSelectionUIService(CharacterSelectionUIPrefab));
        }

        private void UnegisterCharacterSelectionServices()
        {
            ServiceLocator.Unregister<CharacterSelectionUIService>();
        }
    }
}
