using UnityEngine;
using System.Collections;
using BattleRoyale.Main;
using BattleRoyale.Player;

namespace BattleRoyale.Level
{
    public class GameOverTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            PlayerView _playerView = other.gameObject.GetComponent<PlayerView>();
            if (_playerView!=null)
            {
                TriggerGameOver();
            }
        }

        void TriggerGameOver()
        {
            Debug.Log("Game Over!");
            //GameManager.Instance.ChangeGameState(GameState.GameOver);
        }
    }
}
