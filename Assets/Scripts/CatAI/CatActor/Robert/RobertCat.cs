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
        [SerializeField]
        string catTag;

        public bool trapIntent = false;
        public UnityEngine.AI.NavMeshAgent navMeshAgent;

        public ShakeWithAnimation shakeWithAnimation;

        // Start is called before the first frame update
        void Start()
        {
            navMeshAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
            navMeshAgent.speed = baseSpeed;
            navMeshAgent.angularSpeed = angularSpeed;
            navMeshAgent.acceleration = acceleration;

            // First thing when game starts is to search for all mouse, holes, traps and cats. When stuck go wander. When destination reached search again.
            SearchAll();
            Move.Stuck += Wander;
            Move.DestinationReached += SearchAll;
        }

        // Update is called once per frame
        void Update()
        {
            stateMachine.ExecuteStateUpdate();
        }

        public void Wander()
        {
            stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
        }
        
        public void SearchAll()
        {
            stateMachine.ChangeState(new SearchAll(this.gameObject, this.viewRange, FindMouseAndTrap));
        }

        public void FindMouseAndTrap(AllSearchResults searchResultsMouseAndTrap)
        {
            var findMouse = searchResultsMouseAndTrap.AllMice;
            //var findHole = searchResults.AllHole;
            var findTrap = searchResultsMouseAndTrap.AllTrap;
            //var findCat = searchResults.AllCat;

            if(findMouse.Count == 0) 
            {
                if(findTrap.Count == 0) 
                {
                    trapIntent = false;
                    Wander();
                    return;
                }
                else if (findTrap.Count != 0)
                {
                    CheckTrap();
                }

                Wander();
                return;
            }
            else if (findMouse.Count != 0)
            {
                SearchCatAndHole();
            }
        }

        public void CheckTrap()
        {
            if(TrapStateManager.TrapActivated == true)
            {
                trapIntent = false;
                stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
                return;
            }
            else if (TrapStateManager.TrapActivated == false)
            {
                stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.trapTag, SetTrap));
            }
        }

        public void SetTrap(SearchResults searchResultsMouseAndTrap)
        {
            var foundtrap = searchResultsMouseAndTrap.AllHitObjectsWithRequiredTag;
            if (Vector3.Distance(transform.position, foundtrap[0].transform.position)> 1f)
            {
                stateMachine.ChangeState(new Move(navMeshAgent, foundtrap[0].transform.position));
            }
            else
            {
                trapIntent = true;
                stateMachine.ChangeState(new SetTrap(this.navMeshAgent, this.gameObject, this.trapDuration, this.stateMachine, foundtrap[0].transform.position));
            }
        }

        public void SearchCatAndHole()
        {
            stateMachine.ChangeState(new SearchAll(this.gameObject, this.viewRange, FindCatAndMouse));
        }

        public void FindCatAndMouse(AllSearchResults searchResultsCatAndHole)
        {
            var findCat = searchResultsCatAndHole.AllCat;
            var findHole = searchResultsCatAndHole.AllHole;
            

            if(findCat.Count == 0) 
            {
                if(findHole.Count == 0)
                {
                    // no cat or hole found, chase the found mouse.
                    stateMachine.ChangeState(new SearchAll(this.gameObject, this.viewRange, Chase));
                }
                else if(findHole.Count != 0)
                {
                    //Move between hidey hole and mouse. Then chase

                }
            }
            else if(findCat.Count != 0)
            {
                if(findHole.Count == 0)
                {
                    //move behind mouse, pin mouse in between cats. Then chase.
                }
                else if(findHole.Count != 0)
                {
                    //Move infront of hidey hole. Then chase.
                    stateMachine.ChangeState(new Move(navMeshAgent, findHole[0].transform.position));
                    if (Vector3.Distance(transform.position, findHole[0].transform.position)> 2f)
                    {
                        stateMachine.ChangeState(new SearchAll(this.gameObject, this.viewRange, Chase));
                    }
                }
            }
        }

        public void Chase(AllSearchResults searchResultsCatAndHole)
        {
            var findMouse = searchResultsCatAndHole.AllMice;
            stateMachine.ChangeState(new Move(this.navMeshAgent, findMouse[0].transform.position));
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