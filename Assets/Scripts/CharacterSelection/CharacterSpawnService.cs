using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.CharacterSelection
{
    public class CharacterSpawnService
    {
        private CharacterScriptableObject _characterSpawnData;
        private List<GameObject> _playerCharactersSpawnedList;
        public int numberOfClones = 5;

        public CharacterSpawnService(CharacterScriptableObject spawnData)
        {
              _characterSpawnData= spawnData;
            _playerCharactersSpawnedList = new List<GameObject>();
        }

        public void SpawnCharacters()
        {
            for (int i = 0; i < numberOfClones; i++)
            {
                GameObject characterClone = GameObject.Instantiate(_characterSpawnData.PlayerCharacterPrefab, _characterSpawnData.characterTransformList[i].characterPosition, Quaternion.Euler(_characterSpawnData.characterTransformList[i].characterRotation));

                characterClone.name = $"PlayerCharacter_{i + 1}";

                _playerCharactersSpawnedList.Add(characterClone);
            }
        }

        public void DespawnCharacters()
        {
            foreach (var character in _playerCharactersSpawnedList)
            {
                if (character != null)
                {
                    GameObject.Destroy(character);
                }
            }

            _playerCharactersSpawnedList.Clear();
        }
    }
}
