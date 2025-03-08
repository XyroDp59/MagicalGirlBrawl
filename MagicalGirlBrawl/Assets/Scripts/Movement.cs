using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float direction = 0f;
    [SerializeField] private float move_speed = 7f;
    [SerializeField] private float jump_power = 17f;
    [SerializeField] public int nb_double_jump = 2;
    private PlayerInput _playerInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnJump()
    {
        if (nb_double_jump <= 0) return;
        nb_double_jump = nb_double_jump - 1;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump_power);
    }

    private void OnMove(InputValue value)
    {
        direction = value.Get<Vector2>().x;
        Debug.Log(value);
    }

    public void Reset_Double_Jump_Ground()
    {
        nb_double_jump = 2;
    }

    public void Remove_Ground_Jump()
    {
        if (nb_double_jump == 2)
        {
            nb_double_jump = 1;
        }
    }

    public void Reset_Double_Jump_Switch()
    {
        if (nb_double_jump == 0)
        {
            nb_double_jump = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(direction * move_speed, rb.linearVelocity.y);
        //direction = Input.GetAxisRaw("Horizontal");
        //rb.linearVelocity = new Vector2(direction * move_speed, rb.linearVelocity.y);

        // if ((Input.GetButtonDown("Jump"))&&(nb_double_jump > 0))
        // {
        //     nb_double_jump = nb_double_jump - 1;
        //     rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump_power);
        // }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Reset_Double_Jump_Ground();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Remove_Ground_Jump();
        }
    }
}