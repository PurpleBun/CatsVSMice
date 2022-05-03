using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseStateManager : MonoBehaviour
{
    MouseBaseState currentState;
    MouseHealthyState healthyState = new MouseHealthyState();
    MouseSlowState slowState = new MouseSlowState();
    MouseHidingState hidingState = new MouseHidingState();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
