using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class Inventory : MonoBehaviour
{
    [Header("References")] 
    public BlockPlacer blockPlacer; // The block placer script attached to the player
    public GameObject inventoryUI; // To toggle on and off when the inventory is opened and closed
    public GameObject itemSlotPrefab; // The prefab for the item slots that populate the inventory
    public Transform itemSlotContainer; // The container that holds all the item slots
    
    private bool isOpen = false;
    
    public readonly List<InventoryItemStack> stacks = new List<InventoryItemStack>();
    
    // See below method
    public ItemStack AddItem(string id, int quantity)
    {
        var item = ItemRegistry.GetItem(id);
        return AddItem(item, quantity);
    }
    
    // Adds an item to the inventory. Returns the stack that the item was added to
    public ItemStack AddItem(ItemDefinition item, int quantity)
    {
        if (item == null) return null;
        
        // If the item is already in the inventory, add to the stack, otherwise
        // create a new stack
        
        var stack = stacks.Find(x => x.Item == item);
        if (stack == null)
        {
            stack = new InventoryItemStack(item, 0, itemSlotPrefab, itemSlotContainer);
            
            // Make the item slot select the stack for placing when clicked
            stack.Slot.Button.onClick.AddListener(() =>
            {
                blockPlacer.Select(stack);
                SetOpen(false);
            });

            stack.Slot.OnHoverEnter = TooltipManager.BeginHover;
            stack.Slot.OnHoverExit = TooltipManager.EndHover;
            
            stacks.Add(stack);
        }
        stack.Add(quantity);
        return stack;
    }
    
    // See below method
    public int RemoveItem(string id, int quantity)
    {
        var item = ItemRegistry.GetItem(id);
        return RemoveItem(item, quantity);
    }
    
    // Removes an item from the inventory. Returns the amount of items removed
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
    
    // See below method
    public int GetQuantity(string id)
    {
        var item = ItemRegistry.GetItem(id);
        return GetQuantity(item);
    }
    
    // Returns the amount of the given item in the inventory
    public int GetQuantity(ItemDefinition item)
    {
        var stack = stacks.Find(x => x.Item == item);
        return stack?.Quantity ?? 0;
    }
    
    // See below method
    public bool HasItem(string id, int quantity)
    {
        var item = ItemRegistry.GetItem(id);
        return HasItem(item, quantity);
    }
    
    // Returns true if the inventory has at least a certain amount of the given item
    public bool HasItem(ItemDefinition item, int quantity)
    {
        var stack = stacks.Find(x => x.Item == item);
        return stack != null && stack.Quantity >= quantity;
    }
    
    // Gets the item stack with the given item id
    public ItemStack GetItemStack(string id)
    {
        return stacks.Find(x => x.Item.ID == id);
    }
    
    // Apply a filter to every item slot in the inventory
    public void ApplyFilter(Func<ItemDefinition, bool> filter)
    {
        foreach (var stack in stacks)
        {
            stack.ApplyFilter(filter);
        }
    }
    
    // Clear the filter on every item slot in the inventory
    public void ClearFilter()
    {
        foreach (var stack in stacks)
        {
            stack.Slot.SetBright();
        }
    }

    // Make sure to add the items after the icons are all rendered, otherwise we'd get
    // a bunch of null (white) icons. Ideally this sort of thing would be done
    // in a dedicated loading phase, but since it's just a prototype and this is
    // a unique case, I'm just doing it here.
    void Start()
    {
        if (ItemRenderManager.IconsRendered)
        {
            Populate();
        }
        else
        {
            ItemRenderManager.OnIconsRendered += Populate;
        }
    }
    
    // Hardcoded inventory initialization for testing
    private void Populate()
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
        AddItem("slope", 15);
        blockPlacer.Select(GetItemStack("cubit0"));
    }

    void Update()
    {
        // Tab toggles the inventory
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetOpen(!isOpen);
        }
    }

    // Opens or closes the inventory
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
            TooltipManager.EndHover();
        }
    }
}