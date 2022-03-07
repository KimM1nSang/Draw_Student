using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResultCard : MonoBehaviour
{
    [SerializeField]
    private Button checkResultButton;
    [SerializeField]
    private Text resultText;

    private int num;
    public void SetUp(int num)
    {
        checkResultButton.onClick.AddListener(OnClick);
        this.num = num;
        resultText.text = string.Format("{0}",this.num);
    }

    public void OnClick()
    {
        float fadeSpeed = .25f;
        checkResultButton.GetComponent<CanvasGroup>().DOFade(0, fadeSpeed);
    }
}
