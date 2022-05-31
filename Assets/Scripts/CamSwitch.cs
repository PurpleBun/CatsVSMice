using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitch : MonoBehaviour
{   
    public GameObject catCam1;
    public GameObject catCam2;
    public GameObject ratCam1;
    public GameObject ratCam2;
    public GameObject ratCam3;
    public GameObject ratCam4;
    public GameObject observerCam;

    void Start()
    {
        deactivateall();
        observerCam.SetActive(true);
    }

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
            ratCam1.SetActive(true);
        }
        else if (x == 4)
        {
            ratCam2.SetActive(true);
        }
        else if (x == 5)
        {
            ratCam3.SetActive(true);
        }
        else if (x == 6)
        {
            ratCam4.SetActive(true);
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
        ratCam1.SetActive(false);
        ratCam2.SetActive(false);
        ratCam3.SetActive(false);
        ratCam4.SetActive(false);
        observerCam.SetActive(false);
    }


    /*public void CatCamera1() 
    {
        catCam1.SetActive(true);
        catCam2.SetActive(false);
        ratCam1.SetActive(false);
        ratCam2.SetActive(false);
        ratCam3.SetActive(false);
        ratCam4.SetActive(false);
        observerCam.SetActive(false);
    }

    public void CatCamera2() 
    {
        catCam1.SetActive(false);
        catCam2.SetActive(true);
        ratCam1.SetActive(false);
        ratCam2.SetActive(false);
        ratCam3.SetActive(false);
        ratCam4.SetActive(false);
        observerCam.SetActive(false);
    }

    public void RatCamera1() 
    {
        catCam1.SetActive(false);
        catCam2.SetActive(false);
        ratCam1.SetActive(true);
        ratCam2.SetActive(false);
        ratCam3.SetActive(false);
        ratCam4.SetActive(false);
        observerCam.SetActive(false);
    }

    public void RatCamera2() 
    {
        catCam1.SetActive(false);
        catCam2.SetActive(false);
        ratCam1.SetActive(false);
        ratCam2.SetActive(true);
        ratCam3.SetActive(false);
        ratCam4.SetActive(false);
        observerCam.SetActive(false);
    }

    public void RatCamera3() 
    {
        catCam1.SetActive(false);
        catCam2.SetActive(false);
        ratCam1.SetActive(false);
        ratCam2.SetActive(false);
        ratCam3.SetActive(true);
        ratCam4.SetActive(false);
        observerCam.SetActive(false);
    }

    public void RatCamera4() 
    {
        catCam1.SetActive(false);
        catCam2.SetActive(false);
        ratCam1.SetActive(false);
        ratCam2.SetActive(false);
        ratCam3.SetActive(false);
        ratCam4.SetActive(true);
        observerCam.SetActive(false);
    }

    public void ObserverCam() 
    {
        catCam1.SetActive(false);
        catCam2.SetActive(false);
        ratCam1.SetActive(false);
        ratCam2.SetActive(false);
        ratCam3.SetActive(false);
        ratCam4.SetActive(false);
        observerCam.SetActive(true);
    }*/
}
