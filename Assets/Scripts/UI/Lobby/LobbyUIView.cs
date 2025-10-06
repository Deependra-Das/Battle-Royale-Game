using BattleRoyale.Event;
using BattleRoyale.Main;
using BattleRoyale.Network;
using BattleRoyale.Scene;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class LobbyUIView : MonoBehaviour
    {
        [SerializeField] private Button _backToStartMenuButtonPrefab;

        [Header ("Main Tab Toggles")]
        [SerializeField] private Toggle _createLobbyToggle;
        [SerializeField] private Toggle _joinLobbyToggle;
        [SerializeField] private GameObject _createLobbyTabContainer;
        [SerializeField] private GameObject _joinLobbyTabContainer;

        [Header("Create Lobby Content")]
        [SerializeField] private TMP_InputField _lobbyNameInputField;
        [SerializeField] private TMP_Text _lobbyNameErrorMessageText;
        [SerializeField] private Toggle _capacityNumTogglePrefab;
        [SerializeField] private Toggle _publicToggle;
        [SerializeField] private Toggle _privateToggle;    
        [SerializeField] private Transform _capacityNumToggleGroupTransform;
        [SerializeField] private Button _createLobbyButtonPrefab;
        [SerializeField] private Button _resetButtonPrefab;
        private List<Toggle> toggles = new List<Toggle>();
        private int _capacitySelected = 0;
        private bool _privacySelected = false;
        private const int minLobbyNameLength = 3;
        private const int maxLobbyNameLength = 15;

        [Header("Join Lobby Content")]
        [SerializeField] private TMP_InputField _joinCodeInputField;
        [SerializeField] private TMP_Text _lobbyCodeErrorMessageText;
        [SerializeField] private Button _quickJoinButtonPrefab;
        [SerializeField] private Button _joinLobbyWithCodeButtonPrefab;
        private const int lobbyCodeLength = 6;

        [Header("Interstitial PopUp Content")]
        [SerializeField] private GameObject _interstitialPopUp;
        [SerializeField] private TMP_Text _interstitialMessageText;

        [Header("Lobby Message PopUp Content")]
        [SerializeField] private GameObject _lobbyMessagePopUp;
        [SerializeField] private TMP_Text _lobbyMessageText;
        [SerializeField] private Button _okButtonPrefab;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _createLobbyButtonPrefab.onClick.AddListener(OnCreateLobbyButtonClicked);
            _resetButtonPrefab.onClick.AddListener(OnResetButtonClicked);
            _quickJoinButtonPrefab.onClick.AddListener(OnQuickJoinButtonClicked);
            _joinLobbyWithCodeButtonPrefab.onClick.AddListener(OnJoinWithCodeButtonClicked);            
            _backToStartMenuButtonPrefab.onClick.AddListener(OnBackToStartMenuButtonClicked);
            _okButtonPrefab.onClick.AddListener(HideLobbyMessagePopUp);

            _createLobbyToggle.onValueChanged.AddListener((isOn) => HandleMainTabSwitch(isOn, 1));
            _joinLobbyToggle.onValueChanged.AddListener((isOn) => HandleMainTabSwitch(isOn, 2));

            _publicToggle.onValueChanged.AddListener((isOn) => HandlePrivacyToggleSwitch(isOn, 1));
            _privateToggle.onValueChanged.AddListener((isOn) => HandlePrivacyToggleSwitch(isOn, 2));

            EventBusManager.Instance.Subscribe(EventName.TryingToJoinGame, OnTryingToJoinLobbyUI);
            EventBusManager.Instance.Subscribe(EventName.FailedToJoinGame, OnFailedToJoinLobbyUI);
            EventBusManager.Instance.Subscribe(EventName.CreateLobbyStarted, OnCreateLobbyStartedUI);
            EventBusManager.Instance.Subscribe(EventName.CreateLobbyFailed, OnCreateLobbyFailedUI);
            EventBusManager.Instance.Subscribe(EventName.JoinStarted, OnJoinStartedUI);
            EventBusManager.Instance.Subscribe(EventName.JoinFailed, OnJoinFailedUI);
            EventBusManager.Instance.Subscribe(EventName.QuickJoinFailed, OnQuickJoinFailedUI);
        }

        private void UnsubscribeToEvents()
        {
            _createLobbyButtonPrefab.onClick.RemoveListener(OnCreateLobbyButtonClicked);
            _resetButtonPrefab.onClick.RemoveListener(OnResetButtonClicked);
            _quickJoinButtonPrefab.onClick.RemoveListener(OnQuickJoinButtonClicked);
            _joinLobbyWithCodeButtonPrefab.onClick.RemoveListener(OnJoinWithCodeButtonClicked);
            _backToStartMenuButtonPrefab.onClick.RemoveListener(OnBackToStartMenuButtonClicked);
            _okButtonPrefab.onClick.RemoveListener(HideLobbyMessagePopUp);

            _createLobbyToggle.onValueChanged.RemoveListener((isOn) => HandleMainTabSwitch(isOn, 1));
            _joinLobbyToggle.onValueChanged.RemoveListener((isOn) => HandleMainTabSwitch(isOn, 2));

            _publicToggle.onValueChanged.RemoveListener((isOn) => HandlePrivacyToggleSwitch(isOn, 1));
            _privateToggle.onValueChanged.RemoveListener((isOn) => HandlePrivacyToggleSwitch(isOn, 2));

            EventBusManager.Instance.Unsubscribe(EventName.TryingToJoinGame, OnTryingToJoinLobbyUI);
            EventBusManager.Instance.Unsubscribe(EventName.FailedToJoinGame, OnFailedToJoinLobbyUI);
            EventBusManager.Instance.Unsubscribe(EventName.CreateLobbyStarted, OnCreateLobbyStartedUI);
            EventBusManager.Instance.Unsubscribe(EventName.CreateLobbyFailed, OnCreateLobbyFailedUI);
            EventBusManager.Instance.Unsubscribe(EventName.JoinStarted, OnJoinStartedUI);
            EventBusManager.Instance.Unsubscribe(EventName.JoinFailed, OnJoinFailedUI);
            EventBusManager.Instance.Unsubscribe(EventName.QuickJoinFailed, OnQuickJoinFailedUI);


            foreach (var toggle in toggles)
            {
                toggle.onValueChanged.RemoveListener((isOn) => OnCapacityNumToggleChanged(toggle, isOn, toggle.group.transform.GetSiblingIndex()));
            }
        }

        private void Awake()
        {
            CreateCapacityNumToggles();
            HandleMainTabSwitch(true, 1);
            HandlePrivacyToggleSwitch(true, 1);
            HideLobbyMessagePopUp();
            HideInterstitialPopUp();
        }

        void HandleMainTabSwitch(bool isOn, int tabIndex)
        {
            if (isOn)
            {
                if (tabIndex == 1)
                {
                    _joinLobbyToggle.isOn = false;
                    _joinLobbyToggle.image.color = Color.gray;
                    _createLobbyToggle.image.color = Color.white;
                    _createLobbyTabContainer.SetActive(true);
                    _joinLobbyTabContainer.SetActive(false);
                }
                else if (tabIndex == 2)
                {
                    _createLobbyToggle.isOn = false;
                    _createLobbyToggle.image.color = Color.gray;
                    _joinLobbyToggle.image.color = Color.white;
                    _createLobbyTabContainer.SetActive(false);
                    _joinLobbyTabContainer.SetActive(true);
                }
            }
        }

        void CreateCapacityNumToggles()
        {
            int maxLobbySize = MultiplayerManager.MAX_LOBBY_SIZE;

            for (int i = 1; i <= maxLobbySize; i++)
            {
                AddCapacityRadioButton(i);
            }

            toggles[0].isOn = true;
        }

        public void AddCapacityRadioButton(int index)
        {
            GameObject toggleObject = Instantiate(_capacityNumTogglePrefab.gameObject, _capacityNumToggleGroupTransform);
            Toggle newToggle = toggleObject.GetComponent<Toggle>();
            newToggle.image.color = Color.gray;
            newToggle.GetComponentInChildren<TMP_Text>().text = index.ToString();
            toggles.Add(newToggle);
            newToggle.onValueChanged.AddListener((isOn) => OnCapacityNumToggleChanged(newToggle, isOn, index));
        }

        private void OnCapacityNumToggleChanged(Toggle changedToggle, bool isOn, int index)
        {
            if (isOn)
            {
                _capacitySelected = index;
                changedToggle.image.color = Color.white;
                DeactivateOtherCapacityNumToggles(changedToggle);
            }
        }

        private void DeactivateOtherCapacityNumToggles(Toggle changedToggle)
        {
            foreach (var toggle in toggles)
            {
                if (toggle != changedToggle)
                {
                    toggle.image.color = Color.gray;
                    toggle.isOn = false;
                }
            }
        }

        void HandlePrivacyToggleSwitch(bool isOn, int tabIndex)
        {
            if (isOn)
            {
                if (tabIndex == 1)
                {
                    _privateToggle.isOn = false;
                    _privateToggle.image.color = Color.gray;
                    _publicToggle.image.color = Color.white;
                    _privacySelected = false;
                }
                else if (tabIndex == 2)
                {
                    _publicToggle.isOn = false;
                    _publicToggle.image.color = Color.gray;
                    _privateToggle.image.color = Color.white;
                    _privacySelected = true;
                }
            }
        }

        private bool IsLobbyNameValid(string name)
        {
            string pattern = "^[A-Za-z0-9]+$";

            if (string.IsNullOrWhiteSpace(name))
            {
                _lobbyNameErrorMessageText.text = "Lobby name cannot be empty";
                return false;
            }

            if (name.Length < minLobbyNameLength || name.Length > maxLobbyNameLength)
            {
                _lobbyNameErrorMessageText.text = "Lobby name must be 3-16 characters long";
                return false;
            }

            if (!Regex.IsMatch(name, pattern))
            {
                _lobbyNameErrorMessageText.text = "Lobby name contains invalid characters";
                return false;
            }

            return true;
        }

        public bool IsLobbyCodeValid(string lobbyCode)
        {
            string pattern = @"^[A-Z0-9]{6}$";

            if (string.IsNullOrWhiteSpace(lobbyCode))
            {
                _lobbyCodeErrorMessageText.text = "Lobby Code cannot be empty";
                return false;
            }

            if (lobbyCode.Length != lobbyCodeLength)
            {
                _lobbyCodeErrorMessageText.text = "Lobby Code must must be exactly 6 characters long";
                return false;
            }

            if (!Regex.IsMatch(lobbyCode, pattern))
            {
                _lobbyCodeErrorMessageText.text = "Lobby code does not match the required pattern";
                return false;
            }

            return true;
        }

        private void OnCreateLobbyButtonClicked()
        {
            string lobbyName = _lobbyNameInputField.text;

            if (IsLobbyNameValid(lobbyName))
            {
                _lobbyNameErrorMessageText.text = string.Empty;
                LobbyManager.Instance.CreateLobby(lobbyName, _capacitySelected, _privacySelected);
            }
        }

        private void OnResetButtonClicked()
        {
             _lobbyNameInputField.text = string.Empty;
             _lobbyNameErrorMessageText.text = string.Empty;
            toggles[0].isOn = true;
            HandlePrivacyToggleSwitch(true, 1);
        }

        private void OnQuickJoinButtonClicked()
        {
            LobbyManager.Instance.QuickJoin();
        }

        private void OnJoinWithCodeButtonClicked()
        {
            string lobbyCode = _joinCodeInputField.text;

            if (IsLobbyCodeValid(lobbyCode))
            {
                _lobbyCodeErrorMessageText.text = string.Empty;
                LobbyManager.Instance.JoinWithCode(_joinCodeInputField.text);
            }
        }

        private void OnBackToStartMenuButtonClicked()
        {
            LobbyManager.Instance.LeaveLobby();
            SceneLoader.Instance.LoadScene(SceneName.StartScene, false);
        }

        public void EnableView()
        {
            gameObject.SetActive(true);
        }

        public void DisableView()
        {
            gameObject.SetActive(false);
        }

        private void OnTryingToJoinLobbyUI(object[] parameters)
        {
            _interstitialMessageText.text = "Connecting...";
            ShowInterstitialPopUp();
        }

        private void OnFailedToJoinLobbyUI(object[] parameters)
        {
            HideInterstitialPopUp();
            _lobbyMessageText.text = NetworkManager.Singleton.DisconnectReason;

            if (_lobbyMessageText.text == string.Empty)
            {
                _lobbyMessageText.text = "Failed To Connect.";
            }

            ShowLobbyMessagePopUp();
        }

        private void OnCreateLobbyStartedUI(object[] parameters)
        {
            _interstitialMessageText.text = "Creating Lobby...";
            ShowInterstitialPopUp();
        }

        private void OnCreateLobbyFailedUI(object[] parameters)
        {
            HideInterstitialPopUp();
            _lobbyMessageText.text = "Failed To Create Lobby.";
            ShowLobbyMessagePopUp();
        }

        private void OnJoinStartedUI(object[] parameters)
        {
            _interstitialMessageText.text = "Joining Lobby...";
            ShowInterstitialPopUp();
        }

        private void OnQuickJoinFailedUI(object[] parameters)
        {
            HideInterstitialPopUp();
            _lobbyMessageText.text = "Unable to Find a Lobby to Quick Join.";
            ShowLobbyMessagePopUp();
        }

        private void OnJoinFailedUI(object[] parameters)
        {
            HideInterstitialPopUp();
            _lobbyMessageText.text = "Failed To Join Lobby.";
            ShowLobbyMessagePopUp();
        }

        private void ShowInterstitialPopUp()
        {
            _interstitialPopUp.SetActive(true);
        }

        private void HideInterstitialPopUp()
        {
            _interstitialPopUp.SetActive(false);
        }

        private void ShowLobbyMessagePopUp()
        {
            _lobbyMessagePopUp.SetActive(true);
        }

        private void HideLobbyMessagePopUp()
        {
            _lobbyMessagePopUp.SetActive(false);
        }
    }
}
