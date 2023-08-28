using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script manages real-time and pre-generated item renders
// Realtime for hover, and pre-generated for icons.
public class ItemRenderManager : MonoBehaviour
{
    // Index of the layer that the render prefabs are on
    private const int RenderLayer = 6;
    public static ItemRenderManager Instance { get; private set; }
    
    // Called after all icons are rendered
    public static Action OnIconsRendered { get; set; }
    public static bool IconsRendered { get; private set; } = false;
    
    [Header("References")]
    public Transform renderParent; // Parent of the render prefabs
    public Camera renderCamera;

    [Header("Settings")] 
    public float rotationSpeed; // Degrees per second that the realtime render rotates
    
    // Dimensions of the real-time render texture
    public int renderWidth = 190;
    public int renderHeight = 190;
    
    // Dimensions of pregenerated icons
    public int iconWidth = 64;
    public int iconHeight = 64;

    public RenderTexture RenderTexture { get; private set; }
    
    // Cache of render prefabs for each item
    private readonly Dictionary<ItemDefinition, GameObject> _renderPrefabs = new Dictionary<ItemDefinition, GameObject>();

    private GameObject _currentRenderPrefab;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Generate the render texture and set it on the tooltip manager
        RenderTexture = new RenderTexture(renderWidth, renderHeight, 0);
        RenderTexture.Create();
        renderCamera.targetTexture = RenderTexture;
        TooltipManager.SetRenderTexture(RenderTexture);
        
        // Self-explanatory
        WarmupPrefabs();
        PrerenderIcons();
    }

    // Instantiates the render prefabs of every item and registers it in the cache
    private void WarmupPrefabs()
    {
        foreach (var item in ItemRegistry.AllItems)
        {
            // Instantiate the render prefab in the render layer
            var instance = Instantiate(item.RenderPrefab, item.RenderOffset, Quaternion.identity, renderParent);
            SetRenderLayer(instance);
            
            // Disable it and keep it stored for later
            instance.SetActive(false);
            _renderPrefabs.Add(item, instance);
        }
    }

    // Generates icons for every item that doesn't have a custom icon
    private void PrerenderIcons()
    {
        foreach (var item in ItemRegistry.AllItems)
        {
            if (!item.UseCustomIcon)
            {
                item.Icon = GenerateIcon(item);
            }
        }
        
        // Notify any listeners that all icons are rendered
        IconsRendered = true;
        OnIconsRendered?.Invoke();
    }
    
    // Returns a pre-generated icon texture for the given item
    private Texture GenerateIcon(ItemDefinition item)
    {
        // Take a picture of the item
        SetCurrentItemInstance(item);
        renderCamera.Render();
        
        // Copy the render texture to a new texture and return it
        var icon = new RenderTexture(iconWidth, iconHeight, 0);
        icon.Create();
        Graphics.Blit(RenderTexture, icon);
        return icon;
    }
    
    // Sets an object to the render layer recursively
    private void SetRenderLayer(GameObject obj)
    {
        obj.layer = RenderLayer;
        foreach (Transform child in obj.transform)
        {
            SetRenderLayer(child.gameObject);
        }
    }
    
    // See below
    public static void SetCurrentItem(ItemDefinition item)
    {
        Instance.SetCurrentItemInstance(item);
    }
    
    // Sets the current item for the realtime render
    private void SetCurrentItemInstance(ItemDefinition item)
    {
        if (!_renderPrefabs.ContainsKey(item))
        {
            Debug.LogError("Attempt to render item that has no render prefab: " + item.ID);
            return;
        }
        
        // Disable the previous render prefab and enable the new one
        if(_currentRenderPrefab) _currentRenderPrefab.SetActive(false);
        _currentRenderPrefab = _renderPrefabs[item];
        _currentRenderPrefab.SetActive(true);
        
        // Set the render camera's size as specified by the item
        renderCamera.orthographicSize = item.RenderCameraSize == 0? 1 : item.RenderCameraSize;
    }

    void Update()
    {
        // Rotate the current render prefab
        renderParent.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
