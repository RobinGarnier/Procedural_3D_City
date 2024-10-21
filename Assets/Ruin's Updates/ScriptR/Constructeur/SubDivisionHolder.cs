using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using City;

public class SubDivisionHolder : MonoBehaviour
{
    public Vector3 coordinate;
    public typeInHerarchy type;

    [Header("Interior")]
    public List<GameObject> sectorList = new List<GameObject>();
    public int subDivisionLevelIn = 0;

    [Header("Exterior")]
    public bool notUpdated = true;
    public List<Neighbors> neighbors = new List<Neighbors>() { null, null, null, null, null, null };
    public List<GameObject> largeNeighborhood = new List<GameObject>() { null, null, null, null, null, null };

    public void UpdateNeighbors()
    {
        int index = 0;
        int coefForInvertedNeighbors = 1;
        string recap = "recap :";
        foreach (GameObject farNeighbor in largeNeighborhood)
        {
            try//if (farNeighbor != null)
            {
                SubDivisionHolder farNeighborHolderScript = farNeighbor.GetComponent<SubDivisionHolder>();
                //if (farNeighborHolderScript.largeNeighborhood[index + coefForInvertedNeighbors] == null)
                //{
                farNeighborHolderScript.largeNeighborhood[index + coefForInvertedNeighbors] = gameObject;
                farNeighborHolderScript.UpdateSelf();
                //}
                recap += $"{index}, ";
            }
            catch { }

            index++;
            coefForInvertedNeighbors *= -1;
        }
        Debug.Log(recap);
    }

    public void UpdateSelf()
    {
        int index = 0;
        int coefForInvertedNeighbors = 1;
        bool updateNeighbors = false;
        bool allFarNeighborsFound = true;
        foreach (GameObject farNeighbor in largeNeighborhood)
        {
            if (farNeighbor != null) 
            {
                if (neighbors == null)
                {
                    updateNeighbors = true;
                    neighbors[index] = NeighborFromSector(farNeighbor, index);
                }
            }
            else { allFarNeighborsFound = false; }

            index++;
            coefForInvertedNeighbors *= -1;
        }
        if (updateNeighbors) { UpdateNeighbors(); }
        if(allFarNeighborsFound) { notUpdated = false; }

        UpdateInNeighborForSubSector();
    }

    public void UpdateInNeighborForSubSector()
    {
        if(type == typeInHerarchy.holder)
        {
            //how to attribute each neighbor with the correct inSector (negativ for inner sector)
            int index = 0;
            List<List<int>> listDistribNieghborToInSector = new List<List<int>>
            {
                new List<int> { 0, -4, -2, 3, -5, 5},   //1
                new List<int> { 0, -3, 2, -1, -6, 5},  //2
                new List<int> { -2, 1, -2, -4, -7, 5},  //3
                new List<int> { -1, 1, -3, 3, -8, 5},  //4

                new List<int> { 0, -8, -6, 3, 4, -1},  //5
                new List<int> { 0, -7, 2, -5, 4, -2},  //6
                new List<int> { -6, 1, 2, -8, 4, -3},  //7
                new List<int> { -5, 1, -7, 3, 4, 4}   //8
            };
            foreach (GameObject sectorIn in sectorList)
            {
                SubDivisionHolder sectorInHolderScript = sectorIn.GetComponent<SubDivisionHolder>();
                for (int i = 0; i < 6; i++)
                {
                    if (listDistribNieghborToInSector[index][i] >= 0)
                    {
                        sectorInHolderScript.largeNeighborhood[i] = largeNeighborhood[listDistribNieghborToInSector[index][i]];
                    }
                    else
                    {
                        sectorInHolderScript.largeNeighborhood[i] = sectorList[ - (listDistribNieghborToInSector[index][i] + 1)]; // adapt the neighbor code to the list index
                    }
                }
                sectorInHolderScript.UpdateSelf();
                index ++;
            }
        }
        else
        {
            for(int i = 0; i < 6; i++)
            {
                if(largeNeighborhood[i] != null)
                {
                    neighbors[i] = NeighborFromSector(largeNeighborhood[i], i);
                }
            }
            try { if (type == typeInHerarchy.sectorWithSolConstruct) { gameObject.GetComponent<SolConstruct>().WallBuilding(); } } catch { }
        }
    }

    public Neighbors NeighborFromSector(GameObject sector, int positionAround)
    {
        Neighbors.neighborsTypes neighborsTypes = new Neighbors.neighborsTypes();

        if (type == typeInHerarchy.sectorEmpty || sector.tag == "OpenSpace")
        {
            neighborsTypes = Neighbors.neighborsTypes.OpenSpace;
        }
        else if(sector.tag == "EmptySpace" && positionAround<5)
        {
            neighborsTypes = Neighbors.neighborsTypes.EmptySpace;
            if (transform.tag != "EmptySpace") { transform.tag = "OpenSpace"; }
        }

        if(sector.GetComponent<SolConstruct>() != null)
        {
            if(neighborsTypes==Neighbors.neighborsTypes.None) { neighborsTypes = Neighbors.neighborsTypes.Standart; }
            return new Neighbors(neighborsTypes, subDivisionLevelIn, sector.GetComponent<SolConstruct>(), positionAround);
        }

        else
        {
            if(sector.GetComponent<SubDivisionHolder>().type == typeInHerarchy.holder)
            {
                neighborsTypes = Neighbors.neighborsTypes.Holder;
            }
            return new Neighbors(neighborsTypes, subDivisionLevelIn, positionAround);
        }
    }


