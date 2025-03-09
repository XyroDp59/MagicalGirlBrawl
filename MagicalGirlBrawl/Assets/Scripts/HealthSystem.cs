using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private Image fillImage;
    [SerializeField] private AnimationCurve hitCurve;
    [SerializeField] private SpriteRenderer totemRenderer;

    private int currentHealth;
    private SpriteRenderer _spriteRenderer;
    public UnityEvent<Movement> TotemDeath;
    private Color _defaultRendererColorColor;
    private Color _defaultTotemColor;
    Player player;
    
    // --- KAILY Audio ---------
    private FMOD.Studio.EventInstance _hitInstance;

    private void Start()
    {
        currentHealth = maxHealth;
        player = transform.parent.GetComponent<Player>();
        UnityAction<Movement> rm = player.RemoveMovement;
        TotemDeath.AddListener(rm);
        
        _hitInstance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Hit");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultRendererColorColor = _spriteRenderer.color;
        _defaultTotemColor = totemRenderer.color;
    }

    public void SetColor(Color c)
    {
        fillImage.color = c;
    }

    public void addHealth(int health)
    {
        if (health < 0) _hitInstance.start();
        
        currentHealth = Mathf.Clamp(currentHealth + health, 0, maxHealth);
        float f = (float)currentHealth / ((float)maxHealth);
        StartCoroutine(FlashHit());
        fillImage.rectTransform.anchorMax = new Vector2(f, 1);
        if (currentHealth <= 0)
        {
            TotemDeath.Invoke(GetComponent<Movement>());    
        }
    }
    
    public IEnumerator FlashHit()
    {
        var length = hitCurve.keys[^1].time;
        var timer = 0f;
        while (timer < length)
        {
            timer += Time.deltaTime;
            _spriteRenderer.color = Color.Lerp(_defaultRendererColorColor, Color.black, hitCurve.Evaluate(timer / length));
            totemRenderer.color = Color.Lerp(_defaultTotemColor, Color.black, hitCurve.Evaluate(timer / length));
            yield return null;
        }
    }
}