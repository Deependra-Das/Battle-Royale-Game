using BattleRoyale.Main;
using BattleRoyale.Scene;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class StartMenuUIView : MonoBehaviour
    {
        [SerializeField] private Button _newGameButtonPrefab;
        [SerializeField] private Button _quitGameButtonPrefab;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _newGameButtonPrefab.onClick.AddListener(OnNewGameButtonClicked);
            _quitGameButtonPrefab.onClick.AddListener(OnQuitGameButtonClicked);
        }

        private void UnsubscribeToEvents()
        {
            _newGameButtonPrefab.onClick.RemoveListener(OnNewGameButtonClicked);
            _quitGameButtonPrefab.onClick.RemoveListener(OnQuitGameButtonClicked);
        }

        private void OnNewGameButtonClicked()
        {
            SceneLoader.Instance.LoadScene(SceneName.LobbyScene, false);
        }

        private void OnQuitGameButtonClicked()
        {
           Application.Quit();
        }

        public void EnableView()
        {
            gameObject.SetActive(true);
        }

        public void DisableView()
        {
            gameObject.SetActive(false);
        }
    }
}
