using BattleRoyale.AudioModule;
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
        [Header("Main Menu Content")]
        [SerializeField] private GameObject _topBar;
        [SerializeField] private TMP_Text _usernameDisplayText;
        [SerializeField] private Button _changeUsernameButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _quitGameButtonPrefab;

        [Header("Success PopUp")]
        [SerializeField] private GameObject _successPopup;
        [SerializeField] private Button _okButton;

        [Header("Change Username PopUp")]
        [SerializeField] private Button _saveUsernameButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private GameObject _usernameInputPopup;
        [SerializeField] private TMP_InputField _usernameInputField;
        [SerializeField] private TMP_Text _errorMessageText;


        private const int minUserNameLength = 3;
        private const int maxUserNameLength = 15;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _newGameButton.onClick.AddListener(OnNewGameButtonClicked);
            _quitGameButtonPrefab.onClick.AddListener(OnQuitGameButtonClicked);
            _saveUsernameButton.onClick.AddListener(OnSaveUsernameButtonClicked);
            _cancelButton.onClick.AddListener(OnCancelButtonClicked);
            _okButton.onClick.AddListener(OnOkButtonClicked);
            _changeUsernameButton.onClick.AddListener(OnChangeUsernameButtonClicked);
        }

        private void UnsubscribeToEvents()
        {
            _newGameButton.onClick.RemoveListener(OnNewGameButtonClicked);
            _quitGameButtonPrefab.onClick.RemoveListener(OnQuitGameButtonClicked);
            _saveUsernameButton.onClick.RemoveListener(OnSaveUsernameButtonClicked);
            _cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
            _okButton.onClick.RemoveListener(OnOkButtonClicked);
            _changeUsernameButton.onClick.RemoveListener(OnChangeUsernameButtonClicked);
        }

        private void OnNewGameButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ButtonClick);
            SceneLoader.Instance.LoadScene(SceneName.LobbyScene, false);
        }

        private void OnQuitGameButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ButtonClick);
            Application.Quit();
        }

        public void EnableView()
        {
            HideUsernameInputPopup();
            CheckPlayerNameExists();
            gameObject.SetActive(true);
        }

        public void DisableView()
        {
            gameObject.SetActive(false);
        }

        private void OnChangeUsernameButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ButtonClick);
            ShowUsernameInputPopup();
            HideSuccessPopup();
        }

        private void ShowUsernameInputPopup()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ConfirmationPopUp);
            _usernameInputField.text =string.Empty;
            _errorMessageText.text = string.Empty;
            _usernameInputPopup.SetActive(true);
        }

        private void HideUsernameInputPopup()
        {
            _errorMessageText.text = string.Empty;
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
                AudioManager.Instance.PlaySFX(AudioModule.AudioType.ErrorPopUp);
                return;
            }
            if (username.Length < minUserNameLength || username.Length > maxUserNameLength)
            {
                _errorMessageText.text = "Username must be 3-16 characters long!";
                AudioManager.Instance.PlaySFX(AudioModule.AudioType.ErrorPopUp);
                return;
            }

            if (!IsValidPlayerName(username))
            {
                _errorMessageText.text = "Username contains invalid characters!";
                AudioManager.Instance.PlaySFX(AudioModule.AudioType.ErrorPopUp);
                return;
            }

            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ButtonClick);
            PlayerPrefs.SetString(GameManager.UsernameKey, username);
            PlayerPrefs.Save();

            _errorMessageText.text = string.Empty;

            HideUsernameInputPopup();
            CheckPlayerNameExists();
            ShowSuccessPopup();
        }

        private void OnOkButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ButtonClick);
            HideSuccessPopup();
        }

        private void OnCancelButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ButtonClick);
            HideUsernameInputPopup();
        }

        private bool IsValidPlayerName(string username)
        {
            string pattern = "^[A-Za-z0-9]+$";
            return Regex.IsMatch(username, pattern);
        }

        private void ShowSuccessPopup()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.SuccessPopUp);
            _successPopup.SetActive(true);
        }

        private void HideSuccessPopup()
        {
            _successPopup.SetActive(false);
        }
    }
}
