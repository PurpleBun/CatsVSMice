using UnityEngine;

public class MouseIdleState : MouseBaseState
{    

    public override void EnterState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        Debug.Log("Entered idle state.");
        //Debug.Log(mouse.gameObject);
        mouseStats.catsFound = mouseStats.ScanForCats(mouse.gameObject, mouseStats.visionDistance, mouseStats.layerCats, mouseStats.layerEnvironment, mouse);
        if (mouseStats.catsFound == true || mouseStats.ignoresIdleState == true)
        {
            mouse.SwitchState(mouse.runningState);
        }
    }

    public override void ExecuteState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        mouseStats.catsFound = mouseStats.ScanForCats(mouse.gameObject, mouseStats.visionDistance, mouseStats.layerCats, mouseStats.layerEnvironment, mouse);
        if (mouseStats.catsFound == true || mouseStats.ignoresIdleState == true)
        {
            mouse.SwitchState(mouse.runningState);
        }
    }

    public override void ExitState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        Debug.Log("Exiting idle state.");
        mouseStats.catsFound = false;
    }

    public override void OnCollisionEnter(Collision collision, MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {

    }
}



