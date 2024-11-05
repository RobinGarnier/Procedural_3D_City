using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionAppart : MonoBehaviour
{
    public GameObject piece;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CarteStaff")
        {
            piece.SetActive(true);
            try
            {
                //porte.GetComponent<Rigidbody>().isKinematic=false;
            }
            catch { }
        }
    }
}
