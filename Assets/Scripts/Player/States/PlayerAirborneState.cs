using UnityEngine;

namespace Ascent.Player
{
    public class PlayerAirborneState : PlayerBaseState
    {
        private bool isJumping;
        private float lockoutTime;

        public PlayerAirborneState(PlayerController player, bool isJumping = false, float lockoutTime = 0f) : base(player) 
        { 
            this.isJumping = isJumping;
            this.lockoutTime = lockoutTime;
        }

        public override void Enter()
        {
            SetSubState(new PlayerJumpState(player, isJumping, lockoutTime));
            player.InputReader.DashEvent += OnDash;
            player.InputReader.GroundPoundEvent += OnGroundPound;
            base.Enter();
        }

        public override void Tick(float deltaTime)
        {
            // Landing check
            // We use a small positive threshold (0.01f) instead of <= 0f to account for
            // Unity zeroing out velocity upon a physical collision with the ground before Update runs.
            if (player.RB.linearVelocity.y <= 0.01f && player.IsGrounded())
            {
                player.StateMachine.ChangeState(new PlayerGroundedState(player));
                return;
            }

            // Wall slide check
            bool pushingWall = false;
            if (player.IsTouchingWall())
            {
                float inputX = player.InputReader.MovementValue.x;
                pushingWall = (player.transform.eulerAngles.y == 0 && inputX > 0) || 
                              (player.transform.eulerAngles.y == 180 && inputX < 0);
                              
                if (player.Abilities != null && player.Abilities.CanWallSlide && player.RB.linearVelocity.y < 0f && pushingWall)
                {
                    player.StateMachine.ChangeState(new PlayerWallSlideState(player));
                    return;
                }
            }

            // Ledge climb check
            if (player.RB.linearVelocity.y < 0f && player.CheckForLedge(out Vector2 ledgePoint))
            {
                float inputX = player.InputReader.MovementValue.x;
                bool pullingAway = (player.transform.eulerAngles.y == 0 && inputX < 0) || 
                                   (player.transform.eulerAngles.y == 180 && inputX > 0);
                                   
                if (!pullingAway)
                {
                    player.StateMachine.ChangeState(new PlayerLedgeClimbState(player, ledgePoint));
                    return;
                }
            }

            base.Tick(deltaTime);
        }

        public override void Exit()
        {
            player.InputReader.DashEvent -= OnDash;
            player.InputReader.GroundPoundEvent -= OnGroundPound;
            base.Exit();
        }

        private void OnDash()
        {
            if (player.CanDash() && !player.HasAirDashed)
            {
                player.StateMachine.ChangeState(new PlayerDashState(player));
            }
        }

        private void OnGroundPound()
        {
            if (player.Abilities != null && player.Abilities.CanGroundPound)
            {
                player.StateMachine.ChangeState(new PlayerGroundPoundState(player));
            }
        }
    }
}
