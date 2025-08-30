using BattleRoyale.Event;
using BattleRoyale.Main;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.CharacterSelection
{
    public class CharacterManager : NetworkBehaviour
    {
        public static CharacterManager Instance { get; private set; }

        private CharacterScriptableObject _characterSpawnData;
        private Dictionary<ulong, GameObject> _clientCharacterMapping;
        private int numberOfClonesSpawned = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _characterSpawnData = GameManager.Instance.character_SO;
            _clientCharacterMapping = new Dictionary<ulong, GameObject>();

        }


        public override void OnNetworkSpawn()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                SpawnCharacterForConenctedClient(NetworkManager.Singleton.LocalClientId);
            }
        }

        public void SpawnCharacterForConenctedClient(ulong clientID)
        {
            if (!IsServer) return;

            if (!_clientCharacterMapping.ContainsKey(clientID))
            {
                GameObject characterClone = Instantiate(
                _characterSpawnData.PlayerCharacterPrefab,
                _characterSpawnData.characterTransformList[numberOfClonesSpawned].characterPosition,
                Quaternion.Euler(_characterSpawnData.characterTransformList[numberOfClonesSpawned].characterRotation));

                characterClone.name = $"PlayerCharacter_{numberOfClonesSpawned + 1}";
                characterClone.GetComponent<CharacterSelectPlayer>().Initialize(numberOfClonesSpawned);

                NetworkObject networkObject = characterClone.GetComponent<NetworkObject>();
                networkObject.Spawn();
                networkObject.ChangeOwnership(NetworkManager.Singleton.LocalClientId);

                _clientCharacterMapping[clientID] = characterClone;
                numberOfClonesSpawned++;
            }
        }

        public void HandleLateJoin(ulong clientID)
        {
            if (!IsServer) return;

            SpawnCharacterForConenctedClient((ulong)clientID);
            
        }

        public void DespawnCharacterForDisconnectedClient(ulong clientID)
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


    }
}
