using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAISashaEdition : MonoBehaviour
{
    private MouseAbilitiesNValues mouseAbsNVals;
    private Transform thisMouseTarget;
    private Transform thisMouse;
    [SerializeField]
    private float safeDistanceToHole, safeDistanceDifference;
    private float currentDistanceToHole;
    private bool holeWasChosen = false;
    //private 
    // Start is called before the first frame update
    void Awake()
    {
        if (this.gameObject.GetComponent<MouseAbilitiesNValues>() != null)
        {
            mouseAbsNVals = this.gameObject.GetComponent<MouseAbilitiesNValues>();
            thisMouseTarget = mouseAbsNVals.targetTransform;
            thisMouse = mouseAbsNVals.thisMouseTrans;
        }
        else
        {
            Debug.Log(this.gameObject + " has no MouseAbilitiesNValues componenet attached.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseAbsNVals.isHiding == false)
        {
            PrioritizeWhereToGo(mouseAbsNVals);
        }
        else if (mouseAbsNVals.isHiding == true && holeWasChosen == true)
        {
            holeWasChosen = false;
        }
    }

    private void PrioritizeWhereToGo(MouseAbilitiesNValues mouseStats)
    {
        if ((mouseStats.catsFound == null || mouseStats.catsFound.Count == 0) && (mouseStats.holesFound != null && mouseStats.holesFound.Count != 0))
        {
            if (mouseStats.currentCooldown <= 0)
            {
                if (mouseStats.holesFound.Count == 1)
                {
                    Transform holeTransform = mouseStats.holesFound[0].transform;
                    currentDistanceToHole = CalculateDistanceToObject(thisMouse, holeTransform);
                    MoveToSafeDistanceToHole(safeDistanceToHole, currentDistanceToHole, holeTransform);
                }
                else
                {
                    FindAndMoveToClosestHole(mouseStats.holesFound);
                }
            }
        }
        else if ((mouseStats.catsFound != null && mouseStats.catsFound.Count != 0) && (mouseStats.holesFound == null || mouseStats.holesFound.Count == 0))
        {
            holeWasChosen = false;
            OperationRunAway(mouseStats.catsFound, thisMouse, thisMouseTarget);
        }
        else if ((mouseStats.catsFound != null && mouseStats.catsFound.Count != 0) && (mouseStats.holesFound != null && mouseStats.holesFound.Count != 0))
        {
            if (mouseStats.currentCooldown <= 0)
            {
                if (holeWasChosen == false)
                {
                    List<Collider> optimalHoles = new List<Collider>();
                    FindOptimalHoles(optimalHoles, mouseStats.catsFound, mouseStats.holesFound);
                    if (optimalHoles.Count == 0)
                    {
                        OperationRunAway(mouseStats.catsFound, thisMouse, thisMouseTarget);
                    }
                    else if (optimalHoles.Count == 1)
                    {
                        Transform chosenHole = optimalHoles[0].transform;
                        thisMouseTarget.position = new Vector3(chosenHole.position.x, thisMouseTarget.position.y, chosenHole.position.z);
                        Debug.Log("Going to hole " + chosenHole.name);
                        holeWasChosen = true;
                    }
                    else
                    {
                        int randomIndex = Random.Range(0, optimalHoles.Count);
                        Transform chosenHole = optimalHoles[randomIndex].transform;
                        thisMouseTarget.position = new Vector3(chosenHole.position.x, thisMouseTarget.position.y, chosenHole.position.z);
                        Debug.Log("Going to hole " + chosenHole.name);
                        holeWasChosen = true;
                    }
                }
            }
            else
            {
                OperationRunAway(mouseStats.catsFound, thisMouse, thisMouseTarget);
            }
        }
        else if ((mouseStats.catsFound == null || mouseStats.catsFound.Count == 0) && (mouseStats.holesFound == null || mouseStats.holesFound.Count == 0))
        {
            //OperationWander(thisMouse);
        }
    }

    private float CalculateDistanceToObject(Transform targetTransformA, Transform targetTransformB)
    {
        float distance;
        distance = Vector3.Distance(targetTransformA.position, targetTransformB.position);
        //Debug.Log("Distance between " + targetTransformA.name + " and " + targetTransformB.name + " is " + distance + ".");
        return distance;
    }

    private void MoveToSafeDistanceToHole(float safeDistance, float currentDistance, Transform hole)
    {
        if (safeDistance < currentDistance)
        {
            thisMouseTarget.position = new Vector3(hole.position.x, thisMouseTarget.position.y, hole.position.z);
        }
        else if (safeDistanceToHole >= currentDistance && (thisMouseTarget.position.x != thisMouse.position.x || thisMouseTarget.position.z != thisMouse.position.z))
        {
            thisMouseTarget.position = new Vector3(thisMouse.position.x, thisMouseTarget.position.y, thisMouse.position.z);
        }
    }

    private void FindAndMoveToClosestHole(List<Collider> allHoles)
    {
        List<float> distancesToHoles = new List<float>();
        Transform closestHole = null;
        foreach (Collider holeFound in allHoles)
        {
            Transform holeTransform = holeFound.transform;
            currentDistanceToHole = CalculateDistanceToObject(thisMouse, holeTransform);
            distancesToHoles.Add(currentDistanceToHole);
            distancesToHoles.Sort();
            if (distancesToHoles[0] == currentDistanceToHole)
            {
                closestHole = holeTransform;
            }
        }
        MoveToSafeDistanceToHole(safeDistanceToHole, distancesToHoles[0], closestHole);
    }

    private void OperationRunAway(List<Collider> cats, Transform mouse, Transform mouseTarget)
    {
        Vector3 runAwayVector;
        if (cats.Count == 1)
        {
            Transform catTransform = cats[0].transform;
            runAwayVector = DetermineRunAwayVector(mouse, catTransform);
            DetermineTarget(runAwayVector, mouse.position, mouseTarget);
        }
        else
        {
            Vector3 totalRunAwayVector = DetermineTotalRunAwayVector(mouse, cats);
            DetermineTarget(totalRunAwayVector, mouse.position, mouseTarget);
        }
    }

    private Vector3 CalculateTargetAwayFromOneCat(Vector3 mousePosition, Vector3 catPosition)
    {
        Vector3 directionToCat = mousePosition - catPosition;
        Vector3 targetPosition = mousePosition + directionToCat;
        return targetPosition;
    }

    private Vector3 DetermineRunAwayVector(Transform mouse, Transform cat)
    {
        Vector3 runTarget = CalculateTargetAwayFromOneCat(mouse.position, cat.position);
        Vector3 runVector = runTarget - thisMouse.position;
        runVector.Normalize();
        return runVector;
    }

    private Vector3 DetermineTotalRunAwayVector(Transform mouse, List<Collider> cats)
    {
        Vector3 totalRunVector = Vector3.zero;
        foreach (Collider cat in cats)
        {
            Transform catTransform = cat.transform;
            Vector3 runVector = DetermineRunAwayVector(mouse, catTransform);
            totalRunVector += runVector;
        }
        totalRunVector.Normalize();
        return totalRunVector;
    }

    private void DetermineTarget(Vector3 runVector, Vector3 mousePosition, Transform mouseTargetObj)
    {
        float targetX = mousePosition.x + runVector.x;
        float targetZ = mousePosition.z + runVector.z;
        thisMouseTarget.position = new Vector3(targetX, thisMouseTarget.position.y, targetZ);
    }

    private void FindOptimalHoles(List<Collider> optimalHoles, List<Collider> cats, List<Collider> holes)
    {
        foreach (Collider hole in holes)
        {
            int suboptimalDistances = 0;
            //Debug.Log(holeFound.name);
            float distanceToMouse = CalculateDistanceToObject(thisMouse, hole.transform);
            foreach (Collider catFound in cats)
            {
                float distanceToCat = CalculateDistanceToObject(catFound.transform, hole.transform);
                float distanceDifference = GetDistanceDifference(distanceToMouse, distanceToCat);
                if (distanceDifference < safeDistanceDifference || distanceToCat < distanceToMouse)
                {
                    suboptimalDistances++;
                }
            }
            if (suboptimalDistances == 0)
            {
                optimalHoles.Add(hole);
                Debug.Log("Hole " + hole.name + " is optimal");
            }
        }
    }

    private float GetDistanceDifference(float distanceA, float distanceB)
    {
        float differenceRegular = distanceA - distanceB;
        float differenceAbsolute = Mathf.Abs(differenceRegular);
        //Debug.Log("Distance difference between " + distanceA + " and " + distanceB + " is " + differenceAbsolute + ".");
        return differenceAbsolute;
    }

    //private void OperationWander(Transform mouse)
    //{
    //    GameObject obstacle;
    //    List<GameObject> obstacles = new List<GameObject>();
    //    List<Vector3> noObstacles = new List<Vector3>();
    //    List<Vector3> obstaclesFar = new List<Vector3>();
    //    List<Vector3> obstaclesMedium = new List<Vector3>();
    //    List<Vector3> obstaclesNear = new List<Vector3>();
    //    for (int i = 0; i < 8; i++)
    //    {
    //        switch (i)
    //        {
    //            case 0:
    //                obstacle = mouseAbsNVals.ScanInDirectionForObjects(mouse, mouseAbsNVals.layerEnvironment, Vector3.forward, mouseAbsNVals.visionDistance);
    //                if (obstacle == null)
    //                {
    //                    noObstacles.Add(Vector3.forward);
    //                }
    //                else
    //                {
    //                    float distanceToObstacle = Vector3.Distance(mouse.position, obstacle.transform.position);
    //                    if
    //                }
    //                //Debug.Log(Vector3.forward + " " + obstacle);
    //                break;
    //            case 1:
    //                obstacle = mouseAbsNVals.ScanInDirectionForObjects(mouse, mouseAbsNVals.layerEnvironment, new Vector3(1, 0, 1), mouseAbsNVals.visionDistance);
    //                obstacles.Add(obstacle);
    //                //Debug.Log(new Vector3(1, 0, 1) + " " + obstacle);
    //                break;
    //            case 2:
    //                obstacle = mouseAbsNVals.ScanInDirectionForObjects(mouse, mouseAbsNVals.layerEnvironment, Vector3.right, mouseAbsNVals.visionDistance);
    //                obstacles.Add(obstacle);
    //                //Debug.Log(Vector3.right + " " + obstacle);
    //                break;
    //            case 3:
    //                obstacle = mouseAbsNVals.ScanInDirectionForObjects(mouse, mouseAbsNVals.layerEnvironment, new Vector3(1, 0, -1), mouseAbsNVals.visionDistance);
    //                obstacles.Add(obstacle);
    //                //Debug.Log(new Vector3(1, 0, -1) + " " + obstacle);
    //                break;
    //            case 4:
    //                obstacle = mouseAbsNVals.ScanInDirectionForObjects(mouse, mouseAbsNVals.layerEnvironment, Vector3.back, mouseAbsNVals.visionDistance);
    //                obstacles.Add(obstacle);
    //                //Debug.Log(Vector3.back + " " + obstacle);
    //                break;
    //            case 5:
    //                obstacle = mouseAbsNVals.ScanInDirectionForObjects(mouse, mouseAbsNVals.layerEnvironment, new Vector3(-1, 0, -1), mouseAbsNVals.visionDistance);
    //                obstacles.Add(obstacle);
    //                //Debug.Log(new Vector3(-1, 0, -1) + " " + obstacle);
    //                break;
    //            case 6:
    //                obstacle = mouseAbsNVals.ScanInDirectionForObjects(mouse, mouseAbsNVals.layerEnvironment, Vector3.left, mouseAbsNVals.visionDistance);
    //                obstacles.Add(obstacle);
    //                //Debug.Log(Vector3.left + " " + obstacle);
    //                break;
    //            case 7:
    //                obstacle = mouseAbsNVals.ScanInDirectionForObjects(mouse, mouseAbsNVals.layerEnvironment, new Vector3(-1, 0, 1), mouseAbsNVals.visionDistance);
    //                obstacles.Add(obstacle);
    //                //Debug.Log(new Vector3(-1, 0, 1) + " " + obstacle);
    //                break;
    //        }
    //    }
    //}
}
