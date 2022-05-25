using UnityEngine;

public class MouseIdleState : MouseBaseState
{    

    public override void EnterState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        Debug.Log("Entered idle state.");
        //Debug.Log(mouse.gameObject);
        mouseStats.catsFound = mouseStats.ScanForObjects(mouse.gameObject, mouseStats.visionDistance, mouseStats.layerCats, mouseStats.layerEnvironment, mouse);
        if ((mouseStats.catsFound != null && mouseStats.catsFound.Count > 0) || mouseStats.ignoresIdleState == true)
        {
            mouse.SwitchState(mouse.runningState);
        }
        mouseStats.holesFound = mouseStats.ScanForObjects(mouse.gameObject, mouseStats.visionDistance, mouseStats.layerHoles, mouseStats.layerEnvironment, mouse);
        //if (mouseStats.holesFound != null && mouseStats.holesFound.Count > 0)
        //{
        //    Debug.Log("Mouse sees " + mouseStats.holesFound.Count + " holes.");
        //}
    }

    public override void ExecuteState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        mouseStats.catsFound = mouseStats.ScanForObjects(mouse.gameObject, mouseStats.visionDistance, mouseStats.layerCats, mouseStats.layerEnvironment, mouse);
        if ((mouseStats.catsFound != null && mouseStats.catsFound.Count > 0) || mouseStats.ignoresIdleState == true)
        {
            mouse.SwitchState(mouse.runningState);
        }
        mouseStats.holesFound = mouseStats.ScanForObjects(mouse.gameObject, mouseStats.visionDistance, mouseStats.layerHoles, mouseStats.layerEnvironment, mouse);
        //if (mouseStats.holesFound != null && mouseStats.holesFound.Count > 0)
        //{
        //    Debug.Log("Mouse sees " + mouseStats.holesFound.Count + " holes.");
        //}
    }

    public override void ExitState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        Debug.Log("Exiting idle state.");
        //mouseStats.catsFound = null;
    }

    public override void OnCollisionEnter(Collision collision, MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {

    }
}



