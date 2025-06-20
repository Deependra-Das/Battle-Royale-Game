using Unity.Netcode;
using UnityEngine;

public class PlayerSessionData : INetworkSerializable
{
    private ulong _clientId;
    private PlayerState _playerStatus = PlayerState.Waiting;
    private PlayerConnectionState _connectionStatus = PlayerConnectionState.Connected;
    private int _rank = -1;

    public ulong ClientId => _clientId;
    public PlayerState PlayerStatus => _playerStatus;
    public PlayerConnectionState ConnectionStatus => _connectionStatus;
    public int Rank => _rank;

    public PlayerSessionData(ulong clientId)
    {
        _clientId = clientId;
    }

    public void SetRank(int rank)
    {
        _rank = rank;
    }

    public void SetGameplayStatus(PlayerState status)
    {
        _playerStatus = status;
    }

    public void SetConnectionStatus(PlayerConnectionState status)
    {
        _connectionStatus = status;
    }

    public void Reset()
    {
        _playerStatus = PlayerState.Waiting;
        _rank = -1;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _clientId);
        serializer.SerializeValue(ref _playerStatus);
        serializer.SerializeValue(ref _connectionStatus);
        serializer.SerializeValue(ref _rank);
    }
}

public enum PlayerState
{
    Waiting,
    Playing,
    Eliminated,
}

public enum PlayerConnectionState
{
    Disconnected,
    Connected,
}
