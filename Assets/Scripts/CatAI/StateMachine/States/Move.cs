using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace CatAI
{

    public class Move : IState
    {
        //https://learn.unity.com/tutorial/events-uh#
        NavMeshAgent navMeshAgent;
        Vector3 destination;
        float stuckCountdown = 5;
        float stuckTimer;
        public delegate void Moving();
        public static event Moving DestinationReached;
        public static event Moving Stuck;
        //need animator

        //Move towards the destination collider sent by main cat AI script (trap/mouse/etc)
        public Move(NavMeshAgent navMeshAgent, Vector3 destination)
        {
            this.navMeshAgent = navMeshAgent;
            this.destination = destination;
        }
        public override void Enter()
        {
            stuckTimer = stuckCountdown;
        }
        public override void Execute()
        {
            navMeshAgent.SetDestination(destination);
            stuckTimer -= Time.deltaTime;
            if (!navMeshAgent.pathPending)
            {
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    if (navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        DestinationReached();
                    }
                }
            }
            if (stuckTimer <= 0)
            {
                Debug.Log("stuck");
                Stuck();
            }
        }
    }
}
