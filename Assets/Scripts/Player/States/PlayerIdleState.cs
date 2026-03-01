using UnityEngine;

namespace Ascent.Player
{
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            player.RB.linearVelocity = new Vector2(0f, player.RB.linearVelocity.y);
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
    }
}
