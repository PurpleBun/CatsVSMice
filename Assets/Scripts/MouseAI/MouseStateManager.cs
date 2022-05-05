using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseStateManager : MonoBehaviour
{
    MouseBaseState currentState;
    public MouseRunningState runningState = new MouseRunningState();
    public MouseIdleState idleState = new MouseIdleState();
    public MouseHidingState hidingState = new MouseHidingState();
    public MouseAbilityValues thisMouseStats;

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.GetComponent<MouseAbilityValues>() != null)
        {
            thisMouseStats = this.gameObject.GetComponent<MouseAbilityValues>();
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
        
    }

    public void SwitchState(MouseBaseState state)
    {
        if (thisMouseStats != null)
        {
            currentState = state;
            state.EnterState(this, thisMouseStats);
        }
        else
        {
            Debug.Log("Couldn't switch states. thisMouseStats is null.");
        }
    }
}
