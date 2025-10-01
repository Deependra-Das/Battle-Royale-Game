using BattleRoyale.Main;
using BattleRoyale.Network;
using BattleRoyale.Scene;
using System.Collections.Generic;
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
        [SerializeField] private Toggle _capacityNumTogglePrefab;
        [SerializeField] private Transform _capacityNumToggleGroupTransform;
        [SerializeField] private Button _createLobbyButtonPrefab;
        private List<Toggle> toggles = new List<Toggle>();

        [Header("Join Lobby Content")]
        [SerializeField] private Button _quickJoinButtonPrefab;
        [SerializeField] private Button _joinLobbyWithCodeButtonPrefab;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _createLobbyButtonPrefab.onClick.AddListener(OnCreateLobbyButtonClicked);
            _quickJoinButtonPrefab.onClick.AddListener(OnQuickJoinButtonClicked);
            _backToStartMenuButtonPrefab.onClick.AddListener(OnBackToStartMenuButtonClicked);

            _createLobbyToggle.onValueChanged.AddListener((isOn) => HandleMainTabSwitch(isOn, 1));
            _joinLobbyToggle.onValueChanged.AddListener((isOn) => HandleMainTabSwitch(isOn, 2));
        }

        private void UnsubscribeToEvents()
        {
            _createLobbyButtonPrefab.onClick.RemoveListener(OnCreateLobbyButtonClicked);
            _quickJoinButtonPrefab.onClick.RemoveListener(OnQuickJoinButtonClicked);
            _backToStartMenuButtonPrefab.onClick.RemoveListener(OnBackToStartMenuButtonClicked);

            _createLobbyToggle.onValueChanged.RemoveListener((isOn) => HandleMainTabSwitch(isOn, 1));
            _joinLobbyToggle.onValueChanged.RemoveListener((isOn) => HandleMainTabSwitch(isOn, 2));

            foreach (var toggle in toggles)
            {
                toggle.onValueChanged.RemoveListener((isOn) => OnCapacityNumToggleChanged(toggle, isOn, toggle.group.transform.GetSiblingIndex()));
            }
        }

        private void Awake()
        {
            CreateColorButtons();
            HandleMainTabSwitch(true, 1);
        }

        void HandleMainTabSwitch(bool isOn, int tabIndex)
        {
            if (isOn)
            {
                if (tabIndex == 1)
                {
                    _joinLobbyToggle.isOn = false;
                    _createLobbyTabContainer.SetActive(true);
                    _joinLobbyTabContainer.SetActive(false);
                }
                else if (tabIndex == 2)
                {
                    _createLobbyToggle.isOn = false;
                    _createLobbyTabContainer.SetActive(false);
                    _joinLobbyTabContainer.SetActive(true);
                }
            }
        }
        void CreateColorButtons()
        {
            int maxLobbySize = MultiplayerManager.MAX_LOBBY_SIZE;

            for (int i = 1; i <= maxLobbySize; i++)
            {
                AddRadioButton(i);
            }

            toggles[0].isOn = true;
        }

        public void AddRadioButton(int index)
        {
            GameObject toggleObject = Instantiate(_capacityNumTogglePrefab.gameObject, _capacityNumToggleGroupTransform);
            Toggle newToggle = toggleObject.GetComponent<Toggle>();
            newToggle.GetComponentInChildren<TMP_Text>().text = index.ToString();
            toggles.Add(newToggle);
            newToggle.onValueChanged.AddListener((isOn) => OnCapacityNumToggleChanged(newToggle, isOn, index));
        }

        private void OnCapacityNumToggleChanged(Toggle changedToggle, bool isOn, int index)
        {
            if (isOn)
            {
                DeactivateOtherCapacityNumToggles(changedToggle);
            }
        }

        private void DeactivateOtherCapacityNumToggles(Toggle changedToggle)
        {
            foreach (var toggle in toggles)
            {
                if (toggle != changedToggle)
                {
                    toggle.isOn = false;
                }
            }
        }

        private void OnCreateLobbyButtonClicked()
        {
            LobbyManager.Instance.CreateLobby("LobbyName", false);
        }

        private void OnQuickJoinButtonClicked()
        {
            LobbyManager.Instance.QuickJoin();
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
