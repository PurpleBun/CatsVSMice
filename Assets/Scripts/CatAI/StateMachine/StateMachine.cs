using UnityEngine;

namespace CatAI
{
    public class StateMachine: MonoBehaviour
    {
        private IState currentState;
        private IState previousState;

        //Acquire new IState cartridge -> change to new state acquired
        public void ChangeState(IState newState)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }
            //Memorize previous IState
            previousState = currentState;
            //Switch to newest IState cartridge
            currentState = newState;
            currentState.Enter();
        }

        //Execute the IState (following changestate ^)
        public void ExecuteStateUpdate()
        {
            var runningState = currentState;
            if (runningState != null)
                currentState.Execute();
        }

        //Switch back to previous IState if needed
        public void SwitchToPreviousState()
        {
            currentState.Exit();
            currentState = previousState;
            currentState.Enter();
        }
    }
}