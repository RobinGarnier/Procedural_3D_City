using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace City
{
    public class ImmeubleConstruct : MonoBehaviour
    {
        [Header("Look")]
        [SerializeField, Min(1)] public int numberOfPoints = 1;
        [SerializeField, Min(0.01f)] public float space = 3.6101f;
        private float size = 1f;

        [Header("Behaviour")]
        [SerializeField] private bool swapEachFloor = false;
        [SerializeField, Range(0, 5)] private int presenceAd = 1;

        [Header("Object to set")]
        [SerializeField] private GameObject start;
        [SerializeField] private GameObject end;
        [SerializeField] private GameObject[] etages;
        [Tooltip("mettre les plus grandes à la fin")]
        [SerializeField] private GameObject[] pubs;

        [Header("Performance")]
        public List<bool> presenceEtage;
        public List<AppartCreation> presents;
        public bool PIB=false;
        public bool needUpdateUp;

        private AppartCreation codeAppart;
        private List<Transform> points;
        private List<Transform> connectors;

        [Header("Pub")]
        private GameObject pubAttach;
        public float lengthEtage = 35;
        public bool notEveryFloor;
        
        private int indexfloor;
        private bool flatRoof = true;

        private const string cloneText = "Etage";
        

        [Button("Reset points")]
        public void UpdatePoints()
        {
            if (!start || !end )
            {
                Debug.LogWarning("Can't update because one of objects to set is null!");
                return;
            }
            //choose the roof
            
            flatRoof = true;
            if (end.transform.childCount > 1)
            {
                for (int i = 0; i < end.transform.childCount; i++) { end.transform.GetChild(i).gameObject.SetActive(false); }

                int choixRoof = Random.Range(0, end.transform.childCount);
                end.transform.GetChild(choixRoof).gameObject.SetActive(true);
                if (choixRoof == 0) { flatRoof = false; }
            }

            // delete old floor
            int length = transform.childCount;
            for (int i = 0; i < length; i++)
            {
                if (transform.GetChild(i).name.StartsWith(cloneText))
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                    length--;
                    i--;
                }
            }
            //delete old pub
            try
            {
                int nbPub = pubAttach.transform.childCount;
                for (int i = 0; i < nbPub; i++)
                {
                    if (pubAttach.transform.GetChild(i).CompareTag("Wall Ad"))
                    {
                        DestroyImmediate(pubAttach.transform.GetChild(i).gameObject);
                        nbPub--;
                        i--;
                    }
                }
            }
            catch { }
            presenceEtage.Clear();
            presents.Clear();

            // set new
            presents.Add(start.GetComponent<AppartCreation>());
            presenceEtage.Add(new bool());
            start.GetComponent<AppartCreation>().EfficaceInit();
            Vector3 lastPos = start.transform.position;

            for (int i = 1; i < numberOfPoints; i++)
            {
                GameObject cPoint = i == 0 ? start : CreateNewPoint(i,ChooseType());

                Vector3 newPos = CountNewPointPos(lastPos);
                cPoint.transform.position = newPos;
                cPoint.transform.localScale = Vector3.one * size;

                if (swapEachFloor)
                {
                    if (indexfloor == 0)
                    {
                        indexfloor = 1;
                        cPoint.transform.rotation = end.transform.rotation;
                    }
                    else
                    {
                        indexfloor = 0;
                        cPoint.transform.rotation = end.transform.rotation;
                    }
                }
                else
                {
                    cPoint.transform.rotation = end.transform.rotation;
                }

                //integrate the floor to perf system&create correct playing space 
                if (i == numberOfPoints-1 && flatRoof) { cPoint.GetComponent<AppartCreation>().lastFloor = true; }
                else
                { cPoint.GetComponent<AppartCreation>().lastFloor = false; }
                //cPoint.GetComponent<AppartCreation>().Numerotation(i);
                cPoint.GetComponent<AppartCreation>().etageNumber = i;
                cPoint.GetComponent<AppartCreation>().EfficaceInit();

                //cPoint.GetComponent<AppartCreation>().PropGestion();
                presents.Add(cPoint.GetComponent<AppartCreation>());
                presenceEtage.Add(new bool());


                lastPos = newPos;
            }

            //roof on right position
            Vector3 endPos = CountNewPointPos(lastPos);
            end.transform.position = endPos;

            Vector3 CountNewPointPos(Vector3 lastPos) => lastPos + transform.up + new Vector3(0,space,0);

            //Wall Ad
            //found the attach
            for(int i=0; i < start.transform.childCount; i++)
            {
                if (start.transform.GetChild(i).name.StartsWith("PubAttach"))
                {
                    pubAttach=start.transform.GetChild(i).gameObject;
                }
            }

            for(int j=1; j < numberOfPoints; j++)
            {
                bool place = true;
                float parcoursEtage = -1 * (lengthEtage/2) + Random.Range(5, lengthEtage / 2);
                for (int k = 0; k < presenceAd; k++)
                {
                    if (notEveryFloor)
                    {
                        place = false;
                        if (Random.Range(0, presenceAd+1) >= 1)
                        {
                            place = true;
                        }
                    }
                    if (place)
                    {
                        int indexRandom = 0;
                        if (j <= numberOfPoints - 5) { indexRandom = Random.Range(0, pubs.Length); }
                        else { indexRandom = 0; }

                        GameObject ad = Instantiate(pubs[indexRandom],pubAttach.transform);
                        float scale = Random.Range(3, 7) * 0.01f;
                        ad.transform.localScale = new Vector3(scale, scale, scale);
                        ad.GetComponent<pubParametre>().UpdateWidth();
                        
                        if (parcoursEtage >= (lengthEtage - ad.GetComponent<pubParametre>().width))
                        { 
                            place = false;
                            DestroyImmediate(ad);
                            break; 
                        }
                        try { ad.transform.position = pubAttach.transform.position + new Vector3(parcoursEtage*Mathf.Cos(transform.localEulerAngles.y * Mathf.Deg2Rad), j * space, parcoursEtage * Mathf.Sin(transform.localEulerAngles.y * Mathf.Deg2Rad)); } catch { }
                        parcoursEtage += ad.GetComponent<pubParametre>().width + Random.Range(0, lengthEtage/(presenceAd+1));
                    }
                }
            }
        } 

        [Button("Add point")]
        private void AddPoint()
        {
            Transform lastprevPoint = GetPoint(numberOfPoints - 1);
            if (lastprevPoint == null)
            {
                Debug.LogWarning("Dont found point number " + (numberOfPoints - 1));
                return;
            }

            GameObject cPoint = CreateNewPoint(numberOfPoints,ChooseType());
            if (swapEachFloor)
            {
                if (indexfloor == 0)
                {
                    indexfloor = 1;
                    cPoint.transform.rotation = end.transform.rotation; // new Quaternion(end.transform.rotation.x, end.transform.rotation.y + rotation, end.transform.rotation.z, 0);,
                }
                else
                {
                    indexfloor = 0;
                    cPoint.transform.rotation = end.transform.rotation;
                }
            }
            else
            {
                cPoint.transform.rotation = end.transform.rotation;
            }
            cPoint.GetComponent<AppartCreation>().Numerotation(numberOfPoints);

            cPoint.transform.position = end.transform.position;
            
            cPoint.transform.localScale = Vector3.one * size;

            cPoint.GetComponent<AppartCreation>().EfficaceInit();
            cPoint.GetComponent<AppartCreation>().PropGestion();
            presents.Add(cPoint.GetComponent<AppartCreation>());
            presenceEtage.Add(new bool());

            // fix end
            end.transform.position += end.transform.up + new Vector3(0,space,0);

            numberOfPoints++;
        }

        [Button("Remove point")]
        private void RemovePoint()
        {
            if (numberOfPoints < 2)
            {
                Debug.LogWarning("Cable can't be shorter than 1");
                return;
            }

            Transform lastprevPoint = GetPoint(numberOfPoints - 1);
            if (lastprevPoint == null)
            {
                Debug.LogWarning("Dont found point number " + (numberOfPoints - 1));
                return;
            }

            Transform lastprevCon = GetConnector(numberOfPoints);

            Transform lastlastprevPoint = GetPoint(numberOfPoints - 2);
            if (lastlastprevPoint == null)
            {
                Debug.LogWarning("Dont found point number " + (numberOfPoints - 2));
                return;
            }
            presents.RemoveAt(numberOfPoints-1);
            presenceEtage.RemoveAt(numberOfPoints-1);
            numberOfPoints--;

            end.transform.position = lastprevPoint.position;
            end.transform.rotation = lastprevPoint.rotation;

            DestroyImmediate(lastprevPoint.gameObject);
            DestroyImmediate(lastprevCon.gameObject);
        }


        private Vector3 CountConPos(Vector3 start, Vector3 end) => (start + end) / 2f;
        private Vector3 CountSizeOfCon(Vector3 start, Vector3 end) => new Vector3(size, size, (start - end).magnitude / 2f);
        private Quaternion CountRoationOfCon(Vector3 start, Vector3 end) => Quaternion.LookRotation(end - start, Vector3.right);
        private string ConnectorName(int index) => $"{cloneText}_{index}_Conn";
        private string PointName(int index) => $"{cloneText}_{index}_Point";
        private Transform GetConnector(int index) => index > 0 ? transform.Find(ConnectorName(index)) : start.transform;
        private Transform GetPoint(int index) => index > 0 ? transform.Find(PointName(index)) : start.transform;


        private int ChooseType()
        {
            if (swapEachFloor)
            {
                return (int)Random.Range(0f,etages.Length);
            }
            else
            {
                return 0;
            }
        }
        private GameObject CreateNewPoint(int index,int type)
        {
            GameObject temp = Instantiate(etages[type]);
            temp.name = PointName(index);
            temp.transform.parent = transform;
            return temp;
        }
        private GameObject CreateNewCon(int index)
        {
            GameObject temp = Instantiate(start);
            temp.name = ConnectorName(index);
            temp.transform.parent = transform;
            return temp;
        }


        public void PerformPIB()//PIB=Player In Building
        {
            //ajust activness of elemnt according to the player position
            for (int i = 0; i < presents.Count; i++)
            {
                presenceEtage[i] = presents[i].present;

                presents[i].eteint = true;
                if (presenceEtage[i])
                { presents[i].eteint = false; }
                try
                { if (presenceEtage[i - 1]) { presents[i].eteint = false; } }
                catch{ }
                try
                { if (presenceEtage[i + 1]) { presents[i].eteint = false; } }
                catch { }
               

                //update PIB & alert up
                bool pastPIB = PIB;
                PIB = false;
                foreach(bool pres in presenceEtage)
                {
                    if (pres) { PIB = true; }
                }

                if (pastPIB != PIB)
                {
                    needUpdateUp = true;
                }
            }
            
        }


        public void Update()
        {
            PerformPIB();
            for(int i = 0; i < presenceEtage.Count; i++)
            {
                if (presenceEtage[i])
                {
                    try
                    {
                        AppartCreation appartScript = presents[i + 1];
                        appartScript.playerScript = presents[i].perf.playerScript;
                    }
                    catch { }
                }
            }
        }
    }
}