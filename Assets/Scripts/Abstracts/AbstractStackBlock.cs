using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractStackBlock : MonoBehaviour
{
    // Current color state of the block
    private BlockColor colorType;
    public BlockColor ColorType
    {
        get { return colorType; }
        protected set { 
            colorType = value; 
            foreach (var block in blocks)
            {
                block.GetComponent<Block>().SetBlockColor(colorType); // Set the color of each block
            }
        }
    }
    
    protected List<GameObject> blocks = new List<GameObject>();

    public abstract void DespawnBlock(); // Abstract method to despawn the block

    public virtual void SpawnBlock(int amount) 
    {
        // create new blocks
        for (int i = 0; i < amount; i++)
        {
            GameObject block = PoolManage.Instance.GetObject<Block>();
            // block.transform.parent = this.transform;
            blocks.Add(block);
        }
    }
}
