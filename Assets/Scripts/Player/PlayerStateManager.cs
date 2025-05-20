using BattleRoyale.Scene;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStateManager : NetworkBehaviour
{
    public static PlayerStateManager Instance { get; private set; }

    private Dictionary<ulong, bool> _playerStateDictionary;

    private void Awake()
    {
        Instance = this;
        _playerStateDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams =default)
    {
        _playerStateDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;

        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(!_playerStateDictionary.ContainsKey(clientID) || !_playerStateDictionary[clientID])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            SceneLoader.Instance.LoadScene(SceneName.GameScene, true);
        }
    }
}
