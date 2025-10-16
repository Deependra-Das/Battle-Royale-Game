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

        [Range(0, 1)] public float footstepAudioVolume = 1.0f;
        [Range(0, 1)] public float tilepopAudioVolume = 1.0f;

        protected override void Awake()
        {
            base.Awake();
        }

        void Start()
        {
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
                        AudioSource.PlayClipAtPoint(_footstepsAudioList[index], position, footstepAudioVolume);
                    }
                    else
                    {
                        Debug.LogWarning("Audio not found for type: " + audioType);
                    }
                    break;

                case AudioType.PlayerJumpLand:

                    if (_jumpLandAudioClip != null)
                    {
                        AudioSource.PlayClipAtPoint(_jumpLandAudioClip, position, footstepAudioVolume);
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
                AudioSource.PlayClipAtPoint(clip, position, tilepopAudioVolume);
            }
            else
            {
                Debug.LogWarning("Audio not found for type: " + audioType);
            }
        }
    }
}