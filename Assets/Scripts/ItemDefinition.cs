using Newtonsoft.Json;
using UnityEngine;

// This class represents an item definition in the most abstract sense
public class ItemDefinition
{
    [JsonProperty("id")]
    public string ID { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("size")]
    public string SizeClass { get; set; }
    
    [JsonProperty("placementPrefabPath")]
    public string PlacementPrefabPath { get; set; }
    public GameObject PlacementPrefab { get; set; }
    
    [JsonProperty("renderPrefabPath")]
    public string RenderPrefabPath { get; set; }
    public GameObject RenderPrefab { get; set; }
    
    [JsonProperty("renderOffset")]
    public Vector3 RenderOffset { get; set; }
    [JsonProperty("renderCameraSize")]
    public float RenderCameraSize { get; set; }

    [JsonProperty("useCustomIcon")]
    public bool UseCustomIcon { get; set; }
    
    [JsonProperty("iconPath")]
    public string IconPath { get; set; }
    public Texture Icon { get; set; }
    

    // Loads the placement prefab and icon from the given paths
    public void LoadResources()
    {
        PlacementPrefab = Resources.Load<GameObject>(PlacementPrefabPath);
        RenderPrefab = Resources.Load<GameObject>(RenderPrefabPath);
        
        if(UseCustomIcon)
            Icon = Resources.Load<Texture>(IconPath);

        if(!PlacementPrefab)
            Debug.LogError("PlacementPrefab not found: " + PlacementPrefabPath);
        
        if(!RenderPrefab)
            Debug.LogError("RenderPrefab not found: " + RenderPrefabPath);
        
        if (UseCustomIcon && !Icon)
            Debug.LogError("Icon not found: " + IconPath);
    }
}