using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    protected static T instance;
    private static bool applicationIsQuitting = false;

    static public bool isActive {
        get {
            return instance != null;
        }
    }

    public static T Instance
    {
        get {
            if (applicationIsQuitting)
            {
                return null;
            }
            if(instance == null)
            {
                instance = (T) FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    GameObject go = new GameObject("Singleton-" + typeof(T).Name);
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    void OnEnable()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this as T;
    }

    void OnApplicationQuit()
    {
        applicationIsQuitting = true;
        instance = null;
    }
}

