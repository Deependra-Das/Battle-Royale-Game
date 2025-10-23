using BattleRoyale.SceneModule;
using UnityEngine;

namespace BattleRoyale.SplashModule
{
    public class SplashScreenManager : MonoBehaviour
    {
        [Header("Fade Settings")]
        [SerializeField] private CanvasGroup _splashCanvasGroup;
        [SerializeField] private float _fadeDuration = 1.5f;
        [SerializeField] private float _displayDuration = 3f;

        private void Start()
        {
            StartCoroutine(PlaySplashSequence());
        }

        private System.Collections.IEnumerator PlaySplashSequence()
        {
            float fadeInElapsedTime = 0f;

            while (fadeInElapsedTime < _fadeDuration)
            {
                fadeInElapsedTime += Time.deltaTime;
                float fadeInProgress = fadeInElapsedTime / _fadeDuration;
                _splashCanvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeInProgress);
                yield return null;
            }

            yield return new WaitForSeconds(_displayDuration);

            float fadeOutElapsedTime = 0f;
            while (fadeOutElapsedTime < _fadeDuration)
            {
                fadeOutElapsedTime += Time.deltaTime;
                float fadeOutProgress = fadeOutElapsedTime / _fadeDuration;
                _splashCanvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeOutProgress);
                yield return null;
            }

            SceneLoader.Instance.LoadScene(SceneName.StartScene, false);
        }
    }
}