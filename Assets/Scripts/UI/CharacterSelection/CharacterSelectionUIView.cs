using BattleRoyale.Event;
using BattleRoyale.Main;
using BattleRoyale.Network;
using BattleRoyale.Scene;
using System.Collections.Generic;
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
        [SerializeField] private GameObject _hostDisconnectedPanel;
        [SerializeField] private GameObject _clientDisconnectedPanel;
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
            EventBusManager.Instance.Subscribe(EventName.HostDisconnected, HandleHostDisconnectCharSelectionUI);
        }

        private void UnsubscribeToEvents()
        {
            _readyButtonPrefab.onClick.RemoveListener(OnReadyButtonClicked);
            _notReadyButtonPrefab.onClick.RemoveListener(OnNotReadyButtonClicked);
            _backToStartMenuButtonPrefab.onClick.RemoveListener(OnBackToStartMenuButtonClicked);
            EventBusManager.Instance.Unsubscribe(EventName.HostDisconnected, HandleHostDisconnectCharSelectionUI);

            foreach (var toggle in toggles)
            {
                toggle.onValueChanged.RemoveListener((isOn) => OnToggleChanged(toggle, isOn, toggle.group.transform.GetSiblingIndex()));
            }
        }

        private void Start()
        {
            CreateColorButtons();
            _hostDisconnectedPanel.SetActive(false);
            _notReadyButtonPrefab.gameObject.SetActive(false);
            _readyButtonPrefab.gameObject.SetActive(true);
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

        private void OnReadyButtonClicked()
        {
            PlayerLobbyStateManager.Instance.SetPlayerReady();
            _readyButtonPrefab.gameObject.SetActive(false);
            _notReadyButtonPrefab.gameObject.SetActive(true);
        }

        private void OnNotReadyButtonClicked()
        {
            PlayerLobbyStateManager.Instance.SetPlayerNotReady();
            _notReadyButtonPrefab.gameObject.SetActive(false);
            _readyButtonPrefab.gameObject.SetActive(true);
        }

        private void OnBackToStartMenuButtonClicked()
        {
            NetworkManager.Singleton.Shutdown();
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

        private void HandleHostDisconnectCharSelectionUI(object[] parameters)
        {
            _hostDisconnectedPanel.SetActive(true);
        }
    }
}
