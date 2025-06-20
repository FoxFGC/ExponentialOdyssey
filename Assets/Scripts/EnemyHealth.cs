using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    public event Action<double> OnDamaged;

    public double baseHealth = 100;                 
    private double currentHealth;
    private double maxHealth;

    public HealthBarUI healthBarPrefab;

    private HealthBarUI healthBarInstance;

    public bool IsDead { get; private set; }


    void Start()
    {
        IsDead = false;

        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform);

            healthBarInstance.transform.localRotation = Quaternion.identity;

            float yOffset = 0.9f;
            healthBarInstance.transform.localPosition = new Vector3(0f, yOffset, 0f);

            healthBarInstance.SetHealthFraction(1f);
        }
    }
    void Awake()
    {
        var shopObj = GameObject.FindGameObjectWithTag("Shop1");
        ShopSystem shop1 = shopObj.GetComponent<ShopSystem>();
        int upgradeDoubleCoin = shop1.upgrades[1].purchaseCount;
        currentHealth = baseHealth * Mathf.Pow(2, upgradeDoubleCoin);
        maxHealth = baseHealth * Mathf.Pow(2, upgradeDoubleCoin);
    }


    public void TakeDamage(double amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        OnDamaged?.Invoke(amount);

        if (healthBarInstance != null)
        {
            float frac = (float)(currentHealth / maxHealth);
            healthBarInstance.SetHealthFraction(frac);
        }


        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        IsDead = true;
        FindObjectOfType<SlimeSpawner>().NotifySlimeDied();
        ScoreManager.instance.AddCoin(1);
        Destroy(gameObject);
    }
}
