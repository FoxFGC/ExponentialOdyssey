using UnityEngine;
using TMPro;          // or TMPro if using TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int coins = 0;
    public ShopSystem shopSystem;

    [Header("Hook your UI Text here")]
    public TMP_Text coinText;         // Or TMP_Text if using TextMeshPro

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateCoinUI();
    }

    public bool TrySpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateCoinUI();
            return true;
        }
        return false;
    }


    public void AddCoin(int amount = 1)
    {
        int upgradeAddCoin = shopSystem.upgrades[0].purchaseCount;
        coins += amount + upgradeAddCoin;
        UpdateCoinUI();
    }

    void UpdateCoinUI()
    {
        coinText.text = "Eulers: " + coins.ToString();
    }
}