    [System.Serializable]
    public class Neighbors
    {
        public neighborsTypes type;
        public int subdivision;
        public bool useSolConstruct;
        public SolConstruct script;
        public int position;

        public Neighbors(neighborsTypes neiType, int subdiv, int positionAroundSect)
        {
            type = neiType;
            useSolConstruct = false;
            subdivision = subdiv;
            position = positionAroundSect;
        }

        public Neighbors(neighborsTypes neiType, int subdiv, SolConstruct neiScript, int positionAroundSect)
        {
            type = neiType;
            useSolConstruct = true;
            subdivision = subdiv;
            script = neiScript;
            position = positionAroundSect;
        }

        public enum neighborsTypes
        {
            None,
            OpenSpace,
            EmptySpace,
            Standart,
            Holder,
        }
    }


    /*if (subDivisionAsChild) { }
    else
    {
        if (sectorScript.notUpdated && organisation == organizationType.Ruin)
        {
            List<Vector3> suround = new List<Vector3> { new(1, 0, 0), new(-1, 0, 0), new(0, 0, 1), new(0, 0, -1), new(0, 1, 0), new(0, -1, 0) };
            bool needWallCheck = false;
            for (int i = 0; i < suround.Count; i++)
            {
                if (sectorScript.neighbors[i].type == Neighbors.neighborsTypes.None)
                {
                    bool foundSmthg = false;
                    GameObject neighborTest;
                    List<GameObject> neighborList = new List<GameObject>();
                    SolConstruct neighborScript;
                    bool useSolConstruct = false;
                    bool isDivided = false;

                    neighborList = AllSectorFoundAtPosition(SectorCoord(pivotForSectorOrga) + suround[i]);
                    if (neighborList.Count != 0)
                    {
                        foundSmthg = true;
                        needWallCheck = true;
                        switch (neighborList.Count)
                        {
                            case 1:
                                neighborTest = neighborList[0];
                                break;
                            default:

                                break;

                        }
                    }

                    if (foundSmthg)
                    {
                        if (isDivided)
                        {
                            //sectorScript.neighbors[i] = new Neighbors(Neighbors.neighborsTypes.Standart,i);
                        }
                        else
                        {
                            try
                            {
                                neighborScript = transform.Find(SectorNameCoordViaDim(SectorCoord(pivotForSectorOrga) + suround[i], 0)).gameObject.GetComponent<SolConstruct>();
                                //neighborScript = neighborTest.GetComponent<SolConstruct>();
                                useSolConstruct = true;
                                if (transform.Find(SectorNameCoordViaDim(SectorCoord(pivotForSectorOrga) + suround[i], 0)).tag == "OpenSpace")
                                {
                                    sectorScript.neighbors[i] = new Neighbors(Neighbors.neighborsTypes.OpenSpace, 0, neighborScript, i);
                                }
                                else
                                {
                                    if (transform.Find(SectorNameCoordViaDim(SectorCoord(pivotForSectorOrga) + suround[i], 0)).childCount == 0)
                                    {
                                        sectorScript.neighbors[i] = new Neighbors(Neighbors.neighborsTypes.EmptySpace, 0, i);
                                        //Update its status according to its neighbor
                                        sectorScript.gameObject.tag = "OpenSpace";
                                    }
                                    else { sectorScript.neighbors[i] = new Neighbors(Neighbors.neighborsTypes.Standart, 0, neighborScript, i); }
                                }

                            }
                            catch
                            {
                                if (transform.Find(SectorNameCoordViaDim(SectorCoord(pivotForSectorOrga) + suround[i], 0)).childCount == 0)
                                {
                                    sectorScript.neighbors[i] = new Neighbors(Neighbors.neighborsTypes.EmptySpace, 0, i);
                                    //Update its status according to its neighbor
                                    sectorScript.gameObject.tag = "OpenSpace";
                                }
                                else if (transform.Find(SectorNameCoordViaDim(SectorCoord(pivotForSectorOrga) + suround[i], 0)).tag == "OpenSpace")
                                {
                                    sectorScript.neighbors[i] = new Neighbors(Neighbors.neighborsTypes.OpenSpace, 0, i);
                                }
                                else { sectorScript.neighbors[i] = new Neighbors(Neighbors.neighborsTypes.Standart, 0, i); }
                            }
                        }
                    }
                }
            }
            if (needWallCheck)
            {
                sectorScript.WallBuilding();
            }
            sectorScript.notUpdated = false;
        }
    }
    */
}
public enum typeInHerarchy
{
    sectorWithSolConstruct,
    sectorFixed,
    sectorEmpty,
    holder
}
