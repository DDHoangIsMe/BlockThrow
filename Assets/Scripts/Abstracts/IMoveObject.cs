using UnityEngine;

interface IMoveObject
{
    public void MoveStraight(Vector3 target, float speed, System.Action callback);
}

interface IMoveWithCurve
{
    public void MoveCurve(StackBlock senderStack, Transform parent, Vector3 target, float intense, float speed, float delay, System.Action<BlockMoveState, StackBlock> callback);
}

interface IMoveWithPassOver
{
    public void MovePassOver(Vector3 target, float intense, float speed);
}
