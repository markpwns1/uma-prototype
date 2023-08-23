using UnityEngine;
using UnityEngine.Events;

// The script attached to the block placement preview,
// managing valid placement calculations
public class BlockPlacementInfo : MonoBehaviour
{
    public GameObject blockPrefab; // The prefab to instantiate when placing
    
    // The renderers in the preview
    public Renderer[] renderers;
    
    // The bounding boxes that represent connection surfaces
    public BlockBound[] connectionSurfaces;
    
    // The bounding boxes that represent "blocking" zones, that is, zones where there cannot be anything inside
    // This is used to prevent placing blocks inside other blocks
    public BlockBound[] blockingZones;

    // Returns true if the block can be placed at the given position and rotation
    public bool CanPlaceAt(Vector3 position, Quaternion rotation)
    {
        // If any of the blocking zones collide, no placement is possible
        foreach (var zone in blockingZones)
        {
            if (zone.TestCollision(position, rotation))
            {
                return false;
            }   
        }
        
        // If any of the connection surfaces collide, placement is possible
        foreach (var surface in connectionSurfaces)
        {
            if (surface.TestConnection(position, rotation))
            {
                return true;
            }
        }
        
        return false;
    }

    // Sets some shader info for each material in the preview
    public void SetShaderInfo(float placement, float valid)
    {
        foreach (var renderer in renderers)
        {
            renderer.material.SetFloat("_Placement", placement);
            renderer.material.SetFloat("_Valid", valid);
        }
    }
    
}