using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CatAI
{
    [RequireComponent(typeof(NavMeshAgent))]
    //inherit from CatActor
    public class PhuongCat :  CatActor  
    {
        private StateMachine stateMachine = new StateMachine();
        [SerializeField]
        LayerMask mouseLayer;
        [SerializeField]
        float viewRange;
        [SerializeField]
        string mouseTag;
        UnityEngine.AI.NavMeshAgent navMeshAgent;
        
        void Start()
        {
            navMeshAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
            //setting up navmesh agent with inherited stats
            navMeshAgent.speed = baseSpeed;
            navMeshAgent.angularSpeed = angularSpeed;
            navMeshAgent.acceleration = acceleration;
            stateMachine.ChangeState(new SearchFor(this.mouseLayer, this.gameObject, this.viewRange, this.mouseTag, MiceFound));
        }

        void Update()
        {
            stateMachine.ExecuteStateUpdate();
        }

        public void MiceFound(SearchResults searchResults)
        {
            var foundmice = searchResults.AllHitObjectsWithRequiredTag;
            stateMachine.ChangeState(new Move(this.navMeshAgent,foundmice[0]));
        }
        
        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Mouse") {
                //remove mouse from game manager list to check for winning condition
                manager.mouseList.Remove(other.gameObject);
                Destroy(other.gameObject);
            }
        }
    }
}
