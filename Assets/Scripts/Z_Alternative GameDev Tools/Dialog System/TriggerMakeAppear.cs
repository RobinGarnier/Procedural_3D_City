using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMakeAppear : MonoBehaviour
{

    //test
    public GameObject objectAppear;
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            objectAppear.SetActive(true);
        }
    }

    private void Update()
    {
        if(Vector3.Distance(player.transform.position, transform.position)>10)
        {
            objectAppear.SetActive(false);
        }
    }
}
