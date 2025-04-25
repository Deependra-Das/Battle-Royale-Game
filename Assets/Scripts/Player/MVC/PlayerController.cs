using UnityEngine;

namespace BattleRoyale.Player
{
    public class PlayerController
    {
        private PlayerModel _playerModel;
        private PlayerView _playerView;

        public PlayerController(PlayerView playerView, PlayerScriptableObject player_SO, Transform spawnParentTransform, Vector2 spawnPostion)
        {
            _playerModel = new PlayerModel(player_SO);
            _playerView = Object.Instantiate(playerView, new Vector3(spawnPostion.x,spawnParentTransform.position.y, spawnPostion.y), Quaternion.identity);
            _playerView.Initialize(_playerModel);
        }

        public GameObject PlayerCameraRoot { get { return _playerView.PlayerCameraRoot; } }
    }
}
