using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace CatAI
{
    public class SearchFor : IState
    {
        GameObject ownerGameObject;

        private float searchRadius;

        private float viewAngle=60f;

        private string tagToLookFor;

        public bool SearchCompleted;

        private Action<SearchResults> searchResultsCallback;

        //public SearchFor(LayerMask searchLayer, GameObject ownerGameObject, float searchRadius, string tagToLookFor, Action<SearchResults> searchResultsCallback)
        //{
        //    this.searchLayer = searchLayer;
        //    this.ownerGameObject = ownerGameObject;
        //    this.searchRadius = searchRadius;
        //    this.tagToLookFor = tagToLookFor;
        //    this.searchResultsCallback = searchResultsCallback;
         
        //}

        public SearchFor(GameObject ownerGameObject, float searchRadius, string tagToLookFor, Action<SearchResults> searchResultsCallback) 
        {
            this.ownerGameObject = ownerGameObject; 
            this.searchRadius = searchRadius;
            this.tagToLookFor = tagToLookFor;   
            this.searchResultsCallback = searchResultsCallback; 
        }


        public override void Execute()
        {
            if (!SearchCompleted)
            {
                var hitObjects = Physics.OverlapSphere(this.ownerGameObject.transform.position, this.searchRadius);
                List<Collider> allVisibleObjectsWithTheRequiredTag = new List<Collider>();
                for (int i = 0; i < hitObjects.Length; i++)
                {
                    if (hitObjects[i].gameObject.CompareTag(tagToLookFor))
                    {
                        Vector3 dirToTarget = (hitObjects[i].transform.position - this.ownerGameObject.transform.position).normalized;
                        if(Vector3.Angle ( this.ownerGameObject.transform.forward, dirToTarget) < viewAngle / 2)
                        {
                            allVisibleObjectsWithTheRequiredTag.Add(hitObjects[i]);
                        }
                    }
                }
                //sort by distance from game object
                allVisibleObjectsWithTheRequiredTag = allVisibleObjectsWithTheRequiredTag.OrderBy((d) => (d.transform.position - this.ownerGameObject.transform.position).sqrMagnitude).ToList();
                var searchResults = new SearchResults(hitObjects, allVisibleObjectsWithTheRequiredTag);
                //sendback search results
                searchResultsCallback(searchResults);

                SearchCompleted = true;
            } 
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

