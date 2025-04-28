using BattleRoyale.Event;
using BattleRoyale.Scene;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class StartState : IGameState
    {
        public void Enter()
        {
            Debug.Log("Start Enter");
            SceneLoader.Instance.LoadSceneAsync(SceneName.StartScene);
            EventBusManager.Instance.Subscribe(EventName.StartSceneLoadedEvent, HandleStartState);
        }

        private void HandleStartState(object[] parameters)
        {
            GameManager.Instance.ChangeGameState(GameState.Gameplay);
        }

        public void Exit()
        {
            Debug.Log("Start Exit");
            EventBusManager.Instance.Unsubscribe(EventName.StartSceneLoadedEvent, HandleStartState);
        }
    }
}
