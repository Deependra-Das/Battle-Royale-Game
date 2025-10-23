using BattleRoyale.CharacterSelectionModule;
using BattleRoyale.LobbyModule;
using BattleRoyale.SceneModule;
using System.Collections.Generic;
using Unity.Netcode;

namespace BattleRoyale.NetworkModule
{
    public class PlayerLobbyStateManager : NetworkBehaviour
    {
        public static PlayerLobbyStateManager Instance { get; private set; }

        private Dictionary<ulong, (string playerName, bool isReady)> _playerStateDictionary;

        private void Awake()
        {
            Instance = this;
            _playerStateDictionary = new Dictionary<ulong, (string, bool)>();
        }

        private void OnEnable()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void OnDisable()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (_playerStateDictionary.ContainsKey(clientId))
            {
                _playerStateDictionary.Remove(clientId);
            }
        }

        public void SetPlayerReady()
        {
            SetPlayerReadyServerRpc();
        }

        public void SetPlayerNotReady()
        {
            SetPlayerNotReadyServerRpc();
        }

        public void ChangeCharacterSkin(int skinColorIndex)
        {
            ChangeCharacterSkinServerRpc(skinColorIndex);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            ulong clientId = serverRpcParams.Receive.SenderClientId;

            if (!_playerStateDictionary.ContainsKey(clientId))
            {
                _playerStateDictionary.Add(clientId, ("Player" + clientId, true));
            }
            else
            {
                _playerStateDictionary[clientId] = (_playerStateDictionary[clientId].playerName, true);
            }

            CharacterManager.Instance.SetCharacterStatus(clientId, true);

            if (NetworkManager.Singleton.ConnectedClients.Count == MultiplayerManager.Instance.CURRENT_LOBBY_SIZE)
            {
                bool allClientsReady = true;

                foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    if (!_playerStateDictionary.ContainsKey(clientID) || !_playerStateDictionary[clientID].isReady)
                    {
                        allClientsReady = false;
                        break;
                    }
                }

                if (allClientsReady)
                {
                    LobbyManager.Instance.DeleteLobby();
                    SceneLoader.Instance.LoadScene(SceneName.GameScene, true);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerNotReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            ulong clientId = serverRpcParams.Receive.SenderClientId;

            if (_playerStateDictionary.ContainsKey(clientId))
            {
                _playerStateDictionary[clientId] = (_playerStateDictionary[clientId].playerName, false);
            }

            CharacterManager.Instance.SetCharacterStatus(clientId, false);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangeCharacterSkinServerRpc(int skinColorIndex, ServerRpcParams serverRpcParams = default)
        {
            ulong clientId = serverRpcParams.Receive.SenderClientId;
            int colorIndex = skinColorIndex;

            CharacterManager.Instance.SetCharacterSkin(clientId, colorIndex);
            PlayerSessionManager.Instance.SetPlayerCharacterSkinColorServerRpc(clientId, colorIndex);
        }
    }
}
