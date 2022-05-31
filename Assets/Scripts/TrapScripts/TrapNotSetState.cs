using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapNotSetState : TrapBaseState
{

    float switchingCountdown = 5.0f;
    public bool CatInCollision = false;

    public override void EnterState(TrapStateManager trap) {
        Debug.Log("TrapNotSetState");
    }

    public override void UpdateState(TrapStateManager trap){
        Debug.Log("Update State working!");

            //Cat spends y amount of time to set up the trap IF it decides to (individual code)

            if (CatInCollision == true && switchingCountdown > 0){
                switchingCountdown -=Time.deltaTime;

             }else if (switchingCountdown<= 0){

                    trap.SwitchState(trap.TrapSetupState);
                    Debug.Log("StateSwitchedTo TrapSetupState");
                    CatInCollision = false;
                }
            }
        
    

    public override void OnTriggerEnter(TrapStateManager trap, Collider collision)
    {
        // Checking if cat is colliding with the trap
       if (collision.gameObject.tag == "Cat" ) //&& InterestedInTrap == true
        {
             CatInCollision = true;
             Debug.Log("Cat in Collision");
            }  
    }
}