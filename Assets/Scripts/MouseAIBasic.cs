using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAIBasic : MonoBehaviour
{
    [SerializeField]
    private float mouseMoveForce, mouseMaxSpdNorm, mouseMaxSpdSlow;
    public bool triggeredTrap;
    private Rigidbody mouseRigBody; // RigidBidy of this mouse object.

    void Awake()
    {
        // Checking if this mouse object has a RigidBody. If yes - assign it to mouseRigBody. If no - slap an error.
        if (this.gameObject.GetComponent<Rigidbody>() != null)
        {
            mouseRigBody = this.gameObject.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.Log("This mouse has no RigidBody component.");
        }
    }

    void Start()
    {
        triggeredTrap = false;
    }

    // Using FixedUpdate for properly working physics.
    void FixedUpdate()
    {
        if (mouseRigBody.velocity.magnitude >= mouseMaxSpdNorm && triggeredTrap == false)
        {
            mouseRigBody.velocity = Vector3.ClampMagnitude(mouseRigBody.velocity, mouseMaxSpdNorm);
        }
        else if (mouseRigBody.velocity.magnitude >= mouseMaxSpdSlow && triggeredTrap == true)
        {
            mouseRigBody.velocity = Vector3.ClampMagnitude(mouseRigBody.velocity, mouseMaxSpdSlow);
        }
        else
        {
            mouseRigBody.AddForce(mouseMoveForce * Vector3.forward);
        }
    }
}
