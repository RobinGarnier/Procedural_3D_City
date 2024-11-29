using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorScript : MonoBehaviour
{
    [System.Serializable]
    public class Anchor
    {
        public Transform anchor;
        public AnchorType anchorType;
        public int anchorIndex;

        public enum AnchorType 
        { 
            toDefine,
            stair, 
            hole,
            wall
        }

        public Anchor(Transform anchor, AnchorType anchorType = AnchorType.toDefine)
        {
            this.anchor = anchor;
            this.anchorType = anchorType;
        }

        public Anchor(Vector3 position, Vector3 size, AnchorType type = AnchorType.toDefine)
        {
            anchor = new GameObject().transform;
            anchor.position = position;
            anchor.localScale = size;
            anchor.name = "Anchor";

            anchorType = type;
        }
    }


}
