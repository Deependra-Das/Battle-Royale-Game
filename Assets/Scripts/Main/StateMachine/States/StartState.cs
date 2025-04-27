using UnityEngine;

namespace BattleRoyale.Main
{
    public class StartState : IGameState
    {
        public void Enter()
        {
            Debug.Log("Start Enter");
        }

        public void Exit()
        {
            Debug.Log("Start Exit");
        }
    }
}
