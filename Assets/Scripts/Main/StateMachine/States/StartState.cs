using BattleRoyale.Event;
using BattleRoyale.Level;
using BattleRoyale.Player;
using BattleRoyale.Scene;
using BattleRoyale.UI;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class StartState : IGameState
    {
        private StartMenuUIService _startMenuUIObj;

        public void Enter()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneName.StartScene);
            EventBusManager.Instance.Subscribe(EventName.StartSceneLoadedEvent, HandleStartState);
        }

        private void HandleStartState(object[] parameters)
        {
            RegisterGameplayServices();
            _startMenuUIObj = GameManager.Instance.Get<StartMenuUIService>();

            _startMenuUIObj.ShowUI();
        }

        public void Exit()
        {
            EventBusManager.Instance.Unsubscribe(EventName.StartSceneLoadedEvent, HandleStartState);
            Cleanup();
            UnegisterGameplayServices();
        }

        public void Cleanup()
        {
            _startMenuUIObj.Dispose();
        }

        private void RegisterGameplayServices()
        {
            StartMenuUIView startMenuUIPrefab = GameManager.Instance.ui_SO.startMenuUIPrefab;
            ServiceLocator.Register(new StartMenuUIService(startMenuUIPrefab));
        }

        private void UnegisterGameplayServices()
        {
            ServiceLocator.Unregister<StartMenuUIService>();
        }
    }
}
