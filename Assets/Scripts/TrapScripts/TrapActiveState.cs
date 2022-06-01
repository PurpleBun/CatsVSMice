using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapActiveState : TrapBaseState

{
    float switchCountdown = 5.0f;

    public override void EnterState(TrapStateManager trap) {
        Debug.Log("TrapActiveState");
    }

    public override void UpdateState(TrapStateManager trap){

   //trap goes idle for x amount of seconds and then switches back to NotSetState

        if (switchCountdown > 0){
            switchCountdown -=Time.deltaTime;
        }else{
            trap.SwitchState(trap.TrapUnsetState);
        }

    }

    public override void OnTriggerEnter(TrapStateManager trap, Collider collision){
     
    }
}

