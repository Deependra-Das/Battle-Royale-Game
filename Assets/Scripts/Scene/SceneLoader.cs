using BattleRoyale.Event;
using BattleRoyale.Main;
using BattleRoyale.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleRoyale.Scene
{
    public class SceneLoader : GenericMonoSingleton<SceneLoader>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public void LoadScene(SceneName sceneName, bool isNetworked, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (isNetworked)
            {
                if (NetworkManager.Singleton == null) return;

                if (!NetworkManager.Singleton.IsServer) return;

                NetworkManager.Singleton.SceneManager.LoadScene(sceneName.ToString(), mode);
            }
            else
            {
                StartCoroutine(LoadSceneAsyncCoroutine(sceneName, mode));
            }
        }

        private IEnumerator LoadSceneAsyncCoroutine(SceneName sceneName, LoadSceneMode mode)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName.ToString(), mode);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
          
    }
}