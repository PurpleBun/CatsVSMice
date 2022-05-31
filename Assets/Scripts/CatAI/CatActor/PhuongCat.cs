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
            stateMachine.ChangeState(new Idle(navMeshAgent,this.gameObject, stateMachine));
            Move.DestinationReached += Search;
            //stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.mouseTag, FoundMice));
            //stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.trapTag, SetTrap));

        }

        void Update()
        {
            stateMachine.ExecuteStateUpdate();
        }

        public void Search()
        {
            //stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.mouseTag, FoundMice));
            stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.trapTag, SetTrap));
        }
        public void FoundMice(SearchResults searchResults)
        {
            var foundmice = searchResults.AllHitObjectsWithRequiredTag;
            if (foundmice.Count == 0)
            {
                //switchstate
                stateMachine.ChangeState(new Idle(navMeshAgent, this.gameObject, stateMachine));
                return;
            }
            else
            {
                stateMachine.ChangeState(new Move(this.navMeshAgent, foundmice[0].transform.position));
            }
            //FuzzyRule[] rules = new FuzzyRule[]
            //{
            //    new FuzzyRule()
            //    {
            //        comparison = Compare.Greater,
            //        value1= new FuzzyValue()
            //        {
            //            value = Vector3.Distance(transform.position,foundtrap[0]),
            //            result = FuzzyResult.VeryUndesirable
            //        },
            //        value2 = new FuzzyValue()
            //        {
            //            value = Vector3.Distance(transform.position, foundmice[0]),
            //            result = FuzzyResult.VeryDesirable
            //        },
            //    }
            //    //keep adding rules
            //};

            //FuzzyResult result = Fuzzy.CompareRules(rules);
            //switch (result)
            //{
            //    case FuzzyResult.VeryUndesirable:
            //        ChaseMouse();
            //        break;
            //    case FuzzyResult.Undesirable:
            //        break;
            //    case FuzzyResult.Neutral:
            //        break;
            //    case FuzzyResult.Desirable:
            //        break;
            //    case FuzzyResult.VeryDesirable:
            //        SetTrap();
            //        break;
            //}
        }

        public void SetTrap(SearchResults searchResults)
        {
            var foundtrap = searchResults.AllHitObjectsWithRequiredTag;
            trapIntent = true;
            if (foundtrap.Count == 0)
            {
                //switchstate
                trapIntent = false;
                stateMachine.ChangeState(new Idle(navMeshAgent, this.gameObject, stateMachine));
                return;
            }
            else
            {
                stateMachine.ChangeState(new SetTrap(this.navMeshAgent, this.gameObject, this.trapDuration, this.stateMachine, foundtrap[0].transform.position));
            }
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
