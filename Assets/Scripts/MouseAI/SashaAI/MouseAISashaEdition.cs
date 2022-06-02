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
                List<float> distancesToHoles = new List<float>();
                Transform closestHole = null;
                foreach (Collider holeFound in mouseStats.holesFound)
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
        }

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
}
