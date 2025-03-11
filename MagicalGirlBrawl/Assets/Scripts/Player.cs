using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private int playerIndex;
    [SerializeField] private int team;
    [SerializeField] private List<Movement> Available;
    private int _current = 2;

    [SerializeField] private ParticleSystem switchTrail;
    private bool can_switch = true;

    private Color playerColor;
    public UnityEvent<InputAction.CallbackContext> onJump = new();
    public UnityEvent<InputAction.CallbackContext> onMove = new();
    public UnityEvent<InputAction.CallbackContext> onCast = new();
    public UnityEvent<InputAction.CallbackContext> onSmash = new();
    public UnityEvent<InputAction.CallbackContext> onSmashReleased = new();
    public UnityEvent<InputAction.CallbackContext> onGrab = new();
    public UnityEvent<InputAction.CallbackContext> onPrevious = new();
    public UnityEvent<InputAction.CallbackContext> onNext = new();
    public UnityEvent<InputAction.CallbackContext> onPause = new();

    private IEnumerator Trail(Vector3 p1, Movement current)
    {
        float t = 0f;
        ParticleSystem trail = Instantiate(switchTrail);
        Vector3 p2;
        while (t < 1)
        {
            p2 = current.transform.position;
            trail.transform.position = new Vector3(Mathf.Lerp(p1.x,p2.x,t),Mathf.Lerp(p1.y,p2.y,t),0);
            t = t + Time.deltaTime;
            yield return null;
        }
        Destroy(trail);
    }
    
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
        if (context.canceled) onSmashReleased.Invoke(context);
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        onGrab.Invoke(context);
    }
    
    private void Awake()
    {
        playerIndex = GameController.instance.players.Count;
        //Debug.Log(playerIndex);
        GameController.instance.players.Add(this);
        playerColor = GameController.instance.colors[playerIndex];

        foreach (var movement in Available)
        {
            movement.childRenderer.color = playerColor;
            movement.GetComponent<HealthSystem>().SetColor(playerColor);
            movement.playerID = playerIndex;
        }
        transform.position = GameController.instance.restroom.transform.GetChild(playerIndex).GetChild(0).position;
    }

    public void RemoveMovement(Movement m)
    {
        if(Available.Count > 0)
        {
            Available.Remove(m);
            m.isActive = false;
            m.gameObject.SetActive(false);
        }
    }
    
    private IEnumerator Switch(int new_puppet, bool onDeath = false)
    {
        if(Available.Count == 0) { yield return null; }
        can_switch = false;
        Movement oldMovement = Available[_current];
        Vector3 p1 = oldMovement.transform.position;
        // ptdr j'ai compris à moitié ce qu'il se passe ici
        /*     _current += new_puppet;

               if(_current < 0) _current += onDeath ? Available.Count -1 : Available.Count;
               _current %= onDeath ? Available.Count -1 : Available.Count;

               if(onDeath) RemoveMovement(oldMovement);//Note: suppose qu'on ne switch que si le mouvement mort est actif

               Movement newMovement = Available.ElementAt(_current);
           */

        Available.Remove(oldMovement);
        int newPuppetID = 0;
        Movement newMovement = Available[0];

        if(Available.Count != 0)    // S'il n'y a qu'un seul pantin autre que le current, pas besoin de chercher le plus à droite/gauche
        {
            if (new_puppet > 0)
            {
                newPuppetID = (Available[0].transform.position.x > Available[1].transform.position.x) ? 0 : 1;
            }
            if (new_puppet < 0)
            {
                newPuppetID = (Available[0].transform.position.x > Available[1].transform.position.x) ? 1 : 0;
            }
        }
        _current = newPuppetID;
        newMovement = Available[newPuppetID];
        Available.Add(oldMovement);

        //------------------------ en dessous ça a du sens

        StartCoroutine(Trail(p1,newMovement));
        yield return new WaitForSeconds(1f);

        foreach (var player in Available)
        {
            player.SetState(player == newMovement);
        }
        can_switch = true;
    }
    
    private void OnTotemDeath(Movement m)
    {
        if (Available.Count > 1)
        {
            if (m.isActive)
                StartCoroutine(Switch(-1, true));
            else
            {
                RemoveMovement(m);
            }
        }
        else
        {
            RemoveMovement(m);
            GameController.instance.EndGame(playerIndex);
        }
    }


    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (!can_switch) return;
        if (!context.started) return;
        if (Available.Count == 0) return;
        onPrevious.Invoke(context);
        StartCoroutine(Switch(-1));
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        if (!can_switch) return;
        if (!context.started) return;
        if (Available.Count == 0) return;
        onNext.Invoke(context);
        StartCoroutine(Switch(1));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Available[0].SetState(false);
        Available[1].SetState(false);
        
        // Health listener code
        HealthSystem[] healthSystems = GetComponentsInChildren<HealthSystem>();
        foreach (HealthSystem hs in healthSystems)
        {
            hs.TotemDeath.AddListener(OnTotemDeath);
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        GameController.instance.OnPause();
    }
}
