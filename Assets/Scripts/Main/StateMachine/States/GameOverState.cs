using BattleRoyale.Event;
using BattleRoyale.Scene;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class GameOverState : IGameState
    {
        public void Enter()
        {
            Debug.Log("Game Over Enter");
            SceneLoader.Instance.LoadSceneAsync(SceneName.GameOverScene);
            EventBusManager.Instance.Subscribe(EventName.GameOverSceneLoadedEvent, HandleGameOverState);
        }

        private void HandleGameOverState(object[] parameters)
        {
            GameManager.Instance.ChangeGameState(GameState.Start);
        }

        public void Exit()
        {
            Debug.Log("Game Over Exit");
            EventBusManager.Instance.Unsubscribe(EventName.GameOverSceneLoadedEvent, HandleGameOverState);
        }

    }
}
