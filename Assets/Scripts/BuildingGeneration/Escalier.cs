using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escalier : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private GameObject wall;
    [SerializeField] private Transform transformDetector;
    public bool walled;
    public bool comble;

    [Header("ElevationSettings")]
    [SerializeField,Range(0,3)] public int height;
    private Vector3 positionFondation;
    private Vector3 positionComble;
    [SerializeField] private GameObject comb;
    public GameObject combHolder;
    [SerializeField] private GameObject fondation;
    [SerializeField] private GameObject fondationHolder;

    public void Updated()
    {
        void CheckNeedElem(bool need, GameObject elem)
        {
            if (need)
            {
                elem.SetActive(true);
            }
            else { elem.SetActive(false); }
        }

        void SpawnUnderElem(GameObject model, GameObject holder, Vector3 position)
        {
            GameObject elem = Instantiate(model);
            elem.transform.position = position + model.transform.position;
            elem.transform.parent = holder.transform;
            elem.transform.localEulerAngles = model.transform.localEulerAngles;
            elem.transform.localScale = new Vector3(model.transform.localScale.x, fondation.transform.localScale.y, model.transform.localScale.z);
        }

        void DeleteChildrenExptOne(GameObject holder)
        {
            int childHolder = holder.transform.childCount;
            for (int i = 1; i < childHolder; i++)
            {
                DestroyImmediate(holder.transform.GetChild(i).gameObject);
                childHolder--;
                i--;
            }
        }

        //check bool parameter
        CheckNeedElem(walled, wall);
        CheckNeedElem(comble, combHolder);

        //check height
        DeleteChildrenExptOne(fondationHolder);
        fondation.SetActive(false);
        DeleteChildrenExptOne(combHolder);

        if (height > 0)
        {
            positionFondation = Vector3.zero;
            positionComble = Vector3.zero;
            fondation.SetActive(true);

            positionComble += new Vector3(0, -2.86f, 0);
            SpawnUnderElem(comb, combHolder, positionComble);

            for (int i = 0; i < height-1; i++)
            {
                positionFondation += new Vector3(0, -fondation.transform.localScale.y, 0);
                SpawnUnderElem(fondation, fondationHolder, positionFondation);

                positionComble += new Vector3(0, -fondation.transform.localScale.y, 0);
                SpawnUnderElem(comb, combHolder, positionComble);
            }
        }
    }

    public void CheckNeighbours()
    {
        foreach (Collider collider in Physics.OverlapSphere(transformDetector.position, 2f))
        {
            if (collider.gameObject.name.StartsWith("Cube") || collider.gameObject.name.StartsWith("Comble")) 
            {
                GameObject colliderHolder = collider.gameObject.transform.parent.gameObject;
                Escalier escalier = colliderHolder.GetComponentInParent<Escalier>();
                escalier.comble = false; 
                escalier.combHolder.SetActive(false);
                break;
            }
        }
    }
}
