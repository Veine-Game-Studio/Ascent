using UnityEngine;

namespace Ascent.Player
{
    public class PlayerWalkState : PlayerBaseState
    {
        public PlayerWalkState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Tick(float deltaTime)
        {
            if (player.InputReader.MovementValue.x == 0f)
            {
                currentSuperState.SetSubState(new PlayerIdleState(player));
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
