using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Performance : MonoBehaviour
{
    public bool present;
    public bool arrived;
    public Supervisor playerScript;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            present = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            present = false;
            arrived = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            arrived = true;
            playerScript = other.gameObject.GetComponent<Supervisor>();
        }
    }
}
