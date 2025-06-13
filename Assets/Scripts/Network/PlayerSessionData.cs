using UnityEngine;

public class PlayerSessionData
{
    public ulong ClientId { get; private set; }
    public PlayerState PlayerStatus { get; private set; } = PlayerState.Waiting;
    public PlayerConnectionState ConnectionStatus { get; private set; } = PlayerConnectionState.Connected;
    public int Rank { get; private set; } = -1;

    public PlayerSessionData(ulong clientId)
    {
        ClientId = clientId;
    }

    public void SetRank(int rank)
    {
        Rank = rank;
    }

    public void SetGameplayStatus(PlayerState status)
    {
        PlayerStatus = status;
    }

    public void SetConnectionStatus(PlayerConnectionState status)
    {
        ConnectionStatus = status;
    }

    public void Reset()
    {
        PlayerStatus = PlayerState.Waiting;
        Rank = -1;
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



