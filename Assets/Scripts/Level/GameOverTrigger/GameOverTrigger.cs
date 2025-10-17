using BattleRoyale.MainModule;
using BattleRoyale.PlayerModule;
using BattleRoyale.NetworkModule;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.LevelModule
{
    public class GameOverTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            PlayerController _playerController = other.gameObject.GetComponent<PlayerController>();
            if (_playerController!=null)
            {
                if (_playerController != null && NetworkManager.Singleton.IsServer)
                {
                    GameplayManager.Instance.HandlePlayerGameOver(_playerController.OwnerClientId);
                }
            }
        }
    }
}
