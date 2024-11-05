using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speaker1 : MonoBehaviour
{
    public List<GameObject> listTriggered;
    public DialogAI dialogCode;

    public void BlockPreviousDialog(int indexSubstitue)
    {
        dialogCode.indexConv = dialogCode.FindTheRightIndexConv(indexSubstitue);
        dialogCode.indexConvforNewExchange = dialogCode.FindTheRightIndexConv(indexSubstitue);
    }

   
}
/*foreach(GameObject trigger in listTriggered)
        {
            if (trigger.name == indexSubstitue.ToString())
            {
                trigger.SetActive(true);
                break;
            }
        }*/
