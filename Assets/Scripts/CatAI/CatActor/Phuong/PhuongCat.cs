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

        public ShakeWithAnimation shakeWithAnimation;
        
        void Start()
        {
            navMeshAgent = this.GetComponent<NavMeshAgent>();
            //setting up navmesh agent with inherited stats
            navMeshAgent.speed = baseSpeed;
            navMeshAgent.angularSpeed = angularSpeed;
            navMeshAgent.acceleration = acceleration;
            //look around when the game first started
            Search();
            //event subscription
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
            trapIntent = false;
            stateMachine.ChangeState(new SearchAll(this.gameObject, this.viewRange, FoundMice));
        }

        public void FoundMice(AllSearchResults searchResults)
        {
            //deal with the search results
            var foundMice = searchResults.AllMice;
            var foundTrap = searchResults.AllTrap;
            var foundHole = searchResults.AllHole;
            float distanceToMouse;
            float distanceToHole;
            if (foundMice.Count == 0 && foundTrap.Count == 0)
            {
                //switchstate
                stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
                return;
            };
            //in case theres no mouse/hole
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
            //fuzzyrules
            FuzzyRule[] rules = new FuzzyRule[]
            {
                new FuzzyRule()
                {
                    //comparing mouse distance to threshold->benefitable or not if pursue the chase
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
                    //comparing mouth distance to the closest hole -> chance of mouse escaping
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
                    //comparing number of mice to number of holes
                    comparison = Compare.Greater,
                    value1=new FuzzyValue()
                    {
                        value = foundMice.Count,
                        result = FuzzyResult.VeryDesirable
                    },
                    value2 =new FuzzyValue()
                    {
                        value= foundHole.Count,
                        result = FuzzyResult.Neutral
                    }
                }
                //keep adding rules
            };

            FuzzyResult result = FuzzyClasses.CompareRules(rules);
            //actions taken based on the result of fuzzy classes
            switch (result)
            {
                case FuzzyResult.VeryUndesirable:
                    stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.trapTag, SetTrap));
                    //stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
                    break;
                case FuzzyResult.Undesirable:
                    stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
                    break;
                case FuzzyResult.Neutral:
                    stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
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
            if (foundtrap.Count == 0)
            {
                trapIntent = false;
                stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
                return;
            }
            else
            {
                //check if trap is activated or not
                if (TrapStateManager.TrapActivated == true)
                {
                    trapIntent = false;
                    stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
                    return;
                }
                else
                {
                    //move if trap not close by
                    if (Vector3.Distance(transform.position, foundtrap[0].transform.position)> 1.5f)
                    {
                        stateMachine.ChangeState(new Move(navMeshAgent, foundtrap[0].transform.position));
                    }
                    else
                    {
                        //set trap state
                        trapIntent = true;
                        stateMachine.ChangeState(new SetTrap(this.navMeshAgent, this.gameObject, this.trapDuration, this.stateMachine, foundtrap[0].transform.position));
                    }
                }
            }
        }
        
        
        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Mouse") {
                Shake();
                //remove mouse from game manager list to check for winning condition
                manager.mouseList.Remove(other.gameObject);
                other.gameObject.SetActive(false);
            }
        }

        void Shake()
        {
            shakeWithAnimation.camAnim.SetTrigger("Shake");
        }
    }
}
