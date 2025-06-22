using BattleRoyale.Main;
using BattleRoyale.Scene;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UI
{
    public class StartMenuUIView : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _quitGameButtonPrefab;
        [SerializeField] private Button _saveUsernameButton;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _changeUsernameButton;
        [SerializeField] private GameObject _usernameInputPopup;
        [SerializeField] private GameObject successPopup;
        [SerializeField] private TMP_InputField _usernameInputField;
        [SerializeField] private TMP_Text errorMessageText;
        [SerializeField] private GameObject _topBar;
        [SerializeField] private TMP_Text usernameDisplayText;

        private const string UsernameKey = "Username";

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _newGameButton.onClick.AddListener(OnNewGameButtonClicked);
            _quitGameButtonPrefab.onClick.AddListener(OnQuitGameButtonClicked);
            _saveUsernameButton.onClick.AddListener(OnSaveUsernameButtonClicked);
            _okButton.onClick.AddListener(HideSuccessPopup);
            _changeUsernameButton.onClick.AddListener(ShowUsernameInputPopup);
        }

        private void UnsubscribeToEvents()
        {
            _newGameButton.onClick.RemoveListener(OnNewGameButtonClicked);
            _quitGameButtonPrefab.onClick.RemoveListener(OnQuitGameButtonClicked);
            _saveUsernameButton.onClick.RemoveListener(OnSaveUsernameButtonClicked);
            _okButton.onClick.RemoveListener(HideSuccessPopup);
            _changeUsernameButton.onClick.RemoveListener(ShowUsernameInputPopup);
        }

        private void OnNewGameButtonClicked()
        {
            SceneLoader.Instance.LoadScene(SceneName.LobbyScene, false);
        }

        private void OnQuitGameButtonClicked()
        {
           Application.Quit();
        }

        public void EnableView()
        {
            CheckPlayerNameExists();
            gameObject.SetActive(true);
        }

        public void DisableView()
        {
            gameObject.SetActive(false);
        }

        private void ShowUsernameInputPopup()
        {
            errorMessageText.gameObject.SetActive(false);
            HideSuccessPopup();
            _usernameInputPopup.SetActive(true);
        }
        private void HideUsernameInputPopup()
        {
            errorMessageText.gameObject.SetActive(false);
            _usernameInputPopup.SetActive(false);
        }


        void CheckPlayerNameExists()
        {
            string username = PlayerPrefs.GetString(UsernameKey).ToString();

            if (string.IsNullOrEmpty(username))
            {
                _topBar.SetActive(false);
                ShowUsernameInputPopup();
            }
            else
            {
                usernameDisplayText.text = username;
                _topBar.SetActive(true);
            }
        }

        private void OnSaveUsernameButtonClicked()
        {
            string username = _usernameInputField.text;

            if (string.IsNullOrEmpty(username))
            {
                errorMessageText.text = "Username cannot be empty!";
                errorMessageText.gameObject.SetActive(true);
                return;
            }
            if (username.Length < 3 || username.Length > 16)
            {
                errorMessageText.text = "Username must be 3-16 characters long!";
                errorMessageText.gameObject.SetActive(true);
                return;
            }

            if (!IsValidPlayerName(username))
            {
                errorMessageText.text = "Username contains invalid characters!";
                errorMessageText.gameObject.SetActive(true);
                return;
            }

            PlayerPrefs.SetString(UsernameKey, username);
            PlayerPrefs.Save();

            errorMessageText.text = string.Empty;
            errorMessageText.gameObject.SetActive(false);

            HideUsernameInputPopup();
            CheckPlayerNameExists();
            ShowSuccessPopup();
        }

        private bool IsValidPlayerName(string username)
        {
            string pattern = "^[A-Za-z0-9]+$";
            return Regex.IsMatch(username, pattern);
        }

        private void ShowSuccessPopup()
        {
            successPopup.SetActive(true);
        }

        private void HideSuccessPopup()
        {
            successPopup.SetActive(false);
        }
    }
}
