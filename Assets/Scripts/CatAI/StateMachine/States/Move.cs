using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace CatAI
{

    public class Move : IState
    {
        NavMeshAgent navMeshAgent;
        Vector3 destination;

        //need animator

        //Move towards the destination collider sent by main cat AI script (trap/mouse/etc)
        public Move(NavMeshAgent navMeshAgent, Vector3 destination)
        {
            this.navMeshAgent = navMeshAgent;
            this.destination = destination;
        }

        public override void Execute()
        {
            navMeshAgent.SetDestination(destination);
            //if (!navMeshAgent.pathPending)
            //{
            //    if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            //    {
            //        if (navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
            //        {
            //            destinationReached();
            //        }
            //    }
            //}
        }
    }
}
