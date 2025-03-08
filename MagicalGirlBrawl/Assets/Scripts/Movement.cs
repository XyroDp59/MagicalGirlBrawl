using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private ProjectileBehaviour Projectile_Prefab;
    [SerializeField] private Area_of_Attack Smash_Prefab;
    [SerializeField] private Transform Launch_Offset;
    [SerializeField] private GameObject grabber;
    [SerializeField] private float move_speed = 7f;
    [SerializeField] private float jump_power = 17f;
    [SerializeField] private Vector2 throwStrength;
    [SerializeField] public int nb_double_jump = 2;
    
    public SpriteRenderer childRenderer;
    private SpriteRenderer _renderer;
    private HealthSystem _healthSystem;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Movement _grabbed;
    
    private bool _charging;
    private bool _canThrow;
    public bool isActive = false;

    private int _walkBoolHash = Animator.StringToHash("Walking");
    private int _castTriggerHash = Animator.StringToHash("Cast");
    
    private float _defaultYRotation;
    private float _direction = 0f;
    private float _chargedTime = 0f;
   
    
    [SerializeField] private ChargingParticle chargingSmashParticles;

    private GrabState _grabState;

    private enum GrabState
    {
        Normal,
        Grabbed,
        Thrown,
        Grabber
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        //_playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _grabState = GrabState.Normal;
        _healthSystem = GetComponent<HealthSystem>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started || _grabState != GrabState.Normal) return;
        if(!isActive) return;
        if (nb_double_jump <= 0) return;
        nb_double_jump = nb_double_jump - 1;
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jump_power);
    }

    public void SetState(bool state)
    {
        isActive = state;
        childRenderer.enabled = !state;
        _renderer.enabled = state;
        if (!state)
        {
            _animator.SetBool(_walkBoolHash, false);
            _direction = 0;
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

    private void Grab(Movement grabbed)
    {
        _grabState = GrabState.Grabber;
        grabber.SetActive(false);
        _grabbed = grabbed;
        StartCoroutine(ThrowDelay());
    }

    private IEnumerator ThrowDelay()
    {
        yield return new WaitForSeconds(0.3f);
        _canThrow = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<Movement>().Grab(this);
            _grabState = GrabState.Grabbed;
        }
    }

    public IEnumerator GetThrown(float direction)
    {
        _grabState = GrabState.Thrown;
        _rb.linearVelocity = new Vector2(direction * throwStrength.x, throwStrength.y);
        yield return new WaitForSeconds(1f);
        _grabState = GrabState.Normal;
    }

    private IEnumerator Throw(float direction)
    {
        StartCoroutine(_grabbed.GetThrown(direction));
        yield return new WaitForSeconds(0.3f);
        _canThrow = false;
        _grabState = GrabState.Normal;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        
        if(!isActive) return;
        
        _direction = context.ReadValue<Vector2>().x;

        if (_grabState == GrabState.Grabber && _direction != 0)
        {
            StartCoroutine(Throw(_direction));
        }

        if (_grabState != GrabState.Normal) return;
        
        _animator.SetBool(_walkBoolHash, _direction != 0);
        float newYRotation = transform.rotation.eulerAngles.y;
        if (_direction < 0) newYRotation = - 180;
        if (_direction > 0) newYRotation = 0;
        transform.rotation = Quaternion.Euler(new Vector3(0f, newYRotation, 0f));
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.started || _grabState != GrabState.Normal) return;
        if (!isActive) return;
        _animator.SetTrigger(_castTriggerHash);
        Instantiate(Projectile_Prefab, Launch_Offset.position, transform.rotation);
        ProjectileBehaviour p = Instantiate(Projectile_Prefab, Launch_Offset.position, transform.rotation);
    }
    private void Smash()
    {
        if (!isActive) return;
        Area_of_Attack a = Instantiate(Smash_Prefab, Launch_Offset.position, transform.rotation);
        a.charged_time = _chargedTime;
        _chargedTime = 0f;
    }
    public void OnSmash(InputAction.CallbackContext context)
    {
        if (_grabState != GrabState.Normal) return;
        _charging = true;
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
        if (_grabState == GrabState.Grabbed)
        {
            return;
        }
        
        if (_charging)
        {
            _chargedTime += Time.deltaTime;
            chargingSmashParticles.Evaluate(charged_time);
            if ((_chargedTime > 3)/*||()*/)
            {
                Smash();
                _charging = false;
            }
        }
        if(isActive)
        {
            _rb.linearVelocity = new Vector2(_direction * move_speed, _rb.linearVelocity.y);
        }
        else
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
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