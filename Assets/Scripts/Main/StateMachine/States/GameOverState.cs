using BattleRoyale.Event;
using BattleRoyale.Scene;
using BattleRoyale.UI;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class GameOverState : IGameState
    {
        private GameOverUIService gameOverUIObj;

        public void Enter()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneName.GameOverScene);
            EventBusManager.Instance.Subscribe(EventName.GameOverSceneLoadedEvent, HandleGameOverState);
        }

        private void HandleGameOverState(object[] parameters)
        {
            RegisterGameOverServices();
            gameOverUIObj = GameManager.Instance.Get<GameOverUIService>();

            gameOverUIObj.ShowUI();
        }

        public void Exit()
        {
            EventBusManager.Instance.Unsubscribe(EventName.GameOverSceneLoadedEvent, HandleGameOverState);
            gameOverUIObj.HideUI();
            Cleanup();
            UnegisterGameOverServices();
        }

        public void Cleanup()
        {
            gameOverUIObj.Dispose();
        }

        private void RegisterGameOverServices()
        {
            GameOverUIView gameOverUIPrefab = GameManager.Instance.ui_SO.gameOverUIPrefab;
            ServiceLocator.Register(new GameOverUIService(gameOverUIPrefab));
        }

        private void UnegisterGameOverServices()
        {
            ServiceLocator.Unregister<GameOverUIService>();
        }

    }
}
