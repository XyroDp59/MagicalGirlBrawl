using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float move_speed = 7f;
    [SerializeField] private float jump_power = 17f;
    [SerializeField] public int nb_double_jump = 2;
    
    public SpriteRenderer childRenderer;
    private SpriteRenderer _renderer;
    private HealthSystem _healthSystem;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Movement _grabbed;
    public Transform grabbedTransform;
    
    private bool _charging;
    private bool _canThrow;
    public bool isActive = false;

    private readonly int _walkBoolHash = Animator.StringToHash("Walking");
    private readonly int _castTriggerHash = Animator.StringToHash("Cast");
    private readonly int _grabTrigHash = Animator.StringToHash("Grab");
    private readonly int _throwTrigHash = Animator.StringToHash("Throw");
    private readonly int _missedGrabTrigHash = Animator.StringToHash("MissedGrab");
    private readonly int _switchTriggerHash = Animator.StringToHash("Switch");
    
    private float _defaultYRotation;
    private float _direction = 0f;
    private float _chargedTime = 0f;
   
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        //_playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        grabState = GrabState.Normal;
        _healthSystem = GetComponent<HealthSystem>();
    }

    void Update()
    {
        if (grabState == GrabState.Grabbed)
        {
            _rb.AddForce(5*(grabbedTransform.position - transform.position), ForceMode2D.Force);
            return;
        }

        if (grabState == GrabState.Grabber && !isActive)
        {
            grabber.SetActive(false);
            _grabbed.grabState = GrabState.Normal;
            _canThrow = false;
            grabState = GrabState.Normal;
        }

        if (grabState == GrabState.Thrown)
        {
            return;
        }

        if (_charging && isActive)
        {
            _chargedTime += Time.deltaTime;
            chargingSmashParticles.gameObject.SetActive(true);
            chargingSmashParticles.Evaluate(_chargedTime);
            if ((_chargedTime > 3) || (_cancelSmash))
            {
                Smash();
                _charging = false;
                _cancelSmash = false;
            }
        }
        if (isActive)
        {
            _rb.linearVelocity = new Vector2(_direction * move_speed, _rb.linearVelocity.y);
        }
        else
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        }
    }

    #region Misc

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

    public void OnMove(InputAction.CallbackContext context)
    {

        if (!isActive || _charging) return;

        _direction = context.ReadValue<Vector2>().x;

        if (grabState == GrabState.Grabber && _direction != 0 && _canThrow)
        {
            StartCoroutine(Throw(_direction));
        }

        if (grabState != GrabState.Normal && grabState != GrabState.Grabber) return;

        _animator.SetBool(_walkBoolHash, _direction != 0);
        float newYRotation = transform.rotation.eulerAngles.y;
        if (_direction < 0) newYRotation = -180;
        if (_direction > 0) newYRotation = 0;
        transform.rotation = Quaternion.Euler(new Vector3(0f, newYRotation, 0f));
    }

    #endregion

    #region Switch Animation

    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (!context.started || grabState != GrabState.Normal) return;
        if (!isActive) return;
        _animator.SetTrigger(_switchTriggerHash);
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        if (!context.started || grabState != GrabState.Normal) return;
        if (!isActive) return;
        _animator.SetTrigger(_switchTriggerHash);
    }


    #endregion

    #region jump and dbJump
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started || grabState != GrabState.Normal) return;
        if (!isActive) return;
        if (nb_double_jump <= 0) return;
        nb_double_jump = nb_double_jump - 1;
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jump_power);
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

    #endregion

    #region grab
    [Header("Grab")]
    [SerializeField] private GameObject grabber;
    [SerializeField] private Vector2 throwStrength;
    [SerializeField] private float grabRange;

    public GrabState grabState;
    public enum GrabState
    {
        Normal,
        Grabbed,
        Thrown,
        Grabber
    }

    public void OnGrab(InputAction.CallbackContext context)
    {
        if(!context.started || grabState != GrabState.Normal) return;
        if(!isActive) return;
        TryGrab();
    }
    
    private void TryGrab()
    {
        _animator.SetTrigger(_grabTrigHash);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, grabRange);
        Debug.DrawRay(transform.position, transform.right * grabRange, Color.red, 1f);
        
        if(hit.transform && hit.transform.gameObject.layer == 8)
        {
            grabState = GrabState.Grabber;
            Movement grabbedMovement = hit.transform.GetComponent<Movement>();
            grabbedMovement.grabState = GrabState.Grabbed;
            grabbedMovement.grabbedTransform = grabber.transform;
            _grabbed = grabbedMovement;
            grabber.SetActive(true);
            StartCoroutine(ThrowDelay());
        }
        else
        {
            grabState = GrabState.Normal;
            Debug.Log("here");
            _animator.SetTrigger(_missedGrabTrigHash);
        }
    }
    #endregion

    #region throw


    private IEnumerator ThrowDelay()
    {
        yield return new WaitForSeconds(0.3f);
        _canThrow = true;
    }

    public IEnumerator GetThrown(float direction)
    {
        grabState = GrabState.Thrown;
        _rb.linearVelocity = new Vector2(direction > 0 ? throwStrength.x : -throwStrength.x, throwStrength.y);
        yield return new WaitForSeconds(1f);
        grabState = GrabState.Normal;
    }

    private IEnumerator Throw(float direction)
    {
        _animator.SetTrigger(_throwTrigHash);
        yield return new WaitForSeconds(0.2f);
        grabber.SetActive(false);
        StartCoroutine(_grabbed.GetThrown(direction));
        yield return new WaitForSeconds(0.3f);
        _canThrow = false;
        grabState = GrabState.Normal;
    }

    #endregion

    #region Blaster
    [Header("Blaster")]
    [SerializeField] private ProjectileBehaviour Projectile_Prefab;
    [SerializeField] private Transform Launch_Offset;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.started || grabState != GrabState.Normal) return;
        if (!isActive) return;
        _animator.SetTrigger(_castTriggerHash);
        Instantiate(Projectile_Prefab, Launch_Offset.position, transform.rotation);
        ProjectileBehaviour p = Instantiate(Projectile_Prefab, Launch_Offset.position, transform.rotation);
    }

    #endregion

    #region Smash
    [Header("Smash")]
    [SerializeField] private Area_of_Attack Smash_Prefab;
    [SerializeField] private ChargingParticle chargingSmashParticles;

    private bool _cancelSmash;

    public void OnSmash(InputAction.CallbackContext context)
    {
        if (grabState != GrabState.Normal) return;
        _charging = true;
    }

    public void OnSmashRelease(InputAction.CallbackContext context)
    {
        if (grabState != GrabState.Normal) return;
        _cancelSmash = true;
    }

    private void Smash()
    {
        if (!isActive) return;
        Area_of_Attack a = Instantiate(Smash_Prefab, Launch_Offset.position, transform.rotation);
        a.charged_time = _chargedTime;
        chargingSmashParticles.gameObject.SetActive(false);
        _chargedTime = 0f;
    }


    #endregion

    #region collisions and triggers
    
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
    #endregion
}