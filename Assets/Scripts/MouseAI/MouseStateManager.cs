using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MouseStateManager : MonoBehaviour
{
    MouseBaseState currentState;
    public MouseRunningState runningState = new MouseRunningState();
    public MouseIdleState idleState = new MouseIdleState();
    public MouseHidingState hidingState = new MouseHidingState();
    public MouseAbilitiesNValues thisMouseStats;
    public NavMeshAgent mouseNavMeshAgent;
    public Transform targetTransform;

    void Awake()
    {
        if (this.gameObject.GetComponent<NavMeshAgent>() != null)
        {
            mouseNavMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
        }
        else
        {
            Debug.LogError("The mouse " + this.gameObject + " lacks NavMeshAgent component.");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.GetComponent<MouseAbilitiesNValues>() != null)
        {
            thisMouseStats = this.gameObject.GetComponent<MouseAbilitiesNValues>();
            currentState = idleState;
            currentState.EnterState(this, thisMouseStats);
        }
        else
        {
            Debug.Log("This mouse lacks Ability Values script.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentState.ExecuteState(this, thisMouseStats);
    }

    public void SwitchState(MouseBaseState state)
    {
        if (thisMouseStats != null)
        {
            currentState.ExitState(this, thisMouseStats);
            currentState = state;
            currentState.EnterState(this, thisMouseStats);
        }
        else
        {
            Debug.Log("Couldn't switch states. thisMouseStats is null.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (thisMouseStats != null)
        {
            currentState.OnCollisionEnter(collision, this, thisMouseStats);
        }
        else
        {
            Debug.Log("Couldn't execute OnCollisionEnter. thisMouseStats is null.");
        }
    }
}
