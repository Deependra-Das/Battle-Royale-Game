using BattleRoyale.Event;
using BattleRoyale.Network;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

namespace BattleRoyale.CharacterSelection
{
    public class CharacterSelectPlayer : NetworkBehaviour
    { 
        [SerializeField] private TMP_Text _usernameText;
        [SerializeField] private GameObject _readyStatusLabel;
        [SerializeField] private GameObject _notReadyStatusLabel;

        private NetworkVariable<FixedString128Bytes> _usernameNetworkText = new NetworkVariable<FixedString128Bytes>("Player");
        private NetworkVariable<bool> _readyStatusNetworkBool = new NetworkVariable<bool>(false);

        private int _playerIndex = -1;

        private void OnEnable() => SubscribeToEvents();
        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _usernameNetworkText.OnValueChanged += UpdateCharacterUsername;
            _readyStatusNetworkBool.OnValueChanged += UpdateCharacterLobbyStatus;
        }

        private void UnsubscribeToEvents()
        {
            _usernameNetworkText.OnValueChanged -= UpdateCharacterUsername;
            _readyStatusNetworkBool.OnValueChanged -= UpdateCharacterLobbyStatus;
        }

        public override void OnNetworkSpawn()
        {
            _usernameText.text = _usernameNetworkText.Value.ToString();
            _readyStatusLabel.SetActive(_readyStatusNetworkBool.Value);
            _notReadyStatusLabel.SetActive(!_readyStatusNetworkBool.Value);
        }

        public void Initialize(int assignedClientIndex, string usernameText)
        {
            if (IsServer)
            {
                SetPlayerIndexForCharacter(assignedClientIndex);
                SetUsernameServerRpc(usernameText);
            }
        }

        public void SetPlayerIndexForCharacter(int assignedClientIndex)
        {
            _playerIndex = assignedClientIndex;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetUsernameServerRpc(string newUsername)
        {
            _usernameNetworkText.Value = newUsername;
        }

        private void UpdateCharacterUsername(FixedString128Bytes oldValue, FixedString128Bytes newValue)
        {
            _usernameText.text = _usernameNetworkText.Value.ToString();
        }

        public void SetCharacterLobbyStatus(bool isReady)
        {
            SetCharacterReadyStatusServerRpc(isReady);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetCharacterReadyStatusServerRpc(bool isReady)
        {
            _readyStatusNetworkBool.Value = isReady;
        }

        private void UpdateCharacterLobbyStatus(bool oldValue, bool newValue)
        {
            _readyStatusLabel.SetActive(_readyStatusNetworkBool.Value);
            _notReadyStatusLabel.SetActive(!_readyStatusNetworkBool.Value);
        }


        private void ShowCharacter() => this.gameObject.SetActive(true);
        private void HideCharacter() => this.gameObject.SetActive(false);
    }
}
