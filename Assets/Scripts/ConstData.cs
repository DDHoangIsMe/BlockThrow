using System;
using Unity.Mathematics;
using UnityEngine;

public static class ConstData
{
    public const int MAX_BLOCKS = 10;
    public const int MIN_BLOCKS = 1;
    public const int ROW_BLOCKS = 5;
    public const int COL_BLOCKS = 5;
    public const int MERGE_WAIT_TIME = 1;
    public static readonly float SCREEN_HEIGHT = Camera.main.orthographicSize * 2;
    public static readonly float SCREEN_WIDTH = SCREEN_HEIGHT * Screen.width / Screen.height;
    public static readonly float UNIT_DISTANCE = Math.Min(SCREEN_WIDTH / ROW_BLOCKS, SCREEN_HEIGHT / (2 * COL_BLOCKS)) * 0.9f; 
    public static readonly float SCALE = UNIT_DISTANCE / 20;
}