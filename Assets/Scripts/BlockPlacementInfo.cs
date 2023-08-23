using UnityEngine;
using UnityEngine.Events;

public class BlockPlacementInfo : MonoBehaviour
{
    public GameObject blockPrefab;
    public Renderer[] renderers;
    public BlockBound[] connectionSurfaces;
    public BlockBound[] blockingZones;

    public bool CanPlaceAt(Vector3 position, Quaternion rotation)
    {
        foreach (var zone in blockingZones)
        {
            if (zone.TestCollision(position, rotation))
            {
                return false;
            }   
        }
        
        foreach (var surface in connectionSurfaces)
        {
            if (surface.TestConnection(position, rotation))
            {
                return true;
            }
        }
        
        return false;
    }

    public void SetShaderInfo(float placement, float valid)
    {
        foreach (var renderer in renderers)
        {
            renderer.material.SetFloat("_Placement", placement);
            renderer.material.SetFloat("_Valid", valid);
        }
    }
    
}