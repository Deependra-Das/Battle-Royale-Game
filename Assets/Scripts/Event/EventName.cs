using UnityEngine;

namespace BattleRoyale.Event
{
    public enum EventName
    {
        ChangeGameState,
        PlayerSpawnCompleted,
        CountdownTick,
        ActivateTilesForGameplay,
        ActivatePlayerForGameplay,
        PlayerEliminated,
        PlayerAssignedRank,
        UpdateEliminationCount,
    }
}
