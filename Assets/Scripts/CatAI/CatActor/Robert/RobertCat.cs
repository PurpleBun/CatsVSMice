using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatAI
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public class RobertCat : CatActor
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
        //[SerializeField]
        //float chaseThreshold;

        public bool trapIntent = false;
        public UnityEngine.AI.NavMeshAgent navMeshAgent;

        // Start is called before the first frame update
        void Start()
        {
            navMeshAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
            navMeshAgent.speed = baseSpeed;
            navMeshAgent.angularSpeed = angularSpeed;
            navMeshAgent.acceleration = acceleration;
            Search();
            Move.Stuck += Search;
            Move.DestinationReached += Search;
        }

        // Update is called once per frame
        void Update()
        {
            stateMachine.ExecuteStateUpdate();
        }
        
        public void Search()
        {
            stateMachine.ChangeState(new SearchAll(this.gameObject, this.viewRange, FindMouse));
        }

        public void FindMouse(AllSearchResults searchResults)
        {
            var findMouse = searchResults.AllMice;
            if(findMouse.Count ==0) 
            {
                stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
                return;
            }
            
            else if (findMouse.Count != 0)
            {
                stateMachine.ChangeState(new Move(this.navMeshAgent, findMouse[0].transform.position));

            }
        }

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Mouse") {
                //remove mouse from game manager list to check for winning condition
                manager.mouseList.Remove(other.gameObject);
                other.gameObject.SetActive(false);
            }
        }
    }
}