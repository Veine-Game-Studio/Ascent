using UnityEngine;
using Ascent.HSM;

namespace Ascent.Player
{
    public abstract class PlayerBaseState : IState
    {
        protected readonly PlayerController player;
        protected PlayerBaseState currentSubState;
        protected PlayerBaseState currentSuperState;

        protected PlayerBaseState(PlayerController player)
        {
            this.player = player;
        }

        public virtual void Enter()
        {
            currentSubState?.Enter();
        }

        public virtual void Tick(float deltaTime)
        {
            currentSubState?.Tick(deltaTime);
        }

        public virtual void FixedTick(float fixedDeltaTime)
        {
            currentSubState?.FixedTick(fixedDeltaTime);
        }

        public virtual void Exit()
        {
            currentSubState?.Exit();
        }

        public void SetSubState(PlayerBaseState newSubState)
        {
            currentSubState?.Exit();
            currentSubState = newSubState;
            currentSubState.SetSuperState(this);
            currentSubState.Enter();
        }

        public void SetSuperState(PlayerBaseState newSuperState)
        {
            currentSuperState = newSuperState;
        }

        protected void MoveBasedOnInput(float speedMultiplier = 1f)
        {
            Vector2 input = player.InputReader.MovementValue;
            
            // Apply horizontal velocity
            player.RB.linearVelocity = new Vector2(input.x * player.WalkSpeed * speedMultiplier, player.RB.linearVelocity.y);

            // Flip character
            player.FlipPlayerPivot(input.x);
        }
    }
}
