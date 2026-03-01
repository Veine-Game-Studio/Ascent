using UnityEngine;

namespace Ascent.Player
{
    public class PlayerGroundPoundState : PlayerBaseState
    {
        private enum PoundPhase
        {
            Anticipation,
            Plunge,
            Impact
        }

        private PoundPhase currentPhase;
        private float stateTimer;

        public PlayerGroundPoundState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            currentPhase = PoundPhase.Anticipation;
            stateTimer = 0f;

            // Stop momentum in the air
            player.RB.linearVelocity = Vector2.zero;
            player.RB.gravityScale = 0f;

            base.Enter();
        }

        public override void Tick(float deltaTime)
        {
            stateTimer += deltaTime;

            switch (currentPhase)
            {
                case PoundPhase.Anticipation:
                    if (stateTimer >= player.GroundPoundPauseDuration)
                    {
                        currentPhase = PoundPhase.Plunge;
                        player.RB.gravityScale = player.OriginalGravity;
                        player.RB.linearVelocity = new Vector2(0f, player.GroundPoundFallSpeed);
                    }
                    break;

                case PoundPhase.Plunge:
                    if (player.IsGrounded())
                    {
                        currentPhase = PoundPhase.Impact;
                        stateTimer = 0f;
                        player.RB.linearVelocity = Vector2.zero;
                        
                        // TODO: Add camera shake and spawn hitbox for breaking floors
                        Debug.Log("GROUND POUND IMPACT!");
                    }
                    break;

                case PoundPhase.Impact:
                    if (stateTimer >= player.GroundPoundImpactDuration)
                    {
                        player.StateMachine.ChangeState(new PlayerGroundedState(player));
                        return;
                    }
                    break;
            }

            base.Tick(deltaTime);
        }

        public override void FixedTick(float fixedDeltaTime)
        {
            if (currentPhase == PoundPhase.Plunge)
            {
                // Force fast fall speed
                player.RB.linearVelocity = new Vector2(0f, player.GroundPoundFallSpeed);
            }
            else if (currentPhase == PoundPhase.Impact || currentPhase == PoundPhase.Anticipation)
            {
                // Lock velocity
                player.RB.linearVelocity = Vector2.zero;
            }

            base.FixedTick(fixedDeltaTime);
        }

        public override void Exit()
        {
            player.RB.gravityScale = player.OriginalGravity;
            base.Exit();
        }
    }
}
