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

    // In awake, this script tries to access the basic ability script of this mouse.
    // If it finds the MouseAbilitiesNValues component successfully, this script then accesses the mouse's attributes like the mouse's target Transform and this mouse's own Transform and then stores them in the local variables.
    // If the script cannot find the MouseAbilitiesNValues component, it throws a message on the console.
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

    // Making a new list of colliders.
    void Start()
    {
        memorizedHoles = new List<Collider>();
    }

    // Storing miceFound from MouseAbilitiesNValues into a local variable. If a mouse is not in a hiding state - then it should figure out a strategy on where to go.
    void Update()
    {
        miceInSight = mouseAbsNVals.miceFound;
        if (mouseAbsNVals.isHiding == false)
        {
            PrioritizeWhereToGo(mouseAbsNVals);
        }
    }

    // PrioritizeWhereToGo checks which one of the four conditions my mouse is currently facing:
    // 1) is it alone (so no cats around) and near one or more hidey holes? 2) is it surrounded by cats and sees no hidey holes around? 3) is it surrounded by cats AND hidey holes? 4) is it alone in the middle of nowhere (aka no cats or hidey holes near)?
    private void PrioritizeWhereToGo(MouseAbilitiesNValues mouseStats)
    {
        // If my mouse is alone and it can see hidey holes near, it will first memorize the holes it sees and then check if it is on cooldown after using the previous hidey hole.
        if ((mouseStats.catsFound == null || mouseStats.catsFound.Count == 0) && (mouseStats.holesFound != null && mouseStats.holesFound.Count != 0))
        {
            MemorizeHoles(mouseStats.holesFound);
            if (mouseStats.currentHidingCooldown <= 0)
            {
                // If the mouse is not on using hidey hole cooldown, then it will check how many holes are near it. If it can see only one hole - it will calculate the distance to that hole and move close to it.
                if (mouseStats.holesFound.Count == 1)
                {
                    Transform holeTransform = mouseStats.holesFound[0].transform;
                    currentDistanceToHole = CalculateDistanceToObject(thisMouse, holeTransform);
                    MoveToSafeDistanceToHole(safeDistanceToHole, currentDistanceToHole, holeTransform);
                }
                else
                {
                    // If there are multiple holes nearby, then the mouse will figure out which one is the nearest and move close to that hole.
                    FindAndMoveToClosestHole(mouseStats.holesFound);
                }
            }
            else
            {
                // If the mouse is still on hidey hole cooldown, then it will check if it is also being slown down by a trap and then adjust its strategy accoringly.
                AddSlowIntoStrategy();
            }
        }
        // If my mouse is surrounded by cats and no hidey holes, then it will check whether it remembers any holes and whether it's on a hidey hole cooldown.
        else if ((mouseStats.catsFound != null && mouseStats.catsFound.Count != 0) && (mouseStats.holesFound == null || mouseStats.holesFound.Count == 0))
        {
            if (mouseStats.currentHidingCooldown <= 0 && memorizedHoles.Count > 0)
            {
                // If the mouse is not on cooldown and it remembers some hidey holes, it will try to run for the safest hole.
                OperationCatsVSHoles(memorizedHoles, mouseStats);
            }
            else
            {
                // If the mouse is on cooldown or if it doesn't remember any holes, then it will check if it can see any other mice nearby.
                if (miceInSight == null || miceInSight.Count == 0)
                {
                    // If there are no mice nearby, then the mouse will simply run away from cats.
                    OperationRunAway(mouseStats.catsFound, thisMouse, thisMouseTarget);
                }
                else
                {
                    // If there are other mice nearby, then my mouse will also add them into the strategy.
                    AddMiceIntoStrategy(miceInSight, mouseStats);
                }
            }
        }
        // If my mouse is surrounded by both cats and hidey holes, it first memorizes the holes near it and then checks if the it is on hidey hole cooldown.
        else if ((mouseStats.catsFound != null && mouseStats.catsFound.Count != 0) && (mouseStats.holesFound != null && mouseStats.holesFound.Count != 0))
        {
            MemorizeHoles(mouseStats.holesFound);
            if (mouseStats.currentHidingCooldown <= 0)
            {
                // If the mouse is not on cooldown, it will try to find a safe hole to run to. It will then run towards that hole.
                OperationCatsVSHoles(mouseStats.holesFound, mouseStats);
            }
            else
            {
                // If the mouse is on cooldown, then it will check if it can see any other mice nearby.
                if (miceInSight == null || miceInSight.Count == 0)
                {
                    // If there are no mice nearby, then the mouse will simply run away from cats.
                    OperationRunAway(mouseStats.catsFound, thisMouse, thisMouseTarget);
                }
                else
                {
                    // If there are other mice nearby, then my mouse will also add them into the strategy.
                    AddMiceIntoStrategy(miceInSight, mouseStats);
                }
            }
        }
        // If the mouse doesn't see any cats or hidey holes, then it will check if it is also being slown down by a trap and then adjust its strategy accoringly.
        else if ((mouseStats.catsFound == null || mouseStats.catsFound.Count == 0) && (mouseStats.holesFound == null || mouseStats.holesFound.Count == 0))
        {
            AddSlowIntoStrategy();
        }
    }

    // This method calculates distance between two objects.
    private float CalculateDistanceToObject(Transform targetTransformA, Transform targetTransformB)
    {
        float distance;
        distance = Vector3.Distance(targetTransformA.position, targetTransformB.position);
        //Debug.Log("Distance between " + targetTransformA.name + " and " + targetTransformB.name + " is " + distance + ".");
        return distance;
    }

    // This method adjusts the position of the mouse's target depending on whether it is within a safe distance to the hole or not.
    private void MoveToSafeDistanceToHole(float safeDistance, float currentDistance, Transform hole)
    {
        if (safeDistance < currentDistance)
        {
            // If the mouse is outside of the safe range to the hole, its target will be placed on top of the hidey hole.
            thisMouseTarget.position = new Vector3(hole.position.x, thisMouseTarget.position.y, hole.position.z);
        }
        else if (safeDistanceToHole >= currentDistance && (thisMouseTarget.position.x != thisMouse.position.x || thisMouseTarget.position.z != thisMouse.position.z))
        {
            // Once the mouse reaches the safe distance to the hole, its traget will be placed on top of the mouse to freeze the mouse in place.
            thisMouseTarget.position = new Vector3(thisMouse.position.x, thisMouseTarget.position.y, thisMouse.position.z);
        }
        holeNear = hole;
    }

    // This method finds the nearest hole to the mouse by calculating the distances to all of the holes, placing those distances into a list, sorting the list and then picking the lowest value from the sorted list. 
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
        // After finding the nearest hole, this method calls the MoveToSafeDistanceToHole method which handles moving towards the hidey hole.
        MoveToSafeDistanceToHole(safeDistanceToHole, distancesToHoles[0], closestHole);
    }

    // This method determines in which direction my mouse should run when it wants to run away from one or multiple actors.
    private void OperationRunAway(List<Collider> targetsToFleeFrom, Transform mouse, Transform mouseRunToTarget)
    {
        //Debug.Log("Entered operation run away.");
        Vector3 runAwayVector;
        if (targetsToFleeFrom.Count == 1)
        {
            // If the mouse has to run away from only one actor, then it "grabs" that actor's position and calculates the run away vector based on the position of that one actor.
            Transform fleeTargetTransform = targetsToFleeFrom[0].transform;
            runAwayVector = DetermineRunAwayVector(mouse, fleeTargetTransform);
            // Once it knows the run away vector, it updates its target's position to match the run away vector.
            DetermineTarget(runAwayVector, mouse.position, mouseRunToTarget);
        }
        else
        {
            // If the mouse has to run away from multiple actors, then it "grabs" all of their positions and calculates the run away vector based on all of their positions.
            Vector3 totalRunAwayVector = DetermineTotalRunAwayVector(mouse, targetsToFleeFrom);
            // Once it knows the total run away vector, it updates its target's position to match the total run away vector.
            DetermineTarget(totalRunAwayVector, mouse.position, mouseRunToTarget);
        }
    }

    // First, this method calculates the vector between the mouse and the object it tries to flee from. Then it finds the point in space opposite of that object by adding the vector to the mouse's current position.
    private Vector3 CalculateTargetAwayFromOneObject(Vector3 mousePosition, Vector3 objectToFleePosition)
    {
        Vector3 directionToObject = mousePosition - objectToFleePosition;
        Vector3 targetPosition = mousePosition + directionToObject;
        return targetPosition;
    }

    // This method sets the run away vector for the mouse. It first calculates the vector between the mouse and the run target (point in space) and then it normalizes the vector (aka makes the vector one Unity unit long).
    private Vector3 DetermineRunAwayVector(Transform mouse, Transform targetToFleeFrom)
    {
        Vector3 runTarget = CalculateTargetAwayFromOneObject(mouse.position, targetToFleeFrom.position);
        Vector3 runVector = runTarget - thisMouse.position;
        runVector.Normalize();
        return runVector;
    }

    // This method calculates the total run away vector for the mouse. It first calculates the run away vector for each individual object that the mouse has to flee from and then it adds those vectors up to get the total vector.
    // Lastly the method normalizes the total run away vector.
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

    // This method finds the point in space where to put the mouse's target.
    private void DetermineTarget(Vector3 runVector, Vector3 mousePosition, Transform mouseTargetObj)
    {
        float targetX = mousePosition.x + runVector.x;
        float targetZ = mousePosition.z + runVector.z;
        // Since the mice only move on X and Z axes, I keep the Y position of the mouse's target the same.
        thisMouseTarget.position = new Vector3(targetX, thisMouseTarget.position.y, targetZ);
    }

    // This method checks if there are any safe hidey holes that the mouse can use and then based on that it determines where the mouse should run.
    private void DetermineRunOrHide(List<Collider> goodHoles, MouseAbilitiesNValues mouseStats)
    {
        if (goodHoles.Count == 0)
        {
            // If there are no safe hidey holes for the mouse to run to, it checks if there are any mice around instead.
            if (miceInSight == null || miceInSight.Count == 0)
            {
                // If there are no mice in sight, my mouse will run away from the cats.
                OperationRunAway(mouseStats.catsFound, thisMouse, thisMouseTarget);
            }
            else
            {
                // If there are mice in sight, then my mouse will run away from both cats and mice.
                AddMiceIntoStrategy(miceInSight, mouseStats);
            }
        }
        else
        {
            // If there are safe hidey holes for the mouse to use, then it will run towards a random safe hidey hole.
            int randomIndex = Random.Range(0, goodHoles.Count);
            Transform chosenHole = goodHoles[randomIndex].transform;
            thisMouseTarget.position = new Vector3(chosenHole.position.x, thisMouseTarget.position.y, chosenHole.position.z);
            //Debug.Log("Going to hole " + chosenHole.name);
        }
    }

    // This method finds the optimal hidey holes by comparing the distances between the cats and the holes to the distances between the mouse and the holes.
    // If the mouse is closer to the hidey hole than the cats are, that hole is considered optimal.
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

    // This method calculates the absolute difference between the two distances.
    private float GetDistanceDifference(float distanceA, float distanceB)
    {
        float differenceRegular = distanceA - distanceB;
        float differenceAbsolute = Mathf.Abs(differenceRegular);
        //Debug.Log("Distance difference between " + distanceA + " and " + distanceB + " is " + differenceAbsolute + ".");
        return differenceAbsolute;
    }

    // This method determines in which direction the mouse should wander.
    private void OperationWander(Transform mouse, Transform mouseTarget)
    {
        Vector3 directionToGo;
        List<Vector3> directionsWithNoObstacles = new List<Vector3>();
        List<Vector3> directionsWithObstaclesFar = new List<Vector3>();
        List<Vector3> directionsWithObstaclesMidway = new List<Vector3>();
        List<Vector3> directionsWithObstaclesNear = new List<Vector3>();
        // It first checks if the mouse is done wandering in the previous direction(checking the direction change cooldown).
        if (directionChangeColldown > 0)
        {
            // If the mouse is not done wandering in the old direction, then it keeps walking in the same direction as before.
            directionChangeColldown -= Time.deltaTime;
            DetermineTarget(oldDirectionToGo, mouse.position, mouseTarget);
        }
        else
        {
            // If the mouse is done walking in the same direction, then it chooses the new direction where it should go.
            for (int i = 0; i < 8; i++)
            {
                // It chooses where to go by looking into four cardinal directions and into four diagonal directions. If it finds an obstacle in the direction it is looking, then it determines how far the obstacle is.
                // If there are no obstacles in that direction, the mouse marks that direction as a direction with no obstacles. If the obstacle is far, that direction is marked as having an obstacle far away.
                // Same for directions with obsacles at medium and close ranges.
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
            // Once the mouse has looked in all eight directions, it chooses where it wants to go depending o the loactions of the obstacles.
            directionToGo = ChooseDirection(directionsWithNoObstacles, directionsWithObstaclesFar, directionsWithObstaclesMidway, directionsWithObstaclesNear);
            directionToGo.Normalize();
            //Debug.Log("Direction to go is " + directionToGo);
            // Once the new wandering direction has been chosen, the mouse remembers where it walked towards last and sets a new target for walking towards.
            DetermineTarget(directionToGo, mouse.position, mouseTarget);
            oldDirectionToGo = directionToGo;
            //Debug.Log("Old direction to go is " + oldDirectionToGo);
        }
    }

    // This method dtermines whether there are any obstacles in the direction where the mouse is looking - and if there, the how far are they.
    private void FindObstacleAndDistanceToObstacle(Transform mouse, Vector3 directionVector, List<Vector3> noObstacles, List<Vector3> obstaclesFar, List<Vector3> obstaclesMidway, List<Vector3> obstaclesNear)
    {
        GameObject obstacle = mouseAbsNVals.ScanInDirectionForObjects(mouse, mouseAbsNVals.layerEnvironment, directionVector, mouseAbsNVals.visionDistance);
        //Debug.Log(directionVector + " " + obstacle);
        if (obstacle == null)
        {
            // If scanning in the direction yielded no obstacles, then that direction is added to the list of directions without obstacles.
            noObstacles.Add(directionVector);
            //Debug.Log("Adding " + directionVector + " to noObstacles.");
        }
        else
        {
            // If there is an obstacle in the direction where the mouse is looking, the mouse checks how close that obstacle is.
            // There are three categories of closeness to the obstacle: far - between 2/3 of the vision distance to the full length of vision distance; medium range - between 1/3 and 2/3 of vision distance; near - between 1/3 of the vision distance and 0.
            // The mouse will determine which list to put this direction in depending on how close the obstacle is.
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

    // This method determines which direction the mouse should wander towards.
    // It determines the best direction the following way: if there are directions with no obstacles, the mouse will choose a random direction from that list;
    // if there are no directions without obstacles, it tries to pick a random direction with obstacles far away;
    // if there are no directions with obstacles far away, it tries to pick a random direction with obstacles at medium range;
    // if there are no directions with obstacles at medium range, it will pick a random direction with obstacles close by.
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

    // This method literally just picks a random index within the given list.
    private Vector3 ChooseRandomVector(List<Vector3> chosenList)
    {
        int randomIndex = Random.Range(0, chosenList.Count);
        return chosenList[randomIndex];
    }

    // This method memorizes holes that the mouse sees.
    private void MemorizeHoles(List<Collider> holesVisible)
    {
        // If the mouse hasn't memorized any hidey holes yet, it adds a new hole to the list.
        if (memorizedHoles.Count == 0)
        {
            foreach (Collider holeVisible in holesVisible)
            {
                memorizedHoles.Add(holeVisible);
            }
        }
        else
        {
            // If the mouse has memorized some hole already, then it first makes sure that it doesn't try to memorize the same hole twice and then it adds the new hole to the list.
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

    // This method looks for optimal holes and determines the fight-or-flight strategy based on the list of optimal holes.
    private void OperationCatsVSHoles(List<Collider> knownHoles, MouseAbilitiesNValues stats)
    {
        List<Collider> optimalHoles = new List<Collider>();
        FindOptimalHoles(optimalHoles, stats.catsFound, knownHoles);
        DetermineRunOrHide(optimalHoles, stats);
    }

    // This method adds other mice to the list of objects that my mouse has to flee from.
    private void AddMiceIntoStrategy(List<Collider> mice, MouseAbilitiesNValues stats)
    {
        List<Collider> everybody = new List<Collider>();
        everybody.AddRange(stats.catsFound);
        everybody.AddRange(mice);
        OperationRunAway(everybody, thisMouse, thisMouseTarget);
    }

    // This method checks if the mouse is being slown by the trap and based on that determines what it should do.
    private void AddSlowIntoStrategy()
    {
        if (mouseAbsNVals.isSlow == true && memorizedHoles != null && memorizedHoles.Count > 0)
        {
            // If the mouse is slow, but it remembers the positions of some holes, then the mouse plays it safe and runs to the nearest hole.
            FindAndMoveToClosestHole(memorizedHoles);
        }
        else
        {
            // If the mouse is not slow or if doesn't remember any holes, then it wanders in a random optimal direction.
            OperationWander(thisMouse, thisMouseTarget);
        }
    }
}
