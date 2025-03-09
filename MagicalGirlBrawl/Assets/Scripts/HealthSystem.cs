using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private Image fillImage;

    private int currentHealth;
    public UnityEvent<Movement> TotemDeath;
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
        fillImage.rectTransform.anchorMax = new Vector2(f, 1);
        if (currentHealth <= 0)
        {
            TotemDeath.Invoke(GetComponent<Movement>());    
        }
    }
}