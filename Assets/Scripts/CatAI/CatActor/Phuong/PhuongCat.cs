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
            Move.DestinationReached += Search;
            Idle.IdleOver += Search;
            //stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.mouseTag, FoundMice));
            //stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.trapTag, SetTrap));

        }

        void Update()
        {
            stateMachine.ExecuteStateUpdate();
        }

        public void Search()
        {
            stateMachine.ChangeState(new SearchAll(this.gameObject, this.viewRange, FoundMice));
        }
        //public void SearchForMouse()
        //{
        //    stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.mouseTag, FoundMice));
        //}
        public void FoundMice(AllSearchResults searchResults)
        {
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
                        result = FuzzyResult.Undesirable
                    },
                    value2 = new FuzzyValue()
                    {
                        value = distanceToMouse,
                        result = FuzzyResult.VeryDesirable
                    },
                },
                //new FuzzyRule()
                //{
                //    comparison = Compare.Less,
                //    value1= new FuzzyValue()
                //    {
                //        value = distanceToHole - distanceToMouse,
                //        result = FuzzyResult.VeryUndesirable
                //    },
                //    value2 = new FuzzyValue()
                //    {
                //        value = distanceToMouse,
                //        result= FuzzyResult.Desirable
                //    }
                //},
                //new FuzzyRule()
                //{
                //    comparison = Compare.Less,
                //    value1=new FuzzyValue()
                //    {
                //        value = foundMice.Count,
                //        result = FuzzyResult.Desirable
                //    },
                //    value2 =new FuzzyValue()
                //    {
                //        value= foundHole.Count,
                //        result = FuzzyResult.Undesirable
                //    }
                //}
                //keep adding rules
            };

            FuzzyResult result = FuzzyClasses.CompareRules(rules);
            switch (result)
            {
                case FuzzyResult.VeryUndesirable:
                    Debug.Log("VeryUndesirable");
                    stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.trapTag, SetTrap));
                    break;
                case FuzzyResult.Undesirable:
                    Debug.Log("Undesirable");
                    stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.trapTag, SetTrap));
                    break;
                case FuzzyResult.Neutral:
                    Debug.Log("Neutral");
                    stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
                    break;
                case FuzzyResult.Desirable:
                    Debug.Log("Desirable");
                    //ambush + chase?
                    stateMachine.ChangeState(new Move(this.navMeshAgent, foundMice[0].transform.position));
                    break;
                case FuzzyResult.VeryDesirable:
                    Debug.Log("VeryUndesirable");
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
                other.gameObject.SetActive(false);
            }
        }
    }
}
