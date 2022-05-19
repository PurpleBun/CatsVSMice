using UnityEngine;

public class MouseHidingState : MouseBaseState
{    
    private float timeLeft;

    public override void EnterState(MouseStateManager mouse, MouseAbilityValues mouseStats)
    {
        Debug.Log("Mouse is now hiding.");
        mouseStats.mouseRB.useGravity = false;
        mouseStats.mouseCollider.enabled = false;
        mouseStats.mouseMeshRend.enabled = false;
    }

    public override void ExecuteState(MouseStateManager mouse, MouseAbilityValues mouseStats)
    {
        timeLeft -= Time.deltaTime;
        Debug.Log(timeLeft);
        if (timeLeft <= 0)
        {
            mouse.SwitchState(mouse.idleState);
        }
    }

    public override void ExitState(MouseStateManager mouse, MouseAbilityValues mouseStats)
    {
        Debug.Log("Exiting hiding state.");
        timeLeft = mouseStats.hidingTime;
        mouseStats.mouseMeshRend.enabled = true;
        mouseStats.mouseCollider.enabled = true;
        mouseStats.mouseRB.useGravity = true;
    }

    public override void OnCollisionEnter(Collision collision, MouseStateManager mouse, MouseAbilityValues mouseStats)
    {

    }
}
