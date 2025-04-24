using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : Singleton<UIControl>
{
    [SerializeField]
    private Text scoreText;

    private int _score;

    void Awake()
    {
        _score = 0;   
    }

    public void AddScore(int amount) 
    {
        _score += amount;
        UpdateScoreText();
    }

    // Todo: Add animation to the changing score text
    public void UpdateScoreText()
    {
        scoreText.text = _score.ToString();
    }
}