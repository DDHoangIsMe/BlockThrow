
using System.Collections.Generic;
using UnityEngine;

interface IStackable
{
    List<GameObject> GetListObject<T>() where T : IGameObject;
}
