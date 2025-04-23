using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectControl : MonoBehaviour
{
    public Type objectType;
    public List<GameObject> gameObjects = new List<GameObject>();

    private GameObject modelObject;

    public void GetTypeObj<T>() where T : IGameObject
    {
        objectType = typeof(T);
        modelObject = Resources.Load<GameObject>("Prefabs/Prefab" + objectType.Name);
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
        GameObject obj = Instantiate(modelObject);
        // obj.AddComponent(objectType);
        obj.transform.parent = transform;
        obj.SetActive(true);
        gameObjects.Add(obj);
        return obj;
    }
}
