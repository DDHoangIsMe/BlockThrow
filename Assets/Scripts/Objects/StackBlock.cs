using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StackBlock : AbstractStackBlock
{
    [SerializeField]
    private TextMeshProUGUI _textCount;
    public override void DespawnBlock()
    {
        List<GameObject> tempList = GetListObject<Block>();
        if (tempList.Count > 0)
        {
            tempList.RemoveAll(x => true);
        }
    }

    public void MoveToOtherStack(StackBlock block, float speed, System.Action callback)
    {
        List<GameObject> tempList = GetListObject<Block>();
        for (int i = 0; i < tempList.Count; i++) 
        {
            Vector3 tagetPos = block.transform.position + Vector3.up * ConstData.GASP_BLOCK * (block.GetTotalBlocks() + i);
            tempList[i].GetComponent<Block>().MoveStraight(tagetPos, speed, callback);
        }
        // Change in the data
        block.AddBlock(this);
    }

    public void MoveToOtherStack(StackBlock block, float intense, float time)
    {
        List<GameObject> tempList = GetListObject<Block>();
        for (int i = 0; i < tempList.Count; i++) 
        {
            Vector3 tagetPos = block.transform.position + Vector3.up * ConstData.GASP_BLOCK * (block.GetTotalBlocks() + i);
            tempList[i].GetComponent<Block>().MoveCurve(tagetPos, intense, time);
        }
        block.AddBlock(this);
    }

    //Last stack only
    public void MoveOutOfBoard(float speed, System.Action callBack)
    {
        List<GameObject> tempList = GetListObject<Block>();
        for (int i = 0; i < tempList.Count; i++)
        {
            Vector3 addPos = tempList[i].transform.position + Vector3.up * ConstData.UNIT_DISTANCE;
            GameObject tempBlock = tempList[i];
            tempBlock.GetComponent<Block>().MoveStraight(addPos, speed, () => MoveOutOfBoardCallBack(tempBlock, callBack));
        }
    }

    public void MoveOutOfBoardCallBack(GameObject item, System.Action callBack)
    {
        item.SetActive(false);
        callBack();
    }

    // Add more block from param
    public override void AddBlock<T>(T block)
    {
        // Pushed from other stack
        GetListObject<Block>().AddRange(block.GetListObject<Block>());
        ColorType = block.ColorType;
        block.DespawnBlock();
    }

    public override void OrderBlocks()
    {
        base.OrderBlocks();
        _textCount.text = GetTotalBlocks().ToString();
    }

    public void ChangeColor(BlockColor color) 
    {
        ColorType = color;
    } 

    public int GetTotalBlocks()
    {
        return GetListObject<Block>().Count;
    }

    public BlockColor GetBlockColor()
    {
        return ColorType;
    }

    // public void ActionAfterMerge()
    // {
    //     // Check if the stack is full to score
    //     while (GetTotalBlocks() >= ConstData.MAX_BLOCKS)
    //     {
    //         UIControl.Instance.AddScore(ConstData.MAX_BLOCKS);
    //         for (int i = 0; i < ConstData.MAX_BLOCKS; i++)
    //         {
    //             // Todo: Show animation score
                
    //             // Destroy the block
    //             GetListObject<Block>()[GetTotalBlocks() - 1].SetActive(false);
    //             GetListObject<Block>().RemoveAt(GetTotalBlocks() - 1);
    //         }
    //     }
    // }

    public void GetPoint()
    {
        if (GetTotalBlocks() >= 10)
        {
            int keepBlock = GetTotalBlocks() % 10;
            UIControl.Instance.AddScore(GetTotalBlocks() - keepBlock);
            for (int i = keepBlock; i < GetTotalBlocks(); i++)
            {
                GetListObject<Block>()[i].SetActive(false);
            }
            GetListObject<Block>().RemoveRange(0, keepBlock);
        }
    }
}
