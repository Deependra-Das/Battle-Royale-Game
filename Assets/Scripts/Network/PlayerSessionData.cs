using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.Network
{
    public class PlayerSessionData : INetworkSerializable
    {
        private ulong _clientId;
        private PlayerState _playerStatus = PlayerState.Waiting;
        private PlayerConnectionState _connectionStatus = PlayerConnectionState.Connected;
        private int _rank = -1;
        private string _username = "Player";
        private bool _isCharacterVisibleInLobby = false;

        public ulong ClientId => _clientId;
        public PlayerState PlayerStatus => _playerStatus;
        public PlayerConnectionState ConnectionStatus => _connectionStatus;
        public int Rank => _rank;
        public string Username => _username;
        public bool IsCharacterVisibleInLobby => _isCharacterVisibleInLobby;

        public PlayerSessionData(ulong clientId, string username)
        {
            _clientId = clientId;
            SetUsername(username);
        }

        public void SetRank(int rank)
        {
            _rank = rank;
        }

        public void SetUsername(string username)
        {
            _username = username;
        }

        public void SetGameplayStatus(PlayerState status)
        {
            _playerStatus = status;
        }

        public void SetConnectionStatus(PlayerConnectionState status)
        {
            _connectionStatus = status;
        }

        public void SetCharacterVisibilityInLobby(bool isVisible)
        {
            _isCharacterVisibleInLobby = isVisible;
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
            serializer.SerializeValue(ref _username);
            serializer.SerializeValue(ref _isCharacterVisibleInLobby);
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
}
