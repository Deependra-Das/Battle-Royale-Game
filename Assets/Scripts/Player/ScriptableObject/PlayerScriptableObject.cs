using Unity.Cinemachine;
using UnityEngine;

namespace BattleRoyale.PlayerModule
{
    [CreateAssetMenu(fileName = "PlayerScriptableObject", menuName = "ScriptableObjects/PlayerScriptableObject")]
    public class PlayerScriptableObject : ScriptableObject
    {
        public PlayerView playerPrefab;
        public CinemachineCamera playerCameraPrefab;
    }
}
