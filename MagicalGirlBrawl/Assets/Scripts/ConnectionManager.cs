using UnityEngine;
using UnityEngine.InputSystem;

public class ConnectionManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ForceDisconnect()
    {
        PlayerInput[] pArray = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput p in pArray)
        {
            Destroy(p);
            Destroy(p.gameObject);
        }
    }
}
