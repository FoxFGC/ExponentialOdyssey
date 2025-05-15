using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Decoupled ShopSystem: handles in-world shop UI and purchase counts.
/// Score management and effect application are done in separate scripts.
/// Attach this to a GameObject in your scene.
/// </summary>
public class ShopSystem : MonoBehaviour
{
    [Header("UI References")]
    public GameObject shopPanel;           // In-world shop panel UI
    public Transform itemsContainer;       // Container for shop item buttons
    public GameObject itemButtonPrefab;    // Prefab for shop item button

    [Header("Available Upgrades")]
    public List<UpgradeData> upgrades = new List<UpgradeData>();

    public MonoBehaviour[] controlScripts;

    void Awake()
    {
        // Hide shop UI initially
        if (shopPanel != null)
            shopPanel.SetActive(false);

        // Create buttons for each upgrade
        PopulateShop();
    }

    /// <summary>Show the shop UI.</summary>
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // 2) disable movement/look
        foreach (var s in controlScripts) s.enabled = false;
    }

    /// <summary>Hide the shop UI.</summary>
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // 3b) re-enable movement/look
        foreach (var s in controlScripts) s.enabled = true;
    }

    /// <summary>Instantiate and configure a button for each upgrade.</summary>
    private void PopulateShop()
    {
        foreach (var data in upgrades)
        {
            GameObject go = Instantiate(itemButtonPrefab, itemsContainer);

            TMP_Text label = go.GetComponentInChildren<TMP_Text>();
            if (label == null)
            {
                Debug.LogError("ShopItemButton prefab needs a TextMeshPro - Text (UI) child!");
                Destroy(go);
                continue;
            }

            Button btn = go.GetComponent<Button>();
            if (btn == null)
            {
                Debug.LogError("ShopItemButton prefab needs a Button component on the root!");
                Destroy(go);
                continue;
            }

            UpdateButton(label, data);
            btn.onClick.AddListener(() => OnBuy(data, label));
        }
    }

    /// <summary>Update button label based on current purchase count.</summary>
    private void UpdateButton(TMP_Text label, UpgradeData data)
    {
        label.text = $"{data.upgradeName} (Bought: {data.purchaseCount}) - {data.cost} coins";
    }

    /// <summary>Handle purchase: deduct coins, increment count, refresh label.</summary>
    private void OnBuy(UpgradeData data, TMP_Text label)
    {
        // Attempt to spend coins via ScoreManager (separate script)
        if (ScoreManager.instance.TrySpendCoins(data.cost))
        {
            // Track number of times purchased
            data.purchaseCount++;
            UpdateButton(label, data);

            // Other systems can read data.purchaseCount to apply effects
        }
    }

    [Serializable]
    public class UpgradeData
    {
        public string upgradeName;
        public int cost;
        public UpgradeType upgradeType;


        public int modifier;

        [HideInInspector]
        public int purchaseCount = 0;  // Tracks how many times this upgrade was bought
    }

    public enum UpgradeType
    {
        CoinPerKill
    }
}
