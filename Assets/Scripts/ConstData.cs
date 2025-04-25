using System;
using UnityEngine;

public static class ConstData
{
    public const string GAME_OBJECT_PREFAB_PATH = "Prefabs/Prefab";
    public const string GAME_MANAGE_PREFAB_PATH = "Prefabs/Manages/Prefab";

    public const int MAX_BLOCKS = 10;
    public const int MIN_BLOCKS = 1;
    public const int ROW_BLOCKS = 5;
    public const int COL_BLOCKS = 5;
    public const float MERGE_WAIT_TIME = 0.8f;
    public const int REST_WAIT_TIME = 0;
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


    public static readonly int[][] SUROUND_DIRECTION = {
        new int[] { 1, 0}, ////Above Mid
        new int[] { 0,-1}, ////Mid Left
        new int[] { 0, 1}, ////Mid Right
        new int[] {-1, 0}, ////Below Mid
        
        new int[] { 1,-1}, ////Above Left
        new int[] { 1, 1}, ////Above Right
        new int[] {-1,-1}, ////Below Left
        new int[] {-1, 1}  ////Below Right
    };
}