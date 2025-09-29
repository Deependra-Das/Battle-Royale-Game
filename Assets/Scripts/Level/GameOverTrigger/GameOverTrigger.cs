using BattleRoyale.Main;
using BattleRoyale.Player;
using BattleRoyale.Network;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.Level
{
    public class GameOverTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            PlayerView _playerView = other.gameObject.GetComponent<PlayerView>();
            if (_playerView!=null)
            {
                if (_playerView != null && NetworkManager.Singleton.IsServer)
                {
                    GameplayManager.Instance.HandlePlayerGameOver(_playerView.OwnerClientId);
                }
            }
        }
    }
}
