using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public bool isActive = false;
    private Rigidbody2D rb;
    private float direction = 0f;
    [SerializeField] private float move_speed = 7f;
    [SerializeField] private float jump_power = 17f;
    [SerializeField] public int nb_double_jump = 2;
    //private PlayerInput _playerInput;
    private int _walkBoolHash = Animator.StringToHash("Walking");
    private Animator _animator;
    private float _defaultYRotation;
    private PlayerInput _playerInput;
    [SerializeField] private ProjectileBehaviour Projectile_Prefab;
    [SerializeField] private Transform Launch_Offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //_playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
    }

    private void OnJump()
    {
        if(!isActive) return;
        if (nb_double_jump <= 0) return;
        nb_double_jump = nb_double_jump - 1;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump_power);
    }

    public void SetState(bool state)
    {
        isActive = state;
        if (!state)
        {
            _animator.SetBool(_walkBoolHash, false);
        }
    }

    private void OnMove(InputValue value)
    {
        if(!isActive) return;
        direction = value.Get<Vector2>().x;
        _animator.SetBool(_walkBoolHash, direction != 0);
        float newYRotation = transform.rotation.eulerAngles.y;
        if (direction < 0) newYRotation = - 180;
        if (direction > 0) newYRotation = 0;
        transform.rotation = Quaternion.Euler(new Vector3(0f, newYRotation, 0f));
    }

    private void OnAttack()
    {
        Instantiate(Projectile_Prefab, transform.position + Launch_Offset.position, transform.rotation);
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
        if(isActive)
        {
            rb.linearVelocity = new Vector2(direction * move_speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
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