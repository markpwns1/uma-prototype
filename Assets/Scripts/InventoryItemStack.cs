using System;
using UnityEngine;
using Object = UnityEngine.Object;

// A type of ItemStack that has a UI element (an item slot) associated with it
public class InventoryItemStack : ItemStack
{
    public ItemSlotUI Slot { get; private set; }
    
    // The constructor calls the base constructor, then instantiates the item slot
    public InventoryItemStack(ItemDefinition item, int quantity, GameObject slotPrefab, Transform slotParent) : base(item, quantity)
    {
        var slotObject = Object.Instantiate(slotPrefab, slotParent);
        Slot = slotObject.GetComponent<ItemSlotUI>();
        Slot.Init();
        Slot.SetItemStack(this);   
    }

    // The add and remove functions update the amount displayed in the item slot
    
    public override void Add(int quantity)
    {
        base.Add(quantity);
        Slot.UpdateAmount();
    }
    
    public override int Remove(int quantity)
    {
        var removed = base.Remove(quantity);
        Slot.UpdateAmount();
        return removed;
    }
    
    // Upon destruction of the stack, destroy the item slot UI element
    public override void Destroy()
    {
        Object.Destroy(Slot.gameObject);
    }
    
    // Applies a filter to the item slot, to highlight it if it passes the filter
    public void ApplyFilter(Func<ItemDefinition, bool> filter)
    {
        if(filter(Item)) Slot.SetBright();
        else Slot.SetDark();
    }
}