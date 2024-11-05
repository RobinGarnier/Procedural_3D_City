using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTransfer : MonoBehaviour
{
    [Header("Type")]
    public NetType type;
    public int ipAdress;
    public bool connected;
    private NetworkId netId;
    private int indexinNetList;

    [Header("Settings")]
    public List<NetworkId> listConnectedEntity = new List<NetworkId>();
    public DataTransfer dataHolderCode;


    // Start is called before the first frame update
    void Start()
    {
        if(type == NetType.connectable)
        {
            netId = new NetworkId(Vector3.zero, false, ipAdress);
            dataHolderCode.listConnectedEntity.Add(netId);
            indexinNetList = dataHolderCode.listConnectedEntity.Count - 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(type == NetType.connectable)
        {
            Vector3 answer = new(transform.position.x / 150, (transform.position.y + 185) / 40, transform.position.z / 150);
            //assure la cohérance en negatif
            for (int i = 0; i < 3; i++) { if (answer[i] < 0) { answer[i] += -1; } }
            netId.sectorCoordConnexion = new((int)answer[0], (int)answer[1], (int)answer[2]);

            netId.connected = connected;

            dataHolderCode.listConnectedEntity[indexinNetList] = netId;
        }
    }

    [System.Serializable]
    public class NetworkId
    {
        public Vector3 sectorCoordConnexion;
        public bool connected;
        public int ipAdress;

        public NetworkId(Vector3 coordConnexionSec, bool con, int ip)
        {
            sectorCoordConnexion = coordConnexionSec;
            connected = con;
            ipAdress = ip;
        }
    }

}

public enum NetType
{
    dataHolder,
    connectable,
}
