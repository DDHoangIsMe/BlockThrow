using UnityEngine;

interface IMoveObject
{
    public void MoveStraight(Vector3 target, float speed);
}

interface IMoveWithCurve
{
    public void MoveTo(Vector3 target, float intense, float duration, bool isAnimation);
}

interface IMoveWithPassOver
{
    public void MovePassOver(Vector3 target, float intense, float speed);
}
