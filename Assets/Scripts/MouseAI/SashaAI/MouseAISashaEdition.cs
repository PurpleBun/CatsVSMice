using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAISashaEdition : MonoBehaviour
{
    private MouseAbilitiesNValues mouseAbsNVals;
    private Transform thisMouseTarget;
    private Transform thisMouse;
    [SerializeField]
    private float safeDistanceToHole;
    private float currentDistanceToHole;
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
        PrioritizeWhereToGo(mouseAbsNVals);
    }

    private void PrioritizeWhereToGo(MouseAbilitiesNValues mouseStats)
    {
        if ((mouseStats.catsFound == null || mouseStats.catsFound.Count == 0) && (mouseStats.holesFound != null && mouseStats.holesFound.Count != 0))
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
        else if ((mouseStats.catsFound != null && mouseStats.catsFound.Count != 0) && (mouseStats.holesFound == null || mouseStats.holesFound.Count == 0))
        {
            Vector3 runAwayVector;
            if (mouseStats.catsFound.Count == 1)
            {
                Transform catTransform = mouseStats.catsFound[0].transform;
                runAwayVector = DetermineRunAwayVector(thisMouse, catTransform);
                DetermineTarget(runAwayVector, thisMouse.position, thisMouseTarget);
            }
            else
            {
                Vector3 totalRunAwayVector = DetermineTotalRunAwayVector(thisMouse, mouseStats.catsFound);
                DetermineTarget(totalRunAwayVector, thisMouse.position, thisMouseTarget);
            }
        }
        //else if ()
    }

    private float CalculateDistanceToObject(Transform thisMouseTransform, Transform objectInQuestionTransform)
    {
        float distance;
        distance = Vector3.Distance(thisMouseTransform.position, objectInQuestionTransform.position);
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
}
