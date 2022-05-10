using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

namespace CatAI
{
    public class SearchFor : IState
    {
        private LayerMask searchLayer;
        
        GameObject ownerGameObject;

        private float searchRadius;

        private string tagToLookFor;

        public bool SearchCompleted;

        private Action<SearchResults> searchResultsCallback;

        public SearchFor(LayerMask searchLayer, GameObject ownerGameObject, float searchRadius, string tagToLookFor, Action<SearchResults> searchResultsCallback)
        {
            this.searchLayer = searchLayer;
            this.ownerGameObject = ownerGameObject;
            this.searchRadius = searchRadius;
            this.tagToLookFor = tagToLookFor;
            this.searchResultsCallback = searchResultsCallback;
         
        }

        public void Enter()
        {

        }

        public void Execute()
        {
            if (!SearchCompleted)
            {
                var hitObjects = Physics.OverlapSphere(this.ownerGameObject.transform.position, this.searchRadius);
                List<Collider> allObjectsWithTheRequiredTag = new List<Collider>();
                for (int i = 0; i < hitObjects.Length; i++)
                {
                    if (hitObjects[i].gameObject.CompareTag(tagToLookFor))
                    {
                        //this.navMeshAgent.SetDestination(hitObjects[i].transform.position);
                        allObjectsWithTheRequiredTag.Add(hitObjects[i]);
                    }
                }
                var searchResults = new SearchResults(hitObjects, allObjectsWithTheRequiredTag);
                //sendback search results
                searchResultsCallback(searchResults);

                SearchCompleted = true;
            } 
        }

        public void Exit()
        {

        }
    }

    //package the result from search and send back to owner game object
    public class SearchResults
    {
        public Collider[] AllHitObjectsInSearchRadius;

        public List<Collider> AllHitObjectsWithRequiredTag;
        //process objects by distance neeeded

        public SearchResults(Collider[] allHitObjectsInSearchRadius, List<Collider> allHitObjectsWithRequiredTag)
        {
            AllHitObjectsInSearchRadius = allHitObjectsInSearchRadius;
            AllHitObjectsWithRequiredTag = allHitObjectsWithRequiredTag;
        }

    }
}

