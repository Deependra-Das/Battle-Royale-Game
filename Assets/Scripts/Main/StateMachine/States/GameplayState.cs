using BattleRoyale.Level;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class GameplayState : IGameState
    {
        public void Enter()
        {
            Debug.Log("Gameplay Enter");
            SceneLoader.SceneLoader.Instance.LoadSceneAsync("GameScene");
            SceneLoader.SceneLoader.Instance.OnSceneLoaded += HandleGameplayState;
        }

        public void Exit()
        {
            UnegisterGameplayServices();
        }

        private void HandleGameplayState()
        {
            Debug.Log("Gameplay Services");
            RegisterGameplayServices();
            GameManager.Instance.Get<LevelService>().StartLevel(); //For Testing
        }

        private void RegisterGameplayServices()
        {
            ServiceLocator.Register(new LevelService(GameManager.Instance._level_SO));
        }

        private void UnegisterGameplayServices()
        {
            ServiceLocator.Unregister<LevelService>();
        }
    }
}
