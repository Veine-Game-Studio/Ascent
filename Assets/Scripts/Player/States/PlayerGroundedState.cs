using UnityEngine;

namespace Ascent.Player
{
    public class PlayerGroundedState : PlayerBaseState
    {
        public PlayerGroundedState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            // Initial sub-state based on input
            if (player.InputReader.MovementValue.x == 0f)
            {
                SetSubState(new PlayerIdleState(player));
            }
            else
            {
                SetSubState(new PlayerWalkState(player));
            }

            player.HasAirDashed = false; // Reset air dash when touching ground
            player.InputReader.JumpEvent += OnJump;
            player.InputReader.DashEvent += OnDash;
            
            base.Enter();
        }

        public override void Tick(float deltaTime)
        {
            // Check for falling
            if (!player.IsGrounded() && player.RB.linearVelocity.y < 0)
            {
                player.StateMachine.ChangeState(new PlayerAirborneState(player));
                return;
            }

            base.Tick(deltaTime);
        }

        public override void Exit()
        {
            player.InputReader.JumpEvent -= OnJump;
            player.InputReader.DashEvent -= OnDash;
            base.Exit();
        }

        private void OnJump()
        {
            if (player.IsGrounded())
            {
                player.StateMachine.ChangeState(new PlayerAirborneState(player, true));
            }
        }

        private void OnDash()
        {
            if (player.CanDash())
            {
                player.StateMachine.ChangeState(new PlayerDashState(player));
            }
        }
    }
}
