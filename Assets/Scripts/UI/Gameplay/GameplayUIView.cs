using BattleRoyale.EventModule;
using BattleRoyale.MainModule;
using BattleRoyale.SceneModule;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BattleRoyale.UIModule
{
    public class GameplayUIView : MonoBehaviour
    {
        [Header("Gameplay UI PopUp")]
        [SerializeField] private GameObject _eliminationsPanel;
        [SerializeField] private TMP_Text _eliminatedCounterText;
        [SerializeField] private TMP_Text _eliminationCountdownText;
        [SerializeField] private float _animationSpeed = 3f;
        [SerializeField] private GameObject _rankPanel;
        [SerializeField] private TMP_Text _rankText;

        [Header("Eliminated PopUp")]
        [SerializeField] private GameObject _eliminationPopup;
        [SerializeField] private float _popupDuration = 2f;
        [SerializeField] private AnimationCurve _popupScaleCurve;

        [Header("Gameplay Disconnected UI PopUp")]
        [SerializeField] private GameObject _disconnectedMessageGameplayUIPopUp;
        [SerializeField] private TMP_Text _disconnectedCountdownGameplayUIText;

        private Vector3 _originalScale;
        private int _currentCountdownValue = -1;
        private bool _isCountingDown = false;

        private int _totalPlayers = 0;
        private int _currentEliminatedCount = 0;
        public float _disconnectedCountdownTime = 5f;

        private Coroutine _popupCoroutine;
        private Vector3 _popupOriginalScale;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            EventBusManager.Instance.Subscribe(EventName.GameplayStartCountdownTick, HandleCountdownTick);
            NetworkManager.Singleton.OnClientDisconnectCallback += ShowDisconnectionGameplayUI;
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventName.GameplayStartCountdownTick, HandleCountdownTick);
            NetworkManager.Singleton.OnClientDisconnectCallback -= ShowDisconnectionGameplayUI;
        }

        private void Start()
        {
            _originalScale = _eliminationCountdownText.transform.localScale;
            _popupOriginalScale = _eliminationPopup.transform.localScale;
            _eliminationPopup.SetActive(false);
            _rankPanel.SetActive(false);
            _disconnectedMessageGameplayUIPopUp.SetActive(false);
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
                _eliminationCountdownText.text = secondsRemaining.ToString();
                _eliminationCountdownText.gameObject.SetActive(true);
                _currentCountdownValue = secondsRemaining;
                _isCountingDown = true;
            }
            else
            {
                _eliminationCountdownText.text = "GO!";
                _isCountingDown = false;
                _eliminationCountdownText.transform.localScale = _originalScale;
                Invoke(nameof(HideCountdown), 1f);
            }
        }

        private void HideCountdown()
        {
            _eliminationCountdownText.gameObject.SetActive(false);
            _eliminationCountdownText.transform.localScale = _originalScale;
        }

        private void AnimateText()
        {
            float scaleFactor = Mathf.PingPong(Time.time * _animationSpeed, 0.5f) + 1f;
            _eliminationCountdownText.transform.localScale = _originalScale * scaleFactor;
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

        private void ShowDisconnectionGameplayUI(ulong clientID)
        {
            if ((NetworkManager.Singleton.IsServer && clientID == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.ConnectedClients.Count <= 1) || !NetworkManager.Singleton.IsServer)
            {
                _disconnectedMessageGameplayUIPopUp.SetActive(true);
                StartCoroutine(DisconnectedCountdownSequence());
            }
        }

        private IEnumerator DisconnectedCountdownSequence()
        {
            float currentTime = _disconnectedCountdownTime;

            while (currentTime > 0)
            {
                _disconnectedCountdownGameplayUIText.text = "Returning To Main Menu In... " + Mathf.Ceil(currentTime).ToString() + "s";
                currentTime -= 1f;
                yield return new WaitForSeconds(1f);
            }

            SceneLoader.Instance.LoadScene(SceneName.StartScene, false);
        }
    }
}