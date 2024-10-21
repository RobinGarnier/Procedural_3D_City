using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkLift : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 1; i < 4; i++)
        {
            transform.GetChild(i).eulerAngles += Time.deltaTime * new Vector3(0, 20, 20);
        }
    }
}
