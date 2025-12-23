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
        stats.MaxHp += amount;
        Debug.Log($"Добавлено {amount} здоровья");
    }
    
    public void AddDamage(float amount)
    {
        stats.Dmg += amount;
        Debug.Log($"Добавлено {amount} урона");
    }
    
    public void AddRange(float amount)
    {
        stats.Range += amount;
        Debug.Log($"Добавлено {amount} дальности");
    }
}