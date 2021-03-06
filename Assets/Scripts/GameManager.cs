using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string mouseTag;
    public List<GameObject> mouseList;
    public int playTime;
    public TMP_Text gameText;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    void CatWin()
    {
        gameText.text = "Cat Wins";
        StartCoroutine(MainMenuCoroutine());
    }

    void MouseWin()
    {
        gameText.text = "Mouse Wins";
        StartCoroutine(MainMenuCoroutine());
    }

    IEnumerator MainMenuCoroutine()
    {
        yield return new WaitForSeconds(8);
        SceneManager.LoadScene("Main Menu");
    }
}
