using UnityEngine;

public class MouseHidingState : MouseBaseState
{    
    private float timeLeft;

    public override void EnterState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        timeLeft = mouseStats.hidingTime;
        Debug.Log("Mouse is now hiding.");
        mouseStats.mouseRB.useGravity = false;
        mouseStats.mouseCollider.enabled = false;
        mouseStats.mouseMeshRend.enabled = false;
    }

    public override void ExecuteState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        timeLeft -= Time.deltaTime;
        Debug.Log(timeLeft);
        if (timeLeft <= 0 && mouseStats.ignoresIdleState == true)
        {
            mouse.SwitchState(mouse.runningState);
        }
        else if (timeLeft <= 0 && mouseStats.ignoresIdleState == false)
        {
            mouse.SwitchState(mouse.idleState);
        }
    }

    public override void ExitState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        Debug.Log("Exiting hiding state.");
        timeLeft = mouseStats.hidingTime;
        mouseStats.mouseMeshRend.enabled = true;
        mouseStats.mouseCollider.enabled = true;
        mouseStats.mouseRB.useGravity = true;
        mouseStats.catsFound = null;
        mouseStats.currentCooldown = mouseStats.hidingCooldown;
    }

    public override void OnCollisionEnter(Collision collision, MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {

    }
}
