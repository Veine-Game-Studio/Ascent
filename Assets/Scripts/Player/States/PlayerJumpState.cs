using UnityEngine;

namespace Ascent.Player
{
    public class PlayerJumpState : PlayerBaseState
    {
        private readonly bool isJumping;
        private bool hasAppliedJumpForce;
        private float wallJumpLockoutTimer;

        public PlayerJumpState(PlayerController player, bool isJumping = true, float lockoutTime = 0f) : base(player)
        {
            this.isJumping = isJumping;
            this.wallJumpLockoutTimer = lockoutTime;
        }

        public override void Enter()
        {
            hasAppliedJumpForce = !isJumping;
            base.Enter();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
        }

        public override void FixedTick(float fixedDeltaTime)
        {
            if (isJumping && !hasAppliedJumpForce)
            {
                player.RB.linearVelocity = new Vector2(player.RB.linearVelocity.x, player.JumpForce);
                hasAppliedJumpForce = true;
            }
            
            if (wallJumpLockoutTimer > 0f)
            {
                wallJumpLockoutTimer -= fixedDeltaTime;
                // Let momentum carry the player during the lockout period
            }
            else
            {
                // Allow PlayerBaseState to handle horizontal movement with air speed multiplier
                MoveBasedOnInput(player.AirSpeedMultiplier);
            }

            base.FixedTick(fixedDeltaTime);
        }
    }
}
