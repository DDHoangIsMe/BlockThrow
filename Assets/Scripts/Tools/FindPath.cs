using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class FindPath : MonoBehaviour
{
    public static HashSet<(int, int)> FindBestPath(StackBlock[,] matrix, (int, int) pos, HashSet<(int, int)> passed, BlockColor color)
    {
        int max = 0;
        HashSet<(int, int)> result = new HashSet<(int, int)>();
        Queue<(int x, int y, HashSet<(int, int)> visited, int total)> queue = new Queue<(int, int, HashSet<(int, int)>, int)>();
        queue.Enqueue((pos.Item1, pos.Item2, new HashSet<(int, int)>(passed ?? new HashSet<(int, int)>()){(pos.Item1, pos.Item2)}, max));

        while (queue.Count > 0)
        {
            var (x, y, visited, total) = queue.Dequeue();
            foreach (int[] item in ConstData.SUROUND_DIRECTION)
            {
                int nextCol = x + item[0];
                int nextRow = y + item[1];
                if (nextCol >= 0 &&
                    nextCol < ConstData.ROW_BLOCKS &&
                    nextRow >= 0 &&
                    nextRow < ConstData.COL_BLOCKS &&
                    !visited.Contains((nextCol, nextRow)))
                {
                    if (matrix[nextCol, nextRow].ColorType == color)
                    {
                        HashSet<(int, int)> newVisited = new HashSet<(int, int)>(visited) {(nextCol, nextRow)};
                        queue.Enqueue((nextCol, nextRow, newVisited, total + matrix[nextCol, nextRow].GetNumberBlocks()));
                    }
                }
                if (max < total)
                {
                    max = total;
                    result = visited;
                }
            }
        }
        
        if (passed != null)
        {
            result.RemoveWhere(x => passed.Contains(x) && passed.First() != x);
        }
        if (result.Count <= 1)
        {
            result = new HashSet<(int, int)>();
        }
        return result;
    }
}
