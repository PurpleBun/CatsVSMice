using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    private double timeRemaining = 0, timePlayed = 0, timeRemainingWhole = 0;
    private GameManager thisGameManager;
    private Text timerText;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindObjectOfType<GameManager>() != null)
        {
            thisGameManager = GameObject.FindObjectOfType<GameManager>();
            Debug.Log(thisGameManager.name);
        }
        if (this.gameObject.GetComponent<Text>() != null)
        {
            timerText = this.gameObject.GetComponent<Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        timePlayed = Convert.ToDouble(Time.time);
        //Debug.Log(timePlayed);
        timeRemaining = thisGameManager.playTime - timePlayed;
        //Debug.Log(timeRemaining);
        timeRemainingWhole = Math.Truncate(timeRemaining);
        //Debug.Log(timeRemainingWhole);
        if ((timeRemainingWhole % 60) >= 10 && timeRemainingWhole > 0)
        {
            timerText.text = Math.Floor(timeRemainingWhole / 60) + ":" + (timeRemainingWhole % 60);
        }
        else if ((timeRemainingWhole % 60) < 10 && timeRemainingWhole > 0)
        {
            timerText.text = Math.Floor(timeRemainingWhole / 60) + ":0" + (timeRemainingWhole % 60);
        }
        else
        {
            timerText.text = "0:00";
        }
    }
}
