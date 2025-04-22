using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockColor
{
    Red,
    Green,
    Blue,
    Yellow,
    Purple
}

public class StackBlock : AbstractStackBlock
{
    public GameObject blockPrefab; // Prefab for the block

    public override void DespawnBlock()
    {
        if (blocks.Count > 0)
        {
            blocks = new List<GameObject>();
        }
    }

    public void MoveToOtherStack(StackBlock block, bool curveAnimate = false)
    {
        if (curveAnimate)
        {
            //Todo: Show animation to move the block
        }
        else
        {
            //Todo: Show animation with curve
        }

        // Change in the data
        block.AddBlock(this);
    }

    public void AddBlock(StackBlock block)
    {
        // Pushed from other stack
        ColorType = block.ColorType;
        blocks.AddRange(block.GetBlocks());
        block.DespawnBlock();
    }

    public void AddBlock(StackBlockShooter block)
    {
        // Pushed from shooter
        blocks = new List<GameObject>(block.GetBlocks());
    }

    public void ChangeColor(BlockColor color) 
    {
        ColorType = color;
    } 

    public int GetTotalBlocks()
    {
        return blocks.Count;
    }

    public BlockColor GetBlockColor()
    {
        return ColorType;
    }

    public List<GameObject> GetBlocks()
    {
        return blocks;
    }

    public void ActionAfterMerge()
    {
        // Check if the stack is full to score
        while (blocks.Count >= ConstData.MAX_BLOCKS)
        {
            UIControl.Instance.AddScore(ConstData.MAX_BLOCKS);
            for (int i = 0; i < ConstData.MAX_BLOCKS; i++)
            {
                // Todo: Show animation score

                // Destroy the block
                blocks[blocks.Count - 1].SetActive(false);
                blocks.RemoveAt(blocks.Count - 1);
            }
        }
    }

    public void OrderBlocks() {
        for (int i = 0; i < blocks.Count; i++)
        {
            //Todo: Animation ordering blocks
            blocks[i].transform.position = transform.position + Vector3.up * i * ConstData.UNIT_DISTANCE;
        }
    }
}
