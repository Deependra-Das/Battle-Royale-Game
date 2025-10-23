using BattleRoyale.AudioModule;
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
        [SerializeField] private GameObject _rankPanel;
        [SerializeField] private TMP_Text _rankText;

        [SerializeField] private RawImage _gameplayStartCountdownImage;
        [SerializeField] private Texture2D _countdownImage_3;
        [SerializeField] private Texture2D _countdownImage_2;
        [SerializeField] private Texture2D _countdownImage_1;
        [SerializeField] private Texture2D _countdownImage_GO;
        [SerializeField] private float _animationSpeed = 1f; 

        [Header("Eliminated PopUp")]
        [SerializeField] private GameObject _eliminationPopup;
        [SerializeField] private float _popupDuration = 3f;

        [Header("Gameplay Disconnected UI PopUp")]
        [SerializeField] private GameObject _disconnectedMessageGameplayUIPopUp;
        [SerializeField] private TMP_Text _disconnectedCountdownGameplayUIText;

        private Vector3 _originalScale;
        private int _currentCountdownValue = -1;
        private bool _isCountingDown = false;

        private int _totalPlayers = 0;
        private int _currentEliminatedCount = 0;
        public float _disconnectedCountdownTime = 5f;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            EventBusManager.Instance.Subscribe(EventName.GameplayStartCountdownTick, HandleCountdownTick);
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnectCallbackGameplayUI;
        }

        private void UnsubscribeToEvents()
        {
            EventBusManager.Instance.Unsubscribe(EventName.GameplayStartCountdownTick, HandleCountdownTick);
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnectCallbackGameplayUI;
        }

        private void Start()
        {
            _originalScale = _gameplayStartCountdownImage.transform.localScale;
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
                AudioManager.Instance.PlaySFX(AudioModule.AudioType.GameStartCountdown);
                SetCountdownImage(secondsRemaining);
                _gameplayStartCountdownImage.gameObject.SetActive(true);
                _currentCountdownValue = secondsRemaining;
                _isCountingDown = true;
            }
            else
            {
                AudioManager.Instance.PlaySFX(AudioModule.AudioType.GameStarted);
                _gameplayStartCountdownImage.texture = _countdownImage_GO;
                _isCountingDown = false;
                _gameplayStartCountdownImage.transform.localScale = _originalScale;
                Invoke(nameof(HideCountdown), 1f);
            }
        }

        private void SetCountdownImage(int secondsRemaining)
        {
            switch(secondsRemaining)
            {
                case 1:
                    _gameplayStartCountdownImage.texture = _countdownImage_1;
                    break;
                case 2:
                    _gameplayStartCountdownImage.texture = _countdownImage_2;
                    break;
                case 3:
                    _gameplayStartCountdownImage.texture = _countdownImage_3;
                    break;
            }
        }

        private void HideCountdown()
        {
            _gameplayStartCountdownImage.gameObject.SetActive(false);
            _gameplayStartCountdownImage.transform.localScale = _originalScale;
        }

        private void AnimateText()
        {
            float scaleFactor = Mathf.PingPong(Time.time * _animationSpeed, 0.5f) + 1f;
            _gameplayStartCountdownImage.transform.localScale = _originalScale * scaleFactor;
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
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.PlayerEliminated);
            StartCoroutine(EliminationPopupSequence());
        }

        private IEnumerator EliminationPopupSequence()
        {
            _eliminationPopup.SetActive(true);
            yield return new WaitForSeconds(_popupDuration);
            _eliminationPopup.SetActive(false);
        }

        public void UpdatePlayerRank(int rank)
        {
            _rankText.text = $"Rank: {rank}";
            _rankPanel.SetActive(true);
        }

        private void HandleClientDisconnectCallbackGameplayUI(ulong clientID)
        {
            if ((NetworkManager.Singleton.IsServer && clientID == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.ConnectedClients.Count <= 1) || !NetworkManager.Singleton.IsServer)
            {
                ShowDisconnectionGameplayUI();
            }
        }

        private void ShowDisconnectionGameplayUI()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.DisconnectionPopUp);
            _disconnectedMessageGameplayUIPopUp.SetActive(true);
            StartCoroutine(DisconnectedCountdownSequence());
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