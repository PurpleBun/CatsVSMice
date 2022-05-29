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
        Collider trap;
        NavMeshAgent navMeshAgent;
        StateMachine stateMachine;
        float trapDuration;
        float trapDistance=5;

        public SetTrap(NavMeshAgent navMeshAgent, GameObject ownerGameObject, float trapDuration, StateMachine stateMachine, Collider trap )
        {
            this.navMeshAgent = navMeshAgent;
            this.ownerGameObject = ownerGameObject;
            this.trapDuration = trapDuration;
            this.stateMachine = stateMachine;
            this.trap = trap;
        }
        public override void Enter()
        {
            if (Vector3.Distance(ownerGameObject.transform.position, trap.transform.position) > trapDistance)
            {
                stateMachine.ChangeState(new Move(navMeshAgent, trap));
            }
        }

        public override void Execute()
        {
           if (Vector3.Distance(ownerGameObject.transform.position, trap.transform.position) < trapDistance)
            {
                trapDuration -= Time.deltaTime;
                if (trapDuration <= 0)
                {
                    Debug.Log("TrapSet");
                    stateMachine.ChangeState(new Idle());
                }
            }
        }
    }
}
