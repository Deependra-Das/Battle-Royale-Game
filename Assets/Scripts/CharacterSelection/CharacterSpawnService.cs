using BattleRoyale.Event;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.CharacterSelection
{
    public class CharacterSpawnService : NetworkBehaviour
    {
        private CharacterScriptableObject _characterSpawnData;
        private Dictionary<int, GameObject> _clientCharacterMapping;
        public int numberOfClonesSpawned = 0;

        public CharacterSpawnService(CharacterScriptableObject spawnData)
        {
            _characterSpawnData = spawnData;
            _clientCharacterMapping = new Dictionary<int, GameObject>();
        }

        private void OnEnable() => SubscribeToEvents();

        private void OnDisable() => UnsubscribeToEvents();

        private void SubscribeToEvents()
        {
            if (IsServer)
            {
                EventBusManager.Instance.Subscribe(EventName.ClientConnected, OnClientConnected);
                EventBusManager.Instance.Subscribe(EventName.ClientDisconnected, OnClientDisconnected);
            }
        }

        private void UnsubscribeToEvents()
        {
            if (IsServer)
            {
                EventBusManager.Instance.Unsubscribe(EventName.ClientConnected, OnClientConnected);
                EventBusManager.Instance.Unsubscribe(EventName.ClientDisconnected, OnClientDisconnected);
            }
        }

        private void OnClientConnected(object[] parameters)
        {
            if (IsServer)
            {
                int clientId = (int)parameters[0];
                SpawnCharacterForConenctedClient(clientId);
            }
        }

        private void OnClientDisconnected(object[] parameters)
        {
            if (IsServer)
            {
                int clientId = (int)parameters[0];
                DespawnCharacterForDisconnectedClient(clientId);
            }
        }

        public void SpawnCharacterForConenctedClient(int clientID)
        {
            GameObject characterClone = Instantiate(
                _characterSpawnData.PlayerCharacterPrefab,
                _characterSpawnData.characterTransformList[numberOfClonesSpawned].characterPosition,
                Quaternion.Euler(_characterSpawnData.characterTransformList[numberOfClonesSpawned].characterRotation)
            );

            characterClone.name = $"PlayerCharacter_{numberOfClonesSpawned + 1}";
            characterClone.GetComponent<CharacterSelectPlayer>().Initialize(numberOfClonesSpawned);

            NetworkObject networkObject = characterClone.GetComponent<NetworkObject>();
            networkObject.SpawnAsPlayerObject((ulong)clientID);

            _clientCharacterMapping[clientID] = characterClone;
            numberOfClonesSpawned++;

            NotifyNewClientAboutExistingPlayers(clientID);
        }

        public void DespawnCharacterForDisconnectedClient(int clientID)
        {
            if (_clientCharacterMapping.TryGetValue(clientID, out var character))
            {
                if (character != null)
                {
                    NetworkObject networkObject = character.GetComponent<NetworkObject>();
                    networkObject.Despawn();
                    Destroy(character);
                    _clientCharacterMapping.Remove(clientID);
                    numberOfClonesSpawned--;
                }
                else
                {
                    Debug.LogWarning($"Character for client {clientID} is already destroyed or null.");
                }
            }
            else
            {
                Debug.LogWarning($"No character found for client {clientID}.");
            }
        }

        public void DespawnAllSpawnedCharacters()
        {
            foreach (var character in _clientCharacterMapping.Values)
            {
                if (character != null && character.activeSelf)
                {
                    NetworkObject networkObject = character.GetComponent<NetworkObject>();
                    networkObject.Despawn();
                    Destroy(character);
                }
            }

            _clientCharacterMapping.Clear();
            numberOfClonesSpawned = 0;
        }

        private void NotifyNewClientAboutExistingPlayers(int newClientID)
        {
            foreach (var kvp in _clientCharacterMapping)
            {
                int existingClientID = kvp.Key;
                GameObject existingCharacter = kvp.Value;

                if (existingClientID == newClientID) continue;

                ulong existingNetworkObjectId = existingCharacter.GetComponent<NetworkObject>().NetworkObjectId;
                SpawnOtherCharacterClientRpc(newClientID, existingClientID, existingNetworkObjectId);
            }
        }

        [ClientRpc]
        private void SpawnOtherCharacterClientRpc(int newClientID, int existingClientID, ulong existingNetworkObjectId)
        {
            if (NetworkManager.Singleton.LocalClientId == (ulong)newClientID)
            {
                NetworkObject existingNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[existingNetworkObjectId];

                if (existingNetworkObject != null)
                {
                    existingNetworkObject.SpawnAsPlayerObject((ulong)newClientID);
                }
            }
        }
    }

}
