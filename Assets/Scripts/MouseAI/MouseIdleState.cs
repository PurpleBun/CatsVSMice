using UnityEngine;

public class MouseIdleState : MouseBaseState
{
    public override void EnterState(MouseStateManager mouse, MouseAbilityValues mouseStats)
    {
        Debug.Log("Entered idle state.");
        //Debug.Log(mouse.gameObject);
        ScanForCats(mouse.gameObject, mouseStats.visionDistance, mouseStats.layerCats, mouseStats.layerEnvironment);
    }

    public override void ExecuteState(MouseStateManager mouse, MouseAbilityValues mouseStats)
    {

    }

    public override void ExitState(MouseStateManager mouse, MouseAbilityValues mouseStats)
    {

    }

    public override void OnCollisionEnter(MouseStateManager mouse, MouseAbilityValues mouseStats)
    {

    }

    void ScanForCats(GameObject mouseObject, float scanRadius, LayerMask catsLayer, LayerMask environmentLayer)
    {
        //First, scan for any cats that are within mouse's vicinity, don't care for obstacles between them.
        Collider[] spottedCats = XRayScanForCats(mouseObject, scanRadius, catsLayer);
        if (spottedCats != null)
        {
            //If there are cats nearby - for every cat, check if there are any objects directly between the cat and the mouse.
            foreach (Collider spottedCat in spottedCats)
            {
                GameObject obstacle = ScanForObstacles(spottedCat, mouseObject, environmentLayer);
                if (obstacle != null)
                {
                    Debug.Log("Cat is behind " + obstacle);
                }
                else
                {
                    Debug.Log("Cat " + spottedCat.gameObject + " is visible.");
                }
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

    GameObject ScanForObstacles(Collider hitCat, GameObject mouse, LayerMask enviroLayer)
    {
        Transform catTranform = hitCat.transform;
        RaycastHit hit;
        if (Physics.Linecast(mouse.transform.position, catTranform.position, out hit, enviroLayer) == true)
        {            
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }
    }

    //bool CompareSize()
    //{ 
    //    if ()
    //}
}



