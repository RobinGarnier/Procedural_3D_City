using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Stairsgeneration : MonoBehaviour
{
    public List<StairArchitect> stairArchitectList;

    [System.Serializable]
    public class SimpleStair
    {
        [Header("Anchor")]
        public Transform anchor;

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

    //Stair Anchor Definition :
    //      topStep
    //    ___\___   Stair Anchor    ______
    //   |\ ______\                |\_____\
    //   | |    | |                ||_____|
    //   |_|____| |         ->     \ \_____\
    //    \|_____\|                 \|_____|
    //        \              x y
    //        bottomStep   z _\|

    [System.Serializable]
    public class StairArchitect
    {
        [Header("Anchor")]
        public Transform anchor;
        public StairType type;
        bool anchorUsed;

        [Header("Constituing Stairs")]
        List<SimpleStair> stairsList;
        public int totalSubStairNumber;
        Vector3 stepDimension;

        public enum StairType
        {
            toDefine,
            straight,
            O_Shape,
            V_Shape,
            Z_Shape
        }

        //Default Settings for " stairArchitect " creation
        public StairArchitect(Transform anchor)
        {
            this.anchor = anchor;
            type = StairType.toDefine;
            stairsList = new List<SimpleStair>();
            stepDimension = new Vector3(0.3f, 0.2f, 1);
        }
        //Specific creation
        public StairArchitect(Transform anchor, StairType type, int totalSubStairNumber = 1)
        {
            this.anchor = anchor;
            this.type = type;
            this.totalSubStairNumber = totalSubStairNumber;
            stepDimension = new Vector3(0.3f, 0.2f, anchor.localScale.z);
            stairsList = new List<SimpleStair>();
        }

        //Getter
        public List<SimpleStair> GetStairList() => stairsList;
    }


    //From SimpleStair to a real stair in the scene
    public GameObject GenerateStairs(SimpleStair stairOrder)
    {
        Vector3 bottomStep = - new Vector3(stairOrder.anchor.localScale.x / 2, stairOrder.anchor.localScale.y / 2, 0) + new Vector3(stairOrder.stepDimension.x/2, 0, 0);
        MeshFilter[] stepMeshFilter = new MeshFilter[stairOrder.numberOfStep];

        for (int i = 0; i < stairOrder.numberOfStep; i++)
        {
            GameObject step = GameObject.CreatePrimitive(PrimitiveType.Cube);
            step.transform.localScale = stairOrder.stepDimension;
            /*if (stairOrder.hoverStep!) {*/ step.transform.localScale += i * new Vector3(0, stairOrder.stepDimension.y, 0); //}
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

        return staircase;
    }
    //Fills stairArchi's stairList
    public void SubdivideStairs(StairArchitect stairArchi, bool destroySubElement = true)
    {
        List<SimpleStair> stairsList = stairArchi.GetStairList();
        List<GameObject> staircaseSubElement = new List<GameObject>();
        List<GameObject> otherConstructorObj = new List<GameObject>();

        float bottomPositionOfAnchor(Transform anchor) => anchor.position.y - anchor.localScale.y / 2;

        switch (stairArchi.type)
        {
            case StairType.straight:
                stairsList.Add(new SimpleStair(stairArchi.anchor));
                break;
            case StairType.Z_Shape:
                for (int i = 0; i < stairArchi.totalSubStairNumber; i++)
                {
                    GameObject simpleStairAnchor = new GameObject();
                    simpleStairAnchor.transform.localScale = new Vector3(stairArchi.anchor.localScale.x, stairArchi.anchor.localScale.y / stairArchi.totalSubStairNumber, stairArchi.anchor.localScale.z / (2 * stairArchi.totalSubStairNumber));
                    simpleStairAnchor.transform.position = stairArchi.anchor.position + (i - stairArchi.totalSubStairNumber / 2) * new Vector3(0, simpleStairAnchor.transform.localScale.y, simpleStairAnchor.transform.localScale.z * 2) - new Vector3(0, 0, simpleStairAnchor.transform.localScale.z/2);

                    GameObject stairBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //stairBase.transform.localScale = simpleStairAnchor.transform.localScale;
                    stairBase.transform.localScale = new Vector3(simpleStairAnchor.transform.localScale.x, bottomPositionOfAnchor(simpleStairAnchor.transform) - bottomPositionOfAnchor(stairArchi.anchor), 2 * simpleStairAnchor.transform.localScale.z);
                    stairBase.transform.position = simpleStairAnchor.transform.position;
                    stairBase.transform.position -= new Vector3(0, simpleStairAnchor.transform.localScale.y / 2 + stairBase.transform.localScale.y / 2, -simpleStairAnchor.transform.localScale.z/2);

                    GameObject floorBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    floorBase.transform.localScale = simpleStairAnchor.transform.localScale;
                    floorBase.transform.position = simpleStairAnchor.transform.position + new Vector3(0, 0, simpleStairAnchor.transform.localScale.z);

                    stairsList.Add(new SimpleStair(simpleStairAnchor.transform));
                    staircaseSubElement.Add(stairBase);
                    staircaseSubElement.Add(floorBase);
                    otherConstructorObj.Add(simpleStairAnchor);
                }
                break;
            case StairType.V_Shape:
                for (int i = 0; i < stairArchi.totalSubStairNumber; i++)
                {
                    GameObject simpleStairAnchor = new GameObject();
                    simpleStairAnchor.transform.localScale = new Vector3(stairArchi.anchor.localScale.x, stairArchi.anchor.localScale.y / stairArchi.totalSubStairNumber, stairArchi.anchor.localScale.z / stairArchi.totalSubStairNumber);
                    simpleStairAnchor.transform.position = stairArchi.anchor.position + (i - (stairArchi.totalSubStairNumber - 1) / 2) * new Vector3(0, simpleStairAnchor.transform.localScale.y, simpleStairAnchor.transform.localScale.z);
                    simpleStairAnchor.transform.localEulerAngles = new Vector3(0, i * 180, 0);

                    GameObject stairBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //stairBase.transform.localScale = simpleStairAnchor.transform.localScale;
                    stairBase.transform.localScale = new Vector3(simpleStairAnchor.transform.localScale.x, bottomPositionOfAnchor(simpleStairAnchor.transform) - bottomPositionOfAnchor(stairArchi.anchor), simpleStairAnchor.transform.localScale.z);
                    stairBase.transform.position = simpleStairAnchor.transform.position;
                    stairBase.transform.position -= new Vector3(0, simpleStairAnchor.transform.localScale.y/2 + stairBase.transform.localScale.y/2, 0);

                    stairsList.Add(new SimpleStair(simpleStairAnchor.transform));
                    staircaseSubElement.Add(stairBase);
                    otherConstructorObj.Add(simpleStairAnchor);
                }
                break;
        }

        foreach (SimpleStair stair in stairsList)
        {
            staircaseSubElement.Add(GenerateStairs(stair));
        }

        CombineInstance[] combine = new CombineInstance[staircaseSubElement.Count];
        for (int i = 0; i < staircaseSubElement.Count; i++)
        {
            staircaseSubElement[i].transform.position -= stairArchi.anchor.position;
            combine[i].mesh = staircaseSubElement[i].GetComponent<MeshFilter>().sharedMesh;
            combine[i].transform = staircaseSubElement[i].transform.localToWorldMatrix;
            if (destroySubElement) { Destroy(staircaseSubElement[i]); } else { DestroyImmediate(staircaseSubElement[i]); }
        }

        GameObject staircase = new GameObject("Staircase");
        MeshFilter meshFilter = staircase.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);

        staircase.AddComponent<MeshRenderer>().material = staircaseSubElement[0].GetComponent<MeshFilter>().GetComponent<Renderer>().material;

        staircase.transform.position = stairArchi.anchor.position;
        staircase.transform.localRotation = stairArchi.anchor.localRotation;

        foreach(GameObject obj in otherConstructorObj) { if (Application.isPlaying) { Destroy(obj); } else { DestroyImmediate(obj); } }
    }

    [Button("Add a StairObject")]
    public void AddStairArchitect(StairArchitect.StairType type = StairArchitect.StairType.V_Shape, int totalSubStairNumber = 3)
    {
        //Initialisation
        GameObject anchorObj = new GameObject("StairAnchor");
        anchorObj.transform.position = transform.position;
        anchorObj.transform.localScale = new Vector3(3, 5, 3);

        BoxCollider collider = anchorObj.AddComponent<BoxCollider>();
        collider.size = new Vector3(1, 1, 1);

        StairArchitect newStairArchi = new StairArchitect(anchorObj.transform, type, totalSubStairNumber);
        stairArchitectList.Add(newStairArchi);

        //Create the stair
        SubdivideStairs(newStairArchi, false);
    }


    // For Inspector editing 
    /*private void OnValidate()
    {
        foreach (var stairArchi in stairArchitectList)
        {
            UpdateStaircase(stairArchi);
        }
    }
    [Button("UpdateStair")]
    private void UpdateStaircase(StairArchitect stairArchi)
    {
        // Clear and regenerate the stair setup based on any changes
        SubdivideStairs(stairArchi);
    }*/
}
    
