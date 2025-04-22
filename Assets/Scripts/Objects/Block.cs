using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour, IGameObject
{
    private SpriteRenderer blockSprite;

    void Awake()
    {
        blockSprite = GetComponent<SpriteRenderer>();
    }
    public void OnCreateObject()
    {
        // Initialize the block object here
        Debug.Log("Block created!");
    }

    public void SetBlockColor(BlockColor color)
    {
        // Set the block color based on the BlockColor enum
        switch (color)
        {
            case BlockColor.Red:
                blockSprite.color = Color.red;
                break;
            case BlockColor.Green:
                blockSprite.color = Color.green;
                break;
            case BlockColor.Blue:
                blockSprite.color = Color.blue;
                break;
            case BlockColor.Yellow:
                blockSprite.color = Color.yellow;
                break;
            case BlockColor.Purple:
                blockSprite.color = new Color(0.5f, 0, 0.5f); // Purple color RGB
                break;
        }
    }

    public void PlaceBlock(int order)
    {
        // Set the block's position in the game world
        transform.position = transform.parent.position + Vector3.up * order * ConstData.SCALE;
    }
}
