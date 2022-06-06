using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MouseAbilitiesNValues : MonoBehaviour
{
    public LayerMask layerCats, layerEnvironment, layerHoles, layerMice;
    public float visionDistance;
    public bool isSlow;
    public float slowCooldown, currentSlowCooldown;
    public Rigidbody mouseRB;
    public MeshRenderer mouseMeshRend;
    public Collider mouseCollider;
    public NavMeshAgent mouseNavMeshAgent;
    public Transform targetTransform;
    public Transform thisMouseTrans;
    public float hidingTime;
    public List<Collider> catsFound;
    public List<Collider> holesFound;
    public List<Collider> miceFound;
    public bool ignoresIdleState;
    public float hidingCooldown, currentHidingCooldown;
    public bool isHiding;
    public float normalSpeed, trappedSpeed;

    void Awake()
    {
        catsFound = new List<Collider>();
        holesFound = new List<Collider>();
        if (this.gameObject.GetComponent<NavMeshAgent>() != null)
        {
            mouseNavMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
            //Debug.Log(mouseNavMeshAgent);
        }
        else
        {
            Debug.LogError("The mouse " + this.gameObject + " lacks NavMeshAgent component.");
        }
        thisMouseTrans = this.gameObject.transform;
    }

    public List<Collider> ScanForObjects(GameObject mouseObject, float scanRadius, LayerMask objectsLayer, LayerMask environmentLayer, MouseStateManager mouseManager)
    {
        //First, scan for any cats that are within mouse's vicinity, don't care for obstacles between them.
        List<Collider> visibleObjects = new List<Collider>();
        Collider[] xRaySpottedObjects = XRayScanForObjects(mouseObject, scanRadius, objectsLayer);
        if (xRaySpottedObjects != null)
        {            
            //If there are objects nearby - for every object, check if there are any objects directly between the object and the mouse.
            foreach (Collider xRaySpottedObject in xRaySpottedObjects)
            {
                if (xRaySpottedObject.gameObject != this.gameObject)
                {
                    GameObject obstacleCenter = ScanForObstacles(mouseObject, environmentLayer, xRaySpottedObject.bounds.center);
                    GameObject obstacleMaxBound = ScanForObstacles(mouseObject, environmentLayer, xRaySpottedObject.bounds.max);
                    GameObject obstacleMinBound = ScanForObstacles(mouseObject, environmentLayer, xRaySpottedObject.bounds.min);
                    //Debug.Log(obstacleCenter + " " + obstacleMaxBound + " " + obstacleMinBound);
                    if (obstacleCenter != null && obstacleMaxBound != null && obstacleMinBound != null)
                    {
                        //Debug.Log("Object is hidden.");
                    }
                    else
                    {
                        //Debug.Log("Object " + spottedCat.gameObject + " is visible.");
                        visibleObjects.Add(xRaySpottedObject);
                    }
                }
            }
            if (visibleObjects.Count > 0)
            {
                return visibleObjects;
            }
            else
            {
                return null;
            }            
        }
        else
        {
            return null;
        }
    }

    Collider[] XRayScanForObjects(GameObject mouseObj, float xRayRadius, LayerMask objectLayer)
    {
        Collider[] hitObjectColliders = Physics.OverlapSphere(mouseObj.transform.position, xRayRadius, objectLayer);
        if (hitObjectColliders.Length != 0)
        {
            return hitObjectColliders;
        }
        else
        {
            return null;
        }
    }

    GameObject ScanForObstacles(GameObject mouse, LayerMask enviroLayer, Vector3 colliderBoundPos)
    {
        RaycastHit hit;
        if (Physics.Linecast(mouse.transform.position, colliderBoundPos, out hit, enviroLayer) == true)
        {
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }
    }

    public GameObject ScanInDirectionForObjects(Transform mouse, LayerMask desiredScanLayer, Vector3 scanDirection, float scanDistance)
    {
        RaycastHit hit;
        if (Physics.Raycast(mouse.position, scanDirection, out hit, scanDistance, desiredScanLayer) == true)
        {
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }
    }

    public void UseHideyHole(GameObject holeInUse)
    {
        if (holeInUse.GetComponent<HoleConnections>() != null)
        {
            HoleConnections thisHoleStats = holeInUse.GetComponent<HoleConnections>();
            if (thisHoleStats.ConnectedHoles.Count == 1)
            {
                GameObject connectingHole = thisHoleStats.ConnectedHoles[0];
                if (connectingHole.GetComponent<HoleConnections>() != null)
                {
                    HoleConnections newHoleStats = connectingHole.GetComponent<HoleConnections>();
                    thisMouseTrans.position = newHoleStats.spawnLocation;
                    targetTransform.position = new Vector3(newHoleStats.spawnLocation.x, targetTransform.position.y, newHoleStats.spawnLocation.z);
                }
            }
            else if (thisHoleStats.ConnectedHoles.Count > 1)
            {
                int randomIndex = Random.Range(0, thisHoleStats.ConnectedHoles.Count);
                GameObject connectingHole = thisHoleStats.ConnectedHoles[randomIndex];
                if (connectingHole.GetComponent<HoleConnections>() != null)
                {
                    HoleConnections newHoleStats = connectingHole.GetComponent<HoleConnections>();
                    thisMouseTrans.position = newHoleStats.spawnLocation;
                    targetTransform.position = new Vector3(newHoleStats.spawnLocation.x, targetTransform.position.y, newHoleStats.spawnLocation.z);
                }
            }
        }
    }
}
