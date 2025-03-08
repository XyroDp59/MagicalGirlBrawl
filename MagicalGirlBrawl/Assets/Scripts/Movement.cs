using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float direction = 0f; 
    [SerializeField] private float move_speed = 7f;
    [SerializeField] private float jump_power = 17f;
    [SerializeField] public int nb_double_jump = 2;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Reset_Double_Jump_Ground()
    {
        nb_double_jump = 2;
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
        direction = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(direction * move_speed, rb.linearVelocity.y);

        if ((Input.GetButtonDown("Jump"))&&(nb_double_jump > 0))
        {
            nb_double_jump = nb_double_jump - 1;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump_power);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Reset_Double_Jump_Ground();
        }
    }
}
