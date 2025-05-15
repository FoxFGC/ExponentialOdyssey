using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This attribute makes it easy to create the asset from the Project window
[CreateAssetMenu(
    fileName = "New Upgrade Item",
    menuName = "Shop/Upgrade Item",
    order = 1)]
public class UpgradeItem : ScriptableObject
{
    [Header("Display")]
    public string upgradeName;
    //public Sprite icon;                   // optional icon for UI

    [Header("Cost & Effect")]
    public int cost;
    public int extraCoinsPerKill;

    [HideInInspector]
    public bool isPurchased = false;
}
