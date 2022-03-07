using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GOTCHAManager : MonoBehaviour
{

    private static GOTCHAManager instance = null;
    [SerializeField]
    private InputField numInputField;

    [SerializeField]
    private InputField NOPInputField;

    [SerializeField]
    private InputField countInputField;

    [SerializeField]
    private Button resetButton;
    [SerializeField]
    private Button rejectButton;
    [SerializeField]
    private Button GOTCHAButton;

    [SerializeField]
    private Transform exeptParentObject;
    public Transform ExeptParentObject { get { return exeptParentObject; } }

    [SerializeField]
    private GameObject exeptItemPrefab;
    public GameObject ExeptItemPrefab { get { return exeptItemPrefab; } }

    [SerializeField]
    private Transform resultCardParentObject;
    public Transform ResultCardParentObject { get { return resultCardParentObject; } }
    [SerializeField]
    private RectTransform resultScrollView;
    [SerializeField]
    private GameObject resultCardPrefab;
    public GameObject ResultCardPrefab { get { return resultCardPrefab; } }
    /*
        [SerializeField]
        private Transform resultParentObject;

        [SerializeField]
        private GameObject resultItemPrefab;*/
    [SerializeField]
    private GameObject blinkPanel;
    [SerializeField]
    private GameObject exeptPanel;
    [SerializeField]
    private GameObject resultPanel;
    [SerializeField]
    private Button[] exeptPanelActiveButton;

    [SerializeField]
    private RectTransform tailEffect;
    [SerializeField]
    private RectTransform packTop;
    [SerializeField]
    private RectTransform pack;

    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private Button[] exitGOTCHAButtons;
    [SerializeField]
    private Toggle isSkipProcess;
    [SerializeField]
    private Button checkAllButton;
    [SerializeField]
    private Button resetResultButton;

    [SerializeField]
    private Button checkResultButton;

    public Dictionary<int, GameObject> rejectDict = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> resultDict = new Dictionary<int, GameObject>();

    private bool checkAllResultProcessing = false;
    private bool isGOTCHAProcessing = false;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
                Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    public static GOTCHAManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    private void Start()
    {
        rejectButton.onClick.AddListener(AddReject);
        resetButton.onClick.AddListener(ResetRejects);
        GOTCHAButton.onClick.AddListener(GOTCHA);
        checkAllButton.onClick.AddListener(CheckAllResult);
        resetResultButton.onClick.AddListener(ResetRejects);
        checkResultButton.onClick.AddListener(CheckResults);
        foreach (var item in exeptPanelActiveButton)
        {
            item.onClick.AddListener(exeptPanelActive);
        }

        foreach (var item in exitGOTCHAButtons)
        {
            item.onClick.AddListener(CallOnEndGOTCHA);
        }
        SaveManager.Instance.LoadGameData();
    }
    //17,14,16,15 ¹ø È®·ü ¿Ã¸®±â

    // »Ì±â ¿¬Ãâ Áß ½ºÅµ
    private void Update()
    {
        if(isGOTCHAProcessing&&Input.GetKeyDown(KeyCode.Space))
        {
            isGOTCHAProcessing = false;
            resultPopup();
        }

        if(checkAllResultProcessing && Input.GetKeyDown(KeyCode.Space))
        {
            StopCoroutine(CheckAllResultProcess());
            checkAllResultProcessing = false;
            foreach (var item in resultDict)
            {
                item.Value.GetComponent<ResultCard>().OnClick();
            }
        }
    }
    public void AddReject()
    {
        if (numInputField.text == "" || numInputField.text == null) return;

        int num = int.Parse(numInputField.text);
        foreach (var item in rejectDict)
        {
            if (item.Key == num)
            {
                return;
            }
        }

        AddReject(num);
    }
    public void AddReject(int num)
    {
        GameObject GO = Instantiate(exeptItemPrefab, exeptParentObject);
        GO.GetComponent<ExeptItem>().SetUp(num, CallReject);
        numInputField.text = "";
        rejectDict.Add(num, GO);
        SaveManager.Instance.saveData.rejectItem.Add(num);
    }

    public void ResetRejects()
    {
        if (checkAllResultProcessing) return;

        foreach (var item in resultDict)
        {
            Destroy(item.Value);
        }
        resultDict.Clear();

        foreach (var item in rejectDict)
        {
            Destroy(item.Value);
            SaveManager.Instance.saveData.rejectItem.Remove(item.Key);
        }
        rejectDict.Clear();
        
        SaveManager.Instance.saveData.rejectItem.Clear();
    }
    public void CallReject(int num)
    {
        Destroy(rejectDict[num].gameObject);
        Destroy(resultDict[num].gameObject);
        resultDict.Remove(num);
        rejectDict.Remove(num);
        SaveManager.Instance.saveData.rejectItem.Remove(num);
    }

    public void exeptPanelActive()
    {
        exeptPanel.SetActive(!exeptPanel.activeSelf);
    }

    public void GOTCHA()
    {
        if (isGOTCHAProcessing) return;
        isGOTCHAProcessing = true;

        
        int num = int.Parse(NOPInputField.text);
        int count = countInputField.text == "" || countInputField.text == null ? 1:int.Parse(countInputField.text);

        if (count > num || /*count == num ||*/ num - rejectDict.Count <= count)
        {
            isGOTCHAProcessing = false;

            return;
        }

        resultScrollView.gameObject.SetActive(false);
        resultScrollView.DOAnchorPosY(-1000, 0);
        resultPanel.SetActive(true);

        int result = 0;


        result = Random.Range(1, num+1);
        print(count);
        for (int i = 0; i < count;)
        {
            print(i);
            if (rejectDict.ContainsKey(result) || resultDict.ContainsKey(result))
            {
                result = Random.Range(1, num+1);
            }
            else
            {
                print("AA");
                AddReject(result);
                print(result);

                AddResult(result);  
                i++;
            }

        }
      

     
        if(isSkipProcess.isOn)
        {
            resultPopup();
        }
        else
        {
            StartCoroutine(CardGOTCHAProcess(count));
        }
    }
    private IEnumerator CardGOTCHAProcess(int count)
    {

        pack.gameObject.SetActive(true);
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if(Input.anyKey)
            {
                break;
            }
        }

        GameObject[] cards = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            cards[i] = Instantiate(cardPrefab,pack.transform);
            cards[i].transform.SetAsFirstSibling();
        }

        tailEffect.anchoredPosition = new Vector2(-1920, 240);
        float packTopSpeed = .5f;
        float tailSpeed = .25f;
        Blink();
        tailEffect.DOAnchorPosX(1920, tailSpeed).OnComplete(() => { 
            packTop.DORotate(new Vector3(0, 0, -30), packTopSpeed);
            packTop.DOAnchorPos(new Vector2(150, 300), packTopSpeed);
        });
        yield return new WaitForSeconds(packTopSpeed + tailSpeed);
        float packDownSpeed = .5f;
        pack.DOAnchorPos(new Vector2(0, -255),packDownSpeed);

        float packTopFadeSpeed = .25f;
        packTop.GetComponent<Image>().DOFade(0, packTopFadeSpeed);
        
        yield return new WaitForSeconds( packDownSpeed);

        float cardSpeed = .25f;
        for (int i = 0; i < count; i++)
        {
            cards[i].GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 1200), cardSpeed).OnComplete(()=> { Destroy(cards[i]); }) ;
            yield return new WaitForSeconds(cardSpeed);
        }
        pack.DOAnchorPos(new Vector2(0, -800), packDownSpeed);
        pack.GetComponent<CanvasGroup>().DOFade(0, packDownSpeed);

        yield return new WaitForSeconds(packDownSpeed);

        resultPopup();

        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (!isGOTCHAProcessing)
            {
                print("AAAAAAAAAAAAAAAAAA");
                packTop.GetComponent<Image>().DOFade(1, 0);
                packTop.DORotate(new Vector3(0, 0, 0), 0);
                packTop.DOAnchorPos(new Vector2(150, 245), 0);
                pack.DOAnchorPos(new Vector2(0, 0), 0);
                pack.GetComponent<CanvasGroup>().DOFade(1, 0);
                break;
            }
        }
        
    }
    public void resultPopup()
    {
        pack.gameObject.SetActive(false);

        float resultUpSpeed = .5f;
        resultScrollView.gameObject.SetActive(true);
        resultScrollView.DOAnchorPosY(0, resultUpSpeed);
    }
    public void CallOnEndGOTCHA()
    {
        isGOTCHAProcessing = false;
        resultPanel.SetActive(false);

    }
    public void AddResult(int result)
    {

        GameObject cardGO = Instantiate(resultCardPrefab, resultCardParentObject);
        cardGO.GetComponent<ResultCard>().SetUp(result);
        resultDict.Add(result,cardGO);
        print("ADDRESULT");
    }

    public void Blink()
    {
        blinkPanel.SetActive(true);
        float fadeSpeed = .1f;
        blinkPanel.GetComponent<Image>().DOFade(.7f, fadeSpeed).OnComplete(()=> { blinkPanel.GetComponent<Image>().DOFade(0, fadeSpeed).OnComplete(()=> { blinkPanel.SetActive(false); }); });
    }
    public void CheckAllResult()
    {
        StartCoroutine(CheckAllResultProcess());
    }
    private IEnumerator CheckAllResultProcess()
    {
        checkAllResultProcessing = true;
        foreach (var item in resultDict)
        {
            item.Value.GetComponent<ResultCard>().OnClick();
            yield return new WaitForSeconds(0.5f);
        }
        checkAllResultProcessing = false;
    }
    public void CheckResults()
    {
        resultScrollView.DOAnchorPosY(-1000, 0);
        resultPanel.SetActive(true);

        resultPopup();
    }
}
