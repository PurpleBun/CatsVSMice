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
        float viewRange;
        [SerializeField]
        string mouseTag;
        [SerializeField]
        string trapTag;
        [SerializeField]
        string holeTag;

        public bool trapIntent = false;
        public NavMeshAgent navMeshAgent;
        
        void Start()
        {
            navMeshAgent = this.GetComponent<NavMeshAgent>();
            //setting up navmesh agent with inherited stats
            navMeshAgent.speed = baseSpeed;
            navMeshAgent.angularSpeed = angularSpeed;
            navMeshAgent.acceleration = acceleration;
            stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.mouseTag, MiceFound));
            //stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.trapTag, SetTrap));
        }

        void Update()
        {
            stateMachine.ExecuteStateUpdate();
        }

        public void MiceFound(SearchResults searchResults)
        {
            var foundmice = searchResults.AllHitObjectsWithRequiredTag;
            if (foundmice.Count == 0)
            {
                //switchstate
                return;
            }
            else
            {
                stateMachine.ChangeState(new Move(this.navMeshAgent, foundmice[0]));
            }
        }

        public void SetTrap(SearchResults searchResults)
        {
            var foundtrap = searchResults.AllHitObjectsWithRequiredTag;
            trapIntent = true;
            while (trapIntent = true)
            {
                if (foundtrap.Count == 0)
                {
                    //switch state
                    trapIntent = false;
                    return;
                }
                else
                {
                    //check if trap is set or not
                    stateMachine.ChangeState(new SetTrap(this.navMeshAgent, this.gameObject, this.trapDuration, this.stateMachine, foundtrap[0]));
                }
            }
        }

        public void ChaseMouse()
        {

        }


        public void Ambush()
        {

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
