using BattleRoyale.SceneModule;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleRoyale.SplashModule
{
    public class SplashScreenManager : MonoBehaviour
    {
        [Header("Fade Settings")]
        [SerializeField] private CanvasGroup splashCanvasGroup;
        [SerializeField] private float fadeDuration = 1.5f;
        [SerializeField] private float displayDuration = 3f;

        private void Start()
        {
            StartCoroutine(PlaySplashSequence());
        }

        private System.Collections.IEnumerator PlaySplashSequence()
        {
            float fadeInElapsedTime = 0f;

            while (fadeInElapsedTime < fadeDuration)
            {
                fadeInElapsedTime += Time.deltaTime;
                float fadeInProgress = fadeInElapsedTime / fadeDuration;
                splashCanvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeInProgress);
                yield return null;
            }

            yield return new WaitForSeconds(displayDuration);

            float fadeOutElapsedTime = 0f;
            while (fadeOutElapsedTime < fadeDuration)
            {
                fadeOutElapsedTime += Time.deltaTime;
                float fadeOutProgress = fadeOutElapsedTime / fadeDuration;
                splashCanvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeOutProgress);
                yield return null;
            }

            SceneLoader.Instance.LoadScene(SceneName.StartScene, false);
        }
    }
}