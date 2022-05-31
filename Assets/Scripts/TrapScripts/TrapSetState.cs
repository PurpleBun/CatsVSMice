
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSetState : TrapBaseState
{
    public override void EnterState(TrapStateManager trap) {
        Debug.Log("TrapSetState");
    }

    public override void UpdateState(TrapStateManager trap){

    }

    public override void OnTriggerEnter(TrapStateManager trap, Collider collision){

        // if mice is colliding with the trap, the state of the trap will switch to Active

        if (collision.gameObject.tag == "Mice")
        {   
            trap.SwitchState(trap.TrapActivatedState);
            Debug.Log("StateSwitchedTo TrapActivatedState");
        }
    }
}