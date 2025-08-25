using UnityEngine;

namespace BattleRoyale.Event
{
    public enum EventName
    {
        ChangeGameState,
        PlayerSpawnCompleted,
        GameplayCountdownTick,
        GameOverCountdownTick,
        ActivateTilesForGameplay,
        ActivatePlayerForGameplay,
        PlayerEliminated,
        PlayerAssignedRank,
        UpdateEliminationCount,
        HostDisconnected,
        ClientConnected,
        ClientDisconnected,
        ConnectedClientNetworkListChanged,
    }
}
