using BattleRoyale.Event;
using BattleRoyale.Utilities;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleRoyale.Scene
{
    public class SceneLoader : GenericMonoSingleton<SceneLoader>
    {
        public void LoadSceneAsync(SceneName sceneName)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName.ToString()));
        }

        private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                Debug.Log("Loading scene: " + asyncLoad.progress * 100 + "%");
                yield return null;
            }

            OnSceneLoaded();
        }

        private void OnSceneLoaded()
        {
            EventBusManager.Instance.RaiseNoParams(EventName.GameplaySceneLoadedEvent);
        }
    }
}