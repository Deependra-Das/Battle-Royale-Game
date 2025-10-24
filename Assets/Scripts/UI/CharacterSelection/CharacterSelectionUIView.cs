using BattleRoyale.AudioModule;
using BattleRoyale.LobbyModule;
using BattleRoyale.NetworkModule;
using BattleRoyale.PlayerModule;
using BattleRoyale.SceneModule;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UIModule
{
    public class CharacterSelectionUIView : MonoBehaviour
    {
        [SerializeField] private Button _readyButtonPrefab;
        [SerializeField] private Button _notReadyButtonPrefab;
        [SerializeField] private Button _backToStartMenuButtonPrefab;

        [SerializeField] private TMP_Text _lobbyNameText;
        [SerializeField] private TMP_Text _lobbyCodeText;
        [SerializeField] private TMP_Text _lobbyPrivacyText;
        [SerializeField] private TMP_Text _lobbyCapacityText;

        [Header ("Disconnected PopUp")] 
        [SerializeField] private GameObject _disconnectedPopUp;
        [SerializeField] private TMP_Text _disconnectedCountdownCharSelectUIText;
        [SerializeField] float _disconnectedCountdownTime = 5f;

        [Header("Leave Lobby PopUp")]
        [SerializeField] private GameObject _leaveLobbyConfirmationPopUp;
        [SerializeField] private TMP_Text _hostLobbyNoticeText;
        [SerializeField] private Button _yesConfirmationButtonPrefab;
        [SerializeField] private Button _noConfirmationButtonPrefab;

        [Header("Character Skin Color")]
        [SerializeField] private Toggle _colorTogglePrefab;
        [SerializeField] private Transform _colorToggleGroupTransform;
        [SerializeField] private PlayerCharMatSkinColorScriptableObject _charSkinMatInfo_SO;

        [Header("Kick Bar")]
        [SerializeField] private GameObject _kickButtonPrefab;
        [SerializeField] private Transform _kickButtonContainer;
        [SerializeField] private HorizontalLayoutGroup _kickButtonContainerLayoutGroup;

        private List<Toggle> _toggles = new List<Toggle>();
        private int[] buttonOrder = { 6, 4, 2, 0, 1, 3, 5, 7 };
        private List<GameObject> _buttonsList = new List<GameObject>();
        private bool _isInitialSetup = true;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _readyButtonPrefab.onClick.AddListener(OnReadyButtonClicked);
            _notReadyButtonPrefab.onClick.AddListener(OnNotReadyButtonClicked);
            _backToStartMenuButtonPrefab.onClick.AddListener(OnBackToStartMenuButtonClicked);
            _yesConfirmationButtonPrefab.onClick.AddListener(OnYesButtonClicked);
            _noConfirmationButtonPrefab.onClick.AddListener(OnNoButtonClicked);
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnectCallbackCharSelectUI;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnectCallbackCharSelectUI;
        }

        private void UnsubscribeToEvents()
        {
            _readyButtonPrefab.onClick.RemoveListener(OnReadyButtonClicked);
            _notReadyButtonPrefab.onClick.RemoveListener(OnNotReadyButtonClicked);
            _backToStartMenuButtonPrefab.onClick.RemoveListener(OnBackToStartMenuButtonClicked);
            _yesConfirmationButtonPrefab.onClick.RemoveListener(OnYesButtonClicked);
            _noConfirmationButtonPrefab.onClick.RemoveListener(OnNoButtonClicked);
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnectCallbackCharSelectUI;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnectCallbackCharSelectUI;

            foreach (var toggle in _toggles)
            {
                toggle.onValueChanged.RemoveListener((isOn) => OnToggleChanged(toggle, isOn, toggle.group.transform.GetSiblingIndex()));
            }
        }

        private void Start()
        {
            SetLobbyInformation();
            CreateColorButtons();
            _disconnectedPopUp.SetActive(false);
            _notReadyButtonPrefab.gameObject.SetActive(false);
            _readyButtonPrefab.gameObject.SetActive(true);
            SetHostLobbyNoticeText();
            HideBackToMainMenuConfirmationPopup();

            if (NetworkManager.Singleton.IsServer)
            {
                CreateKickButtons();
            }

            _isInitialSetup = false;
        }

        private void SetLobbyInformation()
        {
            Lobby lobby = LobbyManager.Instance.GetLobby();
            _lobbyNameText.text = lobby.Name;
            _lobbyCodeText.text = lobby.LobbyCode;
            _lobbyPrivacyText.text = lobby.IsPrivate ? "Private" : "Public";
            _lobbyCapacityText.text = lobby.MaxPlayers.ToString();
        }

        void CreateColorButtons()
        {
            for (int i = 0; i < _charSkinMatInfo_SO.charSkinInfoList.Length; i++)
            {
                AddRadioButton(_charSkinMatInfo_SO.charSkinInfoList[i].skincolorIndex, _charSkinMatInfo_SO.charSkinInfoList[i].skinColorName, _charSkinMatInfo_SO.charSkinInfoList[i].skincolorHexValue);
            }

            _toggles[0].isOn = true;
        }

        public void AddRadioButton(int togglecolorIndex, string toggleColorName, string toggleColor)
        {
            GameObject toggleObject = Instantiate(_colorTogglePrefab.gameObject, _colorToggleGroupTransform);
            CharacterSkinColorSelectView colorButtonSelectObj = toggleObject.GetComponent<CharacterSkinColorSelectView>();
            Toggle newToggle = toggleObject.GetComponent<Toggle>();
            colorButtonSelectObj.Initialize(togglecolorIndex, toggleColorName, toggleColor);

            _toggles.Add(newToggle);
            newToggle.onValueChanged.AddListener((isOn) => OnToggleChanged(newToggle, isOn, togglecolorIndex));
        }

        private void OnToggleChanged(Toggle changedToggle, bool isOn, int index)
        {
            if (isOn)
            {
                DeactivateOtherToggles(changedToggle);
                PlayerLobbyStateManager.Instance.ChangeCharacterSkin(index);
            }

            if (_isInitialSetup) return;
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.CharacterSkinChanged);
        }

        private void DeactivateOtherToggles(Toggle changedToggle)
        {
            foreach (var toggle in _toggles)
            {
                if (toggle != changedToggle)
                {
                    toggle.isOn = false;
                }
            }
        }

        public void HideAllToggles()
        {
            foreach (var toggle in _toggles)
            {
                toggle.gameObject.SetActive(false);
            }
        }

        public void ShowAllToggles()
        {
            foreach (var toggle in _toggles)
            {
                toggle.gameObject.SetActive(true);
            }
        }

        private void OnReadyButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ButtonClick);
            PlayerLobbyStateManager.Instance.SetPlayerReady();
            _readyButtonPrefab.gameObject.SetActive(false);
            _notReadyButtonPrefab.gameObject.SetActive(true);
            HideAllToggles();
        }

        private void OnNotReadyButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ButtonClick);
            PlayerLobbyStateManager.Instance.SetPlayerNotReady();
            _notReadyButtonPrefab.gameObject.SetActive(false);
            _readyButtonPrefab.gameObject.SetActive(true);
            ShowAllToggles();
        }

        private void OnBackToStartMenuButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ButtonClick);
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
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ButtonClick);

            if (NetworkManager.Singleton.IsServer)
            {
                LobbyManager.Instance.DeleteLobby();
            }
            else
            {
                LobbyManager.Instance.LeaveLobby();
                NetworkManager.Singleton.Shutdown();
            }
            SceneLoader.Instance.LoadScene(SceneName.StartScene, false);
        }

        private void OnNoButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ButtonClick);
            HideBackToMainMenuConfirmationPopup();
        }

        public void EnableView()
        {
            gameObject.SetActive(true);
        }

        public void DisableView()
        {
            gameObject.SetActive(false);
        }

        private void HandleClientConnectCallbackCharSelectUI(ulong clientID)
        {
            UpdateKickButtonVisibility();
        }

        private void HandleClientDisconnectCallbackCharSelectUI(ulong clientID)
        {
            UpdateKickButtonVisibility();

            if ((NetworkManager.Singleton.IsServer && clientID == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.ConnectedClients.Count <= 1) || !NetworkManager.Singleton.IsServer)
            {
                ShowDisconnectionCharSelectUI();
            }
        }

        private void ShowDisconnectionCharSelectUI()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.DisconnectionPopUp);
            _disconnectedPopUp.SetActive(true);
            StartCoroutine(DisconnectedCountdownSequence());
        }

        private IEnumerator DisconnectedCountdownSequence()
        {
            float currentTime = _disconnectedCountdownTime;

            while (currentTime > 0)
            {
                _disconnectedCountdownCharSelectUIText.text = "Returning To Main Menu In... " + Mathf.Ceil(currentTime).ToString() + "s";
                currentTime -= 1f;
                yield return new WaitForSeconds(1f);
            }

            _disconnectedPopUp.SetActive(false);
            SceneLoader.Instance.LoadScene(SceneName.StartScene, false);
        }

        private void ShowBackToMainMenuConfirmationPopup()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ConfirmationPopUp);
            _leaveLobbyConfirmationPopUp.SetActive(true);
        }

        private void HideBackToMainMenuConfirmationPopup()
        {
            _leaveLobbyConfirmationPopUp.SetActive(false);
        }

        private void CreateKickButtons()
        {
            RectTransform rt = _kickButtonPrefab.GetComponent<RectTransform>();
            float spacing = (rt.rect.size.x) + 20f;
            float startX = -((buttonOrder.Length - 1) * spacing) / 2f;

            for (int i = 0; i < buttonOrder.Length; i++)
            {
                GameObject newButton = Instantiate(_kickButtonPrefab, _kickButtonContainer);

                float posX = startX + i * spacing;
                newButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0);

                int buttonIndex = buttonOrder[i];
                newButton.GetComponent<Button>().onClick.AddListener(() => OnKickButtonClicked(buttonIndex));
                _buttonsList.Add(newButton);
            }

            foreach (var button in _buttonsList)
            {
                button.SetActive(false);
            }
        }

        void OnKickButtonClicked(int buttonIndex)
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ButtonClick);
            if (NetworkManager.Singleton.IsServer)
            {
                MultiplayerManager.Instance.KickPlayer(buttonIndex);
            }
        }

        private void UpdateKickButtonVisibility()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var clients = NetworkManager.Singleton.ConnectedClients;

                for (int i = 0; i < buttonOrder.Length; i++)
                {
                    if (buttonOrder[i] < clients.Count && buttonOrder[i] > 0)
                    {
                        _buttonsList[i].SetActive(true);
                    }
                    else
                    {
                        _buttonsList[i].SetActive(false);
                    }
                }
            }
        }
    }
}
