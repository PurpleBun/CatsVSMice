using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CatAI {
    public class Trap : MonoBehaviour
    {
        public bool trapActivate = false;
        float trapDuration = 5f;
        int trapUse = 0;
        float trapCooldown = 0.3f;
        float mouseSpeed = 4.375f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cat"))
            {
                if(other.gameObject.GetComponent<PhuongCat>().trapIntent == true)
                {
                    StartCoroutine("SettingTrap");
                }
            }

            if (other.CompareTag("Mouse") && trapActivate && trapUse >0)
            {
                NavMeshAgent navmesh = other.gameObject.GetComponent<NavMeshAgent>();
                //float originalSpeed = navmesh.speed;
                navmesh.speed = 0.01f;
                StartCoroutine(Trapped(navmesh, mouseSpeed));
                //navmesh.speed = originalSpeed;
            }
        }

        IEnumerator SettingTrap()
        {
            yield return new WaitForSeconds(trapDuration);
            trapActivate = true;
            trapUse = 2;
        }

        IEnumerator Trapped(NavMeshAgent navmesh, float originalSpeed)
        {
            yield return new WaitForSeconds(3);
            navmesh.speed = originalSpeed;
            trapUse -= 1;
            trapActivate = false;
            Invoke("CoolDown", trapCooldown);
        }

        public void CoolDown()
        {
            if (trapUse > 0)
            {
                trapActivate = true;
            }
        }
    }

}
