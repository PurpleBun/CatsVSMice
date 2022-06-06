using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string mouseTag;
    public List<GameObject> mouseList;
    public int playTime;
    // Start is called before the first frame update
    void Start()
    {
        //make a list of all mice in the scene
        mouseList = new List<GameObject>();
        foreach(GameObject mouse in GameObject.FindGameObjectsWithTag(mouseTag))
        {
            mouseList.Add(mouse);
        }
        //mouse should be removed once caught by a cat
    }

    // Update is called once per frame
    void Update()
    {
        //check winning condition
        if (mouseList.Count == 0 && Time.time < playTime)
        {
            CatWin();
        };
        if (mouseList.Count > 0 && Time.time >= playTime){
            MouseWin();
        }
    }

    void CatWin()
    {
        //do something
        //Debug.Log("CatWon");
    }

    void MouseWin()
    {
        //do something
        //Debug.Log("MouseWon"); 
    }
}
