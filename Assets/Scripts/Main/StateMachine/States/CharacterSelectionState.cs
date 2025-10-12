using BattleRoyale.CharacterSelectionModule;
using BattleRoyale.EnvironmentModule;
using BattleRoyale.UIModule;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.MainModule
{
    public class CharacterSelectionState : IGameState
    {
        private CharacterSelectionUIService _characterSelectionUIObj;
        private SkyboxService _skyboxObj;

        public void Enter()
        {    
            RegisterCharacterSelectionServices();

            _skyboxObj = GameManager.Instance.Get<SkyboxService>();
            _skyboxObj.ApplySkyboxMaterialByType(SkyboxType.Night);

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
            _skyboxObj.Dispose();
        }

        private void RegisterCharacterSelectionServices()
        {
            CharacterSelectionUIView characterSelectionUIPrefab = GameManager.Instance.ui_SO.characterSelectionUIPrefab;            
            ServiceLocator.Register(new CharacterSelectionUIService(characterSelectionUIPrefab));
            ServiceLocator.Register(new SkyboxService(GameManager.Instance.environment_SO.skyboxTypeMaterialMappings));
        }

        private void UnegisterCharacterSelectionServices()
        {
            ServiceLocator.Unregister<CharacterSelectionUIService>();
            ServiceLocator.Unregister<SkyboxService>();
        }
    }
}
