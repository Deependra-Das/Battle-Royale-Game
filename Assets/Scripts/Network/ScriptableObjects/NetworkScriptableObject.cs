using BattleRoyale.CharacterSelectionModule;
using UnityEngine;

namespace BattleRoyale.NetworkModule
{
    [CreateAssetMenu(fileName = "NetworkScriptableObject", menuName = "ScriptableObjects/NetworkScriptableObject")]
    public class NetworkScriptableObject : ScriptableObject
    {
        public CharacterManager characterManagerPrefab;
        public TileRegistry tileRegistryPrefab; 
    }
}
