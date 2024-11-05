using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using City;

public class Playermovment : MonoBehaviour
{
    [Header("Player Movment")]
    public int speed;
    public int angle;
    public float height;
    private int speedFactor;
    private int angleFactor;
    private int heightIndex = 0;
    private float rotaX = 0;
    private float rotaY = 0;
    private bool mouseLock = true;


    [Header("Dialog interface")]
    public bool inDialog;
    public bool yes;
    public bool no;

    [Header("Settings")]
    public bool ruinner = false;
    public Sectorassembly networkCode;
    public RealityControler realityCode;
    public List<SectorDistributor> structureCodeList = new List<SectorDistributor>();

    [Header("Signal")]
    public GameObject antenna;
    private Antenna antennaCode;

    [Header("Grabbing System")]
    public Material ropeMat;
    public float grabForce;
    public GameObject anchorToFiregrab;
    private LineRenderer lineRenderer;
    public bool lineConnected;
    public Rigidbody rigid;
    private Vector3 destinationGrab;

    
    //Get Codes
    private void Start()
    {
        if(ruinner == false)
        {
            antennaCode = antenna.GetComponent<Antenna>();
            if(rigid == null) { rigid = gameObject.GetComponent<Rigidbody>(); }

            int nbDistrib = realityCode.gameObject.transform.childCount;
            structureCodeList.Clear();
            for (int i = 0; i < nbDistrib; i++)
            {
                if (realityCode.gameObject.transform.GetChild(i).gameObject.GetComponent<SectorDistributor>() != null)
                {
                    structureCodeList.Add(realityCode.gameObject.transform.GetChild(i).gameObject.GetComponent<SectorDistributor>());
                }
            }
        }
        
        if (structureCodeList.Count == 0) { Debug.LogWarning("NoStructureInCharge"); }
    }

    //Player physics and Perf
    private void Update()
    {
        if(ruinner == false)
        {
            //rigidBody Component
            rigid.isKinematic = !lineConnected;
            rigid.useGravity = lineConnected;

            //pulling system
            if (lineConnected)
            {
                lineRenderer.SetPosition(0, transform.position + new Vector3(-0.2f, -0.3f, -0.1f));
                if (Vector3.Distance(transform.position, destinationGrab) > 3) { rigid.AddForce(grabForce * Vector3.Normalize(destinationGrab - transform.position)); }
                else if (Vector3.Distance(transform.position, destinationGrab) < 2) { GestionLineRenderer(false); }
            }
        }
    }

