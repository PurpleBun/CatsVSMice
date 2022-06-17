using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitch : MonoBehaviour
{   
    public GameObject catCam1;
    public GameObject catCam2;
    public GameObject MouseCam1;
    public GameObject MouseCam2;
    public GameObject MouseCam3;
    public GameObject observerCam;

    //Deactivate all but observer cam
    void Start()
    {
        deactivateall();
        observerCam.SetActive(true);
    }

    //Switching camera, deactivate all and setactive the one switched to
    public void switchcam(int x)
    {
        deactivateall();
        if (x == 1)
        {
            catCam1.SetActive(true);
        }
        else if (x == 2)
        {
            catCam2.SetActive(true);
        }
        else if (x == 3)
        {
            MouseCam1.SetActive(true);
        }
        else if (x == 4)
        {
            MouseCam2.SetActive(true);
        }
        else if (x == 5)
        {
            MouseCam3.SetActive(true);
        }
        else 
        {
            observerCam.SetActive(true);
        }
    }

    public void deactivateall()
    {
        catCam1.SetActive(false);
        catCam2.SetActive(false);
        MouseCam1.SetActive(false);
        MouseCam2.SetActive(false);
        MouseCam3.SetActive(false);
        observerCam.SetActive(false);
    }
}
