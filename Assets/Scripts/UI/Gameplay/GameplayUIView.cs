using BattleRoyale.Main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class GameplayUIView : MonoBehaviour
    {
        [SerializeField] private GameObject _currentEliminationsPanel;
        [SerializeField] private TMP_Text _currentEliminationsText;

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
    }
}