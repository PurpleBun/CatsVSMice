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

        //Random location on map
        public void Wander()
        {
            stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
        }
        
        public void SearchAll()
        {
            stateMachine.ChangeState(new SearchAll(this.gameObject, this.viewRange, FindMouseAndTrap));
        }

        //Search for mouse and trap.
        public void FindMouseAndTrap(AllSearchResults searchResultsMouseAndTrap)
        {
            var findMouse = searchResultsMouseAndTrap.AllMice;
            var findTrap = searchResultsMouseAndTrap.AllTrap;

            //If no mouse found and no trap found, go wander
            if(findMouse.Count == 0) 
            {
                if(findTrap.Count == 0) 
                {
                    trapIntent = false;
                    Wander();
                    return;
                }
                //If no mouse found but there is a trap, go check if the trap has been set already
                else if (findTrap.Count != 0)
                {
                    CheckTrap();
                }

                Wander();
                return;
            }
            //If there is a mouse, go check if there's a cat and hole nearby
            else if (findMouse.Count != 0)
            {
                SearchCatAndHole();
            }
        }

        //Check if hte trap has been set, if set already, go wander
        public void CheckTrap()
        {
            if(TrapStateManager.TrapActivated == true)
            {
                trapIntent = false;
                stateMachine.ChangeState(new Wander(navMeshAgent, this.gameObject, stateMachine));
                return;
            }
            //If trap not yet set, go set trap
            else if (TrapStateManager.TrapActivated == false)
            {
                stateMachine.ChangeState(new SearchFor(this.gameObject, this.viewRange, this.trapTag, SetTrap));
            }
        }

        //Setting the trap, move closer if out of reach, set trap
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

        // Search for a cat and hole
        public void SearchCatAndHole()
        {
            stateMachine.ChangeState(new SearchAll(this.gameObject, this.viewRange, FindCatAndHole));
        }

        public void FindCatAndHole(AllSearchResults searchResultsCatAndHole)
        {
            var findCat = searchResultsCatAndHole.AllCat;
            var findHole = searchResultsCatAndHole.AllHole;
            
            //If ther is no cat and no hole, go chase the mouse
            if(findCat.Count == 0) 
            {
                if(findHole.Count == 0)
                {
                    // no cat or hole found, chase the found mouse.
                    stateMachine.ChangeState(new SearchAll(this.gameObject, this.viewRange, Chase));
                }
                //If there is no cat but there is a hole
                else if(findHole.Count != 0)
                {
                    //Move between hidey hole and mouse. Then chase
                    stateMachine.ChangeState(new SearchAll(this.gameObject, this.viewRange, Chase));

                }
            }
            //If there is a cat but no hidey hole
            else if(findCat.Count != 0)
            {
                if(findHole.Count == 0)
                {
                    //move behind mouse, pin mouse in between cats. Then chase.
                    stateMachine.ChangeState(new SearchAll(this.gameObject, this.viewRange, Chase));
                }
                //If there is a cat and a hidey hole, move infront of hidey hole, then go chase
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

        //Chase the mouse
        public void Chase(AllSearchResults searchResultsCatAndHole)
        {
            var findMouse = searchResultsCatAndHole.AllMice;
            stateMachine.ChangeState(new Move(this.navMeshAgent, findMouse[0].transform.position));
        }

        //On collision with mouse, remoe mouse from list, deactivate it and shake screen
        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Mouse") {
                Shake();
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