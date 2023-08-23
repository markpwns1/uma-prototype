using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

// The item registry is a static class that loads, holds, and manages all the item definitions
public class ItemRegistry : MonoBehaviour
{
    // The folder that contains all the item definition files
    private const string ItemDefinitionsPath = "Item Definitions";
    
    private static readonly List<ItemDefinition> Items = new List<ItemDefinition>();

    // Loads all the item definition json files in the item definitions folder
    private static void LoadAll()
    {
        var files = Resources.LoadAll<TextAsset>(ItemDefinitionsPath);

        foreach (var file in files)
        {
            var item = JsonConvert.DeserializeObject<ItemDefinition>(file.text);
            if (item == null)
            {
                Debug.LogError("Failed to load item definition: " + file.name);
            }
            else
            {
                // Load the item's icon and placement prefab immediately
                item.LoadResources();
                Items.Add(item);
            }
        }
        
        Debug.Log("Loaded " + Items.Count + " items.");
    }

    // Returns the item definition with the given id
    public static ItemDefinition GetItem(string id)
    {
        var found = Items.Find(x => x.ID == id);
        if (found == null)
        {
            Debug.LogError("Item not found: " + id);
        }
        return found;
    }

    // Initialise the registry on awake
    void Awake()
    {
        LoadAll();
    }
}