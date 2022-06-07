using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace CatAI
{
    public class SearchAll : IState
    {
        GameObject ownerGameObject;

        private float searchRadius;

        private float viewAngle=90f;

        private string tagToLookFor;

        public bool SearchCompleted;

        private Action<AllSearchResults> searchResultsCallback;

        //public SearchFor(LayerMask searchLayer, GameObject ownerGameObject, float searchRadius, string tagToLookFor, Action<SearchResults> searchResultsCallback)
        //{
        //    this.searchLayer = searchLayer;
        //    this.ownerGameObject = ownerGameObject;
        //    this.searchRadius = searchRadius;
        //    this.tagToLookFor = tagToLookFor;
        //    this.searchResultsCallback = searchResultsCallback;
         
        //}

        public SearchAll(GameObject ownerGameObject, float searchRadius, Action<AllSearchResults> searchResultsCallback) 
        {
            this.ownerGameObject = ownerGameObject; 
            this.searchRadius = searchRadius;  
            this.searchResultsCallback = searchResultsCallback; 
        }


        public override void Execute()
        {
            if (!SearchCompleted)
            {
                var hitObjects = Physics.OverlapSphere(this.ownerGameObject.transform.position, this.searchRadius);
                List<Collider> allMice = new List<Collider>();
                List<Collider> allTrap = new List<Collider>();
                List<Collider> allHole = new List<Collider>();
                for (int i = 0; i < hitObjects.Length; i++)
                {
                    Vector3 dirToTarget = (hitObjects[i].transform.position - this.ownerGameObject.transform.position).normalized;
                    if(Vector3.Angle ( this.ownerGameObject.transform.forward, dirToTarget) < viewAngle / 2)
                    {
                        if (hitObjects[i].gameObject.CompareTag("Mouse"))
                        {
                            allMice.Add(hitObjects[i]);
                        }
                        if (hitObjects[i].gameObject.CompareTag("trap"))
                        {
                            allTrap.Add(hitObjects[i]);
                        }
                        if (hitObjects[i].gameObject.CompareTag("Hole"))
                        {
                            allHole.Add(hitObjects[i]);
                        }
                    }
                    
                }
                //sort by distance from game object
                allMice = allMice.OrderBy((d) => (d.transform.position - this.ownerGameObject.transform.position).sqrMagnitude).ToList();
                allTrap = allTrap.OrderBy((d) => (d.transform.position - this.ownerGameObject.transform.position).sqrMagnitude).ToList();
                allHole = allHole.OrderBy((d) => (d.transform.position - this.ownerGameObject.transform.position).sqrMagnitude).ToList();
                var searchResults = new AllSearchResults(hitObjects, allMice, allTrap, allHole);
                //sendback search results
                searchResultsCallback(searchResults);

                SearchCompleted = true;
            } 
        }

    }

    //package the result from search and send back to owner game object
    public class AllSearchResults
    {
        public Collider[] AllHitObjectsInSearchRadius;

        public List<Collider> AllMice;
        public List<Collider> AllTrap;
        public List<Collider> AllHole;
        //process objects by distance neeeded

        public AllSearchResults(Collider[] allHitObjectsInSearchRadius, List<Collider> allMice, List<Collider> allTrap, List<Collider> allHole)
        {
            AllHitObjectsInSearchRadius = allHitObjectsInSearchRadius;
            AllMice = allMice;
            AllTrap = allTrap;
            AllHole = allHole;

        }
    }
}

