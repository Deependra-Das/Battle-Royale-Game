using BattleRoyale.Event;
using BattleRoyale.Scene;
using BattleRoyale.UI;
using UnityEngine;

namespace BattleRoyale.Main
{
    public class LobbyState : IGameState
    {
        private LobbyUIService _lobbyUIObj;

        public void Enter()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneName.LobbyScene);
            EventBusManager.Instance.Subscribe(EventName.LobbySceneLoadedEvent, HandleLobbyState);
        }

        private void HandleLobbyState(object[] parameters)
        {
            RegisterLobbyServices();
            _lobbyUIObj = GameManager.Instance.Get<LobbyUIService>();

            _lobbyUIObj.ShowUI();
        }

        public void Exit()
        {
            EventBusManager.Instance.Unsubscribe(EventName.LobbySceneLoadedEvent, HandleLobbyState);
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
