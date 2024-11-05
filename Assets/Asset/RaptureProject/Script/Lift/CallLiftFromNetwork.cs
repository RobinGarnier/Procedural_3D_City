using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallLiftFromNetwork : MonoBehaviour
{
    public RealityControler realityControler;
    public LiftControl liftController;
    public InputFieldLift translator;
    public GameObject privateRoom;
    public bool fromNetwork;
    public bool enterNetworkAutorize = true;
    public bool callLift = false;
    public bool enterPrivateRoom = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (fromNetwork)
            {
                if (enterPrivateRoom)
                {
                    privateRoom.SetActive(false);
                }
                else
                {
                    realityControler.reality = true;
                    if (callLift) { liftController.Control(translator.translateStringToCoord(transform.parent.parent.name)); }
                }
            }
            else
            {
                if (enterPrivateRoom)
                {
                    privateRoom.SetActive(true);
                }
                else if (enterNetworkAutorize)
                {
                    realityControler.reality = false;
                    enterNetworkAutorize = false;
                }
                else { enterNetworkAutorize = true; }
            }
        }
    }
}
