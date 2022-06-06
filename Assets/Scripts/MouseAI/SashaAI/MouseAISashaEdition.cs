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
    private Transform holeNear;
    private float thisVisionDistance;
    private float directionChangeColldown;
    [SerializeField]
    private float cooldownVeryLong, cooldownLong, cooldownMedium, cooldownShort;
    private Vector3 oldDirectionToGo;
    [SerializeField]
    private List<Collider> memorizedHoles, miceInSight;

    // Start is called before the first frame update
    void Awake()
    {
        if (this.gameObject.GetComponent<MouseAbilitiesNValues>() != null)
        {
            mouseAbsNVals = this.gameObject.GetComponent<MouseAbilitiesNValues>();
            thisMouseTarget = mouseAbsNVals.targetTransform;
            thisMouse = mouseAbsNVals.thisMouseTrans;
            thisVisionDistance = mouseAbsNVals.visionDistance;
        }
        else
        {
            Debug.Log(this.gameObject + " has no MouseAbilitiesNValues componenet attached.");
        }
    }

    void Start()
    {
        memorizedHoles = new List<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        miceInSight = mouseAbsNVals.miceFound;
        if (mouseAbsNVals.isHiding == false)
        {
            PrioritizeWhereToGo(mouseAbsNVals);
        }
    }

    private void PrioritizeWhereToGo(MouseAbilitiesNValues mouseStats)
    {
        if ((mouseStats.catsFound == null || mouseStats.catsFound.Count == 0) && (mouseStats.holesFound != null && mouseStats.holesFound.Count != 0))
        {
            MemorizeHoles(mouseStats.holesFound);
            if (mouseStats.currentHidingCooldown <= 0)
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
            else
            {
                AddSlowIntoStrategy();
            }
        }
        else if ((mouseStats.catsFound != null && mouseStats.catsFound.Count != 0) && (mouseStats.holesFound == null || mouseStats.holesFound.Count == 0))
        {
            if (mouseStats.currentHidingCooldown <= 0 && memorizedHoles.Count > 0)
            {

                OperationCatsVSHoles(memorizedHoles, mouseStats);
            }
            else
            {
                if (miceInSight == null || miceInSight.Count == 0)
                {
                    OperationRunAway(mouseStats.catsFound, thisMouse, thisMouseTarget);
                }
                else
                {
                    AddMiceIntoStrategy(miceInSight, mouseStats);
                }
            }
        }
        else if ((mouseStats.catsFound != null && mouseStats.catsFound.Count != 0) && (mouseStats.holesFound != null && mouseStats.holesFound.Count != 0))
        {
            MemorizeHoles(mouseStats.holesFound);
            if (mouseStats.currentHidingCooldown <= 0)
            {
                OperationCatsVSHoles(mouseStats.holesFound, mouseStats);
            }
            else
            {
                OperationRunAway(mouseStats.catsFound, thisMouse, thisMouseTarget);
            }
        }
        else if ((mouseStats.catsFound == null || mouseStats.catsFound.Count == 0) && (mouseStats.holesFound == null || mouseStats.holesFound.Count == 0))
        {
            AddSlowIntoStrategy();
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
        holeNear = hole;
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

    private void OperationRunAway(List<Collider> targetsToFleeFrom, Transform mouse, Transform mouseRunToTarget)
    {
        //Debug.Log("Entered operation run away.");
        Vector3 runAwayVector;
        if (targetsToFleeFrom.Count == 1)
        {
            Transform fleeTargetTransform = targetsToFleeFrom[0].transform;
            runAwayVector = DetermineRunAwayVector(mouse, fleeTargetTransform);
            DetermineTarget(runAwayVector, mouse.position, mouseRunToTarget);
        }
        else
        {
            Vector3 totalRunAwayVector = DetermineTotalRunAwayVector(mouse, targetsToFleeFrom);
            DetermineTarget(totalRunAwayVector, mouse.position, mouseRunToTarget);
        }
    }

    private Vector3 CalculateTargetAwayFromOneObject(Vector3 mousePosition, Vector3 objectToFleePosition)
    {
        Vector3 directionToObject = mousePosition - objectToFleePosition;
        Vector3 targetPosition = mousePosition + directionToObject;
        return targetPosition;
    }

    private Vector3 DetermineRunAwayVector(Transform mouse, Transform targetToFleeFrom)
    {
        Vector3 runTarget = CalculateTargetAwayFromOneObject(mouse.position, targetToFleeFrom.position);
        Vector3 runVector = runTarget - thisMouse.position;
        runVector.Normalize();
        return runVector;
    }

    private Vector3 DetermineTotalRunAwayVector(Transform mouse, List<Collider> fleeTargets)
    {
        Vector3 totalRunVector = Vector3.zero;
        foreach (Collider fleeTarget in fleeTargets)
        {
            Transform fleeTargetTransform = fleeTarget.transform;
            Vector3 runVector = DetermineRunAwayVector(mouse, fleeTargetTransform);
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

    private void DetermineRunOrHide(List<Collider> goodHoles, MouseAbilitiesNValues mouseStats)
    {
        if (goodHoles.Count == 0)
        {
            if (miceInSight == null || miceInSight.Count == 0)
            {
                OperationRunAway(mouseStats.catsFound, thisMouse, thisMouseTarget);
            }
            else
            {
                AddMiceIntoStrategy(miceInSight, mouseStats);
            }
        }
        else
        {
            int randomIndex = Random.Range(0, goodHoles.Count);
            Transform chosenHole = goodHoles[randomIndex].transform;
            thisMouseTarget.position = new Vector3(chosenHole.position.x, thisMouseTarget.position.y, chosenHole.position.z);
            //Debug.Log("Going to hole " + chosenHole.name);
        }
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
                //Debug.Log("Hole " + hole.name + " is optimal");
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

    private void OperationWander(Transform mouse, Transform mouseTarget)
    {
        Vector3 directionToGo;
        List<Vector3> directionsWithNoObstacles = new List<Vector3>();
        List<Vector3> directionsWithObstaclesFar = new List<Vector3>();
        List<Vector3> directionsWithObstaclesMidway = new List<Vector3>();
        List<Vector3> directionsWithObstaclesNear = new List<Vector3>();
        if (directionChangeColldown > 0)
        {
            directionChangeColldown -= Time.deltaTime;
            DetermineTarget(oldDirectionToGo, mouse.position, mouseTarget);
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 0:
                        FindObstacleAndDistanceToObstacle(mouse, Vector3.forward, directionsWithNoObstacles, directionsWithObstaclesFar, directionsWithObstaclesMidway, directionsWithObstaclesNear);
                        break;
                    case 1:
                        FindObstacleAndDistanceToObstacle(mouse, new Vector3(1, 0, 1), directionsWithNoObstacles, directionsWithObstaclesFar, directionsWithObstaclesMidway, directionsWithObstaclesNear);
                        break;
                    case 2:
                        FindObstacleAndDistanceToObstacle(mouse, Vector3.right, directionsWithNoObstacles, directionsWithObstaclesFar, directionsWithObstaclesMidway, directionsWithObstaclesNear);
                        break;
                    case 3:
                        FindObstacleAndDistanceToObstacle(mouse, new Vector3(1, 0, -1), directionsWithNoObstacles, directionsWithObstaclesFar, directionsWithObstaclesMidway, directionsWithObstaclesNear);
                        break;
                    case 4:
                        FindObstacleAndDistanceToObstacle(mouse, Vector3.back, directionsWithNoObstacles, directionsWithObstaclesFar, directionsWithObstaclesMidway, directionsWithObstaclesNear);
                        break;
                    case 5:
                        FindObstacleAndDistanceToObstacle(mouse, new Vector3(-1, 0, -1), directionsWithNoObstacles, directionsWithObstaclesFar, directionsWithObstaclesMidway, directionsWithObstaclesNear);
                        break;
                    case 6:
                        FindObstacleAndDistanceToObstacle(mouse, Vector3.left, directionsWithNoObstacles, directionsWithObstaclesFar, directionsWithObstaclesMidway, directionsWithObstaclesNear);
                        break;
                    case 7:
                        FindObstacleAndDistanceToObstacle(mouse, new Vector3(-1, 0, 1), directionsWithNoObstacles, directionsWithObstaclesFar, directionsWithObstaclesMidway, directionsWithObstaclesNear);
                        break;
                }
            }
            directionToGo = ChooseDirection(directionsWithNoObstacles, directionsWithObstaclesFar, directionsWithObstaclesMidway, directionsWithObstaclesNear);
            directionToGo.Normalize();
            //Debug.Log("Direction to go is " + directionToGo);
            DetermineTarget(directionToGo, mouse.position, mouseTarget);
            oldDirectionToGo = directionToGo;
            //Debug.Log("Old direction to go is " + oldDirectionToGo);
        }
    }

    private void FindObstacleAndDistanceToObstacle(Transform mouse, Vector3 directionVector, List<Vector3> noObstacles, List<Vector3> obstaclesFar, List<Vector3> obstaclesMidway, List<Vector3> obstaclesNear)
    {
        GameObject obstacle = mouseAbsNVals.ScanInDirectionForObjects(mouse, mouseAbsNVals.layerEnvironment, directionVector, mouseAbsNVals.visionDistance);
        //Debug.Log(directionVector + " " + obstacle);
        if (obstacle == null)
        {
            noObstacles.Add(directionVector);
            //Debug.Log("Adding " + directionVector + " to noObstacles.");
        }
        else
        {
            float distanceToObstacle = Vector3.Distance(mouse.position, obstacle.transform.position);
            //Debug.Log(distanceToObstacle);
            if (distanceToObstacle <= thisVisionDistance && distanceToObstacle > (thisVisionDistance * 2 / 3))
            {
                obstaclesFar.Add(directionVector);
                //Debug.Log("Adding " + directionVector + " to obstaclesFar");
            }
            else if (distanceToObstacle <= (thisVisionDistance * 2 / 3) && distanceToObstacle > (thisVisionDistance * 1 / 3))
            {
                obstaclesMidway.Add(directionVector);
                //Debug.Log("Adding " + directionVector + " to obstaclesMidway");
            }
            else if (distanceToObstacle <= (thisVisionDistance * 1 / 3) && distanceToObstacle > 0)
            {
                obstaclesNear.Add(directionVector);
                //Debug.Log("Adding " + directionVector + " to obstaclesNear");
            }
        }
    }
    private Vector3 ChooseDirection(List<Vector3> noObstacles, List<Vector3> obstaclesFar, List<Vector3> obstaclesMidway, List<Vector3> obstaclesNear)
    {
        Vector3 chosenDirection = Vector3.zero;
        if (noObstacles.Count > 0 && noObstacles.Contains(oldDirectionToGo) == false)
        {
            chosenDirection = ChooseRandomVector(noObstacles);
            directionChangeColldown = cooldownVeryLong;
        }
        else if (obstaclesFar.Count > 0 && obstaclesFar.Contains(oldDirectionToGo) == false)
        {
            chosenDirection = ChooseRandomVector(obstaclesFar);
            directionChangeColldown = cooldownLong;
        }
        else if (obstaclesMidway.Count > 0 && obstaclesMidway.Contains(oldDirectionToGo) == false)
        {
            chosenDirection = ChooseRandomVector(obstaclesMidway);
            directionChangeColldown = cooldownMedium;
        }
        else if (obstaclesNear.Count > 0 && obstaclesNear.Contains(oldDirectionToGo) == false)
        {
            chosenDirection = ChooseRandomVector(obstaclesNear);
            directionChangeColldown = cooldownShort;
        }
        return chosenDirection;
    }

    private Vector3 ChooseRandomVector(List<Vector3> chosenList)
    {
        int randomIndex = Random.Range(0, chosenList.Count);
        return chosenList[randomIndex];
    }

    private void MemorizeHoles(List<Collider> holesVisible)
    {
        if (memorizedHoles.Count == 0)
        {
            foreach (Collider holeVisible in holesVisible)
            {
                memorizedHoles.Add(holeVisible);
            }
        }
        else
        {
            foreach (Collider holeVisible in holesVisible)
            {
                if (memorizedHoles.Contains(holeVisible) == false)
                {
                    memorizedHoles.Add(holeVisible);
                }
            }
        }
        //foreach (Collider holeMemorized in memorizedHoles)
        //{
        //    Debug.Log(holeMemorized.name);
        //}
    }

    private void OperationCatsVSHoles(List<Collider> knownHoles, MouseAbilitiesNValues stats)
    {
        List<Collider> optimalHoles = new List<Collider>();
        FindOptimalHoles(optimalHoles, stats.catsFound, knownHoles);
        DetermineRunOrHide(optimalHoles, stats);
    }

    private void AddMiceIntoStrategy(List<Collider> mice, MouseAbilitiesNValues stats)
    {
        List<Collider> everybody = new List<Collider>();
        everybody.AddRange(stats.catsFound);
        everybody.AddRange(mice);
        OperationRunAway(everybody, thisMouse, thisMouseTarget);
    }

    private void AddSlowIntoStrategy()
    {
        if (mouseAbsNVals.isSlow == true && memorizedHoles != null && memorizedHoles.Count > 0)
        {
            FindAndMoveToClosestHole(memorizedHoles);
        }
        else
        {
            OperationWander(thisMouse, thisMouseTarget);
        }
    }
}
