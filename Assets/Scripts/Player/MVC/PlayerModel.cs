using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace BattleRoyale.Player
{
    public class PlayerModel
    {
        public float MoveSpeed { get; private set; }
        public float SpeedChangeRate { get; private set; }
        public float RotationSmoothTime { get; private set; }
        public float JumpHeight { get; private set; }
        public float Gravity { get; private set; }
        public float TerminalVelocity { get; private set; }
        public float JumpTimeout { get; private set; }
        public float FallTimeout { get; private set; }
        public float GroundedOffset { get; private set; }
        public float GroundedRadius { get; private set; }

        public PlayerModel(PlayerScriptableObject player_SO)
        {
            MoveSpeed = player_SO.moveSpeed;
            SpeedChangeRate = player_SO.speedChangeRate;
            RotationSmoothTime = player_SO.rotationSmoothTime;
            JumpHeight = player_SO.jumpHeight;
            Gravity = player_SO.gravity;
            TerminalVelocity = player_SO.terminalVelocity;
            JumpTimeout = player_SO.jumpTimeout;
            FallTimeout = player_SO.fallTimeout;
            GroundedOffset = player_SO.groundedOffset;
            GroundedRadius = player_SO.groundedRadius;
        }
    }
}
