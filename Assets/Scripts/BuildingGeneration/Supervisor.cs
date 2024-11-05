using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using City;

public class Supervisor : MonoBehaviour
{
    [Header("Paramèred'exploration")]
    [SerializeField] private bool assitedExplo;
    [SerializeField, Range(1, 5)] private int helpIntensity;
    public int floorExplored=0;
    public int foundedLevel;

    public AppartCreation appartScript;
    public ImmeubleConstruct imSript;
    public int heightCurrentSFloor = 1;


    public void ChooseAppart(GameObject appart)
    {
        if (assitedExplo)
        {
            appartScript = appart.GetComponent<AppartCreation>();
            floorExplored++;

            if(foundedLevel < 2) { appartScript.ChoisirAlternative(); }
        }
    }

    // Update is called once per frame
    void Start()
    {
        heightCurrentSFloor = 1;
    }

    public void Update()
    {
        
    }
}
