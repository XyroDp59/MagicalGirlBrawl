using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private static int playerCount = -1;
    private int playerIndex;
    [SerializeField] private int team;
    [SerializeField] private List<Movement> Available;
    private int _current;

    private Color playerColor;
    public UnityEvent<InputAction.CallbackContext> onJump = new();
    public UnityEvent<InputAction.CallbackContext> onMove = new();
    public UnityEvent<InputAction.CallbackContext> onCast = new();
    public UnityEvent<InputAction.CallbackContext> onSmash = new();
    public UnityEvent<InputAction.CallbackContext> onGrab = new();

    public void OnJump(InputAction.CallbackContext context)
    {
        onJump.Invoke(context);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        onMove.Invoke(context);
    }

    public void OnCast(InputAction.CallbackContext context)
    {
        onCast.Invoke(context);
    }

    public void OnSmash(InputAction.CallbackContext context)
    {
        onSmash.Invoke(context);
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        onGrab.Invoke(context);
    }
    
    private void Awake()
    {
        playerCount++;
        playerIndex = playerCount;
        Debug.Log(playerIndex);
        GameController.instance.players.Add(this);
        playerColor = GameController.instance.colors[playerIndex];

        foreach (var movement in Available)
        {
            movement.childRenderer.color = playerColor;
            movement.GetComponent<HealthSystem>().SetColor(playerColor);
        }
        transform.position = GameController.instance.restroom.transform.GetChild(playerIndex).position;
    }

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

    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (!context.started) return;
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

    public void OnNext(InputAction.CallbackContext context)
    {
        if (!context.started) return;
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
