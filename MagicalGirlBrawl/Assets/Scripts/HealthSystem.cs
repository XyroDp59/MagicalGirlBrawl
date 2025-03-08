using System;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    [SerializeField] private RectTransform healthBar;
    private int currentHealth;

    public UnityEvent<Movement> TotemDeath;
    Switch switchSystem;

    private void Start()
    {
        currentHealth = maxHealth;
        switchSystem = transform.parent.GetComponent<Switch>();
        UnityAction<Movement> rm = switchSystem.RemoveMovement;
        TotemDeath.AddListener(rm);

    }
    public void addHealth(int health)
    {
        currentHealth = Mathf.Clamp(currentHealth + health, 0, maxHealth);
        float f = (float)currentHealth / ((float)maxHealth);
        healthBar.anchorMax = new Vector2(f, 1);
        if (currentHealth <= 0)
        {
            TotemDeath.Invoke(GetComponent<Movement>());    
        }
    }
}