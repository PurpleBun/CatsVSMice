using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapActiveState : TrapBaseState
{
    public override void EnterState(TrapStateManager trap) {

    }

    public override void UpdateState(TrapStateManager trap){

    }

    public override void OnTriggerEnter(TrapStateManager trap, Collider collision){
        //here goes code for making the trap idle for x amount of seconds and then switching back to NotSetState
    }
}

