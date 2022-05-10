using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CatAI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Cat : MonoBehaviour
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
    }
}
