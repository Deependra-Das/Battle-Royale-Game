using UnityEngine;
using BattleRoyale.Utilities;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : GenericMonoSingleton<GameManager>
{
    void Start()
    {
        StartCoroutine("SceneLoadTest");
    }

    private IEnumerator SceneLoadTest()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("GameScene");
    }
}
