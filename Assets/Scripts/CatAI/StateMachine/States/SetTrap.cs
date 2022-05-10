using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatAI
{
    public class SetTrap : IState
    {
        Animator animator;
        public SetTrap(Animator animator)
        {
            this.animator = animator;
        }
        public void Enter()
        {

        }

        public void Execute()
        {
            //play set trap animation here
            //countdown? timer?
        }

        public void Exit()
        {

        }
    }
}