    //Player mouvment
    void LateUpdate()
    {
        //Move Forward-Backward
        if (Input.GetKey("z"))//(sinRcosH, sinRsinH, cosR)
        {
            transform.position += new Vector3(speedFactor * Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad) * Mathf.Cos(-transform.eulerAngles.x * Mathf.Deg2Rad), 0, 0) * Time.deltaTime;
            transform.position += new Vector3(0, speedFactor * Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad) * Mathf.Sin(-transform.eulerAngles.x * Mathf.Deg2Rad), 0) * Time.deltaTime;
            transform.position += new Vector3(0, 0, speedFactor * Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad)) * Time.deltaTime;
        }
        else if (Input.GetKey("s"))
        {
            transform.position += new Vector3(-speedFactor * Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad) * Mathf.Cos(-transform.eulerAngles.x * Mathf.Deg2Rad), 0, 0) * Time.deltaTime;
            transform.position += new Vector3(0, -speedFactor * Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad) * Mathf.Sin(-transform.eulerAngles.x * Mathf.Deg2Rad), 0) * Time.deltaTime;
            transform.position += new Vector3(0, 0, -speedFactor * Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad)) * Time.deltaTime;
        }
        //Rotation Left-Right       R
        if (Input.GetKey("q"))
        {
            transform.eulerAngles += new Vector3(0, -angle, 0) * Time.deltaTime;
        }
        else if (Input.GetKey("d"))
        {
            transform.eulerAngles += new Vector3(0, angle, 0) * Time.deltaTime;
        }

        //Mouse Control
        if (Input.GetKeyDown("a")) { mouseLock = !mouseLock; }
        if (mouseLock == false) 
        {
            rotaX += Input.GetAxis("Mouse X") * angle;
            rotaY += Input.GetAxis("Mouse Y") * angle;
            transform.localEulerAngles = new Vector3(-rotaY, -rotaX, 0); 
        }

        //Adapt speed
        if (heightIndex == 0) { speedFactor = speed; }
        else 
        { 
            transform.position += new Vector3(0, heightIndex * 1.5f * Time.deltaTime, 0); 
            speedFactor = speed / 2 + 1;
        }
    }

    //Player Interactions 
    private void FixedUpdate()
    {
        //Maintain constant height 
        if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), height)) { heightIndex = 1; }
        else if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), height + 0.5f)) { heightIndex = 0; }
        else if (Physics.Raycast(transform.position, new Vector3(0, -1, 0), height + 1.5f)) { heightIndex = -1; }
        else { heightIndex = 0; }

        if(ruinner == false)
        {
            //NetWork Mouvment
            if (realityCode.reality == false)
            {
                if (realityCode.PIL)
                {
                    speedFactor += speed * 3;
                    angleFactor += angle * 2;
                    //Exit the network at anytime with the networkLift
                    if (Input.GetKey("f"))
                    {
                        realityCode.reality = true;
                    }
                }
                else
                {
                    speedFactor = speed;
                    angleFactor = angle;
                    //TP system for short range mvt
                    if (Input.GetKey("f"))
                    {
                        RaycastHit hit;
                        //Vector3 forwardVector = new Vector3(Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad) * Mathf.Cos(-transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad) * Mathf.Sin(-transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad));
                        Vector3 forwardVector = Vector3.Normalize(anchorToFiregrab.transform.position - transform.position);
                        if (Physics.Raycast(transform.position, forwardVector, out hit))
                        {
                            transform.position = hit.transform.position + new Vector3(0, 11, 0);
                            try
                            {
                                hit.transform.parent.gameObject.GetComponent<Node>().player = gameObject;
                                hit.transform.parent.gameObject.GetComponent<Node>().PerparePlayerArrive();

                            }
                            catch { }
                        }
                    }
                }
            }

            //antenna holding&posing
            if (Input.GetKeyDown("w"))
            {
                if (antennaCode.holdByPlayer)
                {
                    antennaCode.holdByPlayer = false;
                    antenna.transform.localPosition = antennaCode.positionPose.position;
                }
                else if (antennaCode.interactionPossible && antennaCode.holdByPlayer == false)
                {
                    antennaCode.holdByPlayer = true;
                }
            }

            //grab fireing
            if (Input.GetKeyDown("f") && realityCode.reality)
            {
                RaycastHit hit;
                //Vector3 forwardVector = new Vector3(Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad) * Mathf.Cos(-transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.y * Mathf.Deg2Rad) * Mathf.Sin(-transform.eulerAngles.x * Mathf.Deg2Rad), Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad));
                Vector3 forwardVector = Vector3.Normalize(anchorToFiregrab.transform.position - transform.position);
                if (Physics.Raycast(transform.position, forwardVector, out hit))
                {
                    Debug.Log("touché");
                    try
                    {
                        GestionLineRenderer(true, hit);
                    }
                    catch { }
                }
            }
            if (Input.GetKeyDown("g") && lineConnected)
            {
                GestionLineRenderer(false);
            }
        }
        
        //Dialog Answers
        if (inDialog)
        {
            if (Input.GetKeyDown("e"))
            {
                yes = true;
                no = false;
            }
            else if (Input.GetKeyDown("r"))
            {
                no = true;
                yes = false;
            }
            else
            {
                no = false;
                yes = false;
            }
        }
    }


    void GestionLineRenderer(bool needForALine, RaycastHit hit = new RaycastHit())
    {
        bool changeTheState = !needForALine;
        if (needForALine)
        {
            //For creating line renderer object
            if (lineConnected == false)
            {
                lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
                lineRenderer.material = ropeMat;
                lineRenderer.startWidth = 0.05f;
                lineRenderer.endWidth = 0.05f;
                lineRenderer.positionCount = 2;
                lineRenderer.useWorldSpace = true;
                lineConnected = true;

                changeTheState = true;
            }
            //For drawing line in the world space, provide the x,y,z values
            lineRenderer.SetPosition(0, transform.position + new Vector3(0, 0.1f, 0));
            lineRenderer.SetPosition(1, hit.point);
            destinationGrab = hit.point;
        }
        else
        {
            Destroy(lineRenderer);
            lineConnected = false;
        }

        //Warn the strucuture for the generation
        if (changeTheState)
        {
            foreach (SectorDistributor structureCode in structureCodeList)
            {
                foreach (SectorDistributor.SectorVariation sectVariation in structureCode.listVariation)
                {
                    if (sectVariation.replaceDefaultForPerf)
                    {
                        sectVariation.useSolConstruct = !needForALine;
                    }
                }
                structureCode.ArchiveRequestsAndVariations();
            }
        }
    }
}

