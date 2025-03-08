using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private List<Movement> Available;
    private int _current;

    public bool Empty(List<Movement> l)
    {
        return l.Count == 0;
    }

    public void OnPrevious()
    {
        if (Available.Count == 0) return;
        _current -= 1;
        if(_current < 0) _current += Available.Count;
        Debug.Log(_current);
        int i = 0;
        foreach (var player in Available)
        {
            player.isActive = i == _current;
            i += 1;
        }
    }

    public void OnNext()
    {
        if (Available.Count == 0) return;
        _current += 1;
        _current %= Available.Count;
        Debug.Log(_current);
        int i = 0;
        foreach (var player in Available)
        {
            player.isActive = i == _current;
            i += 1;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Available[0].isActive = false;
        Available[1].isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
