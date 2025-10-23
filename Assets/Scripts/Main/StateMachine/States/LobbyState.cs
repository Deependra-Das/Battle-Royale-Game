using BattleRoyale.AudioModule;
using BattleRoyale.UIModule;

namespace BattleRoyale.MainModule
{
    public class LobbyState : IGameState
    {
        private LobbyUIService _lobbyUIObj;

        public void Enter()
        {
            RegisterLobbyServices();
            _lobbyUIObj = GameManager.Instance.Get<LobbyUIService>();

            _lobbyUIObj.ShowUI();
            AudioManager.Instance.PlayBGM(AudioModule.AudioType.LobbyBGM);
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
