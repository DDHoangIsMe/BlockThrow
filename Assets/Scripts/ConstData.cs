using System;
using UnityEngine;

public static class ConstData
{
    public const int MAX_BLOCKS = 10;
    public const int MIN_BLOCKS = 1;
    public const int ROW_BLOCKS = 5;
    public const int COL_BLOCKS = 5;
    public const float MERGE_WAIT_TIME = 1.5f;
    public const int REST_WAIT_TIME = 1;
    public const float SHOOTER_SIZE = 1f;
    public const float BLOCK_SPEED = 10f;
    public const float DEFAULT_SPEED = 5f;
    public const float INTENSE_DISTANCE = 0.25f;
    public const float OFF_SET = 0.01f;
    
    public const float TWEEN_SCALE = 1.25f;
    public const float TWEEN_SCALE_SPEED = 20;

    public const int NEGATIVE_ONE = -1;


    //________________________________________________________________
    public static readonly float SCREEN_HEIGHT = Camera.main.orthographicSize * 2;
    public static readonly float SCREEN_WIDTH = SCREEN_HEIGHT * Screen.width / Screen.height;
    public static readonly float UNIT_DISTANCE = Math.Min(SCREEN_WIDTH / ROW_BLOCKS, SCREEN_HEIGHT / (2 * COL_BLOCKS)); 
    public static readonly float GASP_BLOCK = UNIT_DISTANCE / 20;
    public static readonly float INTENSE_CURVE = UNIT_DISTANCE * 1.5f;
}