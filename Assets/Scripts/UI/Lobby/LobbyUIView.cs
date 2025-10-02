using BattleRoyale.Main;
using BattleRoyale.Network;
using BattleRoyale.Scene;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
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
        [SerializeField] private TMP_Text _errorMessageText;
        [SerializeField] private Toggle _capacityNumTogglePrefab;
        [SerializeField] private Toggle _publicToggle;
        [SerializeField] private Toggle _privateToggle;    
        [SerializeField] private Transform _capacityNumToggleGroupTransform;
        [SerializeField] private Button _createLobbyButtonPrefab;
        [SerializeField] private Button _resetButtonPrefab;
        private List<Toggle> toggles = new List<Toggle>();
        private int _capacitySelected = 0;
        private bool _privacySelected = false;

        [Header("Join Lobby Content")]
        [SerializeField] private TMP_InputField _joinCodeInputField;
        [SerializeField] private Button _quickJoinButtonPrefab;
        [SerializeField] private Button _joinLobbyWithCodeButtonPrefab;

        private const int minLobbyNameLength = 3;
        private const int maxLobbyNameLength = 15;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _createLobbyButtonPrefab.onClick.AddListener(OnCreateLobbyButtonClicked);
            _resetButtonPrefab.onClick.AddListener(OnResetButtonClicked);
            _quickJoinButtonPrefab.onClick.AddListener(OnQuickJoinButtonClicked);
            _joinLobbyWithCodeButtonPrefab.onClick.AddListener(OnJoinWithCodeButtonClicked);
            
            _backToStartMenuButtonPrefab.onClick.AddListener(OnBackToStartMenuButtonClicked);

            _createLobbyToggle.onValueChanged.AddListener((isOn) => HandleMainTabSwitch(isOn, 1));
            _joinLobbyToggle.onValueChanged.AddListener((isOn) => HandleMainTabSwitch(isOn, 2));

            _publicToggle.onValueChanged.AddListener((isOn) => HandlePrivacyToggleSwitch(isOn, 1));
            _privateToggle.onValueChanged.AddListener((isOn) => HandlePrivacyToggleSwitch(isOn, 2));
        }

        private void UnsubscribeToEvents()
        {
            _createLobbyButtonPrefab.onClick.RemoveListener(OnCreateLobbyButtonClicked);
            _resetButtonPrefab.onClick.RemoveListener(OnResetButtonClicked);
            _quickJoinButtonPrefab.onClick.RemoveListener(OnQuickJoinButtonClicked);
            _joinLobbyWithCodeButtonPrefab.onClick.RemoveListener(OnJoinWithCodeButtonClicked);

            _backToStartMenuButtonPrefab.onClick.RemoveListener(OnBackToStartMenuButtonClicked);

            _createLobbyToggle.onValueChanged.RemoveListener((isOn) => HandleMainTabSwitch(isOn, 1));
            _joinLobbyToggle.onValueChanged.RemoveListener((isOn) => HandleMainTabSwitch(isOn, 2));

            _publicToggle.onValueChanged.RemoveListener((isOn) => HandlePrivacyToggleSwitch(isOn, 1));
            _privateToggle.onValueChanged.RemoveListener((isOn) => HandlePrivacyToggleSwitch(isOn, 2));

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
                _errorMessageText.text = "Lobby name cannot be empty!";
                return false;
            }

            if (name.Length < minLobbyNameLength || name.Length > maxLobbyNameLength)
            {
                _errorMessageText.text = "Lobby name must be 3-16 characters long!";
                return false;
            }

            if (!Regex.IsMatch(name, pattern))
            {
                _errorMessageText.text = "Lobby name contains invalid characters!";
                return false;
            }

            return true;
        }

        private void OnCreateLobbyButtonClicked()
        {
            string lobbyName = _lobbyNameInputField.text;

            if (IsLobbyNameValid(lobbyName))
            {
                LobbyManager.Instance.CreateLobby(lobbyName, _capacitySelected, _privacySelected);

                _errorMessageText.text = string.Empty;
            }
        }

        private void OnResetButtonClicked()
        {
             _lobbyNameInputField.text = string.Empty;
             _errorMessageText.text = string.Empty;
            toggles[0].isOn = true;
            HandlePrivacyToggleSwitch(true, 1);
        }

        private void OnQuickJoinButtonClicked()
        {
            LobbyManager.Instance.QuickJoin();
        }

        private void OnJoinWithCodeButtonClicked()
        {
            LobbyManager.Instance.JoinWithCode(_joinCodeInputField.text);
        }

        private void OnBackToStartMenuButtonClicked()
        {
            GameManager.Instance.ChangeGameState(GameState.Start);
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
