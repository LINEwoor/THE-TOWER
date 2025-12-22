using TMPro;
using UnityEngine;

public class TestStats : MonoBehaviour
{
    PlayerStats stats;
    TextMeshProUGUI text;
    
    void Start()
    {
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        text = GetComponent<TextMeshProUGUI>();
        UpdateUI();
        
        UpgradeManager.Instance.OnUpgradeSelected.AddListener(UpdateUI);
    }
    

    void UpdateUI()
    {
        if (stats != null && text != null)
        {
            text.text = $"Max hp: {stats.maxHp} \n" +
                        $"Range: {stats.range} \n" +
                        $"Damage: {stats.dmg}";
        }
        else
        {
            stats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
        }
    }
}