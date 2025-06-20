using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerSessionManager : NetworkBehaviour
{
    public static PlayerSessionManager Instance { get; private set; }

    private readonly Dictionary<ulong, PlayerSessionData> _sessionData = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterPlayer(ulong clientId)
    {
        if (IsServer)
        {
            RegisterPlayerServerRpc(clientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RegisterPlayerServerRpc(ulong clientId, ServerRpcParams rpcParams = default)
    {
        if (!_sessionData.ContainsKey(clientId))
        {
            _sessionData[clientId] = new PlayerSessionData(clientId);
        }

        SyncSessionDataToClient(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerStatusServerRpc(ulong clientId, PlayerState gameplayState)
    {
        if (_sessionData.TryGetValue(clientId, out var data))
        {
            data.SetGameplayStatus(gameplayState);
            SyncPlayerStatusClientRpc(clientId, gameplayState);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerConnectionStatusServerRpc(ulong clientId, PlayerConnectionState connectionState)
    {
        if (_sessionData.TryGetValue(clientId, out var data))
        {
            data.SetConnectionStatus(connectionState);
            SyncPlayerConnectionStatusClientRpc(clientId, connectionState);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerRankServerRpc(ulong clientId, int rank)
    {
        if (_sessionData.TryGetValue(clientId, out var data))
        {
            data.SetRank(rank);
            SyncPlayerRankClientRpc(clientId, rank);
        }
    }

    [ClientRpc]
    private void SyncPlayerStatusClientRpc(ulong clientId, PlayerState gameplayState)
    {
        if (_sessionData.TryGetValue(clientId, out var data))
        {
            data.SetGameplayStatus(gameplayState);
        }
    }

    [ClientRpc]
    private void SyncPlayerConnectionStatusClientRpc(ulong clientId, PlayerConnectionState connectionState)
    {
        if (_sessionData.TryGetValue(clientId, out var data))
        {
            data.SetConnectionStatus(connectionState);
        }
    }

    [ClientRpc]
    private void SyncPlayerRankClientRpc(ulong clientId, int rank)
    {
        if (_sessionData.TryGetValue(clientId, out var data))
        {
            data.SetRank(rank);
        }
    }

    [ClientRpc]
    private void SyncAllSessionDataClientRpc(PlayerSessionDataDTO[] dataArray, ClientRpcParams clientRpcParams = default)
    {
        foreach (var dto in dataArray)
        {
            if (!_sessionData.ContainsKey(dto.ClientId))
            {
                _sessionData[dto.ClientId] = new PlayerSessionData(dto.ClientId);
            }

            _sessionData[dto.ClientId].SetGameplayStatus(dto.Status);
            _sessionData[dto.ClientId].SetConnectionStatus(dto.ConnectionStatus);
            _sessionData[dto.ClientId].SetRank(dto.Rank);
        }
    }

    public void SyncSessionDataToClient(ulong targetClientId)
    {
        if (!IsServer) return;

        var dataArray = _sessionData.Values
            .Select(data => new PlayerSessionDataDTO(data))
            .ToArray();

        var rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { targetClientId }
            }
        };

        SyncAllSessionDataClientRpc(dataArray, rpcParams);
    }


    public PlayerSessionData GetPlayerSessionData(ulong clientId)
    {
        return _sessionData.TryGetValue(clientId, out var data) ? data : null;
    }

    public Dictionary<ulong, PlayerSessionData> GetAllPlayerSessionData()
    {
        return new Dictionary<ulong, PlayerSessionData>(_sessionData);
    }

    public void ResetAllSessions()
    {
        if (IsServer)
        {
            foreach (var data in _sessionData.Values)
            {
                data.Reset();
                SyncPlayerStatusClientRpc(data.ClientId, PlayerState.Waiting);
                SyncPlayerRankClientRpc(data.ClientId, -1);
            }
        }
    }

    public void ClearAllSessions()
    {
        if (IsServer)
        {
            _sessionData.Clear();
        }
    }
}
