using UnityEngine;

public abstract class MouseBaseState 
{
    public abstract void EnterState(MouseStateManager mouse, MouseAbilityValues mouseStats);

    public abstract void ExecuteState(MouseStateManager mouse, MouseAbilityValues mouseStats);

    public abstract void ExitState(MouseStateManager mouse, MouseAbilityValues mouseStats);

    public abstract void OnCollisionEnter(Collision collision, MouseStateManager mouse, MouseAbilityValues mouseStats);

}
