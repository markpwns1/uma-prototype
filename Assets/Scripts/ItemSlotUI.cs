using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [Header("References")]
    public Text amountLabel;
    public Image iconImage;
    
    public ItemStack Stack { get; private set; }

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

    public void UpdateAmount()
    {
        if (Stack == null) return;
        amountLabel.text = Stack.Quantity.ToString();
    }
    
    public void UpdateIcon()
    {
        if (Stack == null) return;
        iconImage.sprite = Stack.Item.Icon;
    }
}