using BattleRoyale.Main;
using BattleRoyale.Scene;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DelayedExecution());
    }

    IEnumerator DelayedExecution()
    {
        yield return null;

        string activeSceneName = SceneManager.GetActiveScene().name.ToString();
        GameState newGameState = GameState.Start;
        Enum.TryParse<SceneName>(activeSceneName, out var sceneEnumValue);

        switch (sceneEnumValue)
        {
            case SceneName.StartScene:
                newGameState = GameState.Start;
                break;
            case SceneName.LobbyScene:
                newGameState = GameState.Lobby;
                break;
            case SceneName.CharacterSelectionScene:
                newGameState = GameState.CharacterSelection;
                break;
            case SceneName.GameScene:
                newGameState = GameState.Gameplay;
                break;
            case SceneName.GameOverScene:
                newGameState = GameState.GameOver;
                break;
        }

        GameManager.Instance.ChangeGameState(newGameState);
    }
}
