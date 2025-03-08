using System;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    [SerializeField] private RectTransform healthBar;
    private int currentHealth;

    public UnityEvent<Movement> TotemDeath;
    Player player;

    private void Start()
    {
        currentHealth = maxHealth;
        player = transform.parent.GetComponent<Player>();
        UnityAction<Movement> rm = player.RemoveMovement;
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