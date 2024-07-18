using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private Stack<GameObject> uiStack = new Stack<GameObject>(); //매번 생성될 UI를 저장할 장소
    public Canvas canvas;
    public GameObject myInven;
    public GameObject mySkillWindow;

    public GameObject itemQuantityCheckPrefab; // Quantity를 표시할 프리팹
    private GameObject currentQuantityUI = null;
    private ItemKind itemWithPendingQuantity = null;
    private System.Action quantityConfirmCallback;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        myInven.gameObject.SetActive(false);
        mySkillWindow.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && uiStack.Count > 0) // esc로 ui종료 매서드호출
        {
            CloseTopUi();
        }

        //인벤토리 상호작용
        if (Input.GetKeyDown(KeyCode.I)) // !ChatSystem.Instance.IsActive &&  채팅 생겼을때 쓸것
        {
            myInven.gameObject.SetActive(!myInven.gameObject.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (myInven.gameObject.activeSelf)
            {
                myInven.gameObject.SetActive(!myInven.gameObject.activeSelf);
            }
        }

        // 스킬창 상호작용
        if (Input.GetKeyDown(KeyCode.K)) // !ChatSystem.Instance.IsActive &&  채팅 생겼을때 쓸것
        {
            mySkillWindow.gameObject.SetActive(!mySkillWindow.gameObject.activeSelf);
        }
    }
    public void CloseTopUi() // 가장위에 존재하는 UI종료
    {
        if(uiStack.Count > 0)
        {
            GameObject topUI = uiStack.Pop();
            Destroy(topUI);
        }
    }

    public void OpenQuantityUI(ItemKind item, System.Action onConfirm)
    {
        // 기존에 열린 UI가 있으면 닫기
        if (currentQuantityUI != null)
        {
            CloseTopUi();
        }

        // Quantity UI 열기
        currentQuantityUI = ShowUI(itemQuantityCheckPrefab);
        var quantityCheck = currentQuantityUI.GetComponent<ItemQuantityCheck>();
        if (quantityCheck != null)
        {
            quantityCheck.Initialize(item, onConfirm);
        }
    }


    public GameObject ShowUI(GameObject uiPrefab) // 호출자가 가지고있는 프리펩 ui생성
    { 
        if (canvas != null) 
        { 
            GameObject uiInstance = Instantiate(uiPrefab, canvas.transform); 
            uiStack.Push(uiInstance); 
            return uiInstance;
        } 
        else 
        { 
            Debug.LogError("Canvas가 설정되지 않음");
            return null;
        } 
    }
}
