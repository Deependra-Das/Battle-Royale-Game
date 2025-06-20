using BattleRoyale.Event;
using BattleRoyale.Scene;
using BattleRoyale.UI;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class LobbyState : IGameState
    {
        private LobbyUIService _lobbyUIObj;

        public void Enter()
        {
            RegisterLobbyServices();
            _lobbyUIObj = GameManager.Instance.Get<LobbyUIService>();

            _lobbyUIObj.ShowUI();
        }

        public void Exit()
        {
            _lobbyUIObj.HideUI();
            Cleanup();
            UnegisterLobbyServices();
        }

        public void Cleanup()
        {
            _lobbyUIObj.Dispose();
        }

        private void RegisterLobbyServices()
        {
            LobbyUIView lobbyUIPrefab = GameManager.Instance.ui_SO.lobbyUIPrefab;
            ServiceLocator.Register(new LobbyUIService(lobbyUIPrefab));
        }

        private void UnegisterLobbyServices()
        {
            ServiceLocator.Unregister<LobbyUIService>();
        }
    }
}
