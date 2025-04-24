using UnityEngine;
using System.Collections;

namespace BattleRoyale.Tile
{
    public class HexTileView : MonoBehaviour
    {
        public MeshRenderer meshRenderer;
        public Material deactivateMaterial;
        public float lifetime;
        public Vector3 targetScale = new Vector3(1f, 0.5f, 1f);

        private IEnumerator DeactivateCoroutine()
        {
            yield return new WaitForSeconds(lifetime);
            gameObject.SetActive(false);
        }

        public void PlayerOnTheTileDetected()
        {
            StartCoroutine(DeactivateCoroutine());
            StartCoroutine(ChangeMaterial());
            StartCoroutine(ScaleObject(transform.localScale, targetScale, lifetime));
        }

        private IEnumerator ChangeMaterial()
        {
            if (meshRenderer != null && deactivateMaterial != null)
            {
                meshRenderer.material = deactivateMaterial;
                yield return new WaitForSeconds(1f);
            }
        }

        private IEnumerator ScaleObject(Vector3 initialScale, Vector3 finalScale, float time)
        {
            float elapsedTime = 0f;

            while (elapsedTime < time)
            {
                transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime / time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = finalScale;
        }
    }
}
