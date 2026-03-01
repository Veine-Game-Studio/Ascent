using UnityEngine;

namespace Ascent.HSM
{
    public class StateMachine
    {
        private IState currentState;

        public void ChangeState(IState newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }

            currentState = newState;
            
            if (currentState != null)
            {
                currentState.Enter();
            }
        }

        public void Tick(float deltaTime)
        {
            if (currentState != null)
            {
                currentState.Tick(deltaTime);
            }
        }

        public void FixedTick(float fixedDeltaTime)
        {
            if (currentState != null)
            {
                currentState.FixedTick(fixedDeltaTime);
            }
        }
    }
}
