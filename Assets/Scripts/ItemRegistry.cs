using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ItemRegistry : MonoBehaviour
{
    private const string ItemDefinitionsPath = "Item Definitions";
    
    private static readonly List<ItemDefinition> Items = new List<ItemDefinition>();

    private static void Init()
    {
        var files = Resources.LoadAll<TextAsset>(ItemDefinitionsPath);

        foreach (var file in files)
        {
            // var item = JsonUtility.FromJson<ItemDefinition>(file.text);
            var item = JsonConvert.DeserializeObject<ItemDefinition>(file.text);
            item.LoadResources();
            Items.Add(item);
        }
        
        Debug.Log("Loaded " + Items.Count + " items.");
    }

    public static ItemDefinition GetItem(string id)
    {
        var found = Items.Find(x => x.ID == id);
        if (found == null)
        {
            Debug.LogError("Item not found: " + id);
        }
        return found;
    }

    void Awake()
    {
        Init();
    }
}