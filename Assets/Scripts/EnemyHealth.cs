using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;                  // Starting health
    private int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    // Called by PlayerAttack when hit
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        // Play death animation or effects here
        ScoreManager.instance.AddCoin(1);
        Destroy(gameObject);
    }
}
