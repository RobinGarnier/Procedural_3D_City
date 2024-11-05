using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


namespace City
{
    public class CityControl : MonoBehaviour
    {
        public List<GameObject> baseImmeuble;
        [SerializeField, Min(1)] public int nbFloor;

        bool nonUpdated=true;
        public List<ImmeubleConstruct> imConstructList;

        [Button("Construct")]
        public void Construct()
        {
            if (nonUpdated)
            {
                UpdateValue();
            }

            foreach(ImmeubleConstruct partCode in imConstructList)
            {
                partCode.numberOfPoints = nbFloor;
                partCode.UpdatePoints();
            }

        }
        [Button("Update the Base")]
        public void UpdateValue()
        {
            imConstructList.Clear();
            for(int i = 0; i < baseImmeuble.Count; i++)
            {
                imConstructList.Add(baseImmeuble[i].GetComponent<ImmeubleConstruct>());
            }
            nonUpdated = false;
        }
    }
}

