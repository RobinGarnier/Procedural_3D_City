using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using HPhysic;
using Random = System.Random;
using NaughtyAttributes;
using City;

public class AppartCreation : MonoBehaviour
{
    [Header("Look")]
    public TMP_Text[] numeroAppart;
    public GameObject propsHolder;
    public List<GameObject> propsCouloir;
    public GameObject escal;
    public bool lastFloor;
    public int indexFloorSelect = 0;
    [SerializeField, Range(1f, 10f)] private int crowded = 0;
    public Supervisor playerScript;
    [SerializeField] private int heightMaxSpecialFloor;

    [Header("Efficiency & Perf")]
    public GameObject DetectCodeHolder;
    public GameObject deletable;
    public GameObject[] deletablePrefab;
    public Vector3 dimension;
    public bool efficienceActivitated;
    public Performance perf;
    public bool present;
    public bool eteint = true;
    public bool fillerInstanciated;
    public BoxCollider boxColid;
    public int etageNumber;
    public bool isFiller;

    // Start is called before the first frame update
    public void EfficaceInit()
    {
        if (efficienceActivitated)
        {
            //BoxCollider boxColid = DetectCodeHolder.AddComponent<BoxCollider>(); ;
            
            boxColid = DetectCodeHolder.GetComponent<BoxCollider>();

            boxColid.isTrigger = true;
            boxColid.size = dimension;

            //DetectCodeHolder.AddComponent<Performance>();
            perf = DetectCodeHolder.GetComponent<Performance>(); 
        }
    }

    public void Numerotation(int etage)
    { 
        int i = 0;
        foreach(TMP_Text texte in numeroAppart)
        {
             texte.text= (etage*10 + i).ToString();
            i++;
        }
        
    }

    [Button("Reset Props")]
    public void PropGestion() 
    {
        //prepare the new arrangement
        try
        {
            foreach (GameObject props in propsCouloir)
            {
                props.SetActive(true);
            }
        }
        catch { }
        propsCouloir.Clear();
        
        //create the arrangement
        try
        {
            Transform[] ts = propsHolder.GetComponentsInChildren<Transform>();
           
            foreach (Transform trans in ts)
            {
                if (trans.gameObject.tag == "Props")
                {
                    propsCouloir.Add(trans.gameObject);
                    trans.gameObject.SetActive(false);
                }
            }
            
        
            foreach (GameObject props in propsCouloir)
            {
                props.SetActive(false);
                Random rdn = new Random();
                if ((int)rdn.Next(crowded) == 0)
                {
                    props.SetActive(true);
                }
            }
        }
        catch { }

        if (lastFloor)
        { try { escal.SetActive(false); } catch { }}
    }

    public void ChoisirAlternative()
    {
        if (deletablePrefab.Length > 1 )
        {
            ImmeubleConstruct imScript = gameObject.GetComponentInParent<ImmeubleConstruct>();
            if (playerScript.foundedLevel == 0)
            {
                playerScript.imSript = imScript;
                playerScript.foundedLevel = 1;
            }

            if (imScript == playerScript.imSript)
            {
                indexFloorSelect = playerScript.heightCurrentSFloor;
            }
            if (playerScript.heightCurrentSFloor >= heightMaxSpecialFloor) { playerScript.foundedLevel = 2; }
            else { playerScript.heightCurrentSFloor++; }
        }
    }

    public void Start()
    {
        if (isFiller == false) { EfficaceInit(); }
    }

    public void Update()
    {
        if(isFiller == false)
        {
            //perf
            try { present = perf.present; } catch { }
            try { if (gameObject.GetComponentInParent<ImmeubleConstruct>().PIB && etageNumber == 1) { eteint = false; } } catch { }

            if (eteint && fillerInstanciated)
            {
                deletable.SetActive(false);
            }
            else if(eteint == false)
            {
                //choix d'appart
                if (fillerInstanciated == false)
                {
                    try { playerScript.ChooseAppart(gameObject); } catch { }
                    perf.arrived = false;
                }

                if (fillerInstanciated == false)
                {
                    GameObject filler = Instantiate(deletablePrefab[indexFloorSelect],transform);
                    filler.transform.parent = transform;
                    filler.transform.localPosition = Vector3.zero;

                    deletable = filler;

                    AppartCreation fillerScript = filler.GetComponent<AppartCreation>();
                    fillerScript.lastFloor = lastFloor;
                    try { fillerScript.Numerotation(etageNumber); } catch { }
                    fillerScript.PropGestion();
                    fillerScript.isFiller = true;

                    fillerInstanciated = true;
                }

                deletable.SetActive(true);
            }
        }
    }
}
