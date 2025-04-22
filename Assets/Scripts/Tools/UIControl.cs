using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : Singleton<UIControl>
{
    [SerializeField]
    private Text scoreText;

    private int score;

    void Awake()
    {
        score = 0;   
    }

    public void AddScore(int amount) 
    {
        score += amount;
        UpdateScoreText();
    }

    // Todo: Add animation to the changing score text
    public void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }
}