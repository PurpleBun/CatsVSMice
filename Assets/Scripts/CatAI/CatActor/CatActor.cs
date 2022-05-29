using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatAI
{
    public class CatActor : MonoBehaviour
    {
        protected float baseSpeed = 3.5f;

        protected float angularSpeed = 120f;

        protected float acceleration = 8f;

        protected float trapDuration = 7f;

        [SerializeField]
        protected GameManager manager;

        //somehow inheritance doesnt work?
        //public virtual void onCollisionEnter(Collision other)
        //{
        //    Debug.Log("a");
        //    if (other.gameObject.tag=="Mouse") {
        //        manager.mouseList.Remove(other.gameObject);
        //        Destroy(other.gameObject);
        //    }
        //}

    }
}
