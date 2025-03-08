using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{
    [SerializeField] private SpriteRenderer childRenderer;
    private SpriteRenderer _renderer;
    private HealthSystem _healthSystem;
    public bool isActive = false;
    private Rigidbody2D rb;
    private float direction = 0f;
    [SerializeField] private float move_speed = 7f;
    [SerializeField] private float jump_power = 17f;
    [SerializeField] public int nb_double_jump = 2;
    //private PlayerInput _playerInput;
    private int _walkBoolHash = Animator.StringToHash("Walking");
    private int _castTriggerHash = Animator.StringToHash("Cast");
    private Animator _animator;
    private float _defaultYRotation;
    private PlayerInput _playerInput;
    private InputAction smashAttack;
    [SerializeField] private ProjectileBehaviour Projectile_Prefab;
    [SerializeField] private Area_of_Attack Smash_Prefab;
    [SerializeField] private Transform Launch_Offset;
    [SerializeField] private GameObject grabber;
    private float charged_time = 0f;
    private bool charging = false;
    
    private GrabState _grabState;

    private enum GrabState
    {
        Normal,
        Grabbed,
        Grabber
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //_playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _grabState = GrabState.Normal;
        _healthSystem = GetComponent<HealthSystem>();
        smashAttack = InputSystem.actions.FindAction("Smash");
    }

    private void OnJump(InputValue value)
    {
        Debug.Log(value.isPressed);
        if(!isActive) return;
        if (nb_double_jump <= 0) return;
        nb_double_jump = nb_double_jump - 1;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump_power);
    }

    public void SetState(bool state)
    {
        isActive = state;
        childRenderer.enabled = !state;
        _renderer.enabled = state;
        if (!state)
        {
            _animator.SetBool(_walkBoolHash, false);
            direction = 0;
        }
        else
        {
            Reset_Double_Jump_Switch();
        }
    }

    private void OnGrab()
    {
        StartCoroutine(TryGrab());
    }

    private IEnumerator TryGrab()
    {
        grabber.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        grabber.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Debug.Log("gwabbed owo");
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
        if (!isActive) return;
        _animator.SetTrigger(_castTriggerHash);
        Instantiate(Projectile_Prefab, Launch_Offset.position, transform.rotation);
        ProjectileBehaviour p = Instantiate(Projectile_Prefab, Launch_Offset.position, transform.rotation);
    }
    private void Smash()
    {
        if (!isActive) return;
        Area_of_Attack a = Instantiate(Smash_Prefab, Launch_Offset.position, transform.rotation);
        a.charged_time = charged_time;
        charged_time = 0f;
    }
    private void OnSmash()
    {
        charging = true;
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
        if (charging)
        {
            charged_time += Time.deltaTime;
            if ((charged_time > 3)/*||()*/)
            {
                Smash();
                charging = false;
            }
        }
        if(isActive)
        {
            rb.linearVelocity = new Vector2(direction * move_speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (smashAttack.WasReleasedThisFrame())
        {
            Debug.Log("smash");
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Reset_Double_Jump_Ground();
        }

        if (other.gameObject.layer == 7)
        {
            _healthSystem.addHealth(-30);
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