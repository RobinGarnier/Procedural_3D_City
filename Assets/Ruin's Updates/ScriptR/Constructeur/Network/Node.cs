using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    public int probaSpawn=6;
    public bool startingStateActive;
    public TMP_Text[] nameList;
    public GameObject rotativeName;

    public bool openingCinematic = false;
    private int direction = 1;
    public bool nodeClose = true;
    private List<Vector3> listPositionBloc;

    public bool PIN;
    public GameObject player;

    
    void Start()
    {
        if(transform.parent.name == "SectorActive")
        { startingStateActive = true; }
        else { startingStateActive = Random.Range(0, probaSpawn) == 0 ? true : false; }
        
        transform.GetChild(0).gameObject.SetActive(startingStateActive); gameObject.GetComponent<BoxCollider>().enabled = startingStateActive;

        foreach(TMP_Text name in nameList)
        {
            name.text = transform.name;
        }
        rotativeName.SetActive(false);
        nodeClose = true;
    }

    private void Update()
    {
        if(startingStateActive && transform.GetChild(0).gameObject.activeSelf == false) { transform.GetChild(0).gameObject.SetActive(true); gameObject.GetComponent<BoxCollider>().enabled = startingStateActive; }

        if (PIN)
        {
            //rotativeName.transform.eulerAngles = new Vector3(0, Vector3.Angle(player.transform.position, transform.position), 0);
        }

        if (openingCinematic)
        {
            listPositionBloc = new List<Vector3>();
            GameObject blocHolder = transform.GetChild(0).GetChild(0).gameObject;
            for(int i = 0; i < 5; i++)
            {
                listPositionBloc.Add(blocHolder.transform.GetChild(i).position);
            }

            if (blocHolder.transform.GetChild(4).localPosition.y > 5.6f && direction == 1 || blocHolder.transform.GetChild(4).localPosition.y < 9.6f && direction == -1)
            {
                blocHolder.transform.GetChild(4).localPosition += -Time.deltaTime * direction * new Vector3(0, 1.5f, 0);

                blocHolder.transform.GetChild(0).localPosition += Time.deltaTime * direction * new Vector3(-2, 0.5f, -2);
                blocHolder.transform.GetChild(1).localPosition += Time.deltaTime * direction * new Vector3(-2, 0.5f, 2);
                blocHolder.transform.GetChild(2).localPosition += Time.deltaTime * direction * new Vector3(2, 0.5f, 2);
                blocHolder.transform.GetChild(3).localPosition += Time.deltaTime * direction * new Vector3(2, 0.5f, -2);
            }
            else
            {
                if(direction == -1)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        blocHolder.transform.GetChild(i).position = listPositionBloc[i];
                    }
                    transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                    nodeClose = true;
                }
                openingCinematic = false;
            }
        }
    }

    public void PerparePlayerArrive()
    {
        startingStateActive = true;
        PIN = true;
        rotativeName.SetActive(true);
    }

    public void OnTriggerStay(Collider other)
    {
        startingStateActive = true;
        if (other.gameObject.CompareTag("Player") && openingCinematic == false)
        {
            if (Input.GetKey("r") && nodeClose)
            {
                openingCinematic = true;
                direction = 1;
                transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                nodeClose = false;
            }
            else if (Input.GetKey("r"))
            {
                openingCinematic = true;
                direction = -1;
            }
        }
    }
}
