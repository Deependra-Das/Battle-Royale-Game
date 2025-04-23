using UnityEngine;

namespace BattleRoyale.Player
{
    [CreateAssetMenu(fileName = "PlayerScriptableObject", menuName = "ScriptableObjects/PlayerScriptableObject")]
    public class PlayerScriptableObject : ScriptableObject
    {
        [Header("Player")]
        public float moveSpeed = 10.0f;
        public float speedChangeRate = 10.0f;
        [Range(0.0f, 0.3f)]
        public float rotationSmoothTime = 0.12f;
        public float jumpHeight = 1.2f;
        public float gravity = -15.0f;
        public float terminalVelocity = 53.0f;
        public float jumpTimeout = 0.50f;
        public float fallTimeout = 0.15f;
        public float groundedOffset = -0.14f;
        public float groundedRadius = 0.28f;
    }
}
