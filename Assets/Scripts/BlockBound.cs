using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBound : MonoBehaviour
{
    public bool TestConnection(Vector3 position, Quaternion rotation)
    {
        var t = transform;
        var hits = Physics.OverlapBox((rotation * t.localPosition) + position, t.localScale / 2, rotation * t.localRotation, BlockPlacer.PlacingLayerMask);
        return hits.Length > 0;
    }

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
