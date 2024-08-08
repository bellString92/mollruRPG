using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private Stack<GameObject> uiStack= new Stack<GameObject>(); //매번 생성될 UI를 저장할 장소
    public Canvas canvas;
    public GameObject UiForNPC; // npc가 ui를 불러올때 이곳에 생성시켜 그려지는 순서조정
    public GameObject DontTouch; // ui가 활성화되면 활성화시키기

    // player조작 ui
    public Inventory myInven;
    public GameObject mySkill;
    public PlayerStateUiManager myStateWindow;

    // 상점에 필요한 요소
    public GameObject itemQuantityCheckPrefab; // Quantity를 표시할 프리팹
    private GameObject currentQuantityUI = null;
    private System.Action quantityConfirmCallback;

    // 마우스 포인트 관련
    bool isCursorVisible = false;
    bool isPressed = true;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        // 마우스를 숨기기
        Cursor.visible = isCursorVisible;
        // 마우스 중앙 고정 및 잠구기
        Cursor.lockState = CursorLockMode.Locked;

        if (myInven != null)
        {
            myInven.gameObject.SetActive(false);
        }
        if (myStateWindow != null)
        {
            myStateWindow.gameObject.SetActive(false);
        }
        if (mySkill != null)
        {
            mySkill.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // esc로 ui종료 매서드호출
        if (Input.GetKeyDown(KeyCode.Escape) && uiStack.Count > 0) 
        {
            CloseTopUi();
        }

        // UI 상호작용
        {
            if (Input.GetKeyDown(KeyCode.I)) // !ChatSystem.Instance.IsActive &&  채팅 생겼을때 쓸것
            {
                myInven.gameObject.SetActive(!myInven.gameObject.activeSelf);
                isCursorVisible = myInven.gameObject.activeSelf;
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                myStateWindow.gameObject.SetActive(!myStateWindow.gameObject.activeSelf);
                isCursorVisible = myStateWindow.gameObject.activeSelf;
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                mySkill.gameObject.SetActive(!mySkill.gameObject.activeSelf);
                isCursorVisible = mySkill.gameObject.activeSelf;
            }
        }
        // 마우스 제어 관련
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isCursorVisible = !isCursorVisible;
            }

            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && isPressed)
            {
                isCursorVisible = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                isPressed = false;
                isCursorVisible = true;
            }

            if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                isPressed = true;
                isCursorVisible = false;
            }
            UpdateCursorState();
        }
    }

    void UpdateCursorState()
    {
        // isCursorVisible 변수에 따라 마우스 커서 상태를 업데이트합니다.
        Cursor.visible = isCursorVisible;
        Cursor.lockState = isCursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
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
        currentQuantityUI = Instantiate(itemQuantityCheckPrefab, canvas.transform);
        //currentQuantityUI = ShowUI(itemQuantityCheckPrefab);

        var quantityCheck = currentQuantityUI.AddComponent<ItemQuantityCheck>();
        if (quantityCheck != null)
        {
            quantityCheck.Initialize(item, onConfirm);
        }
        uiStack.Push(currentQuantityUI);
    }
    public void OpenSellQuantityCheckUI(ItemKind item, System.Action onConfirm)
    {
        // 기존에 열린 UI가 있으면 닫기
        if (currentQuantityUI != null)
        {
            CloseTopUi();
        }

        // Quantity UI 열기
        currentQuantityUI = Instantiate(itemQuantityCheckPrefab, canvas.transform);
        //currentQuantityUI = ShowUI(itemQuantityCheckPrefab);

        var quantityCheck = currentQuantityUI.AddComponent<SellQuantityCheck>();
        if (quantityCheck != null)
        {
            quantityCheck.Initialize(item, onConfirm);
        }
        uiStack.Push(currentQuantityUI);
    }

    public GameObject ShowUI(GameObject uiPrefab) // 호출자가 가지고있는 프리펩 ui생성
    { 
        if (canvas != null) 
        {
            Transform parentTransform = UiForNPC != null ? UiForNPC.transform : canvas.transform;
            GameObject uiInstance = Instantiate(uiPrefab, parentTransform); 
            uiStack.Push(uiInstance);
            return uiInstance;
        } 
        else 
        { 
            Debug.LogError("Canvas가 설정되지 않음");
            return null;
        } 

    }
    public void ShowOkbuttonUI(GameObject uiPrefab)
    {
        GameObject uiInstance = Instantiate(uiPrefab, canvas.transform);
        uiStack.Push(uiInstance);
    }

    public string GetSellQuantityText()
    {
        // 현재 열려 있는 SellQuantityCheck UI에서 텍스트를 가져옴
        SellQuantityCheck quantityCheckUI = FindObjectOfType<SellQuantityCheck>();
        if (quantityCheckUI != null)
        {
            return quantityCheckUI.centerText.text;
        }
        return "0"; // 기본값
    }
}
