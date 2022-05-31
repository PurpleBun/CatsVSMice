using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatAI
{
    public class Idle : IState
    {
        float idleTime;
        public delegate void Idling();
        public static event Idling IdleOver;
        public Idle(float idleTime)
        {
            this.idleTime = idleTime;   
        }
        public override void Execute()
        {
            idleTime -= Time.deltaTime;
            //idledurationover
            IdleOver();
        }
    }
}
