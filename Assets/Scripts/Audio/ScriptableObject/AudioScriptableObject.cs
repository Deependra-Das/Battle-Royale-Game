using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.AudioModule
{
    [CreateAssetMenu(fileName = "AudioScriptableObject", menuName = "ScriptableObjects/AudioScriptableObject")]
    public class AudioScriptableObject : ScriptableObject
    {
        public List<AudioEntry> bgmAudioList;
        public List<AudioEntry> sfxAudioList;
        public List<AudioEntry> playerAudioList;
    }

    [Serializable]
    public struct AudioEntry
    {
        public AudioType audioType;
        public AudioClip audioClip;
    }
}