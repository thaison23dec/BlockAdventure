using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
    public int _currentScore;
    public Text scoreText;
    void Start()
    {
        _currentScore = 0;
        UpdateScoreText();
    }

    private void OnEnable()
    {
        GameEvents.AddScores += AddScores;
    }

    private void OnDisable()
    {
        GameEvents.AddScores -= AddScores;
    }


    void Update()
    {
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = _currentScore.ToString();
    }

    private void AddScores(int scores)
    {
        _currentScore += scores;
    }
}
