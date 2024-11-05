using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
//using City;

namespace City
{
    public class EchafConstructor : MonoBehaviour
    {

        [Header("Parameter of the volumic Elements")]
        [SerializeField, Min(0.01f)] public float lenghtCoeff = 6f;
        [SerializeField, Min(0.01f)] public float heightCoeff = 5.5f;
        [SerializeField, Min(0.01f)] public float thicknessCoeff = 2f;
        [SerializeField, Required] private string tagName = "Volumic Catwalk";

        [Header("Object to set")]
        public constructType constructionType = constructType.CatWalk;
        [SerializeField] private GameObject[] etageComposant;
        public GameObject[] etages;
        [SerializeField] private GameObject[] transition;
        [SerializeField] private List<GameObject> volumesHolder;

        private const string cloneText = "Test_";
        private GameObject repetitionLong;

        [Header("Look")]
        [SerializeField] private bool invertTheOrient;
        [SerializeField] private bool invertTheTransi;
        private float orientation = 0;
        private GameObject prefabModel;
        private GameObject escalier;
        private bool nonPresOE=true;//OE=(immeuble)Ouvert avec Escalier
        public bool planMode = true;


        [Button("Construct")]
        public void Construct()
        {
            if (planMode)
            {
                //define the list of volumic element
                int length = transform.childCount;
                for (int i = 0; i < length; i++)
                {
                    if (transform.GetChild(i).gameObject.tag == tagName && transform.GetChild(i).gameObject.activeSelf == true)
                    {
                        volumesHolder.Add(transform.GetChild(i).gameObject);
                    }
                }

                float nbElem = volumesHolder.Count;
                for (int i = 0; i < nbElem; i++)
                {
                    //detect the parameter of the construct area
                    GameObject cube = volumesHolder[i];
                    float thickness = 0;
                    float height = 0;
                    float lengthEt = 0;
                    bool invertTransform = false;
                    if (cube.transform.GetChild(0).localScale.x % lenghtCoeff == 0)
                    {
                        thickness = (cube.transform.GetChild(0).localScale.z) / thicknessCoeff;
                        height = (cube.transform.GetChild(0).localScale.y) / heightCoeff;
                        lengthEt = (cube.transform.GetChild(0).localScale.x) / lenghtCoeff;

                        invertTransform = true;
                    }
                    else
                    {
                        thickness = (cube.transform.GetChild(0).localScale.x) / thicknessCoeff;
                        height = (cube.transform.GetChild(0).localScale.y) / heightCoeff;
                        lengthEt = (cube.transform.GetChild(0).localScale.z) / lenghtCoeff;
                    }
                    
                    //define the default etage prefab

                    //CatWalk
                    if (constructionType == constructType.CatWalk)
                    {
                        //tickness
                        GameObject plateforme = Instantiate(etageComposant[0]);
                        plateforme.transform.parent = etages[0].transform;
                        plateforme.transform.localPosition = Vector3.zero;
                        plateforme.transform.localScale = new Vector3(1, 1, 1);

                        CatWalkSol(0, thickness);
                        Rembarde(0, thickness);

                        if (transition.Length > 0)
                        {
                            bool escNonAligne = true;
                            if (thickness == 3) { escalier = Instantiate(transition[0]); escNonAligne = false; }
                            else { escalier = Instantiate(transition[1]); }

                            escalier.transform.parent = etages[2].transform;
                            escalier.transform.localPosition = Vector3.zero;
                            escalier.transform.localScale = new Vector3(1, 1, 1);

                            if (escNonAligne) { CatWalkSol(2, thickness); Rembarde(2, thickness); }
                        }

                        //lenght
                        GameObject originalEtage = etages[0];
                        GameObject originalTransi = etages[2];

                        bool lengthPair = true;
                        if ((int)(lengthEt - 1) % 2 != 0) { lengthPair = false; }
                        for (int h = 0; h < lengthEt; h++)
                        {
                            if (lengthPair)
                            {
                                if ((lengthEt - 1 <= h && h < lengthEt) && transition.Length > 0 && invertTheTransi == false)
                                { repetitionLong = Instantiate(originalTransi); }
                                else if ((lengthEt - 2 <= h && h < lengthEt - 1) && invertTheTransi && transition.Length > 0)
                                { repetitionLong = Instantiate(originalTransi); }
                                else
                                { repetitionLong = Instantiate(originalEtage); }
                            }
                            else
                            {
                                if ((lengthEt - 2 <= h && h < lengthEt - 1) && transition.Length > 0 && invertTheTransi == false)
                                { repetitionLong = Instantiate(originalTransi); }
                                else if ((lengthEt - 1 <= h && h < lengthEt) && invertTheTransi && transition.Length > 0)
                                { repetitionLong = Instantiate(originalTransi); }
                                else
                                { repetitionLong = Instantiate(originalEtage); }
                            }

                            repetitionLong.transform.parent = etages[1].transform;

                            if (h == 0)
                            { repetitionLong.transform.localPosition = new Vector3(0, 0, 0); }
                            else if (h % 2 != 0)
                            { repetitionLong.transform.localPosition = new Vector3(0, 0, -lenghtCoeff * (h / 2 + 1)); }
                            else
                            { repetitionLong.transform.localPosition = new Vector3(0, 0, lenghtCoeff * (h / 2)); }

                            repetitionLong.transform.localScale = new Vector3(1, 1, 1);
                        }
                    }

                    //ImmeubleFerme & Ouvert
                    else if (constructionType == constructType.ImmeubleFerme || constructionType == constructType.ImmeubleOuvert)
                    {

                        //choose the right orientation for the block

                        float longu = 0;
                        if (thickness > lengthEt)
                        {
                            orientation = 0;
                            longu = thickness;
                        }
                        else
                        {
                            orientation = -90;
                            longu = lengthEt;
                        }
                        if (invertTheOrient) { orientation += 180; }

                        //Choose the right prefab for the model
                        if (constructionType == constructType.ImmeubleFerme) { prefabModel = etageComposant[3]; }

                        //create the etageLong with the right dimension & orientation
                        for (int h = 0; h < longu; h++)
                        {
                            if (constructionType == constructType.ImmeubleOuvert)
                            {
                                if (h == longu - 1 && nonPresOE)
                                {
                                    prefabModel = etageComposant[4];
                                    nonPresOE = false;
                                }
                                else
                                {
                                    int indice = (int)Random.Range(0, 2);
                                    if (indice == 0) { prefabModel = etageComposant[4]; }
                                    else { prefabModel = etageComposant[5]; }
                                }
                            }

                            GameObject appart = Instantiate(prefabModel);
                            appart.transform.parent = etages[1].transform;

                            if (constructionType == constructType.ImmeubleOuvert && lengthEt > thickness)
                            {
                                float save = lenghtCoeff;
                                lenghtCoeff = thicknessCoeff;
                                thicknessCoeff = save;
                            }

                            if (h == 0)
                            { 
                                if ((longu - 1) % 2 == 0) { appart.transform.localPosition = new Vector3(0, 0, 0); }
                                else 
                                {
                                    if (longu == thickness) { appart.transform.localPosition = new Vector3(thicknessCoeff/2, 0, 0); }
                                    else { appart.transform.localPosition = new Vector3(0, 0, lenghtCoeff/2); }    
                                }
                            }
                            else if (longu == thickness)
                            {
                                if (h % 2 != 0)
                                { 
                                    if ((longu - 1) % 2 == 0) { appart.transform.localPosition = new Vector3(-thicknessCoeff * (h / 2 + 1), 0, 0); }
                                    else { appart.transform.localPosition = new Vector3(-thicknessCoeff * (h / 2 + 1) + thicknessCoeff / 2, 0, 0); }
                                }
                                else
                                {
                                    if ((longu - 1) % 2 == 0) { appart.transform.localPosition = new Vector3(thicknessCoeff * (h / 2), 0, 0); }
                                    else { appart.transform.localPosition = new Vector3(thicknessCoeff * (h / 2) + thicknessCoeff / 2, 0, 0); }//
                                }
                            }
                            else if (longu == lengthEt)
                            {
                                if (h % 2 != 0)
                                {
                                    if ((longu - 1) % 2 == 0) { appart.transform.localPosition = new Vector3(0, 0, -lenghtCoeff * (h / 2 + 1)); }
                                    else { appart.transform.localPosition = new Vector3(0, 0, -lenghtCoeff * (h / 2 + 1) + lenghtCoeff/2); }
                                }
                                else
                                {
                                    if ((longu - 1) % 2 == 0) { appart.transform.localPosition = new Vector3(0, 0, lenghtCoeff * (h / 2)); }
                                    else { appart.transform.localPosition = new Vector3(0, 0, lenghtCoeff * (h / 2) + lenghtCoeff / 2); }//
                                }
                            }

                            if (constructionType == constructType.ImmeubleOuvert && lengthEt > thickness)
                            {
                                float save = lenghtCoeff;
                                lenghtCoeff = thicknessCoeff;
                                thicknessCoeff = save;
                            }

                            appart.transform.Rotate(new Vector3(0, orientation, 0));
                            appart.transform.localScale = new Vector3(1, 1, 1);
                        }
                        nonPresOE = true;

                        //create the correct immeuble
                        int tailleEtagei = etages[1].transform.childCount;
                        for (int l = 0; l < tailleEtagei; l++)
                        {
                            ImmeubleConstruct im = etages[1].transform.GetChild(l).GetComponent<ImmeubleConstruct>();
                            im.numberOfPoints = (int)height;
                            im.UpdatePoints();
                        }
                    }

                    orientation = 0;

                    GameObject test = new GameObject();
                    test.transform.localScale = new Vector3(1, 1, 1);
                    test.transform.parent = transform;

                    etages[1].transform.position = cube.transform.GetChild(0).position;
                    etages[1].transform.rotation = cube.transform.GetChild(0).rotation;
                    if (invertTransform) { etages[1].transform.Rotate(0, -90, 0); }
                    etages[1].name = PointName(i);

                    etages[1] = test;

                    test.name = "EtageEmptLong";

                    //empty the default etage model&transi for the next iteration
                    int tailleEtageO = etages[0].transform.childCount;
                    for (int j = 0; j < tailleEtageO; j++)
                    {
                        DestroyImmediate(etages[0].transform.GetChild(j).gameObject);
                        tailleEtageO--;
                        j--;
                    }

                    if (transition.Length > 0)
                    {
                        int tailleEtageII = etages[2].transform.childCount;
                        for (int a = 0; a < tailleEtageII; a++)
                        {
                            DestroyImmediate(etages[2].transform.GetChild(a).gameObject);
                            tailleEtageII--;
                            a--;
                        }
                    }
                }

                //desactive the cubes to keep the final product
                foreach (GameObject elem in volumesHolder)
                { elem.SetActive(false); }

                planMode = false;
            }
            else
            {
                Revert();
            }
        }
        private void Rembarde(int index, float thickness)
        {
            GameObject rembarde = Instantiate(etageComposant[2]);
            rembarde.transform.parent = etages[index].transform;
            rembarde.transform.localPosition = new Vector3(-thicknessCoeff * (thickness - 1), 0, 0);
            rembarde.transform.localScale = new Vector3(1, 1, 1);
        }
        private void CatWalkSol(int index, float thickness)
        {
            if (thickness > 1)
            {
                for (int k = 1; k < thickness; k++)
                {
                    GameObject floorCatWalk = Instantiate(etageComposant[1]);
                    floorCatWalk.transform.parent = etages[index].transform;
                    floorCatWalk.transform.localPosition = Vector3.zero + new Vector3(-thicknessCoeff * k, 0, 0);
                }
            }
        }

        [Button("Revert")]
        private void Revert()
        {
            //delete constructed element
            int length = transform.childCount;
            for (int i = 0; i < length; i++)
            {
                if (transform.GetChild(i).name.StartsWith(cloneText))
                {
                    try { Destroy(transform.GetChild(i).gameObject); }
                    catch { DestroyImmediate(transform.GetChild(i).gameObject); }
                    length--;
                    i--;
                }
                
            }

            //prepare the empty for the next construct
            foreach (GameObject elem in volumesHolder)
            { elem.SetActive(true); }

            volumesHolder.Clear();

            planMode = true;
        }

        private string PointName(int index) => $"{cloneText}_{index}_Point";
    }


    public enum constructType
    {
        CatWalk,
        ImmeubleFerme,
        ImmeubleOuvert,
    }
}
