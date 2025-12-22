using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeSlotScript : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI costText;
    
    private UpgradeData myUpgradeData;
    private bool isSelected = false;
    
    public event Action<UpgradeData, GameObject> OnCardSelected;
    
    public void Setup(UpgradeData upgradeData)
    {
        myUpgradeData = upgradeData;
        
        if (iconImage != null) iconImage.sprite = upgradeData.icon;
        if (nameText != null) nameText.text = upgradeData.displayName;
        if (descriptionText != null) descriptionText.text = upgradeData.description;
        if (rarityText != null) rarityText.text = upgradeData.rarity.ToString();
        if (costText != null) costText.text = upgradeData.GetCostString();
    }
    
    private void OnCardClicked()
    {
        if (isSelected) return;
        
        if (UpgradeManager.Instance != null && myUpgradeData != null)
        {
            UpgradeManager.Instance.SelectUpgrade(myUpgradeData);
            
            isSelected = true;
            
            OnCardSelected?.Invoke(myUpgradeData, gameObject);
        }
    }
    
    private void OnMouseDown()
    {
        OnCardClicked();
    }
    
    private void OnDestroy()
    {
        OnCardSelected = null;
    }
}