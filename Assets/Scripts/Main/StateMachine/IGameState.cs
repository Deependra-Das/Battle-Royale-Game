using UnityEngine;

namespace BattleRoyale.Main
{
    public interface IGameState
    {
        public void Enter();
        public void Exit();
    }
}
