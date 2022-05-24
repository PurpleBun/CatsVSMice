using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CatAI
{

    public class Move : IState
    {
        NavMeshAgent navMeshAgent;
        Collider destination;
        //need animator

        //Move towards the destination collider sent by main cat AI script (trap/mouse/etc)
        public Move(NavMeshAgent navMeshAgent, Collider destination)
        {
            this.navMeshAgent = navMeshAgent;
            this.destination = destination;
        }

        public void Enter()
        {

        }

        public void Execute()
        {
            navMeshAgent.SetDestination(destination.transform.position);
        }

        public void Exit()
        {

        }
    }
}
