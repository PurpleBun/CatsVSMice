using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatAI
{
    public class Attack : IState
    {
        Animator animator;
        public Attack(Animator animator)
        {
            this.animator = animator;       
        }
       public void Enter()
        {

        }

        public void Execute()
        {
            //play attack animation here(bool?clip?)
            //deduct mouse health
        }

        public void Exit()
        {

        }
    }
}
