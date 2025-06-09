using UnityEngine;

public class PlayerSessionData
{
    public ulong ClientId { get; private set; }
    public bool IsEliminated { get; private set; } = false;
    public bool IsDisconnected { get; private set; } = false;
    public int Rank { get; private set; } = -1;

    public PlayerSessionData(ulong clientId)
    {
        ClientId = clientId;
    }

    public void MarkEliminated(int rank)
    {
        IsEliminated = true;
        Rank = rank;
    }

    public void MarkDisconnected()
    {
        IsDisconnected = true;
        IsEliminated = true;
    }
}
