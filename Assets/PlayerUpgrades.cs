using System;
using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    PlayerStats stats;
    
    private void Start()
    {
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    public void AddHealth(float amount)
    {
        stats.maxHp += amount;
        Debug.Log($"Добавлено {amount} здоровья");
    }
    
    public void AddDamage(float amount)
    {
        stats.dmg += amount;
        Debug.Log($"Добавлено {amount} урона");
    }
    
    public void AddRange(float amount)
    {
        stats.range += amount;
        Debug.Log($"Добавлено {amount} дальности");
    }
}