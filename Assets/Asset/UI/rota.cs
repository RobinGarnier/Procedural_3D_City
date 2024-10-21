using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rota : MonoBehaviour
{
    public GameObject objectToRotate; // The GameObject to rotate
    public float rotationSpeed = 60f; // Rotation speed in degrees per second

    void Update()
    {
        if (objectToRotate != null)
        {
            // Calculate the rotation amount based on the rotation speed and frame time
            float rotationAmount = rotationSpeed * Time.deltaTime;

            // Rotate the object around its local Z-axis
            objectToRotate.transform.Rotate(Vector3.forward, rotationAmount);
        }
    }
}
