using UnityEngine;

namespace BattleRoyale.MainModule
{
    public interface IGameState
    {
        public void Enter();
        public void Exit();
    }
}
