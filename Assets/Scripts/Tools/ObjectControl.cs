using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectControl : MonoBehaviour
{
    public List<GameObject> gameObjects = new List<GameObject>();

    private GameObject _modelObject;

    public void GetTypeObj(Type type)
    {
        if (typeof(IGameObject).IsAssignableFrom(type))
        {
            _modelObject = Resources.Load<GameObject>(ConstData.GAME_OBJECT_PREFAB_PATH + type.Name);
        }
        else if (typeof(IStackable).IsAssignableFrom(type))
        {   
            _modelObject = Resources.Load<GameObject>(ConstData.GAME_MANAGE_PREFAB_PATH + type.Name);
        }
    }

    public GameObject GetObject()
    {
        foreach (GameObject obj in gameObjects)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        return GenerateObject();
    }

    private GameObject GenerateObject() 
    {
        GameObject obj = Instantiate(_modelObject);
        // obj.AddComponent(objectType);
        obj.transform.parent = transform;
        obj.SetActive(true);
        gameObjects.Add(obj);
        return obj;
    }
}
