using System;
using UnityEngine;

[Serializable]
public enum RARITY
{
    Common,
    Rare,
    Epic,
    Legend
}

[Serializable]
public enum ResourceType
{
    Food,
    Wood,
    Stone,
    Gold,
    None
}

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/Upgrade Data")]
[Serializable]
public class UpgradeData : ScriptableObject
{
    [Header("Basic Info")]
    public string displayName;
    public string description;
    public Sprite icon;
    public RARITY rarity;
    
    [Header("Effect")]
    public string idEffect;
    public float value;
    
    public ResourceType resourceType = ResourceType.None;
    public int costAmount = 0;
    
    public bool CanBuy(object resourceManager = null)
    {
        return true;
    }
    
    public bool Purchase(object resourceManager = null)
    {
        return true;
    }
    
    public string GetCostString()
    {
        if (resourceType == ResourceType.None || costAmount <= 0)
            return "Free";
        
        return $"{costAmount} {resourceType}";
    }
}