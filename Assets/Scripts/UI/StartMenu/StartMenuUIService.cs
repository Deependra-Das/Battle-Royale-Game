using UnityEngine;

namespace BattleRoyale.UIModule
{
    public class StartMenuUIService
    {
        private StartMenuUIView _startMenuUIView; 

        public StartMenuUIService(StartMenuUIView startMenuUIPrefab)
        {
            Transform canvasTransform = CanvasUIManager.Instance.canvasTransform;
            _startMenuUIView = Object.Instantiate(startMenuUIPrefab, canvasTransform);
            HideUI();
        }

        public void ShowUI()
        {
            _startMenuUIView.EnableView();
        }

        public void HideUI()
        {
            _startMenuUIView.DisableView();
        }

        public void Dispose()
        {
            Object.Destroy(_startMenuUIView.gameObject);
            _startMenuUIView = null;
        }
    }
}
