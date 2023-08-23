using UnityEngine;
using UnityEngine.UI;

// A UI element that displays an item stack
public class ItemSlotUI : MonoBehaviour
{
    [Header("References")]
    public Text amountLabel;
    public Image iconImage;
    
    public ItemStack Stack { get; private set; } // The item stack to display

    public void SetItemStack(ItemStack stack)
    {
        // Disable the slot if there is no stack on it
        amountLabel.gameObject.SetActive(stack != null);
        iconImage.gameObject.SetActive(stack != null);
        
        Stack = stack;
        
        if(stack == null) return;
        
        UpdateAmount();
        UpdateIcon();
    }

    // Updates the amount displayed in the item slot
    public void UpdateAmount()
    {
        if (Stack == null) return;
        amountLabel.text = Stack.Quantity.ToString();
    }
    
    // Updates the icon displayed in the item slot. Not usually called more than once
    public void UpdateIcon()
    {
        if (Stack == null) return;
        iconImage.sprite = Stack.Item.Icon;
    }
}