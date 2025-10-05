using Unity.Netcode;

namespace BattleRoyale.Network
{
    [System.Serializable]
    public struct PlayerSessionDataDTO : INetworkSerializable
    {
        public ulong ClientId;
        public string PlayerId;
        public PlayerState Status;
        public PlayerConnectionState ConnectionStatus;
        public int Rank;
        public string Username;
        public int SkinColorIndex;

        public PlayerSessionDataDTO(PlayerSessionData data)
        {
            ClientId = data.ClientId;
            PlayerId = data.PlayerId;
            Status = data.PlayerStatus;
            ConnectionStatus = data.ConnectionStatus;
            Rank = data.Rank;
            Username = data.Username;
            SkinColorIndex = data.SkinColorIndex;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref PlayerId);
            serializer.SerializeValue(ref Status);
            serializer.SerializeValue(ref ConnectionStatus);
            serializer.SerializeValue(ref Rank);
            serializer.SerializeValue(ref Username);
            serializer.SerializeValue(ref SkinColorIndex);
        }
    }
}
