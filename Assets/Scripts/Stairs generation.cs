using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Stairsgeneration : MonoBehaviour
{
    Transform anchor;
    bool anchorUsed;
    bool simplifiedVersion = false;




    [Button("GenerateStairs")]
    public void GenerateStairs()
    {
        if (anchorUsed) 
        {
            SwitchToSimplifiedVersion(anchor);
            anchorUsed = false;
        }
        else
        {

        }
    }

    public void SwitchToSimplifiedVersion(Transform anchor, bool useScale = false)
    {
        Vector3 anchorScale;
        bool anchorInScene = true;
        
        if (useScale)
        {
            anchorScale = anchor.localScale;
            try { GameObject anchorObject = anchor.gameObject; } catch { anchorInScene = false; }
        }
        else
        {
            anchorScale = anchor.GetComponent<MeshFilter>().mesh.bounds.size;
        }
        if (anchorInScene) 
        { 

        }
        else 
        { 
        
        }
        simplifiedVersion = true;
    }
}
