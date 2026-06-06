using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverPopUp : MonoBehaviour
{
    public GameObject gameOverPopUp;
    public GameObject loosePopUp;
    public GameObject newBestScorePopUp;
    public TextMeshProUGUI scoreText;
    public Scores score;

    public bool isNewBestScore;

    private void Awake()
    {
        isNewBestScore = false;
    }

    void Start()
    {
        gameOverPopUp.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= OnGameOver;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.O))
        {
            OnGameOver();
        }
    }

    private void OnGameOver()
    {
        gameOverPopUp.SetActive(true);
        if (!score.newBestScore)
        {
            newBestScorePopUp.SetActive(false);
            loosePopUp.SetActive(true);
        }
        else
        {
            newBestScorePopUp.SetActive(true);
            loosePopUp.SetActive(false);
        }
        scoreText.text = score._currentScore.ToString();
    }


}
