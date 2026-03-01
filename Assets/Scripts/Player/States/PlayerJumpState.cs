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
            hasAppliedJumpForce = true;
            if (isJumping)
            {
                player.RB.linearVelocity = new Vector2(player.RB.linearVelocity.x, player.JumpForce);
            }
            player.InputReader.JumpCanceledEvent += OnJumpCanceled;
            base.Enter();
        }

        public override void Exit()
        {
            player.InputReader.JumpCanceledEvent -= OnJumpCanceled;
            base.Exit();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
        }

        public override void FixedTick(float fixedDeltaTime)
        {
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

        private void OnJumpCanceled()
        {
            if (player.RB.linearVelocity.y > 0f)
            {
                player.RB.linearVelocity = new Vector2(player.RB.linearVelocity.x, player.RB.linearVelocity.y * player.JumpCutMultiplier);
            }
        }
    }
}
