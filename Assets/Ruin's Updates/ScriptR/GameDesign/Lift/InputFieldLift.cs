using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class InputFieldLift : MonoBehaviour
{
    [Header("Input Review")]
    public string inputText;
    public LiftControl liftControl;

    public void GrabFromInputField(string input)
    {
        inputText = input;
        //translate the sectorCode to coordinate
        

        liftControl.Control(translateStringToCoord(input));
    }

    [Button("MoveTo")]
    public void MoveliftToDestination()
    {
        liftControl.Control(translateStringToCoord(inputText));
    }

    public Vector3 translateStringToCoord(string sectorCode)
    {
        List<string> listCoordFromString = new List<string>();
        for (int i = 0; i < 3; i++) { listCoordFromString.Add(null); }

        int nextLetterInIndex = -1;
        bool charRelevent = false;
        List<int> indexCoord = new List<int> { 1, 0, 2 };
        foreach (char letter in sectorCode)
        {
            if (nextLetterInIndex >= 0) { charRelevent = true; }
            if (letter.ToString() == "_" || letter.ToString() == "," || letter.ToString() == ".")
            {
                nextLetterInIndex += 1;
                charRelevent = false;
            }
            if (nextLetterInIndex >= 0 && charRelevent) { listCoordFromString[indexCoord[nextLetterInIndex]] += letter.ToString(); }
        }
        return new Vector3(float.Parse(listCoordFromString[0].ToString()), float.Parse(listCoordFromString[1].ToString()), float.Parse(listCoordFromString[2].ToString()));
    }
}
