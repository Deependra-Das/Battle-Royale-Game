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
            RegisterGameplayServices();
            _startMenuUIObj = GameManager.Instance.Get<StartMenuUIService>();

            _startMenuUIObj.ShowUI();
        }

        public void Exit()
        {
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
