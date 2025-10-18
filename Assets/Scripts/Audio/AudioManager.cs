using BattleRoyale.UtilitiesModule;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace BattleRoyale.AudioModule
{
    public class AudioManager : GenericMonoSingleton<AudioManager>
    {
        [SerializeField] private AudioSource audioSource_SFX;
        [SerializeField] private AudioSource audioSource_BGM;
        [SerializeField] private AudioScriptableObject audio_SO;

        private Dictionary<AudioType, AudioClip> _sfxAudioDictionary;
        private Dictionary<AudioType, AudioClip> _bgmAudioDictionary;
        private List<AudioClip> _footstepsAudioList;
        private AudioClip _jumpLandAudioClip;

        [Range(0, 1)] [SerializeField] private float _tilepopAudioVolume = 1.0f;
        [Range(0, 1)] [SerializeField] private float _bgmVolume = 1.0f;
        [Range(0, 1)] [SerializeField] private float _playerSFXVolume = 1.0f;
        [Range(0, 1)] [SerializeField] private float _uiSFXVolume = 1.0f;

        private const float _defaultBGMVolume = 0.05f;
        private const float _defaultPlayerSFXVolume = 1.0f;
        private const float _defaultUIVolume = 0.5f;
        private const float _defaultTilePopVolume = 1.0f;

        private const string BGMMusicVolumeKey = "BGMMusicVolume";
        private const string PlayerSFXVolumeKey = "PlayerSFXVolume";
        private const string UISFXVolumeKey = "UISFXVolume";
        private const string TilePopVolumeKey = "TilePopVolume";

        protected override void Awake()
        {
            base.Awake();
        }

        void Start()
        {
            LoadVolumeSettings();
            InitializeSFXAudio();
            InitializeBGMAudio();
            InitializePlayerAudio();
        }

        void InitializeSFXAudio()
        {
            _sfxAudioDictionary = new Dictionary<AudioType, AudioClip>();

            foreach (var entry in audio_SO.sfxAudioList)
            {
                if (!_sfxAudioDictionary.ContainsKey(entry.audioType))
                {
                    _sfxAudioDictionary.Add(entry.audioType, entry.audioClip);
                }
            }
        }

        void InitializeBGMAudio()
        {
            _bgmAudioDictionary = new Dictionary<AudioType, AudioClip>();

            foreach (var entry in audio_SO.bgmAudioList)
            {
                if (!_bgmAudioDictionary.ContainsKey(entry.audioType))
                {
                    _bgmAudioDictionary.Add(entry.audioType, entry.audioClip);
                }
            }
        }

        void InitializePlayerAudio()
        {
            _footstepsAudioList = new List<AudioClip>();

            var footstepClips = audio_SO.playerAudioList.FindAll(x => x.audioType == AudioType.PlayerFootstep);

            foreach (var entry in footstepClips)
            {
                _footstepsAudioList.Add(entry.audioClip);
            }

            _jumpLandAudioClip = audio_SO.playerAudioList.Find(x => x.audioType == AudioType.PlayerJumpLand).audioClip;
        }

        public void PlaySFX(AudioType audioType)
        {
            if (_sfxAudioDictionary.TryGetValue(audioType, out AudioClip clip))
            {
                audioSource_SFX.volume = _uiSFXVolume;
                audioSource_SFX.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning("Audio not found for type: " + audioType);
            }
        }

        public void PlayBGM(AudioType audioType)
        {
            if (_bgmAudioDictionary.TryGetValue(audioType, out AudioClip clip))
            {
                audioSource_BGM.loop = true;
                audioSource_BGM.clip = clip;
                audioSource_BGM.volume = _bgmVolume;
                audioSource_BGM.Play();
            }
            else
            {
                Debug.LogWarning("Audio not found for type: " + audioType);
            }
        }

        public void PlayFootStepsAudio(AudioType audioType, Vector3 position)
        {
            switch (audioType)
            {
                case AudioType.PlayerFootstep:
                    if (_footstepsAudioList.Count > 0)
                    {
                        var index = UnityEngine.Random.Range(0, _footstepsAudioList.Count);
                        AudioSource.PlayClipAtPoint(_footstepsAudioList[index], position, _playerSFXVolume);
                    }
                    else
                    {
                        Debug.LogWarning("Audio not found for type: " + audioType);
                    }
                    break;

                case AudioType.PlayerJumpLand:
                    if (_jumpLandAudioClip != null)
                    {
                        AudioSource.PlayClipAtPoint(_jumpLandAudioClip, position, _playerSFXVolume);
                    }
                    else
                    {
                        Debug.LogWarning("Audio not found for type: " + audioType);
                    }
                    break;
            }
        }

        public void PlayTilePopSFX(AudioType audioType, Vector3 position)
        {
            if (_sfxAudioDictionary.TryGetValue(audioType, out AudioClip clip))
            {
                AudioSource.PlayClipAtPoint(clip, position, _tilepopAudioVolume);
            }
            else
            {
                Debug.LogWarning("Audio not found for type: " + audioType);
            }
        }

        #region Volume Control Methods

        public void SetBGMVolume(float volume)
        {
            _bgmVolume = Mathf.Clamp(volume, 0f, 1f);
            SaveVolumeSetting(BGMMusicVolumeKey, _bgmVolume);
        }

        public void SetPlayerSFXVolume(float volume)
        {
            _playerSFXVolume = Mathf.Clamp(volume, 0f, 1f);
            SaveVolumeSetting(PlayerSFXVolumeKey, _playerSFXVolume);
        }

        public void SetUISFXVolume(float volume)
        {
            _uiSFXVolume = Mathf.Clamp(volume, 0f, 1f);
            SaveVolumeSetting(UISFXVolumeKey, _uiSFXVolume);
        }

        public void SetTilePopVolume(float volume)
        {
            _tilepopAudioVolume = Mathf.Clamp(volume, 0f, 1f);
            SaveVolumeSetting(TilePopVolumeKey, _tilepopAudioVolume);
        }

        public void SetAllAudioVolumes(float bgmVol, float playerSFXVol, float uiSFXVol, float tilePopVol)
        {
            SetBGMVolume(bgmVol);
            SetPlayerSFXVolume(playerSFXVol);
            SetUISFXVolume(uiSFXVol);
            SetTilePopVolume(tilePopVol);
        }

        public float BGMVolume { get { return _bgmVolume; } }
        public float PlayerSFXVolume { get { return _playerSFXVolume; } }
        public float UIVolume { get { return _uiSFXVolume; } }
        public float TilePopVolume { get { return _tilepopAudioVolume; } }

        public float DefaultBGMVolume { get { return _defaultBGMVolume; } }
        public float DefaultPlayerSFXVolume { get { return _defaultPlayerSFXVolume; } }
        public float DefaultUIVolume { get { return _defaultUIVolume; } }
        public float DefaultTilePopVolume { get { return _defaultTilePopVolume; } }

        #endregion

        #region Save & Load Methods

        private void SaveVolumeSetting(string key, float volume)
        {
            PlayerPrefs.SetFloat(key, volume);
            PlayerPrefs.Save();
        }

        private void LoadVolumeSettings()
        {
            _bgmVolume = PlayerPrefs.GetFloat(BGMMusicVolumeKey, _defaultBGMVolume);
            _playerSFXVolume = PlayerPrefs.GetFloat(PlayerSFXVolumeKey, _defaultPlayerSFXVolume);
            _uiSFXVolume = PlayerPrefs.GetFloat(UISFXVolumeKey, _defaultUIVolume);
            _tilepopAudioVolume = PlayerPrefs.GetFloat(TilePopVolumeKey, _defaultTilePopVolume);

            audioSource_BGM.volume = _bgmVolume;
        }

        #endregion
    }

}