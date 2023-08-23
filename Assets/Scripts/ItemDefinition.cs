using Newtonsoft.Json;
using UnityEngine;

// This class represents an item definition in the most abstract sense
public class ItemDefinition
{
    [JsonProperty("id")]
    public string ID { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("placementPrefabPath")]
    public string PlacementPrefabPath { get; set; }
    public GameObject PlacementPrefab { get; set; }
    
    [JsonProperty("iconPath")]
    public string IconPath { get; set; }
    public Sprite Icon { get; set; }

    // Loads the placement prefab and icon from the given paths
    public void LoadResources()
    {
        PlacementPrefab = Resources.Load<GameObject>(PlacementPrefabPath);
        Icon = Resources.Load<Sprite>(IconPath);
        
        if(!PlacementPrefab)
            Debug.LogError("PlacementPrefab not found: " + PlacementPrefabPath);
        
        if (!Icon)
            Debug.LogError("Icon not found: " + IconPath);
    }
}