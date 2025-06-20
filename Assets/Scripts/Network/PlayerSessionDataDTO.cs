using Unity.Netcode;

[System.Serializable]
public struct PlayerSessionDataDTO : INetworkSerializable
{
    public ulong ClientId;
    public PlayerState Status;
    public PlayerConnectionState ConnectionStatus;
    public int Rank;

    public PlayerSessionDataDTO(PlayerSessionData data)
    {
        ClientId = data.ClientId;
        Status = data.PlayerStatus;
        ConnectionStatus = data.ConnectionStatus;
        Rank = data.Rank;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref Status);
        serializer.SerializeValue(ref ConnectionStatus);
        serializer.SerializeValue(ref Rank);
    }
}
