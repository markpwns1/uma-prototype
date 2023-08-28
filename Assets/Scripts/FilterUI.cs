using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script handles the 4 buttons in the inventory menu that select the filter
public class FilterUI : MonoBehaviour
{
    [Header("References")] 
    public Inventory inventory;
    public Button[] buttons; // A list of buttons 0-3, representing the size classes

    // Make sure that any freshly opened inventory has the filter cleared
    private void OnEnable()
    {
        ClearFilter();
        SelectButton(0); // Button zero should correspond to "any (no filter)" 
    }

    // Enable all buttons except the given one
    public void SelectButton(int button)
    {
        // Set all buttons to unselected
        foreach (var b in buttons)
        {
            b.interactable = true;
        }
        
        // Set the pressed button to selected
        buttons[button].interactable = false;

    }
    
    // Filter the inventory
    public void FilterBySize(string size)
    {
        inventory.ApplyFilter(item => item.SizeClass == size);
    }

    // Clear the filter
    public void ClearFilter()
    {
        inventory.ClearFilter();
    }
}
