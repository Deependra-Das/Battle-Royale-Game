using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.UIModule
{
    public class StartMenuUIService
    {
        private StartMenuUIView _startMenuUIView; 

        public StartMenuUIService(StartMenuUIView startMenuUIPrefab, List<Sprite> galleryImages)
        {
            Transform canvasTransform = CanvasUIManager.Instance.canvasTransform;
            _startMenuUIView = Object.Instantiate(startMenuUIPrefab, canvasTransform);
            _startMenuUIView.Initialize(galleryImages);
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
