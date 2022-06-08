using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace CatAI
{
    public class SetTrap : IState
    {
        GameObject ownerGameObject;
        Vector3 trap;
        NavMeshAgent navMeshAgent;
        StateMachine stateMachine;
        float trapDuration;
        float trapDistance=0.25f;
        
        public SetTrap(NavMeshAgent navMeshAgent, GameObject ownerGameObject, float trapDuration, StateMachine stateMachine, Vector3 trap )
        {
            this.navMeshAgent = navMeshAgent;
            this.ownerGameObject = ownerGameObject;
            this.trapDuration = trapDuration;
            this.stateMachine = stateMachine;
            this.trap = trap;
        }
        public override void Enter()
        {
            //if not at the trap place yet move to the trap with move state
            if (Vector3.Distance(ownerGameObject.transform.position, trap) >=5f)
            {
                stateMachine.ChangeState(new Move(navMeshAgent, trap));
            }
        }

        public override void Execute()
        {
            if (Vector3.Distance(ownerGameObject.transform.position, trap) < trapDistance)
            {
                //    trapDuration -= Time.deltaTime;
                //    if (trapDuration <= 0)
                //    {
                //        stateMachine.ChangeState(new Wander(navMeshAgent, ownerGameObject, stateMachine));
                //    }
                stateMachine.ChangeState(new Idle(trapDuration));
            }
        }
    }
}
