using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// A UI element that displays an item stack
public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public Text amountLabel;
    public RawImage iconImage;
    
    public Button Button { get; private set; }

    public ItemStack Stack { get; private set; } // The item stack to display
    
    public Action<ItemSlotUI> OnHoverEnter { get; set; }
    public Action OnHoverExit { get; set; }

    public void Init()
    {
        Button = GetComponent<Button>();
    }
    
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
        iconImage.texture = Stack.Item.Icon;
    }

    // Sets the icon to half opacity
    public void SetDark()
    {
        iconImage.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        amountLabel.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    }
    
    // Sets the icon to full opacity
    public void SetBright()
    {
        iconImage.color = new Color(1, 1, 1, 1);
        amountLabel.color = new Color(1, 1, 1, 1);
    }
    
    // Events for hovering, accessible by other scripts
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit?.Invoke();
    }
}