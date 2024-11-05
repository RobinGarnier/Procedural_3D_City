using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using City;

public class RealityControler : MonoBehaviour
{
    [Header("Player&Others")]
    public ControlerType playerType = ControlerType.Pray;
    public GameObject player;
    private GameObject realAsset;
    private GameObject netAsset;

    [Header("City")]
    public GameObject structureParent;
    public GameObject network;
    private Sectorassembly networkCode;
    private Sectorassembly structureCode;

    [Header("Controler")]
    public bool reality = true;
    public bool PIL = false;
    public RealityControler CameraRController;
    private bool inPrivateSpace;

    private Vector3 positionPlayerWhenLeaveR;
    private Vector3 rotationPlayerWhenLeaveR;

    private bool updatedVersion = false;


    void AdaptRealityToNeed(bool needReal)
    {
        if (needReal)
        {
            //Reposition Player
            player.transform.position = positionPlayerWhenLeaveR;
            player.transform.eulerAngles = rotationPlayerWhenLeaveR;

            //Adapt reality
            realAsset.SetActive(true);
            structureParent.SetActive(true);
            netAsset.SetActive(false);
            network.SetActive(false);
        }
        else
        {
            //Save player Position
            positionPlayerWhenLeaveR = player.transform.position;
            rotationPlayerWhenLeaveR = player.transform.eulerAngles;

            //Adapt reality
            bool openingCinematicFlag = structureParent.transform.GetChild(0).GetComponent<Sectorassembly>().playerInSector;
            netAsset.SetActive(true);
            network.SetActive(true);
            netAsset.transform.Find("NetworkLift").gameObject.SetActive(PIL);
            realAsset.SetActive(false);
            structureParent.SetActive(false);

            try { networkCode.ActivateNodalSystem(); } catch { Debug.LogWarning("Probleme de SectorAssembly : Check le constructeur et les lists archives"); }
            AdaptToNetwork(openingCinematicFlag);
        }
    }

    private void AdaptToNetwork(bool needOpenCinematic)
    {
        if (PIL)
        { network.transform.GetChild(0).Find(structureCode.SectorNameCoordViaDim(new Vector3(1, 0, 0) + structureCode.positionPlayerCoordinate())).GetComponent<Node>().startingStateActive = true; }
        else { network.transform.GetChild(0).Find(structureCode.SectorNameCoordViaDim(structureCode.positionPlayerCoordinate())).GetComponent<Node>().startingStateActive = true; }
        player.transform.position = network.transform.GetChild(0).Find(structureCode.SectorNameCoordViaDim(structureCode.positionPlayerCoordinate())).GetChild(0).position;
        _ = PIL ? player.transform.position += new Vector3(15+115, 0, 0) : player.transform.position += new Vector3(0, 11, 0);
        network.transform.GetChild(0).Find(structureCode.SectorNameCoordViaDim(structureCode.positionPlayerCoordinate())).GetComponent<Node>().player = player;
        //network.transform.GetChild(0).Find(structureCode.SectorNameCoordViaDim(structureCode.positionPlayerCoordinate())).GetComponent<Node>().openingCinematic = needOpenCinematic;
        network.transform.GetChild(0).Find(structureCode.SectorNameCoordViaDim(structureCode.positionPlayerCoordinate())).GetComponent<Node>().PerparePlayerArrive();
    }

    void Start()
    {
        realAsset = player.transform.GetChild(0).gameObject;
        netAsset = player.transform.GetChild(1).gameObject;

        if (playerType == ControlerType.Player)
        {
            positionPlayerWhenLeaveR = player.transform.position;
            rotationPlayerWhenLeaveR = player.transform.eulerAngles;

            networkCode = network.transform.GetChild(0).gameObject.GetComponent<Sectorassembly>();
            structureCode = structureParent.transform.GetChild(0).GetComponent<Sectorassembly>();
        }
           
        //AdaptRealityToNeed(reality);
    }

    void LateUpdate()
    {
        if(playerType == ControlerType.Player)
        {
            if (updatedVersion != reality)
            {
                updatedVersion = reality;
                AdaptRealityToNeed(reality);
            }
        }
        else if(playerType == ControlerType.Pray)
        {
            reality = CameraRController.reality;
            if (CameraRController.reality)
            {
                realAsset.SetActive(true);
                netAsset.SetActive(false);
            }
            else
            {
                realAsset.SetActive(false);
                netAsset.SetActive(true);
            }
        }
    }
}

public enum ControlerType
{
    Player,
    Hunter,
    Pray,
}
