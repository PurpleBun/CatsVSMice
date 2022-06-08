
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSetState : TrapBaseState
{

    public MouseAbilitiesNValues moueAbilitiesNValues;

    public override void EnterState(TrapStateManager trap) {
        Debug.Log("TrapSetState");
    }

    public override void UpdateState(TrapStateManager trap){

    }

    public override void OnTriggerEnter(TrapStateManager trap, Collider collision){

        // if mice is colliding with the trap, the state of the trap will switch to Active and mice will be slowed down

        if (collision.gameObject.tag == "Mouse")
        {   
            GameObject collidingObject = collision.gameObject;

            trap.SwitchState(trap.TrapActivatedState);
            Debug.Log("StateSwitchedTo TrapActivatedState");

            if (collidingObject.GetComponent<MouseAbilitiesNValues>()!=null)
            {
                MouseAbilitiesNValues mouseAbsNVals = collidingObject.GetComponent<MouseAbilitiesNValues>();
                mouseAbsNVals.SlowDown();
            }   else

                {
                    Debug.Log("Error, colliding object lacks MouseAbilitiesNValues component");
                }
        } 
    }
}
