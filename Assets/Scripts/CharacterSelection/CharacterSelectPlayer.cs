using BattleRoyale.Event;
using BattleRoyale.Network;
using BattleRoyale.Player;
using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace BattleRoyale.CharacterSelection
{
    public class CharacterSelectPlayer : NetworkBehaviour
    { 
        [SerializeField] private TMP_Text _usernameText;
        [SerializeField] private GameObject _readyStatusLabel;
        [SerializeField] private GameObject _notReadyStatusLabel;
        [SerializeField] private PlayerCharMatSkinColorScriptableObject _charSkinMatInfo_SO;
        [SerializeField] private SkinnedMeshRenderer[] _skinnedMeshRenderersForBodyParts;
        [SerializeField] private Button _kickButton;

        private NetworkVariable<FixedString128Bytes> _usernameNetworkText = new NetworkVariable<FixedString128Bytes>("Player");
        private NetworkVariable<bool> _readyStatusNetworkBool = new NetworkVariable<bool>(false);
        private NetworkVariable<int> _selectedMaterialIndex = new NetworkVariable<int>(0);

        private int _playerIndex = -1;
        private ulong _clientId = 0;

        private void OnEnable() => SubscribeToEvents();
        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            _usernameNetworkText.OnValueChanged += UpdateCharacterUsername;
            _readyStatusNetworkBool.OnValueChanged += UpdateCharacterLobbyStatus;
            _selectedMaterialIndex.OnValueChanged += ApplySelectedMaterial;

            _kickButton.onClick.AddListener(KickPlayer);
        }

        private void UnsubscribeToEvents()
        {
            _usernameNetworkText.OnValueChanged -= UpdateCharacterUsername;
            _readyStatusNetworkBool.OnValueChanged -= UpdateCharacterLobbyStatus;
            _selectedMaterialIndex.OnValueChanged -= ApplySelectedMaterial;

            _kickButton.onClick.RemoveListener(KickPlayer);
        }

        public override void OnNetworkSpawn()
        {
            _usernameText.text = _usernameNetworkText.Value.ToString();
            _readyStatusLabel.SetActive(_readyStatusNetworkBool.Value);
            _notReadyStatusLabel.SetActive(!_readyStatusNetworkBool.Value);

            _kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        }

        public void Initialize(int assignedClientIndex, ulong clientID, string usernameText)
        {
            if (IsServer)
            {
                SetPlayerIndexForCharacter(assignedClientIndex);
                SetClientIdForCharacter(clientID);
                SetUsernameServerRpc(usernameText);          
            }
        }

        private void SetPlayerIndexForCharacter(int assignedPlayerIndex)
        {
            _playerIndex = assignedPlayerIndex;
        }

        private void SetClientIdForCharacter(ulong assignedClientID)
        {
            _clientId = assignedClientID;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetUsernameServerRpc(string newUsername)
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

        public void SetCharacterSkinMaterial(int materialIndex)
        {
            SetMaterialIndexServerRpc(materialIndex);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetMaterialIndexServerRpc(int materialIndex)
        {
            if (materialIndex >= 0 && materialIndex < _charSkinMatInfo_SO.charSkinInfoList.Length)
            {
                _selectedMaterialIndex.Value = materialIndex;
            }
        }

        private void ApplySelectedMaterial(int oldMaterialIndex, int newMaterialIndex)
        {
            if (_skinnedMeshRenderersForBodyParts != null && newMaterialIndex >= 0 && newMaterialIndex < _charSkinMatInfo_SO.charSkinInfoList.Length)
            {
                foreach (SkinnedMeshRenderer renderer in _skinnedMeshRenderersForBodyParts)
                {
                    Material[] materialsToRemap = renderer.materials;
                    materialsToRemap[0] = _charSkinMatInfo_SO.charSkinInfoList[newMaterialIndex].skinColorMaterial;
                    renderer.materials = materialsToRemap;
                }
            }
        }

        public void KickPlayer()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Debug.Log("Kick" + _clientId);
                MultiplayerManager.Instance.KickPlayer(_clientId);
            }
        }

        private void ShowCharacter() => this.gameObject.SetActive(true);
        private void HideCharacter() => this.gameObject.SetActive(false);
    }
}
