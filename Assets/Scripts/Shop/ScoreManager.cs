using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public double eulers = 0;
    public ShopSystem shop1;
    public ShopSystem shop2;
    public double eps = 0;


    public Collider secondShopHitbox;

    public GameObject secondShopVisuals;


    public int secondShopUnlockThreshold = 100;

    
    private bool secondShopUnlocked = false;

    public TMP_Text coinText;  

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateCoinUI();
        StartCoroutine(GenerateIdleCoins());

        if (secondShopHitbox != null) secondShopHitbox.enabled = false;
        if (secondShopVisuals != null) secondShopVisuals.SetActive(false);
    }

    private IEnumerator GenerateIdleCoins()
    {
        while (true)
        {
            UpdateEps();
            yield return new WaitForSeconds(1f);
            eulers += eps;
            UpdateCoinUI();
            if (!secondShopUnlocked && eulers >= secondShopUnlockThreshold)
            {
                secondShopUnlocked = true;

                
                if (secondShopHitbox != null) secondShopHitbox.enabled = true;
                
                if (secondShopVisuals != null) secondShopVisuals.SetActive(true);
            }
        }
    }

    public void UpdateEps()
    {
        int producer1ct = shop2.upgrades[0].purchaseCount;
        int producer2ct = shop2.upgrades[1].purchaseCount;
        int producer3ct = shop2.upgrades[2].purchaseCount;
        int producer4ct = shop2.upgrades[3].purchaseCount;
        eps = ((0.1 * (1 + producer3ct) * producer1ct) + (0.5 * producer2ct)) * Mathf.Pow(2, producer4ct); ;
    }

    public bool TrySpendCoins(double amount)
    {
        if (eulers >= amount)
        {
            eulers -= amount;
            UpdateCoinUI();
            return true;
        }
        return false;
    }


    public void AddCoin(double amount = 1)
    {
        int upgradeAddCoin = shop1.upgrades[0].purchaseCount;
        int upgradeDoubleCoin = shop1.upgrades[1].purchaseCount;
        eulers += (amount + upgradeAddCoin) * (Mathf.Pow(2,upgradeDoubleCoin));
        UpdateCoinUI();
    }

    void UpdateCoinUI()
    {
        coinText.text = "Eulers: " + System.Math.Round(eulers).ToString();
    }
}
