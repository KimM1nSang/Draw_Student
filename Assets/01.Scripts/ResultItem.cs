using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultItem : MonoBehaviour
{
    [SerializeField]
    private Text numText;

    private int num;

    public void SetUp(int num)
    {
        this.num = num;

        numText.text = string.Format("{0} ¹ø", num);

    }
}
