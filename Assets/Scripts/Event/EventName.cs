using UnityEngine;

namespace BattleRoyale.Event
{
    public enum EventName
    {
        ChangeGameState,
        StartSceneLoadedEvent,
        LobbySceneLoadedEvent,
        GameplaySceneLoadedEvent,
        GameOverSceneLoadedEvent,
        PlayerSpawnCompleted,
        StartGameplayCountdown,
        ActivateTilesForGameplay,
        ActivatePlayerForGameplay
    }
}
