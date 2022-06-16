using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapStateManager : MonoBehaviour
{
    TrapBaseState currentState;
    public TrapNotSetState TrapUnsetState = new TrapNotSetState();
    public TrapSetState TrapSetupState = new TrapSetState();
    public TrapActiveState TrapActivatedState = new TrapActiveState();
    static public bool TrapActivated = true;


    // Start is called before the first frame update
    void Start()
    {
        currentState = TrapUnsetState;

        currentState.EnterState(this);
    }

    void OnTriggerEnter(Collider collision)
    {
        currentState.OnTriggerEnter(this, collision);
    }

    //Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(TrapBaseState state){
        currentState = state;
        state.EnterState(this);
    }

}