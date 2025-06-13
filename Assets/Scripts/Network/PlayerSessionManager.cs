using System.Collections.Generic;
using System.Data;
using Unity.Netcode;

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
        if (IsServer && !_sessionData.ContainsKey(clientId))
        {
            _sessionData[clientId] = new PlayerSessionData(clientId);
        }
    }

    public void SetPlayerStatus(ulong clientId, PlayerState gameplayState)
    {
        if (_sessionData.TryGetValue(clientId, out var data))
        {
            data.SetGameplayStatus(gameplayState);
        }
    }

    public void SetPlayerConnectionStatus(ulong clientId, PlayerConnectionState connectionState)
    {
        if (_sessionData.TryGetValue(clientId, out var data))
        {
            data.SetConnectionStatus(connectionState);
        }
    }
    public void SetPlayerRank(ulong clientId, int rank)
    {
        if (_sessionData.TryGetValue(clientId, out var data))
        {
            data.SetRank(rank);
        }
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
        if (!IsServer) return;

        foreach (var data in _sessionData.Values)
        {
            data.Reset();
        }
    }

    public void ClearAllSessions()
    {
        if (!IsServer) return;

        _sessionData.Clear();
    }

}
