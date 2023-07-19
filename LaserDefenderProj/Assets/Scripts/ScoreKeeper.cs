using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    private int score;

    void Start()
    {
        ResetScore();
    }

    public void IncreaseScore(int points)
    {
        score += points;
        Mathf.Clamp(score, 0, int.MaxValue);
        Debug.LogFormat("Score {0}", score);
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0;
    }
}
