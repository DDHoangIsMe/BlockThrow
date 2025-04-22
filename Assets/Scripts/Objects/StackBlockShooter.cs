using UnityEngine;
using System.Collections.Generic;

public class StackBlockShooter : AbstractStackBlock
{
    public GameObject blockPrefab;
    public Transform spawnPoint;

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
}