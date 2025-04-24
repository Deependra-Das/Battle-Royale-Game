using UnityEngine;
using System.Collections;

namespace BattleRoyale.Tile
{
    public class HexTileView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Material _deactivateMaterial;
        [SerializeField] private float _lifetime;
        [SerializeField] private Vector3 _targetScale = new Vector3(1f, 0.5f, 1f);
        private HexTileStates _currentTileState;

        void Start()
        {
            _currentTileState = HexTileStates.Untouched;
        }

        private IEnumerator DeactivateCoroutine()
        {
            yield return new WaitForSeconds(_lifetime);
            _currentTileState = HexTileStates.Inactive;
            gameObject.SetActive(false);
        }

        public void PlayerOnTheTileDetected()
        {
            if (_currentTileState == HexTileStates.Untouched)
            {
                _currentTileState = HexTileStates.Touched;
                StartCoroutine(DeactivateCoroutine());
                StartCoroutine(ChangeMaterial());
                StartCoroutine(ScaleObject(transform.localScale, _targetScale, _lifetime));
            }
        }

        private IEnumerator ChangeMaterial()
        {
            if (_meshRenderer != null && _deactivateMaterial != null)
            {
                _meshRenderer.material = _deactivateMaterial;
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
