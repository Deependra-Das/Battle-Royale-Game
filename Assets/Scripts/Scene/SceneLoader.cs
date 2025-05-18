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
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }

        private IEnumerator LoadSceneAsyncCoroutine(SceneName sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName.ToString());

            while (!asyncLoad.isDone)
            {
                Debug.Log("Loading scene: " + asyncLoad.progress * 100 + "%");
                yield return null;
            }

            OnSceneLoaded(sceneName);
        }

        private void OnSceneLoaded(SceneName sceneName)
        {
            switch(sceneName)
            {
                case SceneName.StartScene:
                    EventBusManager.Instance.RaiseNoParams(EventName.StartSceneLoadedEvent);
                    break;
                case SceneName.LobbyScene:
                    EventBusManager.Instance.RaiseNoParams(EventName.LobbySceneLoadedEvent);
                    break;
                case SceneName.GameScene:
                    EventBusManager.Instance.RaiseNoParams(EventName.GameplaySceneLoadedEvent);
                    break;
                case SceneName.GameOverScene:
                    EventBusManager.Instance.RaiseNoParams(EventName.GameOverSceneLoadedEvent);
                    break;
            }
        }
    }
}