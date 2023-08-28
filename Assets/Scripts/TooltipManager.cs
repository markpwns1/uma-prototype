using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Global inventory item tooltip manager
public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }
    
    [Header("References")] 
    public GameObject tooltip; // Tooltip object itself
    public Text itemNameLabel; 
    public Text itemQuantityLabel;
    public RawImage renderImage; // The image on which to display the realtime item render
    
    private RectTransform _tooltipTransform;
    private ItemSlotUI _currentSlot;
    private bool _isActive = false;
    
    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        _tooltipTransform = tooltip.GetComponent<RectTransform>();
    }
    
    // Sets the render texture on the tooltip. Used only once, to set it to
    // the realtime render texture.
    public static void SetRenderTexture(Texture texture)
    {
        Instance.renderImage.texture = texture;
    }
    
    // Sets the tooltip active with the hovered slot
    public static void BeginHover(ItemSlotUI hoveredSlot)
    {
        Instance.BeginHoverInstance(hoveredSlot);   
        // Make the render manager start rendering the hovered item
        ItemRenderManager.SetCurrentItem(hoveredSlot.Stack.Item); 
    }
    
    // See above
    private void BeginHoverInstance(ItemSlotUI hoveredSlot)
    {
        _currentSlot = hoveredSlot;
        _isActive = true;
        
        // Update the tooltip's text and image
        itemNameLabel.text = _currentSlot.Stack.Item.Name;
        itemQuantityLabel.text = _currentSlot.Stack.Quantity.ToString();
        
        tooltip.SetActive(true);
    }

    void Update()
    {
        if (!_isActive) return;
        
        // Move the tooltip to the mouse position
        var mousePosition = Input.mousePosition;
        _tooltipTransform.position = mousePosition + new Vector3(5, -5, 0);
    }
    
    // Hides the tooltip
    public static void EndHover()
    {
        Instance.EndHoverInstance();
    }
    
    // See above
    public void EndHoverInstance()
    {
        _isActive = false;
        tooltip.SetActive(false);
    }
}
