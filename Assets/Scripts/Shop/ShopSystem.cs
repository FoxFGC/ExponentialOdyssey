using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopSystem : MonoBehaviour
{
    
    public GameObject shopPanel;          
    public Transform itemsContainer;      
    public GameObject itemButtonPrefab;    

    
    public List<UpgradeData> upgrades = new List<UpgradeData>();
    public BoxCollider producerSpawnArea;
    public MonoBehaviour[] controlScripts;

    public GameObject bushPrefab;
    public GameObject flowerPrefab;
    public GameObject mushroomPrefab;
    public GameObject treePrefab;

    void Awake()
    {
        // Hide shop UI initially
        if (shopPanel != null)
            shopPanel.SetActive(false);


        PopulateShop();
    }

    private void SpawnProducer(GameObject producerPrefab)
    {
        if (producerSpawnArea != null)
        {

            Bounds b = producerSpawnArea.bounds;


            float x = UnityEngine.Random.Range(b.min.x, b.max.x);
            float y = 0;
            float z = UnityEngine.Random.Range(b.min.z, b.max.z);

            Vector3 spawnPos = new Vector3(x, y, z);

            Instantiate(producerPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("ProducerSpawnArea not set on " + name);
        }
    }


    public void OpenShop()
    {
        shopPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
   
        foreach (var s in controlScripts) s.enabled = false;
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        foreach (var s in controlScripts) s.enabled = true;
    }

 
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

  
    private void UpdateButton(TMP_Text label, UpgradeData data)
    {
        label.text = $"{data.upgradeName} (Bought: {data.purchaseCount})\nCost: {data.cost} Eulers";
    }
 
    private void OnBuy(UpgradeData data, TMP_Text label)
    {
     
        if (ScoreManager.instance.TrySpendCoins(data.cost))
        {
            // Track number of times purchased
            data.purchaseCount++;
            data.cost = Math.Round(data.cost * data.costModifier);
            UpdateButton(label, data);

            if (data.upgradeType == UpgradeType.Flower)
            {
                SpawnProducer(flowerPrefab);
            }
            if (data.upgradeType == UpgradeType.Bush)
            {
                SpawnProducer(bushPrefab);
            }
            if (data.upgradeType == UpgradeType.Mushroom)
            {
                SpawnProducer(mushroomPrefab);
            }
            if (data.upgradeType == UpgradeType.Tree)
            {
                SpawnProducer(treePrefab);
            }
        }
    }

    [Serializable]
    public class UpgradeData
    {
        public string upgradeName;
        public double cost;
        public double costModifier;
        public UpgradeType upgradeType;


        [HideInInspector]
        public int purchaseCount = 0; 
    }

    public enum UpgradeType
    {
        CoinPerKill,
        AttackDamage,
        EulersPerKillDouble,
        AttackSpeed,
        Flower,
        Bush,
        Mushroom,
        Tree,
    }
}
