using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CarGame;
using UnityEditor;

public class GameTimer : MonoBehaviour
{

    public Text Timer;
    public GameObject game;
    public GameObject timeUp;
    public GameObject lose;
    public GameObject displayCanvas;
    public Transform gameOverCanvas;
    public float timeStart = 10;

    internal bool isGameOver = false;
    public Transform deathEffect;
    public int score = 0;
    internal int highScore = 0;
    public int scorePerSecond = 1;
    internal GameObject soundSource;
    public AudioClip soundGameOver;
    public float startDelay = 0.5f;
    public Transform scoreText;
    internal int scoreMultiplier = 1;
    

    bool isWin = false;
    bool isLose = false;

    

    void Start()
    {
        
        ChangeScore(0);
        StartCoundownTimer();
        Timer.text = timeStart.ToString();
        highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "HighScore", 0);
    }


    void StartCoundownTimer()
    {
        if (Timer != null)
        {
            Timer.text = "Time Left: 10:00";
            InvokeRepeating("UpdateTimer", 0.0f, 0.01667f);
        }

        if (scorePerSecond > 0) InvokeRepeating("ScorePerSecond", startDelay, 1);


    }

    void Update()
    {
        
        if (timeStart > 0 && !isLose && !isWin)
        {
            timeStart -= Time.deltaTime;
            Timer.text = Mathf.Round(timeStart).ToString();  
        }

        else if (!isLose && !isWin )
        {
            Time.timeScale = 0;
            game.SetActive(false);
            timeUp.SetActive(true);
            displayCanvas.SetActive(false);

            //Написать в текстовое поле TextScore
            gameOverCanvas.Find("Base/TextScore").GetComponent<Text>().text = "SCORE " + score.ToString();

            //Проверка на лучший счет
            if (score > highScore)
            {
                highScore = score;

                //Установить лучший счет
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HighScore", score);
            }

            //Написать лучший счет
            gameOverCanvas.Find("Base/TextHighScore").GetComponent<Text>().text = "HIGH SCORE " + highScore.ToString();

        }

        else
        {
            
            game.SetActive(false);
        }

        if (Timer != null)
        {
            timeStart -= Time.deltaTime;
            string minutes = Mathf.Floor(timeStart / 60).ToString("00");
            string seconds = (timeStart % 60).ToString("00");
            Timer.text = "" + minutes + ":" + seconds;
        }

    }

    public void ChangeScore(int changeValue)
    {
        // Увеличть счет
        score += changeValue;

        //Обновить текстовое поле очков
        if (scoreText)
        {
            scoreText.GetComponent<Text>().text = score.ToString();

            // Анимация объекта score
            if (scoreText.GetComponent<Animation>()) scoreText.GetComponent<Animation>().Play();
        }
    }

    public void ScorePerSecond()
    {
        ChangeScore(scorePerSecond);
    }

    void SetScoreMultiplier(int setValue)
    {
        // Установить множитель баллов
        scoreMultiplier = setValue;
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


