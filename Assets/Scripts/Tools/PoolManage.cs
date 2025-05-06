using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManage : Singleton<PoolManage>
{
    private Dictionary<string, ObjectControl> _poolDictionary = new Dictionary<string, ObjectControl>();

    public GameObject GetObject<T>()
    {
        string objectType = typeof(T).Name;
        if (!_poolDictionary.ContainsKey(objectType))
        {
            GenerateObjectControl<T>();
        }
        return _poolDictionary[objectType].GetObject();  
    }

    private void GenerateObjectControl<T>()
    {
        string objectType = typeof(T).Name;
        if (!_poolDictionary.ContainsKey(objectType)) {
            GameObject controller = new GameObject(objectType + "-Control");
            controller.transform.parent = transform;
            ObjectControl poolObject = controller.AddComponent<ObjectControl>();
            poolObject.GetTypeObj(typeof(T));
            _poolDictionary.Add(objectType, poolObject);
        }
    }

    public GameObject GetObjectControl<T>()
    {
        if (_poolDictionary.ContainsKey(typeof(T).Name)) 
        {
            return _poolDictionary[typeof(T).Name].gameObject;
        }
        return null;
    }
}
