using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexFacade : MonoBehaviour
{
    public List<MeshCreator.Wall> wallList;
    public List<AnchorScript.Anchor> anchorPrefabList;
    public bool facade = true;
    public List<GameObject> delatable = new();
    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i+1);
            (list[i], list[j]) = (list[j], list[i]); // Swap elements
        }
    }
    public void FromWallToFacade(MeshCreator.Wall wall)
    {
        float lenght = Vector3.Distance(wall.points[0], wall.points[1]);
        Vector3 wallAxis = (wall.points[1] - wall.points[0]).normalized;
        float height = wall.height;
        List<float> usedSpaceFront = new();
        //ManageRotation of the facade elements
        //Vector3 xAxis = Vector3.Cross(Vector3.forward, wallAxis);
        //xAxis.Normalize();
        //Vector3 yAxis = Vector3.Cross(wallAxis, xAxis).normalized;
        //Quaternion facadeElemRotation = Quaternion.LookRotation(yAxis, xAxis);
        Vector3 wallAxisNormalized = Vector3.Normalize(wallAxis);
        float angle = -wallAxisNormalized.z / Mathf.Abs(wallAxisNormalized.z) * Mathf.Acos(Vector3.Dot(wallAxisNormalized, Vector3.right));

        List <AnchorScript.Anchor> anchorUsablePrefab = anchorPrefabList;
        bool[,] occupationTable = new bool[Mathf.RoundToInt(lenght),Mathf.RoundToInt(height)];
        for(int l = 0; l < lenght; l++)
        {
            for(int h = 0; h < height; h++)
            {
                Vector3Int position = new(l, h, 0);
                foreach (AnchorScript.Anchor anchorPrefab in anchorUsablePrefab)
                {
                    bool AnchorFits(Vector3Int position, AnchorScript.Anchor anchor)
                    {
                        for (int a = 0; a < anchor.anchor.localScale.x; a++)
                        {
                            for (int b = 0; b < anchor.anchor.localScale.y; b++)
                            {
                                if(position.x + a>=Mathf.RoundToInt(lenght) || position.y + b >= Mathf.RoundToInt(height)) { return false; }
                                if (occupationTable[position.x + a, position.y + b]) { return false; }
                            }
                        }
                        return true;
                    }
                    void FillOccupationTable(Vector3Int position, AnchorScript.Anchor anchor)
                    {
                        for (int a = 0; a < anchor.anchor.localScale.x; a++)
                        {
                            for (int b = 0; b < anchor.anchor.localScale.y; b++)
                            {
                                occupationTable[position.x + a, position.y + b] = true;
                            }
                        }
                    }

                    if (AnchorFits(position, anchorPrefab))
                    {
                        Vector3 wallPartPosition = wall.points[0] + (position.x + anchorPrefab.anchor.localScale.x / 2) * Vector3.Normalize(wall.points[1] - wall.points[0]) + new Vector3(0, h + anchorPrefab.anchor.localScale.y / 2, 0);

                        GameObject wallPart = Instantiate(anchorPrefab.anchor.gameObject, wallPartPosition, new Quaternion());//, facadeElemRotation);
                        wallPart.transform.eulerAngles = new Vector3(0, Mathf.Rad2Deg * angle, 0);
                        delatable.Add(wallPart);
                        FillOccupationTable(position, anchorPrefab);
                        break;
                    }
                }
                Shuffle(anchorUsablePrefab);

            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (facade)
        {
            facade = false;
            foreach(GameObject obj in delatable) { Destroy(obj); }
            FromWallToFacade(wallList[0]);
        }

    }
}
