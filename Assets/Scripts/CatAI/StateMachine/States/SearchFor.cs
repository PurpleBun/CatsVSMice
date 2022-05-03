using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatAI
{
    public class SearchFor : IState
    {
        private LayerMask searchLayer;
        
        GameObject ownerGameObject;

        private float searchRadius;

        private string tagToLookFor;

        UnityEngine.AI.NavMeshAgent navMeshAgent;

        public SearchFor(LayerMask searchLayer, GameObject ownerGameObject, float searchRadius, string tagToLookFor, UnityEngine.AI.NavMeshAgent navMeshAgent)
        {
            this.searchLayer = searchLayer;
            this.ownerGameObject = ownerGameObject;
            this.searchRadius = searchRadius;
            this.tagToLookFor = tagToLookFor;
            this.navMeshAgent = navMeshAgent;    
        }

        public void Enter()
        {

        }

        public void Execute()
        {
            var hitObjects = Physics.OverlapSphere(this.ownerGameObject.transform.position, this.searchRadius);
            for(int i = 0; i < hitObjects.Length; i++)
            {
                if (hitObjects[i].CompareTag(tagToLookFor))
                {
                    this.navMeshAgent.SetDestination(hitObjects[i].transform.position);
                }
                break;
            }
        }

        public void Exit()
        {

        }
    }
}
