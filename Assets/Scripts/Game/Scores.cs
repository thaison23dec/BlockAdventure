using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class BestScoreData
{
    public int score = 0;
}


public class Scores : MonoBehaviour
{

    public SquareTextureData squareTextureData;
    public int _currentScore;
    public Text scoreText;

    private bool newBestScore_ = false;
    private BestScoreData bestScore_ = new BestScoreData();

    private string bestScoreKey_ = "bsdat"; 

    private void Awake()
    {
        if (BinaryDataStream.Exist(bestScoreKey_))
        {
            StartCoroutine(ReadDataFile());
        }
    }

    private IEnumerator ReadDataFile()
    {
        bestScore_ = BinaryDataStream.Read<BestScoreData>(bestScoreKey_);
        yield return new WaitForEndOfFrame();
        GameEvents.UpdateBestScoreBar(_currentScore, bestScore_.score);
    }

    void Start()
    {
        _currentScore = 0;
        newBestScore_ = false;
        squareTextureData.SetStartColor();
        UpdateScoreText();
    }

    private void OnEnable()
    {
        GameEvents.AddScores += AddScores;
        GameEvents.GameOver += SaveBestScore;
    }

    private void OnDisable()
    {
        GameEvents.AddScores -= AddScores;
        GameEvents.GameOver -= SaveBestScore;
    }

    public void SaveBestScore(bool newBestScore)
    {
        BinaryDataStream.Save<BestScoreData>(bestScore_, bestScoreKey_);
    }

    void Update()
    {
        
    }


    private void AddScores(int scores)
    {
        _currentScore += scores;
        if(_currentScore > bestScore_.score)
        {
            newBestScore_ = true;
            bestScore_.score = _currentScore;
            SaveBestScore(true);
        }
        UpdateSquareColor();
        GameEvents.UpdateBestScoreBar(_currentScore, bestScore_.score);
        UpdateScoreText();
    }

    private void UpdateSquareColor()
    {
        if(GameEvents.UpdateSquareColor != null &&  _currentScore >= squareTextureData.thresholdVal)
        {
            squareTextureData.UpdateColors(_currentScore);
            GameEvents.UpdateSquareColor(squareTextureData.currentColor);
        }
    }
    private void UpdateScoreText()
    {
        scoreText.text = _currentScore.ToString();
    }
}
