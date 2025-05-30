using BattleRoyale.Event;
using BattleRoyale.Main;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class GameplayUIView : MonoBehaviour
    {
        [SerializeField] private GameObject _currentEliminationsPanel;
        [SerializeField] private TMP_Text _currentEliminationsText;
        [SerializeField] private TMP_Text _countdownText;
        [SerializeField] private float _countdownTime;
        [SerializeField] private float _animationSpeed;

        private Vector3 _originalScale;
        private float _currentTime;


        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            EventBusManager.Instance.Subscribe(EventName.StartGameplayCountdown, StartCoundown);
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventName.StartGameplayCountdown, StartCoundown);
        }

        public void StartCoundown(object[] parameters)
        {
            _originalScale = _countdownText.transform.localScale;
            _countdownText.gameObject.SetActive(true);
            StartCoroutine(CountdownCoroutine());
        }

        IEnumerator CountdownCoroutine()
        {
            _currentTime = _countdownTime;
            while (_currentTime > 0)
            {
                _currentTime -= Time.deltaTime;
                UpdateCountdownText(_currentTime);
                AnimateText(_currentTime);
                yield return null;
            }

            _countdownText.gameObject.SetActive(false);
            EventBusManager.Instance.Raise(EventName.ActivatePlayerForGameplay, true);
            EventBusManager.Instance.Raise(EventName.ActivateTilesForGameplay, true);            
        }

        void UpdateCountdownText(float time)
        {
            if (_countdownText != null)
            {
                _countdownText.text = Mathf.Ceil(time).ToString();
            }
        }

        void AnimateText(float time)
        {
            float scaleFactor = Mathf.PingPong(Time.time * _animationSpeed, 0.5f) + 1f;
            _countdownText.transform.localScale = _originalScale * scaleFactor;
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