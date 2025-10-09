using UnityEngine;

namespace BattleRoyale.EventModule
{
    public enum EventName
    {
        ChangeGameState,
        PlayerSpawnCompleted,
        GameplayStartCountdownTick,
        ActivateTilesForGameplay,
        ActivatePlayerForGameplay,
        PlayerEliminated,
        PlayerAssignedRank,
        UpdateEliminationCount,
        ConnectedClientNetworkListChanged,
        GameOverScoreCard,
        TryingToJoinGame,
        FailedToJoinGame,
        CreateLobbyStarted,
        CreateLobbyFailed,
        JoinStarted,
        QuickJoinFailed,
        JoinFailed,
        PublicLobbyListChanged,
    }
}
