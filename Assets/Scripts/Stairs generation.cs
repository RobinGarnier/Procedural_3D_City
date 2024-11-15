using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Stairsgeneration : MonoBehaviour
{
    Transform anchor;

    SimpleStair stairOrder;
    SimpleStair stairOrderArchive;
    public List<StairArchitect> stairArchitectList;

    public void GenerateStairs()
    {
        Vector3 bottomStep = - new Vector3(stairOrder.anchor.localScale.x / 2, stairOrder.anchor.localScale.y / 2, 0) + new Vector3(stairOrder.stepDimension.x/2, 0, 0);
        MeshFilter[] stepMeshFilter = new MeshFilter[stairOrder.numberOfStep];

        for (int i = 0; i < stairOrder.numberOfStep; i++)
        {
            GameObject step = GameObject.CreatePrimitive(PrimitiveType.Cube);
            step.transform.localScale = stairOrder.stepDimension;
            if (stairOrder.hoverStep!) { step.transform.localScale += i * new Vector3(0, stairOrder.stepDimension.y, 0); }
            step.transform.position = bottomStep + i * new Vector3(stairOrder.stepDimension.x, stairOrder.stepDimension.y, 0) - new Vector3(0, step.transform.localScale.y/2 - stairOrder.stepDimension.y, 0);

            stepMeshFilter[i] = step.GetComponent<MeshFilter>();
        }

        CombineInstance[] combine = new CombineInstance[stairOrder.numberOfStep];
        for (int i = 0; i < stairOrder.numberOfStep; i++)
        {
            combine[i].mesh = stepMeshFilter[i].sharedMesh;
            combine[i].transform = stepMeshFilter[i].transform.localToWorldMatrix;
            if (Application.isPlaying) { Destroy(stepMeshFilter[i].gameObject); } else { DestroyImmediate(stepMeshFilter[i].gameObject); }
        }

        GameObject staircase = new GameObject("Staircase");
        MeshFilter meshFilter = staircase.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);

        staircase.AddComponent<MeshRenderer>().material = stepMeshFilter[0].GetComponent<Renderer>().material;

        staircase.transform.position = stairOrder.anchor.position;
        staircase.transform.localRotation = stairOrder.anchor.localRotation;
    }

    public void SubdivideStairs(StairArchitect stairArchi)
    {
        List<SimpleStair> stairsList = stairArchi.GetStairList();
        switch (stairArchi.type)
        {
            case StairType.straight:
                stairsList.Add(new SimpleStair(stairArchi.anchor));
                break;
            case StairType.Z_Shape:
                for (int i = 0; i < stairArchi.totalSubStairNumber; i++)
                {
                    GameObject simpleStairAnchor = new GameObject();
                    simpleStairAnchor.transform.localScale = new Vector3(anchor.localScale.x, anchor.localScale.y / stairArchi.totalSubStairNumber, anchor.localScale.z / stairArchi.totalSubStairNumber);
                    simpleStairAnchor.transform.position = anchor.position + (i - stairArchi.totalSubStairNumber / 2) * new Vector3(0, simpleStairAnchor.transform.localScale.y, simpleStairAnchor.transform.localScale.z);
                    stairsList.Add(new SimpleStair(simpleStairAnchor.transform));
                }
                break;
            case StairType.V_Shape:
                for (int i = 0; i < stairArchi.totalSubStairNumber; i++)
                {
                    GameObject simpleStairAnchor = new GameObject();
                    simpleStairAnchor.transform.localScale = new Vector3(stairArchi.anchor.localScale.x, stairArchi.anchor.localScale.y / stairArchi.totalSubStairNumber, stairArchi.anchor.localScale.z / stairArchi.totalSubStairNumber);
                    simpleStairAnchor.transform.position = stairArchi.anchor.position + (i - (stairArchi.totalSubStairNumber - 1) / 2) * new Vector3(0, simpleStairAnchor.transform.localScale.y, simpleStairAnchor.transform.localScale.z);
                    simpleStairAnchor.transform.localEulerAngles = new Vector3(0, i * 180, 0);
                    stairsList.Add(new SimpleStair(simpleStairAnchor.transform));
                }
                break;
        }

        foreach (SimpleStair stair in stairsList)
        {
            stairOrder = stair;
            GenerateStairs();
        }
    }

    private Vector3 stepStandartDimension = new Vector3(0.3f, 0.2f, 1);

    private void OnValidate()
    {
        // Automatically update each stair in `stairArchitectList` when changes occur in the Inspector
        foreach (var stairArchi in stairArchitectList)
        {
            UpdateStaircase(stairArchi);
        }
    }

    [Button("Add a StairObject")]
    public void AddStairArchitect(StairType type = StairType.toDefine, int totalSubStairNumber = 1)
    {
        // Create a new GameObject to act as the anchor with a BoxCollider
        GameObject anchorObj = new GameObject("StairAnchor");
        anchorObj.transform.position = transform.position; // Set default position
        BoxCollider collider = anchorObj.AddComponent<BoxCollider>();

        // Adjust collider size to match typical stair volume, you can customize this as needed
        collider.size = new Vector3(1, 2, 1);

        // Create a new StairArchitect linked to this anchor and add it to the list
        StairArchitect newStairArchi = new StairArchitect(anchorObj.transform, type, totalSubStairNumber);
        stairArchitectList.Add(newStairArchi);

        // Generate initial stairs for this architect
        SubdivideStairs(newStairArchi);
    }

    private void UpdateStaircase(StairArchitect stairArchi)
    {
        // Clear and regenerate the stair setup based on any changes
        SubdivideStairs(stairArchi);
    }

    [System.Serializable]
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

        public SimpleStair(Transform anchor, float stepHeight = 0.2f, bool hoverStep = false)
        {
            this.anchor = anchor;


            numberOfStep = Mathf.CeilToInt(anchor.localScale.y / stepHeight);
            stepDimension = new Vector3(anchor.localScale.x / numberOfStep, stepHeight, anchor.localScale.z);
            this.hoverStep = hoverStep;
        }
    }

    [System.Serializable]
    public class StairArchitect
    {
        public Transform anchor;
        public StairType type;
        bool anchorUsed;

        List<SimpleStair> stairsList;

        public int totalSubStairNumber;
        Vector3 stepDimension;

        public StairArchitect(Transform anchor)
        {
            //Default Settings
            this.anchor = anchor;
            type = StairType.toDefine;
            stairsList = new List<SimpleStair>();
            stepDimension = new Vector3(0.3f, 0.2f, 1);
        }

        public StairArchitect(Transform anchor, StairType type, int totalSubStairNumber = 1)
        {
            this.anchor = anchor;
            this.type = type;
            this.totalSubStairNumber = totalSubStairNumber;
            stepDimension = new Vector3(0.3f, 0.2f, anchor.localScale.z);
            stairsList = new List<SimpleStair>();
        }

        public List<SimpleStair> GetStairList() => stairsList;
    }
}
    public enum StairType
    {
        toDefine,
        straight,
        O_Shape,
        V_Shape,
        Z_Shape
    }

