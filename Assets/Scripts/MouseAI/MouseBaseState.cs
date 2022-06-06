using UnityEngine;

public abstract class MouseBaseState 
{
    public abstract void EnterState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats);

    public abstract void ExecuteState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats);

    public abstract void ExitState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats);

    public abstract void OnCollisionEnter(Collision collision, MouseStateManager mouse, MouseAbilitiesNValues mouseStats);

}
