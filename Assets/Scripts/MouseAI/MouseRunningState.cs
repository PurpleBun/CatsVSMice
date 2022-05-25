using UnityEngine;

public class MouseRunningState : MouseBaseState
{
    public override void EnterState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        Debug.Log("Entered running state.");
        mouseStats.catsFound = mouseStats.ScanForObjects(mouse.gameObject, mouseStats.visionDistance, mouseStats.layerCats, mouseStats.layerEnvironment, mouse);
        if ((mouseStats.catsFound == null || mouseStats.catsFound.Count == 0) && mouseStats.ignoresIdleState == false)
        {
            mouse.SwitchState(mouse.idleState);
        }
        mouseStats.holesFound = mouseStats.ScanForObjects(mouse.gameObject, mouseStats.visionDistance, mouseStats.layerHoles, mouseStats.layerEnvironment, mouse);
        //if (mouseStats.holesFound != null && mouseStats.holesFound.Count > 0)
        //{
        //    Debug.Log("Mouse sees " + mouseStats.holesFound.Count + " holes.");
        //}
    }

    public override void ExecuteState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        mouse.mouseNavMeshAgent.destination = mouse.targetTransform.position;
        mouseStats.catsFound = mouseStats.ScanForObjects(mouse.gameObject, mouseStats.visionDistance, mouseStats.layerCats, mouseStats.layerEnvironment, mouse);
        if ((mouseStats.catsFound == null || mouseStats.catsFound.Count == 0) && mouseStats.ignoresIdleState == false)
        {
            mouse.SwitchState(mouse.idleState);
        }
        if (mouseStats.currentCooldown > 0)
        {
            mouseStats.currentCooldown -= Time.deltaTime;
        }
        mouseStats.holesFound = mouseStats.ScanForObjects(mouse.gameObject, mouseStats.visionDistance, mouseStats.layerHoles, mouseStats.layerEnvironment, mouse);
        //if (mouseStats.holesFound != null && mouseStats.holesFound.Count > 0)
        //{
        //    Debug.Log("Mouse sees " + mouseStats.holesFound.Count + " holes.");
        //    foreach (Collider holeFound in mouseStats.holesFound)
        //    {
        //        Debug.Log(holeFound);
        //    }
        //}
    }

    public override void ExitState(MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        Debug.Log("Exiting running state.");
        mouseStats.catsFound = null;
    }

    public override void OnCollisionEnter(Collision collision, MouseStateManager mouse, MouseAbilitiesNValues mouseStats)
    {
        GameObject collidedObj;
        if (collision.gameObject != null)
        {
            collidedObj = collision.gameObject;
            CheckCollidingObject(collidedObj, mouse, mouseStats);
        }
        else
        {
            Debug.Log("Error: Cannot perform OnCollisionEnter. Collision has no game object attached to it.");
        }
    }

    public void CheckCollidingObject(GameObject collisionObject, MouseStateManager mouseStMangr, MouseAbilitiesNValues mouseStats)
    {
        if (collisionObject.CompareTag("Cat") == true)
        {
            Debug.Log("Mouse got caught.");
            ExitState(mouseStMangr, mouseStMangr.thisMouseStats);
        }
        else if (collisionObject.CompareTag("Hole") == true && mouseStats.currentCooldown <= 0)
        {
            mouseStMangr.SwitchState(mouseStMangr.hidingState);
        }
    }
}
