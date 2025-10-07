using BattleRoyale.MainModule;
using BattleRoyale.SceneModule;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.UIModule
{
    public class StartMenuUIView : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _quitGameButtonPrefab;
        [SerializeField] private Button _saveUsernameButton;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _changeUsernameButton;
        [SerializeField] private GameObject _usernameInputPopup;
        [SerializeField] private GameObject _successPopup;
        [SerializeField] private TMP_InputField _usernameInputField;
        [SerializeField] private TMP_Text _errorMessageText;
        [SerializeField] private GameObject _topBar;
        [SerializeField] private TMP_Text _usernameDisplayText;

        private const int minUserNameLength = 3;
        private const int maxUserNameLength = 15;

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
            _errorMessageText.gameObject.SetActive(false);
            HideSuccessPopup();
            _usernameInputPopup.SetActive(true);
        }
        private void HideUsernameInputPopup()
        {
            _errorMessageText.gameObject.SetActive(false);
            _usernameInputPopup.SetActive(false);
        }


        void CheckPlayerNameExists()
        {
            string username = PlayerPrefs.GetString(GameManager.UsernameKey).ToString();

            if (string.IsNullOrEmpty(username))
            {
                _topBar.SetActive(false);
                ShowUsernameInputPopup();
            }
            else
            {
                _usernameDisplayText.text = username;
                _topBar.SetActive(true);
            }
        }

        private void OnSaveUsernameButtonClicked()
        {
            string username = _usernameInputField.text;

            if (string.IsNullOrEmpty(username))
            {
                _errorMessageText.text = "Username cannot be empty!";
                _errorMessageText.gameObject.SetActive(true);
                return;
            }
            if (username.Length < minUserNameLength || username.Length > maxUserNameLength)
            {
                _errorMessageText.text = "Username must be 3-16 characters long!";
                _errorMessageText.gameObject.SetActive(true);
                return;
            }

            if (!IsValidPlayerName(username))
            {
                _errorMessageText.text = "Username contains invalid characters!";
                _errorMessageText.gameObject.SetActive(true);
                return;
            }

            PlayerPrefs.SetString(GameManager.UsernameKey, username);
            PlayerPrefs.Save();

            _errorMessageText.text = string.Empty;
            _errorMessageText.gameObject.SetActive(false);

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
            _successPopup.SetActive(true);
        }

        private void HideSuccessPopup()
        {
            _successPopup.SetActive(false);
        }
    }
}
