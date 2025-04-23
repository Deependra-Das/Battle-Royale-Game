using UnityEngine;

namespace BattleRoyale.Player
{
    public class PlayerController
    {
        private PlayerModel _playerModel;
        private PlayerView _playerView;

        public PlayerController(PlayerView playerView, PlayerScriptableObject player_SO)
        {
            _playerModel = new PlayerModel(player_SO);
            _playerView = Object.Instantiate(playerView);
            _playerView.Initialize(_playerModel);
        }

        public GameObject PlayerCameraRoot { get { return _playerView.PlayerCameraRoot; } }
    }
}
