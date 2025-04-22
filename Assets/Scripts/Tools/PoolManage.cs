using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManage : Singleton<PoolManage>
{
    public Dictionary<string, ObjectControl> poolDictionary = new Dictionary<string, ObjectControl>();

    public GameObject GetObject<T>() where T : IGameObject
    {
        string objectType = typeof(T).Name;
        if (!poolDictionary.ContainsKey(objectType))
        {
            GenerateObjectControl<T>();
        }
        return poolDictionary[objectType].GetObject();  
    }

    private void GenerateObjectControl<T>()  where T : IGameObject
    {
        string objectType = typeof(T).Name;
        if (!poolDictionary.ContainsKey(objectType)) {
            GameObject controller = new GameObject(objectType + "-Control");
            controller.transform.parent = transform;
            ObjectControl poolObject = controller.AddComponent<ObjectControl>();
            poolObject.GetTypeObj<T>();
            poolDictionary.Add(objectType, poolObject);
        }
    }
}
