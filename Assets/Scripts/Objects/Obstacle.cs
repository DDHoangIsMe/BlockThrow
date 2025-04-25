using UnityEngine;

public class Obstacle : MonoBehaviour, IGameObject
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCreateObject()
    {
        // Initialize the obstacle object here
        Debug.Log("Obstacle created!");
    }

    public void DeactiveCoroutine()
    {
        //
    }
}
