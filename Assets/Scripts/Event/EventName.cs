using UnityEngine;

namespace BattleRoyale.Event
{
    public enum EventName
    {
        ChangeGameState,
        StartSceneLoadedEvent,
        GameplaySceneLoadedEvent,
        GameOverSceneLoadedEvent,
        PlayerSpawnCompleted,
        StartGameplayCountdown,
        ActivateTilesForGameplay,
        ActivatePlayerForGameplay
    }
}
