using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.CharacterSelection
{
    [CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "ScriptableObjects/CharacterScriptableObject")]
    public class CharacterScriptableObject : ScriptableObject
    {
        public GameObject PlayerCharacterPrefab;
        public List<CharacterSelectTransform> characterTransformList;
    }
}
