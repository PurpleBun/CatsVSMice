using UnityEngine;

public class MouseHidingState : MouseBaseState
{    
    private float timeLeft;
    private Vector3 targetPosition;
    private Vector3 mousePosition;

    public override void EnterState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        timeLeft = mouseStats.hidingTime;
        Debug.Log("Mouse is now hiding.");
        mouseStats.mouseRB.useGravity = false;
        mouseStats.mouseCollider.enabled = false;
        mouseStats.mouseMeshRend.enabled = false;
        mouseStats.isHiding = true;
        mouseStats.mouseNavMeshAgent.enabled = false;
        targetPosition = mouseStats.targetTransform.position;
        mousePosition = mouseStats.thisMouseTrans.position;
        mouseStats.thisMouseTrans.position = new Vector3(targetPosition.x, mousePosition.y, targetPosition.z);
    }

    public override void ExecuteState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        timeLeft -= Time.deltaTime;
        mouseStats.isHiding = true;
        //Debug.Log(timeLeft);
        if (timeLeft <= 0 && mouseStats.ignoresIdleState == true)
        {
            mouse.SwitchState(mouse.runningState);
        }
        else if (timeLeft <= 0 && mouseStats.ignoresIdleState == false)
        {
            mouse.SwitchState(mouse.idleState);
        }
        targetPosition = mouseStats.targetTransform.position;
        mousePosition = mouseStats.thisMouseTrans.position;
        mouseStats.thisMouseTrans.position = new Vector3(targetPosition.x, mousePosition.y, targetPosition.z);
    }

    public override void ExitState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        Debug.Log("Exiting hiding state.");
        mouseStats.mouseNavMeshAgent.enabled = true;
        timeLeft = mouseStats.hidingTime;
        mouseStats.mouseMeshRend.enabled = true;
        mouseStats.mouseCollider.enabled = true;
        mouseStats.mouseRB.useGravity = true;
        mouseStats.catsFound = null;
        mouseStats.currentHidingCooldown = mouseStats.hidingCooldown;
        mouseStats.isHiding = false;
        targetPosition = mouseStats.targetTransform.position;
        mousePosition = mouseStats.thisMouseTrans.position;
        mouseStats.thisMouseTrans.position = new Vector3(targetPosition.x, mousePosition.y, targetPosition.z);        
    }

    public override void OnCollisionEnter(Collision collision, MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {

    }
}
