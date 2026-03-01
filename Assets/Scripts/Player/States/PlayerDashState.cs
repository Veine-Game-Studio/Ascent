using UnityEngine;

namespace Ascent.Player
{
    public class PlayerDashState : PlayerBaseState
    {
        private float dashTimer;
        private float dashDirection;

        public PlayerDashState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            dashTimer = 0f;
            player.LastDashTime = Time.time;
            
            if (!player.IsGrounded())
            {
                player.HasAirDashed = true;
            }

            // Determine dash direction based on current facing direction or input
            float inputX = player.InputReader.MovementValue.x;
            if (inputX != 0f)
            {
                dashDirection = Mathf.Sign(inputX);
            }
            else
            {
                dashDirection = player.transform.eulerAngles.y == 180f ? -1f : 1f;
            }

            // Flip visual immediately
            player.FlipPlayerPivot(dashDirection);

            // Set velocity directly, ignore gravity temporarily
            player.RB.linearVelocity = new Vector2(dashDirection * player.DashSpeed, 0f);
            player.RB.gravityScale = 0f; // Disable gravity during dash

            base.Enter();
        }

        public override void Tick(float deltaTime)
        {
            dashTimer += deltaTime;

            if (dashTimer >= player.DashDuration)
            {
                // Dash complete, return to normal states
                if (player.IsGrounded())
                {
                    player.StateMachine.ChangeState(new PlayerGroundedState(player));
                }
                else
                {
                    player.StateMachine.ChangeState(new PlayerAirborneState(player));
                }
                return;
            }

            base.Tick(deltaTime);
        }

        public override void FixedTick(float fixedDeltaTime)
        {
            // Lock velocity during dash
            player.RB.linearVelocity = new Vector2(dashDirection * player.DashSpeed, 0f);
            
            base.FixedTick(fixedDeltaTime);
        }

        public override void Exit()
        {
            // Restore gravity
            player.RB.gravityScale = player.OriginalGravity; 
            
            base.Exit();
        }
    }
}
