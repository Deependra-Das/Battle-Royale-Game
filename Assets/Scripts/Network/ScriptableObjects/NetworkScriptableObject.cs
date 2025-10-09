using BattleRoyale.CharacterSelectionModule;
using UnityEngine;

namespace BattleRoyale.NetworkModule
{
    [CreateAssetMenu(fileName = "NetworkScriptableObject", menuName = "ScriptableObjects/NetworkScriptableObject")]
    public class NetworkScriptableObject : ScriptableObject
    {
        public PlayerSessionManager playerSessionManagerPrefab;
        public CharacterManager characterManagerPrefab;
        public GameplayManager gameplayManagerPrefab;
        public GameOverManager gameOverManagerPrefab;
        public TileRegistry tileRegistryPrefab; 
    }
}
