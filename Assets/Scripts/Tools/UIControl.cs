using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : Singleton<UIControl>
{
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private GameObject _itemTable;

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

    public void UpdateGoalAmount(int index, int amount)
    {
        Transform tempSpot = _itemTable.transform.GetChild(index);
        if (tempSpot != null)
        {
            tempSpot.Find("Amount").GetComponent<TextMeshProUGUI>().text = amount.ToString();
        }
    }
    
    public void UpdateGoalImage(int index, Sprite image)
    {
        Transform tempSpot = _itemTable.transform.GetChild(index);
        if (tempSpot != null)
        {
            tempSpot.Find("Image").GetComponent<Image>().sprite = image;
        }
    }
}