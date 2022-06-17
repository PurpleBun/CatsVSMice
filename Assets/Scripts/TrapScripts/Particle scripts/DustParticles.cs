using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustParticles : MonoBehaviour
{

    ParticleSystem dust;

    // Start is called before the first frame update
    void Start()
    {
        dust = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "trap")
        {
            dust.Play();
        }
        
    }
}
