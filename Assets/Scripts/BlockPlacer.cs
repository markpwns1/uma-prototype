using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BlockPlacer : MonoBehaviour
{
    public const float CubitSize = 0.4f;

    public static int PlacingLayerMask { get; private set; }
    public static int BlockingLayerMask { get; private set; }
    public float floatiness = 3.0f;
    public float scrollSensitivity = 0.5f;
    public float minDistance = 1.0f;
    public float maxDistance = 10.0f;

    private BlockPlacementInfo _block;
    private BlockPlacementInfo _blockPreview;
    
    [FormerlySerializedAs("currentSlot")] public ItemSlotUI placingSlot;
    public Inventory inventory;
    
    private Vector3 _desiredPosition = Vector3.zero;
    private Quaternion _desiredRotation = Quaternion.identity;
    private float _distance;

    private Transform _camera;

    private bool _canPlace;

    void Start()
    {
        PlacingLayerMask = LayerMask.GetMask("Connectible");
        BlockingLayerMask = LayerMask.GetMask("Default", "Connectible");
        
        _camera = Camera.main.transform;
        
        // SwitchBlock(DEBUG_blockPrefab.GetComponent<BlockPlacementInfo>());
    }

    public void Select(ItemStack item)
    {
        _block = item.Item.PlacementPrefab.GetComponent<BlockPlacementInfo>();
        if(_blockPreview) Destroy(_blockPreview.gameObject);
        _blockPreview = Instantiate(_block).GetComponent<BlockPlacementInfo>();
        placingSlot.SetItemStack(item);
    }

    public void UnselectCurrentItem()
    {
        placingSlot.SetItemStack(null);
        _block = null;
        if(_blockPreview) Destroy(_blockPreview.gameObject);
        _blockPreview = null;
    }
    
    // private void SwitchBlock(BlockPlacementInfo block)
    // {
    //     _block = block;
    //     if(_blockPreview) Destroy(_blockPreview);
    //     _blockPreview = Instantiate(_block).GetComponent<BlockPlacementInfo>();
    // }

    void PlaceBlock()
    {
        var block = Instantiate(_block.blockPrefab, _desiredPosition, _desiredRotation);
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

    void DestroyBlock(Transform block)
    {
        var itemStack = inventory.AddItem(block.name, 1);
        placingSlot.UpdateAmount();
        Destroy(block.gameObject);

        if (placingSlot.Stack == null)
        {
            Select(itemStack);
        }
    }
    
    void Update()
    {
        if (MouseLockManager.IsMouseLocked && Input.GetMouseButtonDown(1))
        {
            if(Physics.Raycast(_camera.position, _camera.forward, out var hit, _distance + 1.0f, PlacingLayerMask) && !hit.collider.gameObject.isStatic)
            {
                DestroyBlock(hit.transform.root);
                return;
            }
        }
        
        if(!_block) return;
        
        _canPlace = false;

        _distance = Mathf.Clamp(_distance + Input.mouseScrollDelta.y * scrollSensitivity, minDistance, maxDistance);
        
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            _desiredRotation *= Quaternion.Euler(0, 90, 0);
        }
        
        _blockPreview.SetShaderInfo(1.0f, _canPlace? 1 : 0);

        _blockPreview.transform.rotation = Quaternion.Lerp(_blockPreview.transform.rotation, _desiredRotation, floatiness * Time.deltaTime);
        
        if (_canPlace)
        {
            _blockPreview.transform.position = _desiredPosition;
            
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
    
    public static Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(
            Mathf.Round(position.x / CubitSize) * CubitSize,
            Mathf.Round(position.y / CubitSize) * CubitSize,
            Mathf.Round(position.z / CubitSize) * CubitSize
        );
    }

}