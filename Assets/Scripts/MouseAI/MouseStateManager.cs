using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseStateManager : MonoBehaviour
{
    MouseBaseState currentState;
    public MouseHealthyState healthyState = new MouseHealthyState();
    public MouseSlowState slowState = new MouseSlowState();
    public MouseHidingState hidingState = new MouseHidingState();

    // Start is called before the first frame update
    void Start()
    {
        currentState = healthyState;
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchState(MouseBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
}
