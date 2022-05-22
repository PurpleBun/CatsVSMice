using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapNotSetState : TrapBaseState
{
    public override void EnterState(TrapStateManager trap) {
        Debug.Log("Checking Cheking!");
    }

    public override void UpdateState(TrapStateManager trap){
        Debug.Log("Update State working!");
    }

    public override void OnTriggerEnter(TrapStateManager trap, Collider collision)
    {
       if (collision.gameObject.tag == "Cat")
        {
            //Here goes code for the cat spending y amount of time to set up the trap IF it decides to (individual code)
            trap.SwitchState(trap.TrapSetupState);
            Debug.Log("StateSwitchedTo TrapSetupState");
        }
    }
}