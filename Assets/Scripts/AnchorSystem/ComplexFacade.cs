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
        float height = wall.height;
        List<float> usedSpaceFront = new();

        List<AnchorScript.Anchor> anchorUsablePrefab = anchorPrefabList;
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
                        GameObject wallPart = Instantiate(anchorPrefab.anchor.gameObject);
                        wallPart.transform.position = position + new Vector3(anchorPrefab.anchor.localScale.x/2, anchorPrefab.anchor.localScale.y/2,0);
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
