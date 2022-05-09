using UnityEngine;

public class MouseRunningState : MouseBaseState
{
    public override void EnterState(MouseStateManager mouse, MouseAbilityValues mouseStats)
    {
        Debug.Log("Entered running state.");
    }

    public override void ExecuteState(MouseStateManager mouse, MouseAbilityValues mouseStats)
    {

    }

    public override void ExitState(MouseStateManager mouse, MouseAbilityValues mouseStats)
    {
        Debug.Log("Exiting running state.");
    }

    public override void OnCollisionEnter(Collision collision, MouseStateManager mouse, MouseAbilityValues mouseStats)
    {
        GameObject collidedObj;
        if (collision.gameObject != null)
        {
            collidedObj = collision.gameObject;
            CheckCollidingObject(collidedObj, mouse);
        }
        else
        {
            Debug.Log("Error: Cannot perform OnCollisionEnter. Collision has no game object attached to it.");
        }
    }

    public void CheckCollidingObject(GameObject collisionObject, MouseStateManager mouseStMangr)
    {
        if (collisionObject.CompareTag("Cat") == true)
        {
            Debug.Log("Mouse got caught.");
            ExitState(mouseStMangr, mouseStMangr.thisMouseStats);
        }
        else if (collisionObject.CompareTag("Hole") == true)
        {
            mouseStMangr.SwitchState(mouseStMangr.hidingState);
        }
    }
}
