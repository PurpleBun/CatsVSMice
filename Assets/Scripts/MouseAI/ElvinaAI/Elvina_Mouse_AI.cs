using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elvina_Mouse_AI : MonoBehaviour
{
    private MouseAbilitiesNValues AbsVls;
    private Transform MouseyTarget;
    private Transform Mousey;
    public float rotSpeed = 100f;
    private float VisionDistance;

    private bool isWandering = false;
    private bool RotateLeft = false;
    private bool RotateRight = false;
    private bool Walking = false;


    private List<Collider> listOfHoles;
    //public transform hole;

    //float holePosition = hole.position.holePosition;

   
   void Awake()
    {
        if (this.gameObject.GetComponent<MouseAbilitiesNValues>() != null)
        {
            AbsVls = this.gameObject.GetComponent<MouseAbilitiesNValues>();
            MouseyTarget = AbsVls.targetTransform;
            Mousey = AbsVls.thisMouseTrans;
            VisionDistance = AbsVls.visionDistance;
        }
        else
        {
            Debug.Log(this.gameObject + " Abilities and Values script not working");
        }
    }
   
   
    void Start ()
    {
        MouseyTarget = AbsVls.targetTransform;
        listOfHoles = new List<Collider>();

    }

    void Update ()

    // following code transforms the position of the mouses target, which helps the mouse explore the map more smoothly and because of the random element of it - also less predictibly.
    // the downside of this is that the exploration of the map is not as efficiant. 
    {
        if (isWandering == false)
        {
            StartCoroutine(Wander());
        }

        if(RotateRight == true)
        {
            MouseyTarget.transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
        }

        if(RotateLeft == true)
        {
            MouseyTarget.transform.Rotate(transform.up * Time.deltaTime * -rotSpeed);
        }

        if(Walking ==true)
        {
            MouseyTarget.transform.position += MouseyTarget.transform.forward * AbsVls.normalSpeed * Time.deltaTime;
        }

        //if the mouse notices any cats in its sight, it will execute the runaway function
        if (AbsVls.catsFound != null)
        {
            runFromCat();
        }
        else 
        {
            Wander();
        }

    }


    IEnumerator Wander()
    {
        int rotTime = Random.Range(1, 3);
        int rotateWait =  0;
        int rotateLorR = Random.Range(1, 2);
        int walkWait = 0;
        int walkTime = Random.Range(1, 5);

        isWandering = true;

        yield return new WaitForSeconds(walkWait);
        Walking = true;
        yield return new WaitForSeconds(walkTime);
        Walking = false;
        yield return new WaitForSeconds(rotateWait);

        if(rotateLorR == 1)
        {
            RotateRight = true;
            yield return new WaitForSeconds(rotTime);
            RotateRight = false;
        }

        if(rotateLorR == 2)
        {
            RotateLeft = true;
            yield return new WaitForSeconds(rotTime);
            RotateLeft = false;
        }

        isWandering = false;
    }

    //memorizing the holes

    private void ListingHoles(List<Collider> holeList)
    {
        // If the mouse has not discovered all the holes yet, then it wonders and adds new holes to the list
        if (listOfHoles.Count < 6)
        {
    
            foreach (Collider holeVisible in holeList)
            {
                listOfHoles.Add(holeVisible);
            }

            Wander();
        }
        else
        {
            // if some holes are already discovered it makes sure not to add extra
            foreach (Collider holeVisible in holeList)
            {
                if (listOfHoles.Contains(holeVisible) == false)
                {
                    listOfHoles.Add(holeVisible);
                }
            }
        }
    }   

//transforms the mouse's targets position to the opposite of the cats direction in order to run away from it
    private void runFromCat()
    {
        Vector3 direction = MouseyTarget.position - transform.position;

        transform.Translate(direction.normalized * Time.deltaTime, Space.World);
        transform.forward = direction.normalized;
    }


    // The mouse's movement towards the hole was still in progress

    // private void goToHole()
    // {
    //     MouseyTarget.transform.position = hole.position.holePosition;
    // }
}
