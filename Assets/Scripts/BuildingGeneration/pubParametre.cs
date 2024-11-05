using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class pubParametre : MonoBehaviour
{
    [Header("Ad Settings")]
    public float height;
    public float width;
    public bool randomize;
    public float[] rGB;
    public TMP_FontAsset[] fontAssets;

    public void UpdateWidth()
    {
        width = width * transform.localScale.x/0.05f;
    }

    public void Awake()
    {
        if (randomize)
        {
            int randomIndexFull = Random.Range(0, 3);
            int randomIndexNull = 0;
            while (randomIndexFull == randomIndexNull) { randomIndexNull= Random.Range(0, 3); }
            rGB[randomIndexFull] = 1f;
            rGB[randomIndexNull] = 0f;
            for (int i=0;i<3;i++)
            {
                if(i!=randomIndexFull && i!=randomIndexNull)
                {
                    rGB[i] = ((float)(Random.Range(0, 255)))/(255f);
                    Debug.Log(rGB[i]);
                    break;
                }
            }

            Color32 colorF = new Color(rGB[0], rGB[1], rGB[2], 1f);
            Debug.Log(colorF);

            Color32 colorT = new Color(1f-rGB[0], 1f-rGB[1], 1f-rGB[2], 1f);

            gameObject.GetComponent<RawImage>().color = colorF;
            gameObject.GetComponentInChildren<TMP_Text>().color = colorT;
            gameObject.GetComponentInChildren<TMP_Text>().font = fontAssets[Random.Range(0,fontAssets.Length)];
        }
    }
}
