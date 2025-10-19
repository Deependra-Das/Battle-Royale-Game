using BattleRoyale.AudioModule;
using BattleRoyale.MainModule;
using BattleRoyale.SceneModule;
using NUnit.Framework;
using System.Collections.Generic;
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
        [SerializeField] private Button _howToPlayButton;
        [SerializeField] private Button _audioSettingsButton;
        [SerializeField] private Button _quitGameButtonPrefab;

        [Header("Success PopUp")]
        [SerializeField] private GameObject _successPopup;
        [SerializeField] private TMP_Text _successMessageText;
        [SerializeField] private Button _okButton;

        [Header("Change Username PopUp")]
        [SerializeField] private Button _saveUsernameButton;
        [SerializeField] private Button _cancelUsernameChangeButton;
        [SerializeField] private GameObject _usernameInputPopup;
        [SerializeField] private TMP_InputField _usernameInputField;
        [SerializeField] private TMP_Text _errorMessageText;

        [Header("How To Play PopUp")]
        [SerializeField] private GameObject _howToPlayPopup;
        [SerializeField] private Image _displayImage;
        [SerializeField] private TMP_Text _currentImageIndexText;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _prevButton;
        [SerializeField] private Button _closeHowToPlayButton;

        [Header("Audio Settings PopUp")]
        [SerializeField] private GameObject _audioSettingsPopup;
        [SerializeField] private Slider _bgmVolumeSlider;
        [SerializeField] private Slider _uiSFXVolumeSlider;
        [SerializeField] private Slider _playerSFXVolumeSlider;
        [SerializeField] private Slider _tileFXVolumeSlider;
        [SerializeField] private Button _saveAudioSettingsButton;
        [SerializeField] private Button _cancelAudioSettingsButton;
        [SerializeField] private Button _restoreDefaultButton;

        private List<Sprite> _galleryImageList;
        private int _currentImageIndex = 0;

        private const int _minUserNameLength = 3;
        private const int _maxUserNameLength = 15;

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _newGameButton.onClick.AddListener(OnNewGameButtonClicked);
            _quitGameButtonPrefab.onClick.AddListener(OnQuitGameButtonClicked);
            _saveUsernameButton.onClick.AddListener(OnSaveUsernameButtonClicked);
            _cancelUsernameChangeButton.onClick.AddListener(OnCancelUsernameChangeButtonClicked);
            _okButton.onClick.AddListener(OnOkButtonClicked);
            _changeUsernameButton.onClick.AddListener(OnChangeUsernameButtonClicked);
            _howToPlayButton.onClick.AddListener(OnHowToPlayButonClicked);
            _closeHowToPlayButton.onClick.AddListener(OnCloseHowToPlayButonClicked);
            _audioSettingsButton.onClick.AddListener(OnAudioSettingsButonClicked);
            _saveAudioSettingsButton.onClick.AddListener(OnSaveAudioSettingsButonClicked);
            _cancelAudioSettingsButton.onClick.AddListener(OnCancelAudioSettingsButonClicked);
            _restoreDefaultButton.onClick.AddListener(OnRestoreDefaultAudioSettingsButonClicked);
            _nextButton.onClick.AddListener(OnNextImageButtonClicked);
            _prevButton.onClick.AddListener(OnPreviousImageButtonClicked);
        }

        private void UnsubscribeToEvents()
        {
            _newGameButton.onClick.RemoveListener(OnNewGameButtonClicked);
            _quitGameButtonPrefab.onClick.RemoveListener(OnQuitGameButtonClicked);
            _saveUsernameButton.onClick.RemoveListener(OnSaveUsernameButtonClicked);
            _cancelUsernameChangeButton.onClick.RemoveListener(OnCancelUsernameChangeButtonClicked);
            _okButton.onClick.RemoveListener(OnOkButtonClicked);
            _changeUsernameButton.onClick.RemoveListener(OnChangeUsernameButtonClicked);
            _nextButton.onClick.RemoveListener(OnNextImageButtonClicked);
            _prevButton.onClick.RemoveListener(OnPreviousImageButtonClicked);
            _howToPlayButton.onClick.RemoveListener(OnHowToPlayButonClicked);
            _closeHowToPlayButton.onClick.RemoveListener(OnCloseHowToPlayButonClicked);
            _audioSettingsButton.onClick.RemoveListener(OnAudioSettingsButonClicked);
            _saveAudioSettingsButton.onClick.RemoveListener(OnSaveAudioSettingsButonClicked);
            _cancelAudioSettingsButton.onClick.RemoveListener(OnCancelAudioSettingsButonClicked);
            _restoreDefaultButton.onClick.RemoveListener(OnRestoreDefaultAudioSettingsButonClicked);
            _nextButton.onClick.RemoveListener(OnNextImageButtonClicked);
            _prevButton.onClick.RemoveListener(OnPreviousImageButtonClicked);
        }

        public void Initialize(List<Sprite> galleryImages)
        {
            _galleryImageList = galleryImages;
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
            if (username.Length < _minUserNameLength || username.Length > _maxUserNameLength)
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
            _successMessageText.text = "Username Saved Successfully!";
            ShowSuccessPopup();
        }

        private void OnOkButtonClicked()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ButtonClick);
            HideSuccessPopup();
        }

        private void OnCancelUsernameChangeButtonClicked()
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

        void OnNextImageButtonClicked()
        {
            if (_currentImageIndex < _galleryImageList.Count - 1)
            {
                _currentImageIndex++;
                SetHowToPlayDisplayImage();
                UpdateCurrentImageIndexText();
            }
            else
            {
                _nextButton.interactable = false;
            }

            _prevButton.interactable = true;
        }

        void OnPreviousImageButtonClicked()
        {
            if (_currentImageIndex > 0)
            {
                _currentImageIndex--;
                SetHowToPlayDisplayImage();
                UpdateCurrentImageIndexText();
            }
            else
            {
                _prevButton.interactable = false;
            }

            _nextButton.interactable = true;
        }

        private void UpdateCurrentImageIndexText()
        {
            _currentImageIndexText.text = (_currentImageIndex + 1).ToString()+"/"+_galleryImageList.Count;
        }

        private void SetHowToPlayDisplayImage()
        {
            _displayImage.sprite = _galleryImageList[_currentImageIndex];
        }

        private void OnHowToPlayButonClicked()
        {
            _currentImageIndex = 0;
            SetHowToPlayDisplayImage();
            UpdateCurrentImageIndexText();
            ShowHowTopPlayPopup();
        }

        private void ShowHowTopPlayPopup()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ConfirmationPopUp);
            _howToPlayPopup.SetActive(true);
        }

        private void OnCloseHowToPlayButonClicked()
        {
            HideHowTopPlayPopup();
        }

        private void HideHowTopPlayPopup()
        {
            _howToPlayPopup.SetActive(false);
        }    

        private void OnAudioSettingsButonClicked()
        {
            SetAudioSliderValue();
            ShowAudioSettingsPopup();
        }

        private void ShowAudioSettingsPopup()
        {
            AudioManager.Instance.PlaySFX(AudioModule.AudioType.ConfirmationPopUp);
            _audioSettingsPopup.SetActive(true);
        }
        private void HideAudioSettingsPopup()
        {
            _audioSettingsPopup.SetActive(false);
        }

        private void OnSaveAudioSettingsButonClicked()
        {
            AudioManager.Instance.SetAllAudioVolumes(_bgmVolumeSlider.value, _playerSFXVolumeSlider.value, _uiSFXVolumeSlider.value, _tileFXVolumeSlider.value);
            HideAudioSettingsPopup();
            _successMessageText.text = "Audio Settings Saved Successfully!";
            ShowSuccessPopup();
        }

        private void OnCancelAudioSettingsButonClicked()
        {
            HideAudioSettingsPopup();
        }

        private void OnRestoreDefaultAudioSettingsButonClicked()
        {
            _bgmVolumeSlider.value = AudioManager.Instance.DefaultBGMVolume;
            _playerSFXVolumeSlider.value = AudioManager.Instance.DefaultPlayerSFXVolume;
            _uiSFXVolumeSlider.value = AudioManager.Instance.DefaultUIVolume;
            _tileFXVolumeSlider.value = AudioManager.Instance.DefaultTilePopVolume;
        }
        
        private void SetAudioSliderValue()
        {
            _bgmVolumeSlider.value = AudioManager.Instance.BGMVolume;
            _playerSFXVolumeSlider.value = AudioManager.Instance.PlayerSFXVolume;
            _uiSFXVolumeSlider.value = AudioManager.Instance.UIVolume;
            _tileFXVolumeSlider.value = AudioManager.Instance.TilePopVolume;
        }   
    }
}
