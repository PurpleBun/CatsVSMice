using UnityEngine;

public class MouseHealthyState : MouseBaseState
{
    public override void EnterState(MouseStateManager mouse)
    {
        Debug.Log("Entered healthy state.");
    }

    public override void ExecuteState(MouseStateManager mouse)
    { 
    
    }

    public override void ExitState(MouseStateManager mouse)
    { 
        
    }

    public override void OnCollisionEnter(MouseStateManager mouse)
    { 
        
    }
}
