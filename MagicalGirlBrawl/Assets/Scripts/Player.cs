using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int team;
    [SerializeField] private List<Movement> Available;
    private int _current;

    public void RemoveMovement(Movement m)
    {
        if(Available.Count > 0)
        {
            Available.Remove(m);
        }
        else
        {
            GameController.instance.RestartGame();
        }
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
            player.SetState(i == _current);
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
            player.SetState(i == _current);
            i += 1;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Available[0].SetState(false);
        Available[1].SetState(false);
    }
}
