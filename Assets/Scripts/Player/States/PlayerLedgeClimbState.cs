using UnityEngine;

namespace Ascent.Player
{
    public class PlayerLedgeClimbState : PlayerBaseState
    {
        private Vector2 startPos;
        private Vector2 endPos;
        private float climbTimer;
        private Vector2 ledgeHitPoint;

        public PlayerLedgeClimbState(PlayerController player, Vector2 ledgeHitPoint) : base(player)
        {
            this.ledgeHitPoint = ledgeHitPoint;
        }

        public override void Enter()
        {
            startPos = player.transform.position;
            
            // Re-calculate the actual end position on the ledge.
            float direction = player.transform.eulerAngles.y == 0 ? 1f : -1f;
            endPos = ledgeHitPoint + new Vector2(player.LedgeClimbOffsetTop.x * direction, player.LedgeClimbOffsetTop.y);

            climbTimer = 0f;
            
            // Zero out velocities and make kinematic so we don't fall during climbing
            player.RB.linearVelocity = Vector2.zero;
            player.RB.bodyType = RigidbodyType2D.Kinematic;
        }

        public override void Tick(float deltaTime)
        {
            climbTimer += deltaTime;
            float normalizedTime = climbTimer / player.LedgeClimbDuration;

            if (normalizedTime >= 1f)
            {
                player.transform.position = endPos;
                player.StateMachine.ChangeState(new PlayerGroundedState(player));
                return;
            }

            // Lerp over the climb duration
            Vector2 targetPos = Vector2.Lerp(startPos, endPos, normalizedTime);
            player.transform.position = targetPos;
        }

        public override void FixedTick(float fixedDeltaTime)
        {
        }

        public override void Exit()
        {
            // Restore dynamic rigidbody
            player.RB.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
