using UnityEngine;
using System.Collections;
public class ShakeWithAnimation : MonoBehaviour
{
    public Animator camAnim;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Button is pressed AAAAAAAAAAAAAAAAAAAAAAAAA");
            camAnim.SetTrigger("Shake");
        }
    }
}