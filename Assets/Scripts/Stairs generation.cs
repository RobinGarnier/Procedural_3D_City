using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Stairsgeneration : MonoBehaviour
{
    public Transform anchor;
    public bool anchorUsed;
    public SimpleStair stairOrder;
    public float stepHeight = 0.3f;

    public void Start()
    {
        stairOrder = new SimpleStair(anchor, stepHeight);
    }

    [Button("GenerateStairs")]
    public void GenerateStairs()
    {
        Vector3 bottomStep = stairOrder.anchor.position - new Vector3(stairOrder.anchor.localScale.x / 2, stairOrder.anchor.localScale.y / 2, 0);
        MeshFilter[] stepMeshFilter = new MeshFilter[stairOrder.numberOfStep];

        for (int i = 0; i < stairOrder.numberOfStep; i++)
        {
            GameObject step = GameObject.CreatePrimitive(PrimitiveType.Cube);
            step.transform.localScale = stairOrder.stepDimension;
            step.transform.position = bottomStep + new Vector3(i * stairOrder.stepDimension.x, i* stairOrder.stepDimension.y, 0);

            stepMeshFilter[i] = step.GetComponent<MeshFilter>();
        }

        CombineInstance[] combine = new CombineInstance[stairOrder.numberOfStep];
        for (int i = 0; i < stairOrder.numberOfStep; i++)
        {
            combine[i].mesh = stepMeshFilter[i].sharedMesh;
            combine[i].transform = stepMeshFilter[i].transform.localToWorldMatrix;
            stepMeshFilter[i].gameObject.SetActive(false);
        }

        GameObject staircase = new GameObject("Staircase");
        MeshFilter meshFilter = staircase.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);

        staircase.AddComponent<MeshRenderer>().material = stepMeshFilter[0].GetComponent<Renderer>().material;
    }

    public class SimpleStair
    {
        [Header("General look")]
        public Transform anchor;
        //      topStep
        //    ___\___   Stairs          ______
        //   |\ ______\                |\_____\
        //   | |    | |                ||_____|
        //   |_|____| |         ->     \ \_____\
        //    \|_____\|                 \|_____|
        //        \              x y
        //        bottomStep   z _\|

        [Header("Step specification")]
        public Vector3 stepDimension;
        public int numberOfStep;
        public bool hoverStep;

        public SimpleStair(Transform anchor, float stepHeight = 0.3f, bool hoverStep = false)
        {
            this.anchor = anchor;

            numberOfStep = Mathf.CeilToInt(anchor.localScale.y / stepHeight);
            stepDimension = new Vector3(anchor.localScale.x / numberOfStep, stepHeight, anchor.localScale.z);
            this.hoverStep = hoverStep;
        }
    }
    
    public class StairArchitect
    {
        public Transform anchor;
        public Vector3 bottomStep;
        public Vector3 topStep;
        public StairType type;

        List<SimpleStair> stairsList;

    }
}
    public enum StairType
    {
        straight,
        round,
        backFourth,
    }

