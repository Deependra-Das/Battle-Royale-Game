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
        [SerializeField] private float _animationSpeed = 3f;

        private Vector3 _originalScale;
        private int _currentCountdownValue = -1;
        private bool _isCountingDown = false;


        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            EventBusManager.Instance.Subscribe(EventName.CountdownTick, HandleCountdownTick);
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventName.CountdownTick, HandleCountdownTick);
        }

        private void Start()
        {
            _originalScale = _countdownText.transform.localScale;
        }

        private void Update()
        {
            if (_isCountingDown)
            {
                AnimateText();
            }
        }

        private void HandleCountdownTick(object[] parameters)
        {
            int secondsRemaining = (int) parameters[0];

            if (secondsRemaining > 0)
            {
                _countdownText.text = secondsRemaining.ToString();
                _countdownText.gameObject.SetActive(true);
                _currentCountdownValue = secondsRemaining;
                _isCountingDown = true;
            }
            else
            {
                _countdownText.text = "GO!";
                _isCountingDown = false;
                _countdownText.transform.localScale = _originalScale;
                Invoke(nameof(HideCountdown), 1f);
            }
        }

        private void HideCountdown()
        {
            _countdownText.gameObject.SetActive(false);
            _countdownText.transform.localScale = _originalScale;
        }

        private void AnimateText()
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