using UnityEngine;
using System.Collections.Generic;

public class StackBlockShooter : AbstractStackBlock, IDragableObject
{
    private Vector3 previousPos;
    void Start()
    {
        previousPos = transform.position;
        SpawnBlock(Random.Range(ConstData.MIN_BLOCKS, ConstData.MAX_BLOCKS));
        ColorType = (BlockColor)Random.Range(0, System.Enum.GetValues(typeof(BlockColor)).Length);
    }

    void Update()
    {
        if (transform.position != previousPos)
        {
            for (int i = 0; i < blocks.Count; i++) 
            {
                blocks[i].GetComponent<Block>().MovePassOver(
                    new Vector3(transform.position.x, blocks[i].transform.position.y, blocks[i].transform.position.z),
                    ConstData.INTENSE_DISTANCE, 
                    ConstData.DEFAULT_SPEED * (blocks.Count - i)
                );
            }
            previousPos = transform.position;
        }
    }

    public override void DespawnBlock()
    {
        if (blocks.Count > 0)
        {
            // Remove the current block
            blocks = new List<GameObject>();
        }
    }

    public List<GameObject> GetBlocks()
    {
        return blocks;
    }

    public void ActionEndDrag()
    {
        //Todo: Check status of the game
        //Todo: Check shootable place on board
        //Todo: Check current shooter point to
        //Todo: Place the shooter to the grid 
        //Todo: Action shoot to board and change State
        //Todo: Send callback to board for finish
    }

    public void ResponseFinishShoot()
    {
        
    }
}