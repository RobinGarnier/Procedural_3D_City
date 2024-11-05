using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class DialogAI : MonoBehaviour
{
    [Header("Dialog")]
    public List<DialogExchange> exchangeTree;
    public bool inExchange;
    public bool waitForAnswer;
    public int indexConv;
    public float timer;
    public bool workingTimer = false;
    public bool firstTimeWorkingTimer = true;
    private bool endConv = false;
    public int indexConvforNewExchange = 0;

    [Header("Player Interaction")]
    public GameObject player;
    private TMP_Text textPlayer;
    private GameObject dialogBox;
    private Playermovment playerCode;

    [Header("Conditional_Triggers")]
    public List<DialogCondition> listConditions;

    // Start is called before the first frame update
    void Start()
    {
        dialogBox = player.transform.Find("CanvasDialog").GetChild(0).gameObject;
        if (dialogBox == null) { Debug.LogWarning("No DialogBox Found In Player"); }
        else { textPlayer = dialogBox.GetComponent<TMP_Text>(); }

        playerCode = player.GetComponent<Playermovment>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (exchangeTree[indexConv].endExchange) { indexConv = indexConvforNewExchange; }
        endConv = false;
    }
    private void OnTriggerStay(Collider other)
    {
        playerCode.inDialog = true;
        if (exchangeTree[indexConv].endExchange == false) { inExchange = true; }
    }
    private void OnTriggerExit(Collider other)
    {
        playerCode.inDialog = false;
        inExchange = false;
        workingTimer = false;
    }

    private void Update()
    {
        dialogBox.SetActive(inExchange);

        //update the state of the condition
        bool noTriggerd = true;
        foreach(DialogCondition condition in listConditions)
        {
            condition.triggered = condition.stateCondition.activeSelf;
            if (condition.triggered)
            {
                if (noTriggerd ) { indexConv = FindTheRightIndexConv(condition.indexConversation); }
                noTriggerd = false;
            }
        }

        //check for answers
        if (inExchange)
        {
            UnderstandPlayer();
        }

        //timer for the dialog
        if (exchangeTree[indexConv].timerForEvent > 0)
        {
            if (workingTimer == false && firstTimeWorkingTimer) { workingTimer = true; timer = 0; }
            else if (timer > exchangeTree[indexConv].timerForEvent && workingTimer)
            {
                //endExchange
                if (exchangeTree[indexConv].endExchange) { endConv = true; }
                //GoToyes
                else if (exchangeTree[indexConv].goToYesAfterTimer) 
                { 
                    indexConv = FindTheRightIndexConv(exchangeTree[indexConv].indexIfYes);
                    waitForAnswer = false; workingTimer = false; firstTimeWorkingTimer = true;
                }
                workingTimer = false;
                timer = 0;
                firstTimeWorkingTimer = false;
            }
            else if(workingTimer == true) { timer += Time.deltaTime; }
        }

        //define the right time to allow the player to speak
        if (waitForAnswer == false && playerCode.yes == false && playerCode.no == false && inExchange && exchangeTree[indexConv].endExchange == false && workingTimer == false)
        {
            waitForAnswer = true;
        }
        
    }

    private void UnderstandPlayer()
    {
        textPlayer.text = exchangeTree[indexConv].dialogBox;
        if (waitForAnswer)
        {
            if (playerCode.yes) { indexConv = FindTheRightIndexConv(exchangeTree[indexConv].indexIfYes); waitForAnswer = false; workingTimer = false; firstTimeWorkingTimer = true; }
            else if (playerCode.no) { indexConv = FindTheRightIndexConv(exchangeTree[indexConv].indexIfNo); waitForAnswer = false; workingTimer = false; firstTimeWorkingTimer = true; }

            if (exchangeTree[indexConv].onEvent != null) { exchangeTree[indexConv].onEvent.Invoke(); }
        }

        if (exchangeTree[indexConv].endExchange && endConv) { playerCode.inDialog = false; inExchange = false; }

        if (exchangeTree[indexConv].GetInitialState()) 
        { 
            timer = exchangeTree[indexConv].timerForEvent;
            workingTimer = true;
            exchangeTree[indexConv].SetInitialState(false); 
        }
    }

    public int FindTheRightIndexConv(int indexFromAnswer)
    {
        for(int i=0;i<exchangeTree.Count;i++)
        {
            if(exchangeTree[i].indexConversation == indexFromAnswer)
            {
                return i;
            }
        }
        Debug.LogWarning("Pas de suite trouvée pour le dialogue!");
        return 0;
    }

    [System.Serializable]
    public class DialogExchange
    {
        public int indexConversation;
        public string dialogBox;
        public int indexIfYes;
        public int indexIfNo;
        public bool endExchange;
        public bool goToYesAfterTimer;
        public int timerForEvent;
        public MyEvent onEvent;
        private bool initialState = true;


        public DialogExchange(int indexConv, string dialog, int yes, int no)
        {
            indexConversation = indexConv;
            dialogBox = dialog;
            indexIfNo = no;
            indexIfYes = yes;
            endExchange = false;
        }

        public bool GetInitialState()
        {
            return initialState;
        }
        public void SetInitialState(bool value)
        {
            initialState = value;
        }
    }
    [System.Serializable]
    public class MyEvent : UnityEvent { }

    [System.Serializable]
    public class DialogCondition
    {
        public GameObject stateCondition;
        public int indexConversation;
        public bool triggered;

        public DialogCondition(GameObject conditon, int index)
        {
            stateCondition = conditon;
            indexConversation = index;
            triggered = false; 
        }
    }
}
