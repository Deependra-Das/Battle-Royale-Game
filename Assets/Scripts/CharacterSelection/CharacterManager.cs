using BattleRoyale.Main;
using BattleRoyale.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleRoyale.CharacterSelection
{
    public class CharacterManager : NetworkBehaviour
    {
        public static CharacterManager Instance { get; private set; }

        private CharacterScriptableObject _characterSpawnData;
        private List<ClientCharacterMapping> _clientCharacterMapList;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _characterSpawnData = GameManager.Instance.character_SO;
            _clientCharacterMapList = new List<ClientCharacterMapping>();
        }

        public override void OnNetworkSpawn()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                SpawnCharacterForConnectedClient(NetworkManager.Singleton.LocalClientId);
            }
        }

        public void SpawnCharacterForConnectedClient(ulong clientID)
        {
            if (!IsServer) return;

            if (_clientCharacterMapList.All(x => x.clientID != clientID))
            {
                GameObject characterClone = Instantiate(
                    _characterSpawnData.PlayerCharacterPrefab,
                    _characterSpawnData.characterTransformList[_clientCharacterMapList.Count].characterPosition,
                    Quaternion.Euler(_characterSpawnData.characterTransformList[_clientCharacterMapList.Count].characterRotation)
                );

                characterClone.name = $"PlayerCharacter_{clientID}";
                characterClone.GetComponent<CharacterSelectPlayer>().Initialize(_clientCharacterMapList.Count, PlayerSessionManager.Instance.GetPlayerSessionData(clientID).Username);

                NetworkObject networkObject = characterClone.GetComponent<NetworkObject>();
                networkObject.Spawn();

                _clientCharacterMapList.Add(new ClientCharacterMapping(clientID, characterClone));
            }
        }

        public void HandleLateJoin(ulong clientID)
        {
            if (!IsServer) return;

            SpawnCharacterForConnectedClient(clientID);
        }

        public void DespawnCharacterForDisconnectedClient(ulong clientID)
        {
            var entry = _clientCharacterMapList.FirstOrDefault(x => x.clientID == clientID);

            if (entry != null)
            {
                var character = entry.character;
                int clientIndex = _clientCharacterMapList.IndexOf(entry);
                NetworkObject networkObject = character.GetComponent<NetworkObject>();
                networkObject.Despawn();
                Destroy(character);

                _clientCharacterMapList.Remove(entry);

                AdjustSpawnPositions(clientIndex);
            }
        }

        private void AdjustSpawnPositions(int clientIndex)
        {
            if (_clientCharacterMapList.Count > 1)
            {
                for (int i = clientIndex; i < _clientCharacterMapList.Count; i++)
                {
                    _clientCharacterMapList[i].character.transform.position =
                          _characterSpawnData.characterTransformList[i].characterPosition;
                    _clientCharacterMapList[i].character.transform.rotation =
                     Quaternion.Euler(_characterSpawnData.characterTransformList[i].characterRotation);
                }
            }
        }

        public void DespawnAllSpawnedCharacters()
        {
            foreach (var entry in _clientCharacterMapList)
            {
                var character = entry.character;
                if (character != null && character.activeSelf)
                {
                    NetworkObject networkObject = character.GetComponent<NetworkObject>();
                    networkObject.Despawn();
                    Destroy(character);
                }
            }

            _clientCharacterMapList.Clear();
        }
    }
}
