using UnityEngine;

public class InventoryItemStack : ItemStack
{
    public ItemSlotUI Slot { get; private set; }
    
    public InventoryItemStack(ItemDefinition item, int quantity, GameObject slotPrefab, Transform slotParent) : base(item, quantity)
    {
        var slotObject = Object.Instantiate(slotPrefab, slotParent);
        Slot = slotObject.GetComponent<ItemSlotUI>();
        Slot.SetItemStack(this);   
    }

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
    
    public override void Destroy()
    {
        Object.Destroy(Slot.gameObject);
    }
}