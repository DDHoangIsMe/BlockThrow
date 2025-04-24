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

    public void MoveToOtherStack(StackBlock block, System.Action callback, float speed)
    {
        for (int i = 0; i < blocks.Count; i++) 
        {
            Vector3 tagetPos = block.transform.position + Vector3.up * ConstData.GASP_BLOCK * (block.GetBlocks().Count + i);
            blocks[i].GetComponent<Block>().MoveStraight(tagetPos, speed, callback);
        }
        // Change in the data
        block.AddBlock(this);
    }

    public void MoveToOtherStack(StackBlock block, float intense, float time)
    {
        for (int i = 0; i < blocks.Count; i++) 
        {
            Vector3 tagetPos = block.transform.position + Vector3.up * ConstData.GASP_BLOCK * (block.GetBlocks().Count + i);
            blocks[i].GetComponent<Block>().MoveCurve(tagetPos, intense, time);
        }
        block.AddBlock(this);
    }

    public void AddBlock<T>(T block) where T : AbstractStackBlock
    {
        // Pushed from other stack
        blocks.AddRange(block.GetBlocks());
        ColorType = block.ColorType;
        block.DespawnBlock();
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

    public void GetPoint()
    {
        if (blocks.Count >= 10)
        {
            int keepBlock = blocks.Count % 10;
            UIControl.Instance.AddScore(blocks.Count - keepBlock);
            for (int i = keepBlock; i < blocks.Count; i++)
            {
                blocks[i].SetActive(false);
            }
            blocks = blocks.GetRange(0, keepBlock);
        }
    }
}
