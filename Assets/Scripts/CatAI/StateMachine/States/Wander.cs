using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CatAI
{
    public class Wander : IState
    {
        //idle
        NavMeshAgent navMeshAgent;
        GameObject ownerGameObject;
        StateMachine stateMachine;

        public Wander(NavMeshAgent navMeshAgent, GameObject ownerGameObject, StateMachine stateMachine)
        {
            this.navMeshAgent = navMeshAgent;
            this.ownerGameObject = ownerGameObject;
            this.stateMachine = stateMachine;
        }
        public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * dist;

            randomDirection += origin;

            NavMeshHit navHit;

            NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask);

            return navHit.position;
        }

        public override void Execute()
        {
            Vector3 newpos = RandomNavSphere(ownerGameObject.transform.position, 5, -1);
            stateMachine.ChangeState(new Move(navMeshAgent, newpos));
        }
    }
}
