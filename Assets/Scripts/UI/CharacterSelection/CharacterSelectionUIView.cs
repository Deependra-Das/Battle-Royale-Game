using BattleRoyale.Event;
using BattleRoyale.Main;
using BattleRoyale.Network;
using BattleRoyale.Scene;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class CharacterSelectionUIView : MonoBehaviour
    {
        [SerializeField] private Button _readyButtonPrefab;
        [SerializeField] private Button _notReadyButtonPrefab;
        [SerializeField] private Button _backToStartMenuButtonPrefab;

        [Header ("Disconnected PopUp")] 
        [SerializeField] private GameObject _disconnectedPopUp;
        [SerializeField] private Button _disconnectedBackButtonPrefab;

        [Header("Leave Lobby PopUp")]
        [SerializeField] private GameObject _leaveLobbyConfirmationPopUp;
        [SerializeField] private TMP_Text _hostLobbyNoticeText;
        [SerializeField] private Button _yesConfirmationButtonPrefab;
        [SerializeField] private Button _noConfirmationButtonPrefab;

        [Header("Character Skin Color")]
        [SerializeField] private Toggle _colorTogglePrefab;
        [SerializeField] private Transform _colorToggleGroupTransform;
        [SerializeField] private CharacterSkinColorInfo[] colorInfos;

        private List<Toggle> toggles = new List<Toggle>();

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _readyButtonPrefab.onClick.AddListener(OnReadyButtonClicked);
            _notReadyButtonPrefab.onClick.AddListener(OnNotReadyButtonClicked);
            _backToStartMenuButtonPrefab.onClick.AddListener(OnBackToStartMenuButtonClicked);
            _yesConfirmationButtonPrefab.onClick.AddListener(OnYesButtonClicked);
            _noConfirmationButtonPrefab.onClick.AddListener(OnNoButtonClicked);
            NetworkManager.Singleton.OnClientDisconnectCallback += ShowDisconnectionCharSelectionUI;
            _disconnectedBackButtonPrefab.onClick.AddListener(OnDisconnectedBackButtonClicked);
        }

        private void UnsubscribeToEvents()
        {
            _readyButtonPrefab.onClick.RemoveListener(OnReadyButtonClicked);
            _notReadyButtonPrefab.onClick.RemoveListener(OnNotReadyButtonClicked);
            _backToStartMenuButtonPrefab.onClick.RemoveListener(OnBackToStartMenuButtonClicked);
            _yesConfirmationButtonPrefab.onClick.AddListener(OnYesButtonClicked);
            _noConfirmationButtonPrefab.onClick.AddListener(OnNoButtonClicked);
            NetworkManager.Singleton.OnClientDisconnectCallback -= ShowDisconnectionCharSelectionUI;
            _disconnectedBackButtonPrefab.onClick.AddListener(OnDisconnectedBackButtonClicked);

            foreach (var toggle in toggles)
            {
                toggle.onValueChanged.RemoveListener((isOn) => OnToggleChanged(toggle, isOn, toggle.group.transform.GetSiblingIndex()));
            }
        }

        private void Start()
        {
            CreateColorButtons();
            _disconnectedPopUp.SetActive(false);
            _notReadyButtonPrefab.gameObject.SetActive(false);
            _readyButtonPrefab.gameObject.SetActive(true);
            SetHostLobbyNoticeText();
            HideBackToMainMenuConfirmationPopup();
        }

        void CreateColorButtons()
        {
            for (int i = 0; i < colorInfos.Length; i++)
            {
                AddRadioButton(colorInfos[i].skincolorIndex, colorInfos[i].skincolorName, colorInfos[i].skincolorHexValue);
            }

            toggles[0].isOn = true;
        }

        public void AddRadioButton(int togglecolorIndex, string toggleColorName, string toggleColor)
        {
            GameObject toggleObject = Instantiate(_colorTogglePrefab.gameObject, _colorToggleGroupTransform);
            CharacterSkinColorSelectView colorButtonSelectObj = toggleObject.GetComponent<CharacterSkinColorSelectView>();
            Toggle newToggle = toggleObject.GetComponent<Toggle>();
            colorButtonSelectObj.Initialize(togglecolorIndex, toggleColorName, toggleColor);

            toggles.Add(newToggle);
            newToggle.onValueChanged.AddListener((isOn) => OnToggleChanged(newToggle, isOn, togglecolorIndex));
        }

        private void OnToggleChanged(Toggle changedToggle, bool isOn, int index)
        {
            if (isOn)
            {
                DeactivateOtherToggles(changedToggle);
                PlayerLobbyStateManager.Instance.ChangeCharacterSkin(index);
            }
        }

        private void DeactivateOtherToggles(Toggle changedToggle)
        {
            foreach (var toggle in toggles)
            {
                if (toggle != changedToggle)
                {
                    toggle.isOn = false;
                }
            }
        }

        public void HideAllToggles()
        {
            foreach (var toggle in toggles)
            {
                toggle.gameObject.SetActive(false);
            }
        }

        public void ShowAllToggles()
        {
            foreach (var toggle in toggles)
            {
                toggle.gameObject.SetActive(true);
            }
        }

        private void OnReadyButtonClicked()
        {
            PlayerLobbyStateManager.Instance.SetPlayerReady();
            _readyButtonPrefab.gameObject.SetActive(false);
            _notReadyButtonPrefab.gameObject.SetActive(true);
            HideAllToggles();
        }

        private void OnNotReadyButtonClicked()
        {
            PlayerLobbyStateManager.Instance.SetPlayerNotReady();
            _notReadyButtonPrefab.gameObject.SetActive(false);
            _readyButtonPrefab.gameObject.SetActive(true);
            ShowAllToggles();
        }

        private void OnBackToStartMenuButtonClicked()
        {
            ShowBackToMainMenuConfirmationPopup();
        }

        private void SetHostLobbyNoticeText()
        {
            if(NetworkManager.Singleton.IsHost)
            {
                _hostLobbyNoticeText.gameObject.SetActive(true);
            }
            else
            {
                _hostLobbyNoticeText.gameObject.SetActive(false);
            }
        }

        private void OnYesButtonClicked()
        {
            HideBackToMainMenuConfirmationPopup();
            SceneLoader.Instance.LoadScene(SceneName.StartScene, false);
        }

        private void OnNoButtonClicked()
        {
            HideBackToMainMenuConfirmationPopup();
        }

        private void OnDisconnectedBackButtonClicked()
        {
            _disconnectedPopUp.SetActive(false);
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

        private void ShowDisconnectionCharSelectionUI(ulong clientID)
        {
            if ((NetworkManager.Singleton.IsServer && clientID == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.ConnectedClients.Count <= 1) || !NetworkManager.Singleton.IsServer)
            {
                _disconnectedPopUp.SetActive(true);
            }
        }

        private void ShowBackToMainMenuConfirmationPopup()
        {
            _leaveLobbyConfirmationPopUp.SetActive(true);
        }

        private void HideBackToMainMenuConfirmationPopup()
        {
            _leaveLobbyConfirmationPopUp.SetActive(false);
        }
    }
}
