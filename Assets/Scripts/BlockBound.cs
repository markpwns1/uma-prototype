using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is a bounding box that tests for collisions with certain objects
// It's used for the block preview to check for valid placement
public class BlockBound : MonoBehaviour
{
    // Returns true if this box collides with anything in the placing layer mask
    public bool TestConnection(Vector3 position, Quaternion rotation)
    {
        var t = transform;
        var hits = Physics.OverlapBox((rotation * t.localPosition) + position, t.localScale / 2, rotation * t.localRotation, BlockPlacer.PlacingLayerMask);
        return hits.Length > 0;
    }

    // Returns true if this box collides with anything in the blocking layer mask
    public bool TestCollision(Vector3 position, Quaternion rotation)
    {
        var t = transform;
        var hits = Physics.OverlapBox((rotation * t.localPosition) + position, t.localScale / 2, rotation * t.localRotation, BlockPlacer.BlockingLayerMask);
        foreach (var hit in hits)
        {
            if(hit.transform.root.CompareTag("BlockPreview")) continue;
            return true;
        }
        return false;
    }
}
