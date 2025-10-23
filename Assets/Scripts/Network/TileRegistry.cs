using BattleRoyale.TileModule;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.NetworkModule
{
    public class TileRegistry : NetworkBehaviour
    {
        public static TileRegistry Instance;

        private HashSet<HexTileView> _registeredTiles = new();
        private int _expectedCount = -1;
        private bool _hasNotified = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) RequestDestroyRegistryServerRpc();
            _registeredTiles.Clear();
        }

        public void InitializeTileRegistry(int count)
        {
            _expectedCount = count;
            _hasNotified = false;

            StartCoroutine(WaitForTileRegistration());
        }

        public void RegisterTile(HexTileView tile)
        {
            if (_registeredTiles.Contains(tile)) return;

            _registeredTiles.Add(tile);
        }

        private IEnumerator WaitForTileRegistration()
        {
            yield return new WaitUntil(() =>
                !_hasNotified &&
                _expectedCount > 0 &&
                _registeredTiles.Count >= _expectedCount
            );

            _hasNotified = true;
            NotifyServerReady();
        }

        private void NotifyServerReady()
        {
            NotifyReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void RequestDestroyRegistryServerRpc()
        {
            if (IsServer)
            {
                NetworkObject.Despawn(true);
                Destroy(gameObject);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void NotifyReadyServerRpc(ServerRpcParams rpcParams = default)
        {
            var clientId = rpcParams.Receive.SenderClientId;
            GameplayManager.Instance.RegisterClientTileReadyServerRpc(clientId);
        }
    }
}
