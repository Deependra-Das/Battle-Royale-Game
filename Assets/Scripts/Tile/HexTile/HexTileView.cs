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
        [SerializeField] private float _lifetime = 2f;
        [SerializeField] private Vector3 _targetScale = new Vector3(1f, 0.5f, 1f);

        private Vector3 _originalScale;

        [SerializeField]
        private NetworkVariable<HexTileStates> _networkTileState =
            new NetworkVariable<HexTileStates>(HexTileStates.Untouched, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        [SerializeField]
        private NetworkVariable<bool> _networkIsTileActive =
            new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private void OnEnable()
        {
            EventBusManager.Instance.Subscribe(EventName.ActivateTilesForGameplay, HandleTileActivation);
            _networkTileState.OnValueChanged += OnTileStateChanged;
        }

        private void OnDisable()
        {
            EventBusManager.Instance.Unsubscribe(EventName.ActivateTilesForGameplay, HandleTileActivation);
            _networkTileState.OnValueChanged -= OnTileStateChanged;
        }

        private void Start()
        {
            _originalScale = transform.localScale;
            UpdateTileVisual(_networkTileState.Value);
        }
   
        private void HandleTileActivation(object[] parameters)
        {
            if (!IsServer) return;

            bool activate = (bool)parameters[0];
            _networkIsTileActive.Value = activate;
        }

        public void PlayerOnTheTileDetected()
        {
            if (IsServer &&
                _networkTileState.Value == HexTileStates.Untouched &&
                _networkIsTileActive.Value)
            {
                SetTileTouchedServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetTileTouchedServerRpc()
        {
            if (_networkTileState.Value != HexTileStates.Untouched) return;

            _networkTileState.Value = HexTileStates.Touched;
            PlayTileTouchedVisualsClientRpc();
            StartCoroutine(DeactivateTileAfterDelay());
        }

        [ClientRpc]
        private void PlayTileTouchedVisualsClientRpc()
        {
            StartCoroutine(ChangeMaterial());
            StartCoroutine(ScaleObject(transform.localScale, _targetScale, _lifetime));
        }

        private IEnumerator DeactivateTileAfterDelay()
        {
            yield return new WaitForSeconds(_lifetime);

            _networkTileState.Value = HexTileStates.Inactive;
        }

        private void OnTileStateChanged(HexTileStates previous, HexTileStates current)
        {
            UpdateTileVisual(current);
        }

        private void UpdateTileVisual(HexTileStates state)
        {
            switch (state)
            {
                case HexTileStates.Untouched:
                    gameObject.SetActive(true);
                    transform.localScale = _originalScale;
                    break;

                case HexTileStates.Touched:
                    break;

                case HexTileStates.Inactive:
                    gameObject.SetActive(false);
                    break;
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

        private IEnumerator ScaleObject(Vector3 initialScale, Vector3 finalScale, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localScale = finalScale;
        }
    }
}
