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

    private int currentHealth;
    private SpriteRenderer _spriteRenderer;
    public UnityEvent<Movement> TotemDeath;
    Player player;

    private void Start()
    {
        currentHealth = maxHealth;
        player = transform.parent.GetComponent<Player>();
        UnityAction<Movement> rm = player.RemoveMovement;
        TotemDeath.AddListener(rm);
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color c)
    {
        fillImage.color = c;
    }

    public void addHealth(int health)
    {
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
        Color defaultColor = _spriteRenderer.color;
        while (timer < length)
        {
            timer += Time.deltaTime;
            _spriteRenderer.color = Color.Lerp(defaultColor, Color.white, hitCurve.Evaluate(timer / length)); 
            yield return null;
        }
    }
}