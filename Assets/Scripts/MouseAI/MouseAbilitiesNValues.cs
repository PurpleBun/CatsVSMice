using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAbilitiesNValues : MonoBehaviour
{
    public LayerMask layerCats, layerEnvironment;
    public float visionDistance;
    public bool isSlow;
    public Rigidbody mouseRB;
    public MeshRenderer mouseMeshRend;
    public Collider mouseCollider;
    public float hidingTime;
    public bool catsFound;
    public bool ignoresIdleState;

    public bool ScanForCats(GameObject mouseObject, float scanRadius, LayerMask catsLayer, LayerMask environmentLayer, MouseStateManager mouseManager)
    {
        //First, scan for any cats that are within mouse's vicinity, don't care for obstacles between them.
        int visibleCats = 0;
        Collider[] spottedCats = XRayScanForCats(mouseObject, scanRadius, catsLayer);
        if (spottedCats != null)
        {
            //Debug.Log("Mouse x-ray vision detected " + spottedCats.Length + " cats.");
            //If there are cats nearby - for every cat, check if there are any objects directly between the cat and the mouse.
            foreach (Collider spottedCat in spottedCats)
            {
                GameObject obstacleCenter = ScanForObstacles(spottedCat, mouseObject, environmentLayer, spottedCat.bounds.center);
                GameObject obstacleMaxBound = ScanForObstacles(spottedCat, mouseObject, environmentLayer, spottedCat.bounds.max);
                GameObject obstacleMinBound = ScanForObstacles(spottedCat, mouseObject, environmentLayer, spottedCat.bounds.min);
                //Debug.Log(obstacleCenter + " " + obstacleMaxBound + " " + obstacleMinBound);
                if (obstacleCenter != null && obstacleMaxBound != null && obstacleMinBound != null)
                {
                    //Debug.Log("Cat is hidden.");
                }
                else
                {
                    //Debug.Log("Cat " + spottedCat.gameObject + " is visible.");
                    visibleCats++;
                }
            }
            if (visibleCats > 0)
            {                
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
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
            //Debug.Log(colliderBoundPos);
            return null;
        }
    }
}
