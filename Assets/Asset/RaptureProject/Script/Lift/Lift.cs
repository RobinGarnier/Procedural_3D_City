using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using City;

public class Lift : MonoBehaviour
{
    public bool playerInLift;
    public bool doorOpen;
    public bool openOrderVoid;//forOpenTheDoor
    public bool liftRoomActive;
    private bool doorMoving;
    private Vector3 doorDestination;

    private GameObject referencePos;
    public GameObject door;
    public GameObject liftRoom;

    [Header("PlayingSettings")]
    public bool startAtA500 = false;

    public void OpenTheDoor(bool openOrder)
    {
        openOrderVoid = openOrder;
        if(openOrder != doorOpen)
        {
            _ = openOrder ? doorDestination = referencePos.transform.Find("PositionDoorO").position : doorDestination = referencePos.transform.Find("PositionDoorC").position;
            doorMoving = true;
        }
    }

    private void Update()
    {
        if (doorMoving)
        {
            if (Vector3.Distance(door.transform.position, doorDestination) < 0.1f)
            {
                doorMoving = false;
                door.transform.position = doorDestination;
                doorOpen = openOrderVoid;
            }
            else
            {
                _ = openOrderVoid ? door.transform.position += new Vector3(1.3f, 0, 0) * Time.deltaTime : door.transform.position += new Vector3(-1.3f, 0, 0) * Time.deltaTime;
            }
        }
        if (startAtA500 && GetComponentInParent<LiftControl>().structure.GetComponent<Sectorassembly>().waitForTheFirstGen == false) { transform.Find("Canvas").GetChild(0).GetComponent<InputFieldLift>().MoveliftToDestination(); startAtA500 = false; }

        GetComponentInParent<LiftControl>().realityControllerCode.PIL = playerInLift;
    }

    void Start()
    {
        referencePos = transform.GetChild(0).gameObject;
        liftRoom = transform.Find("LiftSpace").gameObject;
    }

    //Check Player Presence
    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player") { playerInLift = true; }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player") { playerInLift = false; }
    }
}
