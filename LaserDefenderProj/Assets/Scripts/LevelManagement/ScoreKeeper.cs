using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    private int score;
    static private ScoreKeeper scoreKeeperSingletonInstance; // This will store reference to current object for singleton implementation

    private void Awake()
    {
        ManageSingleton();
    }

    void Start()
    {
        ResetScore();
    }

    void ManageSingleton()
    {
        if (scoreKeeperSingletonInstance == null)
        {
            // first instance of this game object in the game scene.
            // Save this so we can access the data again during game over scene.
            scoreKeeperSingletonInstance = this;
            DontDestroyOnLoad(scoreKeeperSingletonInstance);
        }
        else
        {
            // We've already createed an score keeper instance, so destroy the new game object.
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    public void IncreaseScore(int points)
    {
        score += points;
        Mathf.Clamp(score, 0, int.MaxValue);
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
