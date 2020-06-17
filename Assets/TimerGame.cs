using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerGame : MonoBehaviour
{

    public Text Timer;
    public GameObject game;
    public GameObject timeUp;
    //public GameObject lose;
    public GameObject win;
    private float timeStart = 600;

    bool isWin = false;
    bool isLose = false;



    void Start()
    {
        StartCoundownTimer();
        Timer.text = timeStart.ToString();
    }


    void StartCoundownTimer()
    {
        if (Timer != null)
        {
            timeStart = 600;
            Timer.text = "Time Left: 10:00";
            InvokeRepeating("UpdateTimer", 0.0f, 0.01667f);
        }
    }

    void Update()
    {
        if (timeStart > 0 && !isLose && !isWin)
        {
            timeStart -= Time.deltaTime;
            Timer.text = Mathf.Round(timeStart).ToString();
        }

        else if (!isLose && !isWin)
        {
            game.SetActive(false);
            timeUp.SetActive(true);
        }

        //else if (!isLose)
        //{
        //    game.SetActive(false);
        //    lose.SetActive(true);
        //}

        else
        {
            game.SetActive(false);
            win.SetActive(true);
        }

        if (Timer != null)
        {
            timeStart -= Time.deltaTime;
            string minutes = Mathf.Floor(timeStart / 60).ToString("00");
            string seconds = (timeStart % 60).ToString("00");
            Timer.text = "" + minutes + ":" + seconds;
        }

    }

    public void WrongAns()
    {
        isLose = true;
    }

    public void RightAns()
    {
        isWin = true;
    }

}

