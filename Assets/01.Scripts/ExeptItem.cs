using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class ExeptItem : MonoBehaviour
{
    [SerializeField]
    private Button rejectButton;
    [SerializeField]
    private Text numText;

    private int num;

    private Action<int> onReject;

    public void SetUp(int num,Action<int> act)
    {
        this.num = num;
        onReject = act;
        numText.text = string.Format("{0} ¹ø",num);
        rejectButton.onClick.AddListener(OnReject);
    }

    public void OnReject()
    {
        onReject?.Invoke(num);
    }
}
