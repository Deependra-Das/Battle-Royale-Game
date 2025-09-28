using BattleRoyale.Event;
using BattleRoyale.Main;
using BattleRoyale.Scene;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class GameplayUIView : MonoBehaviour
    {
        [Header("Gameplay UI PopUp")]
        [SerializeField] private GameObject _eliminationsPanel;
        [SerializeField] private TMP_Text _eliminatedCounterText;
        [SerializeField] private TMP_Text _countdownText;
        [SerializeField] private float _animationSpeed = 3f;
        [SerializeField] private GameObject _rankPanel;
        [SerializeField] private TMP_Text _rankText;

        [Header("Eliminated PopUp")]
        [SerializeField] private GameObject _eliminationPopup;
        [SerializeField] private float _popupDuration = 2f;
        [SerializeField] private AnimationCurve _popupScaleCurve;

        [Header("Disconnected PopUp")]
        [SerializeField] private GameObject _disconnectedPopUp;
        [SerializeField] private Button _disconnectedBackButtonPrefab;

        private Vector3 _originalScale;
        private int _currentCountdownValue = -1;
        private bool _isCountingDown = false;

        private int _totalPlayers = 0;
        private int _currentEliminatedCount = 0;

        private Coroutine _popupCoroutine;
        private Vector3 _popupOriginalScale;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            EventBusManager.Instance.Subscribe(EventName.GameplayCountdownTick, HandleCountdownTick);
            NetworkManager.Singleton.OnClientDisconnectCallback += ShowDisconnectionCharSelectionUI;
            _disconnectedBackButtonPrefab.onClick.AddListener(OnDisconnectedBackButtonClicked);
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventName.GameplayCountdownTick, HandleCountdownTick);
            NetworkManager.Singleton.OnClientDisconnectCallback -= ShowDisconnectionCharSelectionUI;
            _disconnectedBackButtonPrefab.onClick.AddListener(OnDisconnectedBackButtonClicked);
        }

        private void Start()
        {
            _originalScale = _countdownText.transform.localScale;
            _popupOriginalScale = _eliminationPopup.transform.localScale;
            _eliminationPopup.SetActive(false);
            _rankPanel.SetActive(false);
            _disconnectedPopUp.SetActive(false);
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

        public void SetTotalPlayers(int total)
        {
            _totalPlayers = total;
            UpdateEliminatedCounter();
        }

        public void UpdateEliminatedCount(int newCount)
        {
            _currentEliminatedCount = newCount;
            UpdateEliminatedCounter();
        }

        private void UpdateEliminatedCounter()
        {
            _eliminatedCounterText.text = $"Eliminated: {_currentEliminatedCount} / {_totalPlayers}";
        }

        public void ShowEliminatedPopup()
        {
            if (_popupCoroutine != null)
                StopCoroutine(_popupCoroutine);

            _popupCoroutine = StartCoroutine(EliminationPopupSequence());
        }

        private IEnumerator EliminationPopupSequence()
        {
            _eliminationPopup.SetActive(true);
            _eliminationPopup.transform.localScale = Vector3.zero;

            float time = 0f;
            float duration = 0.5f;

            while (time < duration)
            {
                float scale = _popupScaleCurve.Evaluate(time / duration);
                _eliminationPopup.transform.localScale = _popupOriginalScale * scale;
                time += Time.deltaTime;
                yield return null;
            }

            _eliminationPopup.transform.localScale = _popupOriginalScale;

            yield return new WaitForSeconds(_popupDuration);

            _eliminationPopup.SetActive(false);
        }

        public void UpdatePlayerRank(int rank)
        {
            _rankText.text = $"Rank: {rank}";
            _rankPanel.SetActive(true);
        }

        private void ShowDisconnectionCharSelectionUI(ulong clientID)
        {
            _disconnectedPopUp.SetActive(true);
        }

        private void OnDisconnectedBackButtonClicked()
        {
            _disconnectedPopUp.SetActive(false);
            SceneLoader.Instance.LoadScene(SceneName.StartScene, false);
        }
    }
}