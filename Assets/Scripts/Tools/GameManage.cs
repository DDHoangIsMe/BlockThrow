using System;
using UnityEngine;

public class GameManage : MonoBehaviour
{
    [SerializeField]
    private GameObject _itemTable;


    private GameObject _boardManage;
    private Type[] _listItem = new Type[ConstData.MAX_ITEM_TABLE];
    private int[] _listKind = new int[ConstData.MAX_ITEM_TABLE]; 
    private int[] _listAmount = new int[ConstData.MAX_ITEM_TABLE];


    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        for (int i = 0; i < _listItem.Length; i ++)
        {
            _listItem[i] = typeof(Block);
            _listKind[i] = 1;
            _listAmount[i] = 10;
        }
        GenerateNewGame();
#endif
    }

    public void GenerateNewGame()
    {
        for (int i = 0; i < _listItem.Length; i++)
        {
            if (_listItem[i] != null)
            {
                UIControl.Instance.UpdateGoalAmount(i, _listAmount[i]);
                if (_listItem[i] == typeof(Block))
                {
                    string kindBlock = Enum.GetName(typeof(BlockColor), _listKind[i]);
                    Sprite newSprite = Resources.Load<Sprite>(ConstData.BLOCK_COLOR_TEXTURE_PATH + kindBlock);
                    UIControl.Instance.UpdateGoalImage(i, newSprite);
                }
            }
            else
            {
                break;
            }
        }
    }
}
