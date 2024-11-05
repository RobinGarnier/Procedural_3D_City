using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antenna : MonoBehaviour
{
    [Header("Antenna")]
    public bool holdByPlayer;
    public Transform positionOnPlayer;
    public Transform positionPose;
    public bool interactionPossible;

    [Header("Data_Connexion")]
    public DataTransfer netDataHolderCode;
    public bool connected;
    public DataTransfer.NetworkId target;
    public Vector3 targetCoord= new Vector3();
    private List<bool> coordFound = new List<bool>() { false, false, false };

    [Header("Chip")]
    public bool haveTheChip = true;
    public GameObject chip;
    public GameObject giveBackAnchor;
    private Vector3 locationStored = Vector3.zero;
    

    void Start()
    {
        //netAntennaDataCode = GetComponent<DataTransfer>();
    }

    
    void Update()
    {
        if (holdByPlayer)
        {
            transform.position = positionOnPlayer.position;
            interactionPossible = false;
            gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            gameObject.GetComponent<BoxCollider>().enabled = true;
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            interactionPossible = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && holdByPlayer == false) { interactionPossible = true; }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") { interactionPossible = false; }
    }

    public void Listen()
    {
        foreach(DataTransfer.NetworkId netid in netDataHolderCode.listConnectedEntity)
        {
            if (netid.connected)
            {
                target = netid;
                Debug.Log(target.ipAdress);
                locationStored = target.sectorCoordConnexion;
                break;
            }
        }
    }

    public void GiveChipBack()
    {
        if (haveTheChip)
        {
            chip.SetActive(true);
            haveTheChip = false;
            chip.transform.position = giveBackAnchor.transform.position;
        }
    }
}
