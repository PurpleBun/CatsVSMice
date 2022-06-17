using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeParticles : MonoBehaviour
{

    ParticleSystem smoke;

    //retrieving the particle system 
    void Start()
    {
        smoke = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //playing the particle system once an object collides with the hole
    void OnCollisionEnter(Collision other)
    {
            smoke.Play();
    
    }
}
