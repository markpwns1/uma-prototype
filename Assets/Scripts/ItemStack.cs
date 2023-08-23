
using UnityEngine;

// Represents a certain amount of an item
// The body of this class is pretty self-explanatory
public class ItemStack
{
    public ItemDefinition Item { get; }
    public int Quantity { get; private set; }
    
    public ItemStack(ItemDefinition item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }
    
    public virtual void Add(int quantity)
    {
        Quantity += quantity;
    }
    
    // Returns the amount removed
    public virtual int Remove(int quantity)
    {
        var toRemove = Mathf.Min(Quantity, quantity);
        Quantity -= toRemove;
        return toRemove;
    }

    public virtual void Destroy() { }
    
    public override string ToString()
    {
        return $"{Item.Name} ({Item.ID}) x{Quantity}";
    }
}


