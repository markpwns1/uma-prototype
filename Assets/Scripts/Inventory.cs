using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class Inventory : MonoBehaviour
{
    [Header("References")] 
    public BlockPlacer blockPlacer;
    public GameObject inventoryUI;
    public GameObject itemSlotPrefab;
    public Transform itemSlotContainer;
    
    private bool isOpen = false;
    
    public readonly List<InventoryItemStack> stacks = new List<InventoryItemStack>();
    
    public ItemStack AddItem(string id, int quantity)
    {
        var item = ItemRegistry.GetItem(id);
        return AddItem(item, quantity);
    }
    
    public ItemStack AddItem(ItemDefinition item, int quantity)
    {
        var stack = stacks.Find(x => x.Item == item);
        if (stack == null)
        {
            stack = new InventoryItemStack(item, 0, itemSlotPrefab, itemSlotContainer);
            stack.Slot.GetComponent<Button>().onClick.AddListener(() =>
            {
                blockPlacer.Select(stack);
                SetOpen(false);
            });
            
            stacks.Add(stack);
        }
        stack.Add(quantity);
        return stack;
    }
    
    public int RemoveItem(string id, int quantity)
    {
        var item = ItemRegistry.GetItem(id);
        return RemoveItem(item, quantity);
    }
    
    public int RemoveItem(ItemDefinition item, int quantity)
    {
        var stack = stacks.Find(x => x.Item == item);
        if (stack == null)
        {
            return 0;
        }
        
        var removed = stack.Remove(quantity);
        if (stack.Quantity <= 0)
        {
            stack.Destroy();
            stacks.Remove(stack);
        }
        
        return removed;
    }
    
    public int GetQuantity(string id)
    {
        var item = ItemRegistry.GetItem(id);
        return GetQuantity(item);
    }
    
    public int GetQuantity(ItemDefinition item)
    {
        var stack = stacks.Find(x => x.Item == item);
        return stack?.Quantity ?? 0;
    }
    
    public bool HasItem(string id, int quantity)
    {
        var item = ItemRegistry.GetItem(id);
        return HasItem(item, quantity);
    }
    
    public bool HasItem(ItemDefinition item, int quantity)
    {
        var stack = stacks.Find(x => x.Item == item);
        return stack != null && stack.Quantity >= quantity;
    }
    
    public ItemStack GetItemStack(string id)
    {
        return stacks.Find(x => x.Item.ID == id);
    }

    void Start()
    {
        AddItem("cubit0", 5);
        AddItem("cubit1", 50);
        AddItem("cubit2", 50);
        AddItem("cubit3", 50);
        AddItem("wall0", 30);
        AddItem("floor1", 30);
        AddItem("block2", 20);
        AddItem("pole3", 25);
        AddItem("weird", 4);
        AddItem("cross", 5);
        AddItem("donut", 3);
        blockPlacer.Select(GetItemStack("cubit0"));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetOpen(!isOpen);
        }
    }

    public void SetOpen(bool open)
    {
        isOpen = open;
            
        inventoryUI.SetActive(isOpen);

        if (isOpen)
        {
            MouseLockManager.TakeMouse();
        }
        else
        {
            MouseLockManager.ReleaseMouse();
        }
    }
}