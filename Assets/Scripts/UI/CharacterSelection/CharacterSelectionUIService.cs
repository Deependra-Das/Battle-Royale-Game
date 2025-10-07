using UnityEngine;

namespace BattleRoyale.UIModule
{
    public class CharacterSelectionUIService
    {
        private CharacterSelectionUIView _characterSelectionUIView;

        public CharacterSelectionUIService(CharacterSelectionUIView CharacterSelectionUIPrefab)
        {
            Transform canvasTransform = CanvasUIManager.Instance.canvasTransform;
            _characterSelectionUIView = Object.Instantiate(CharacterSelectionUIPrefab, canvasTransform);
            HideUI();
        }

        public void ShowUI()
        {
            _characterSelectionUIView.EnableView();
        }

        public void HideUI()
        {
            _characterSelectionUIView.DisableView();
        }

        public void Dispose()
        {
            Object.Destroy(_characterSelectionUIView.gameObject);
            _characterSelectionUIView = null;
        }
    }
}
