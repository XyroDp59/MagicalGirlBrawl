using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    [SerializeField] private RectTransform healthBar;
    private int currentHealth;

    UnityEvent TotemDeath;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void addHealth(int health)
    {
        currentHealth = Mathf.Clamp(currentHealth + health, 0, maxHealth);
        float f = (float)currentHealth / ((float)maxHealth);
        healthBar.anchorMax = new Vector2(f, 1);
        if (currentHealth <= 0)
        {
            TotemDeath.Invoke();    
        }
    }
}