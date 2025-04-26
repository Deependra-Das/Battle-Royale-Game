using UnityEngine;

namespace BattleRoyale.Player
{
    public class PlayerController
    {
        private PlayerModel _playerModel;
        private PlayerView _playerView;

        public PlayerController(PlayerView playerView, PlayerScriptableObject player_SO, Vector3 spawnPostion)
        {
            _playerModel = new PlayerModel(player_SO);
            _playerView = Object.Instantiate(playerView, spawnPostion, Quaternion.identity);
            _playerView.Initialize(_playerModel);
        }

        public GameObject PlayerCameraRoot { get { return _playerView.PlayerCameraRoot; } }
    }
}
