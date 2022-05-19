using UnityEngine;

public class MouseIdleState : MouseBaseState
{
    public override void EnterState(MouseStateManager mouse, MouseAbilityValues mouseStats)
    {
        Debug.Log("Entered idle state.");
        //Debug.Log(mouse.gameObject);
        ScanForCats(mouse.gameObject, mouseStats.visionDistance, mouseStats.layerCats, mouseStats.layerEnvironment, mouse);
    }

    public override void ExecuteState(MouseStateManager mouse, MouseAbilityValues mouseStats)
    {
        ScanForCats(mouse.gameObject, mouseStats.visionDistance, mouseStats.layerCats, mouseStats.layerEnvironment, mouse);
    }

    public override void ExitState(MouseStateManager mouse, MouseAbilityValues mouseStats)
    {
        Debug.Log("Exiting idle state.");
    }

    public override void OnCollisionEnter(Collision collision, MouseStateManager mouse, MouseAbilityValues mouseStats)
    {

    }

    void ScanForCats(GameObject mouseObject, float scanRadius, LayerMask catsLayer, LayerMask environmentLayer, MouseStateManager mouseManager)
    {
        //First, scan for any cats that are within mouse's vicinity, don't care for obstacles between them.
        int visibleCats = 0;
        Collider[] spottedCats = XRayScanForCats(mouseObject, scanRadius, catsLayer);
        if (spottedCats != null)
        {
            Debug.Log("Mouse x-ray vision detected " + spottedCats.Length + " cats.");
            //If there are cats nearby - for every cat, check if there are any objects directly between the cat and the mouse.
            foreach (Collider spottedCat in spottedCats)
            {
                GameObject obstacleCenter = ScanForObstacles(spottedCat, mouseObject, environmentLayer, spottedCat.bounds.center);
                GameObject obstacleMaxBound = ScanForObstacles(spottedCat, mouseObject, environmentLayer, spottedCat.bounds.max);
                GameObject obstacleMinBound = ScanForObstacles(spottedCat, mouseObject, environmentLayer, spottedCat.bounds.min);
                Debug.Log(obstacleCenter + " " + obstacleMaxBound + " " + obstacleMinBound);
                if (obstacleCenter != null && obstacleMaxBound != null && obstacleMinBound != null)
                {
                    Debug.Log("Cat is hidden.");
                }
                else
                {
                    Debug.Log("Cat " + spottedCat.gameObject + " is visible.");
                    visibleCats++;
                }
            }
            if (visibleCats > 0)
            {
                mouseManager.SwitchState(mouseManager.runningState);
            }
        }
    }

    Collider[] XRayScanForCats(GameObject mouseObj, float xRayRadius, LayerMask catLayer)
    {
        Collider[] hitCatColliders = Physics.OverlapSphere(mouseObj.transform.position, xRayRadius, catLayer);
        if (hitCatColliders.Length != 0)
        {
            return hitCatColliders;
        }
        else
        {
            return null;
        }
    }

    GameObject ScanForObstacles(Collider hitCat, GameObject mouse, LayerMask enviroLayer, Vector3 colliderBoundPos)
    {        
        RaycastHit hit;
        if (Physics.Linecast(mouse.transform.position, colliderBoundPos, out hit, enviroLayer) == true)
        {            
            return hit.transform.gameObject;
        }
        else
        {
            Debug.Log(colliderBoundPos);
            return null;
        }
    }
}



