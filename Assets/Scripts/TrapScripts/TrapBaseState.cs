using UnityEngine;

public abstract class TrapBaseState 
{
   public abstract void EnterState(TrapStateManager trap);

   public abstract void UpdateState(TrapStateManager trap);

   public abstract void OnTriggerEnter(TrapStateManager trap, Collider collision);
}
