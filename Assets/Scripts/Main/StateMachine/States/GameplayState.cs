using BattleRoyale.Level;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class GameplayState : IGameState
    {
        public void Enter()
        {
            Debug.Log("Gameplay Enter");
            RegisterGameplayServices();
            GameManager.Instance.Get<LevelService>().StartLevel(); //For Testing
        }

        public void Exit()
        {
            UnegisterGameplayServices();
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
