using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BlockPlacer : MonoBehaviour
{
    // Cubit size, currently 40cm
    public const float CubitSize = 0.4f;

    // Layer mask that you're allowed to place blocks on
    public static int PlacingLayerMask { get; private set; }
    
    // Layer mask that blocks placement
    public static int BlockingLayerMask { get; private set; }
    
    [Header("References")]
    // The item slot in the bottom-centre of the screen that shows you what you're placing
    [FormerlySerializedAs("currentSlot")] public ItemSlotUI placingSlot;
    
    // Inventory reference
    public Inventory inventory;
    
    [Header("Settings")]
    
    // How quickly the block moves to the desired position (higher is faster)
    public float floatiness = 3.0f;
    
    // You can use the scroll wheel to change the distance that the block is
    // hovering at. The following are the minimum and maximum distances, as 
    // well as the sensitivity of the scroll wheel
    public float scrollSensitivity = 0.5f;
    public float minDistance = 1.0f;
    public float maxDistance = 10.0f;

    // The block prefab that is currently being placed (NOT instantiated yet)
    private BlockPlacementInfo _block; // Probably don't need this but oh well
    
    // The actually instantiated block preview that hovers in front of you
    private BlockPlacementInfo _blockPreview;
    
    private Vector3 _desiredPosition = Vector3.zero;
    private Quaternion _desiredRotation = Quaternion.identity;
    private float _distance = 5.0f;

    private Transform _camera;

    private bool _canPlace; // Whether or not the block can be placed at _desiredPosition

    // Start method -- self explanatory
    void Start()
    {
        PlacingLayerMask = LayerMask.GetMask("Connectible");
        BlockingLayerMask = LayerMask.GetMask("Default", "Connectible");
        
        _camera = Camera.main.transform;
    }

    // Selects the given item as the item to place
    public void Select(ItemStack item)
    {
        _block = item.Item.PlacementPrefab.GetComponent<BlockPlacementInfo>();
        if(_blockPreview) Destroy(_blockPreview.gameObject); // Destroy the existing block preview
        _blockPreview = Instantiate(_block).GetComponent<BlockPlacementInfo>(); // Instantiate a new one
        placingSlot.SetItemStack(item);
    }

    // Unselects the current placing item
    public void UnselectCurrentItem()
    {
        placingSlot.SetItemStack(null);
        _block = null;
        if(_blockPreview) Destroy(_blockPreview.gameObject); // Destroy the block preview
        _blockPreview = null;
    }
    
    // Place the block at the desired position and rotation, and remove 1 from the item stack
    void PlaceBlock()
    {
        var block = Instantiate(_block.blockPrefab, _desiredPosition, _desiredRotation);
        
        // Set the block's name to the item ID (that's how we know what item it is when the player later picks it up)
        block.name = placingSlot.Stack.Item.ID;
        inventory.RemoveItem(placingSlot.Stack.Item, 1);
        
        if (placingSlot.Stack.Quantity <= 0)
        {
            UnselectCurrentItem();
        }
        else
        {
            placingSlot.UpdateAmount();
        }
    }

    // Destroy a block and add 1 of them to the player's inventory
    void DestroyBlock(Transform block)
    {
        // Get the block's name (which is the item ID) and add it to the player's inventory
        var itemStack = inventory.AddItem(block.name, 1);
        
        placingSlot.UpdateAmount();
        Destroy(block.gameObject);

        // If there's no item currently selected to place, just make that one the selected item
        if (placingSlot.Stack == null && itemStack != null)
        {
            Select(itemStack);
        }
    }
    
    void Update()
    {
        // Block destroying logic with right-click
        if (MouseLockManager.IsMouseLocked && Input.GetMouseButtonDown(1))
        {
            // Note: static objects cannot be destroyed
            if(Physics.Raycast(_camera.position, _camera.forward, out var hit, _distance + 1.0f, PlacingLayerMask) && !hit.collider.gameObject.CompareTag("Indestructible"))
            {
                DestroyBlock(hit.transform.root);
                return;
            }
        }
        
        // Block placing logic
        if(!_block) return;
        
        _canPlace = false;

        // Adjust the distance in front of the camera that the block should be
        _distance = Mathf.Clamp(_distance + Input.mouseScrollDelta.y * scrollSensitivity, minDistance, maxDistance);
        
        // Test if the block can be placed at the position it would theoretically be at
        var potentialPosition = SnapToGrid(_camera.position + _camera.forward * _distance);
        if (_blockPreview.CanPlaceAt(potentialPosition, _desiredRotation))
        {
            _desiredPosition = potentialPosition;
            _canPlace = true;
        }
        else
        {
            _desiredPosition = _camera.position + _camera.forward * _distance;
        }

        // Pressing R rotates the block
        if (Input.GetKeyDown(KeyCode.R))
        {
            _desiredRotation *= Quaternion.Euler(0, 90, 0);
        }
        
        // Set the shader info on the block preview according to whether or not it can be placed
        _blockPreview.SetShaderInfo(1.0f, _canPlace? 1 : 0);

        // From here onwards, lerp the block preview's position and rotation to the desired position and rotation
        
        _blockPreview.transform.rotation = Quaternion.Lerp(_blockPreview.transform.rotation, _desiredRotation, floatiness * Time.deltaTime);
        
        if (_canPlace)
        {
            // If it can be placed, just snap the position directly
            _blockPreview.transform.position = _desiredPosition;
            
            // If it can be placed, there's no UI open, and the player clicks, place the block
            if (MouseLockManager.IsMouseLocked && Input.GetMouseButtonDown(0))
            {
                PlaceBlock();
            }
        }
        else
        {
            _blockPreview.transform.position = Vector3.Lerp(_blockPreview.transform.position, _desiredPosition, floatiness * Time.deltaTime);
        }
    }
    
    // Snaps a position to the grid defined by CubitSize
    public static Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(
            Mathf.Round(position.x / CubitSize) * CubitSize,
            Mathf.Round(position.y / CubitSize) * CubitSize,
            Mathf.Round(position.z / CubitSize) * CubitSize
        );
    }

}