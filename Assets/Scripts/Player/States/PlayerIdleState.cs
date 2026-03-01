using UnityEngine;

namespace Ascent.Player
{
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Tick(float deltaTime)
        {
            if (player.InputReader.MovementValue.x != 0f)
            {
                currentSuperState.SetSubState(new PlayerWalkState(player));
                return;
            }
            
            base.Tick(deltaTime);
        }

        public override void FixedTick(float fixedDeltaTime)
        {
            MoveBasedOnInput();
            base.FixedTick(fixedDeltaTime);
        }
    }
}
