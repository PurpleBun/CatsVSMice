using UnityEngine;

namespace CatAI
{
    public class StateMachine
    {
        [SerializeField]
        private IState currentState;
        private IState previousState;

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
            //Debug.Log(currentState);
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