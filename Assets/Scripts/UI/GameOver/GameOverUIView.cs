using BattleRoyale.Main;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class GameOverUIView : MonoBehaviour
    {
        [SerializeField] private GameObject _scoreboardUI;
        [SerializeField] private Transform _scoreboardContentTransform;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
        }

        private void UnsubscribeToEvents()
        {
        }

        public void EnableView()
        {
            gameObject.SetActive(true);
        }

        public void DisableView()
        {
            gameObject.SetActive(false);
        }

        public void ShowScoreboard()
        {
            _scoreboardUI.SetActive(true);
        }
        public void HideScoreboard()
        {
            _scoreboardUI.SetActive(false);
        }

        public Transform GetScoreboardContentTransform()
        {
            return _scoreboardContentTransform;
        }
    }
}
