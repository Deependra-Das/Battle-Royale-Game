using UnityEngine;
using System.Collections;
using BattleRoyale.Event;
using System.Globalization;
using Unity.Netcode;

namespace BattleRoyale.Tile
{
    public class HexTileView : NetworkBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Material _deactivateMaterial;
        [SerializeField] private float _lifetime;
        [SerializeField] private Vector3 _targetScale = new Vector3(1f, 0.5f, 1f);

        private HexTileStates _currentTileState;
        private bool _isTileActive;

        [SerializeField] private NetworkVariable<HexTileStates> _networkTileState = new NetworkVariable<HexTileStates>(HexTileStates.Untouched);
        [SerializeField] private NetworkVariable<bool> _networkIsTileActive = new NetworkVariable<bool>(false);

        //private void OnEnable()
        //{
        //    EventBusManager.Instance.Subscribe(EventName.ActivateTilesForGameplay, HandleTileActivation);
        //}

        //private void OnDisable()
        //{
        //    EventBusManager.Instance.Unsubscribe(EventName.ActivateTilesForGameplay, HandleTileActivation);
        //}

        void Start()
        {
            _currentTileState = HexTileStates.Untouched;
            _isTileActive = false;
        }

        private void HandleTileActivation(object[] parameters)
        {
            _isTileActive = (bool)parameters[0];
            _networkIsTileActive.Value = _isTileActive;
        }

        private IEnumerator DeactivateCoroutine()
        {
            yield return new WaitForSeconds(_lifetime);
            _currentTileState = HexTileStates.Inactive;
            _networkTileState.Value = _currentTileState;
            gameObject.SetActive(false);
        }

        public void PlayerOnTheTileDetected()
        {
            if (_currentTileState == HexTileStates.Untouched && _isTileActive)
            {
                SetTileTouchedServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetTileTouchedServerRpc()
        {
            _currentTileState = HexTileStates.Touched;
            _networkTileState.Value = _currentTileState;

            StartCoroutine(DeactivateCoroutine());
            StartCoroutine(ChangeMaterial());
            StartCoroutine(ScaleObject(transform.localScale, _targetScale, _lifetime));
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

        private void Update()
        {
            if (IsOwner)
            {
                _currentTileState = _networkTileState.Value;
                _isTileActive = _networkIsTileActive.Value;
            }
        }
    }
}
