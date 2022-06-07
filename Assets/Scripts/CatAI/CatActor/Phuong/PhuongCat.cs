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
        [SerializeField]
        float chaseThreshold;

        public bool trapIntent = false;
        public NavMeshAgent navMeshAgent;
        
        void Start()
        {
            navMeshAgent = this.GetComponent<NavMeshAgent>();
            //setting up navmesh agent with inherited stats
            navMeshAgent.speed = baseSpeed;
            navMeshAgent.angularSpeed = angularSpeed;
            navMeshAgent.acceleration = acceleration;
            Search();
            Move.Stuck += Wander;
            Move.DestinationReached += Search;
            Idle.IdleOver += Search;
           
        }

        void Update()
        {
            stateMachine.ExecuteStateUpdate();
        }

        public void Wander()
        {
            stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
        }
        public void Search()
        {
            stateMachine.ChangeState(new SearchAll(this.gameObject, this.viewRange, FoundMice));
        }

        public void FoundMice(AllSearchResults searchResults)
        {
            var foundMice = searchResults.AllMice;
            var foundTrap = searchResults.AllTrap;
            var foundHole = searchResults.AllHole;
            float distanceToMouse;
            float distanceToHole;
            if (foundMice.Count == 0 && foundHole.Count == 0)
            {
                //switchstate
                stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
                return;
            };
            if (foundMice.Count == 0) {
                distanceToMouse = 99f;
            }
            else
            {
                distanceToMouse = Vector3.Distance(transform.position, foundMice[0].transform.position);
            }

            if (foundHole.Count == 0)
            {
                distanceToHole = 199f;
            }
            else
            {
                distanceToHole = Vector3.Distance(transform.position, foundHole[0].transform.position);
            }
            FuzzyRule[] rules = new FuzzyRule[]
            {
                new FuzzyRule()
                {
                    comparison = Compare.Less,
                    value1= new FuzzyValue()
                    {
                        value = chaseThreshold,
                        result = FuzzyResult.Neutral
                    },
                    value2 = new FuzzyValue()
                    {
                        value = distanceToMouse,
                        result = FuzzyResult.VeryDesirable
                    },
                },
                new FuzzyRule()
                {
                    comparison = Compare.Less,
                    value1= new FuzzyValue()
                    {
                        value = distanceToHole - distanceToMouse,
                        result = FuzzyResult.Undesirable
                    },
                    value2 = new FuzzyValue()
                    {
                        value = distanceToMouse,
                        result= FuzzyResult.VeryDesirable
                    }
                },
                new FuzzyRule()
                {
                    comparison = Compare.Greater,
                    value1=new FuzzyValue()
                    {
                        value = foundMice.Count,
                        result = FuzzyResult.VeryDesirable
                    },
                    value2 =new FuzzyValue()
                    {
                        value= foundHole.Count,
                        result = FuzzyResult.Undesirable
                    }
                }
                //keep adding rules
            };

            FuzzyResult result = FuzzyClasses.CompareRules(rules);
            switch (result)
            {
                case FuzzyResult.VeryUndesirable:
                    stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.trapTag, SetTrap));
                    break;
                case FuzzyResult.Undesirable:
                    stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
                    break;
                case FuzzyResult.Neutral:
                    stateMachine.ChangeState(new Move(this.navMeshAgent, foundHole[0].transform.position));
                    break;
                case FuzzyResult.Desirable:
                    //ambush + chase?
                    stateMachine.ChangeState(new Move(this.navMeshAgent, foundMice[0].transform.position));
                    break;
                case FuzzyResult.VeryDesirable:
                    //chase mouse
                    stateMachine.ChangeState(new Move(this.navMeshAgent, foundMice[0].transform.position));
                    break;
            }
        }

        public void SetTrap(SearchResults searchResults)
        {
            var foundtrap = searchResults.AllHitObjectsWithRequiredTag;
            trapIntent = true;
            if (foundtrap.Count == 0)
            {
                //switchstate
                trapIntent = false;
                stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
                return;
            }
            else
            {
                //check if trap is activated or not
                stateMachine.ChangeState(new SetTrap(this.navMeshAgent, this.gameObject, this.trapDuration, this.stateMachine, foundtrap[0].transform.position));
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
