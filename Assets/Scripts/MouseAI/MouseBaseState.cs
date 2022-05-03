using UnityEngine;

public abstract class MouseBaseState 
{
    public abstract void EnterState(MouseStateManager mouse);

    public abstract void ExecuteState(MouseStateManager mouse);

    public abstract void ExitState(MouseStateManager mouse);

    public abstract void OnCollisionEnter(MouseStateManager mouse);

}
