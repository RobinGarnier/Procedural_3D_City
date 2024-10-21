using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using City;
using NaughtyAttributes;

public class LiftControl : MonoBehaviour
{
    [Header("Control")]
    private Vector3 destination;
    //public Vector3 destinationSector;//for Control
    private Transform destinationPosition;
    public bool reached;
    public bool liftOrdered = false;
    private bool liftMoved;
    private Vector3 lastDestination;
    private bool firstTrip=true;
    private Vector3 destinationOrder = Vector3.zero;


    [Header("Parameters")]
    public GameObject player;
    public GameObject structure;
    public GameObject lift;
    public Lift liftCode;
    public RealityControler realityControllerCode;
    public InputFieldLift codeTarnslateSToCoord;
    public DialogAI dialogLiftCode;
    public Antenna antennaCode;

    //[Button("Launch")]
    public void Control(Vector3 destinationSector)
    {
        if (destinationSector != destination)
        {
            //Check archive
            if (firstTrip) { firstTrip = false; }

            //prepare the destination to the arrival of the lift
            if (structure.transform.Find(structure.GetComponent<Sectorassembly>().SectorNameCoordViaDim(destinationSector)) == null)
            {
                structure.GetComponent<Sectorassembly>().CreateSector(destinationSector, 0, 0, structure);
            }
            try 
            {
                GameObject sectorDestination = structure.transform.Find(structure.GetComponent<Sectorassembly>().SectorNameCoordViaDim(destinationSector)).gameObject;
                Debug.Log("found");
                sectorDestination.GetComponent<SolConstruct>().liftPlateform = true;
                sectorDestination.GetComponent<SolConstruct>().Position();
                Debug.Log("ready");
                try { lastDestination = codeTarnslateSToCoord.translateStringToCoord(destinationPosition.parent.parent.parent.name); } catch { }
                destinationPosition = sectorDestination.transform.GetChild(0).Find("ArrivéeLift").Find("PositionLift");
                Debug.Log("set");
            }
            catch { Debug.Log("fail"); return; }
            
            //move the lift
            liftCode.OpenTheDoor(false);
            liftOrdered = true;
        }
    }

    void LiftMove()
    {
        destination = destinationPosition.position;
        liftMoved = true;
        liftCode.liftRoom.SetActive(false);
        if (liftCode.playerInLift)
        {
            player.transform.position = destination;// + (player.transform.position - lift.transform.GetChild(0).Find("PositionPlayer").position);
            lift.transform.position = destination;
            reached = true;
        }
        else
        {
            lift.transform.position = destinationPosition.position + new Vector3(0, Mathf.Sign(Mathf.Abs(lift.transform.position.y) - Mathf.Abs(destinationPosition.position.y)) * 200, 0);
            reached = false;
        }
    }

    private void Start()
    {
        liftCode = lift.GetComponent<Lift>();
        dialogLiftCode = lift.transform.Find("LiftSpace").Find("Interfaces").Find("Selector").gameObject.GetComponent<DialogAI>();
        try { gameObject.name = transform.parent.parent.parent.parent.parent.parent.name; } catch { }//Canevas Button CallLift ArrivéeLift Prefab A-5,0.0
    }

    private void Update()
    {
        //Lift move when the door are closed
        if(liftCode.doorOpen == false && liftOrdered)
        {
            LiftMove();
            liftOrdered = false;
        }

        //Lift arrival Cinematic
        if(reached == false)
        {
            if (Vector3.Distance(lift.transform.position, destination)<5) 
            {
                reached = true;
                lift.transform.position = destination;
            }
            else
            {
                try { lift.transform.position += new Vector3(0, Mathf.Sign(destination.y - lift.transform.position.y) * 7 * Vector3.Distance(lift.transform.position, destinationPosition.position), 0) * Time.deltaTime; } catch { }
            }
            
        }

        //lift open the door when arrived
        if(liftMoved && reached)
        {
            liftCode.OpenTheDoor(true);
            liftMoved = false;
        }

        try { lift.SetActive(realityControllerCode.reality); } catch { }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "chip") 
        {
            Debug.Log("entrée");
            lift.transform.GetComponentInParent<LiftControl>().Control(antennaCode.target.sectorCoordConnexion);

            //Control(codeTarnslateSToCoord.translateStringToCoord(transform.name));
            //realityControllerCode.reality = true;
            other.gameObject.SetActive(false);
            antennaCode.giveBackAnchor = transform.GetChild(0).Find("LiftSpace").Find("Interfaces").Find("ChipReader").gameObject;
        }
    }

    //EventForDialog
    public void SendLiftToRandomLocation()
    {
        Control(new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10)));
    }

    public void SendLiftToChipLocation()
    {
        Control(destinationOrder);
    }

    public void SendLiftToLastLocation()
    {
        Control(lastDestination);
    }

    public void AllowGoingBack()
    {
        if (firstTrip)
        {

        }
    }
}
