using BattleRoyale.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
            StartCoroutine("SceneLoadTest");
        }

        private IEnumerator SceneLoadTest()
        {
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene("StartScene");
        }
    }
}
