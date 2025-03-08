using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private List<Movement> Available;
    [SerializeField] private Movement Current;

    public bool Empty(List<Movement> l)
    {
        return l.Count == 0;
    }

    public void OnPrevious()
    {
        Debug.Log("Entree");
        if (Available.Count == 2)
        {
            Debug.Log("2 elem");
            Current.enabled = false;
            Available.Add(Current);
            Current = Available[0];
            Available.Remove(Current);
            Current.enabled = true;
        }
        else
        {
            if (Available.Count == 1)
            {
                Current.enabled = false;
                Available.Add(Current);
                Current = Available[0];
                Available.Remove(Current);
                Current.enabled = true;
            }
            else
            {
                return;
            }
        }
    }

    public void OnNext()
    {
        if (Available.Count == 2)
        {
            Current.enabled = false;
            Available.Add(Current);
            Current = Available[1];
            Available.Remove(Current);
            Current.enabled = true;
        }
        else
        {
            if (Available.Count == 1)
            {
                Current.enabled = false;
                Available.Add(Current);
                Current = Available[0];
                Available.Remove(Current);
                Current.enabled = true;
            }
            else
            {
                return;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Available[0].enabled = false;
        Available[1].enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
