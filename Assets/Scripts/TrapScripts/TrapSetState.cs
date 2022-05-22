
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSetState : TrapBaseState
{
    public override void EnterState(TrapStateManager trap) {
        
    }

    public override void UpdateState(TrapStateManager trap){

    }

    public override void OnTriggerEnter(TrapStateManager trap, Collider collision){

        if (collision.gameObject.tag == "Mice")
        {   

            // Here goes code for slowing down the mouse for x amount of seconds
            trap.SwitchState(trap.TrapActivatedState);
            Debug.Log("StateSwitchedTo TrapActivatedState");
        }
    }
}