using BattleRoyale.MainModule;
using BattleRoyale.TileModule;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.NetworkModule
{
    public class TileRegistry : NetworkBehaviour
    {
        public static TileRegistry Instance;

        private HashSet<HexTileView> registeredTiles = new();
        private int expectedCount = -1;
        private bool hasNotified = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) RequestDestroyRegistryServerRpc();
            registeredTiles.Clear();
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

        public void InitializeTileRegistry(int count)
        {
            expectedCount = count;
            hasNotified = false;

            StartCoroutine(WaitForTileRegistration());
        }

        public void RegisterTile(HexTileView tile)
        {
            if (registeredTiles.Contains(tile)) return;

            registeredTiles.Add(tile);
        }

        private IEnumerator WaitForTileRegistration()
        {
            yield return new WaitUntil(() =>
                !hasNotified &&
                expectedCount > 0 &&
                registeredTiles.Count >= expectedCount
            );

            hasNotified = true;
            NotifyServerReady();
        }

        private void NotifyServerReady()
        {
            NotifyReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void NotifyReadyServerRpc(ServerRpcParams rpcParams = default)
        {
            var clientId = rpcParams.Receive.SenderClientId;
            GameplayManager.Instance.RegisterClientTileReadyServerRpc(clientId);
        }
    }
}
