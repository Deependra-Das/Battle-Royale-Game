using UnityEngine;

namespace BattleRoyale.UI
{
    public class LobbyUIService
    {
        private LobbyUIView _lobbyUIView;

        public LobbyUIService(LobbyUIView LobbyUIPrefab)
        {
            Transform canvasTransform = CanvasUIManager.Instance.canvasTransform;
            _lobbyUIView = Object.Instantiate(LobbyUIPrefab, canvasTransform);
            HideUI();
        }

        public void ShowUI()
        {
            _lobbyUIView.EnableView();
        }

        public void HideUI()
        {
            _lobbyUIView.DisableView();
        }

        public void Dispose()
        {
            Object.Destroy(_lobbyUIView.gameObject);
            _lobbyUIView = null;
        }
    }
}
