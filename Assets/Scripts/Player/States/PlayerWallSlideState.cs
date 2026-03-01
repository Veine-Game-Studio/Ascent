using UnityEngine;

namespace Ascent.Player
{
    public class PlayerWallSlideState : PlayerBaseState
    {
        private float clingTimer;

        public PlayerWallSlideState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            player.InputReader.JumpEvent += OnJump;
            
            // Start the cling timer to hold player briefly before sliding
            clingTimer = player.WallClingDuration;
            player.RB.linearVelocity = new Vector2(0f, 0f); // Stop current momentum
            
            base.Enter();
        }

        public override void Tick(float deltaTime)
        {
            // Exit 1: Drop if hitting the ground
            if (player.IsGrounded())
            {
                player.StateMachine.ChangeState(new PlayerGroundedState(player));
                return;
            }

            // Exit 2: Pull away from wall or slide off the bottom of the wall
            float inputX = player.InputReader.MovementValue.x;
            bool pullingAway = (player.transform.eulerAngles.y == 0 && inputX < 0) || 
                               (player.transform.eulerAngles.y == 180 && inputX > 0);

            if (!player.IsTouchingWall() || pullingAway)
            {
                // Give a slight horizontal push away so we don't immediately re-attach
                player.RB.linearVelocity = new Vector2(-inputX * 2f, player.RB.linearVelocity.y);
                player.StateMachine.ChangeState(new PlayerAirborneState(player, false));
                return;
            }
        }

        public override void FixedTick(float fixedDeltaTime)
        {
            float targetVelY = player.RB.linearVelocity.y;
            
            if (clingTimer > 0f)
            {
                // Cling to wall briefly without falling
                targetVelY = 0f;
                clingTimer -= fixedDeltaTime;
            }
            else
            {
                // Cap falling speed
                if (targetVelY < player.WallSlideSpeed)
                {
                    targetVelY = player.WallSlideSpeed;
                }
            }
            
            player.RB.linearVelocity = new Vector2(0f, targetVelY);
            base.FixedTick(fixedDeltaTime);
        }

        public override void Exit()
        {
            player.InputReader.JumpEvent -= OnJump;
            base.Exit();
        }

        private void OnJump()
        {
            // Wall Jump!
            float jumpDirectionX = player.transform.eulerAngles.y == 0 ? -1f : 1f;
            
            player.RB.linearVelocity = new Vector2(player.WallJumpForceX * jumpDirectionX, player.WallJumpForceY);
            
            // Flip the player visuals immediately
            player.FlipPlayerPivot(jumpDirectionX);

            // Change to jump state (isJumping = false tracking manual velocity, lockout = 0.2f seconds)
            player.StateMachine.ChangeState(new PlayerAirborneState(player, false, 0.2f));
        }
    }
}
