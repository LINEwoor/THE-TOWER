using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }
    
    [SerializeField] private int upgradesToShow = 3;
    
    [SerializeField] private List<UpgradeData> allUpgrades;
    
    private List<UpgradeData> currentUpgrades = new List<UpgradeData>();
    
    public UnityEvent<List<UpgradeData>> OnUpgradesReady;
    public UnityEvent OnUpgradeSelected;

    public PlayerUpgrades upgrades;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //upgrades = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUpgrades>();
    }

    public void GenerateRandomUpgrades()
    {
        currentUpgrades.Clear();
    
        if (allUpgrades == null || allUpgrades.Count == 0)
        {
            OnUpgradesReady?.Invoke(currentUpgrades);
            return;
        }
    
        if (allUpgrades.Count < upgradesToShow)
        {
            List<UpgradeData> availableUpgrades = new List<UpgradeData>();
            while (availableUpgrades.Count < upgradesToShow)
            {
                int randomIndex = Random.Range(0, allUpgrades.Count);
                availableUpgrades.Add(allUpgrades[randomIndex]);
            }
        
            for (int i = 0; i < upgradesToShow; i++)
            {
                currentUpgrades.Add(availableUpgrades[i]);
            }
        }
        else
        {
            List<UpgradeData> availableUpgrades = new List<UpgradeData>(allUpgrades);
        
            for (int i = 0; i < upgradesToShow; i++)
            {
                if (availableUpgrades.Count == 0)
                    break;
                
                int index = Random.Range(0, availableUpgrades.Count);
                currentUpgrades.Add(availableUpgrades[index]);
                availableUpgrades.RemoveAt(index);
            }
        }
    
        OnUpgradesReady?.Invoke(currentUpgrades);
    }
    
    public void GenerateRandomUpgradesShuffle()
    {
        currentUpgrades.Clear();
        
        if (allUpgrades == null || allUpgrades.Count == 0)
        {
            OnUpgradesReady?.Invoke(currentUpgrades);
            return;
        }
        
        List<UpgradeData> shuffled = allUpgrades.OrderBy(x => Random.value).ToList();
        
        for (int i = 0; i < Mathf.Min(upgradesToShow, shuffled.Count); i++)
        {
            currentUpgrades.Add(shuffled[i]);
        }
        
        OnUpgradesReady?.Invoke(currentUpgrades);
    }
    
    public void SelectUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null) return;
    
        ApplyEffect(upgrade.idEffect, upgrade.value);
    
        Debug.Log($"Выбран апгрейд: {upgrade.displayName}");
        OnUpgradeSelected?.Invoke();
    
        // currentUpgrades.Remove(upgrade);
    }
    
    void ApplyEffect(string effectId, float value)
    {
        switch (effectId)
        {
            case "health":
                upgrades?.AddHealth(value);
                break;
            case "damage":
                upgrades?.AddDamage(value);
                Debug.LogWarning($"+урон: {value}");
                break;
            case "range":
                upgrades?.AddRange(value);
                break;
            default:
                Debug.LogWarning($"Неизвестный эффект: {effectId}");
                break;
        }
    }
    
    public List<UpgradeData> GetCurrentUpgrades()
    {
        return new List<UpgradeData>(currentUpgrades);
    }
}