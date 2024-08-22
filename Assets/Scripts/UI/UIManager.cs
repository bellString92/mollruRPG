using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum OkBoxType
{
    NoEmptySlot,
    NotEnoughGold,
    NotEnoughMaterial,
    NomoreUpgrade
}
public interface IQuantityCheck
{
    void Initialize(ItemKind item, System.Action onConfirm,string title);
}


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

    //  npc UI 관리 요소
    public GameObject shop;
    public GameObject forge;
    public GameObject itemQuantityCheckUI;
    public GameObject OkButtonUI;
    private GameObject currentQuantityUI = null;
    private System.Action quantityConfirmCallback;

    // 마우스 포인트 관련
    bool isCursorVisible = false;
    bool isPressed = true;

    // 확인 팝업 ui관련
    OkBoxType boxMsg;

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

        // OkButtonUI 오브젝트 풀 생성
        PopupObjectPool.Instance.CreatePool("OkButtonUI", OkButtonUI, 5);

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
            GameObject topUI = uiStack.Peek();
            topUI.SetActive(false);
            uiStack.Pop();
        }
    }

    public void OpenQuantityUI<T>(ItemKind item, Action onConfirm, string title) where T : Component
    {
        if (currentQuantityUI != null)
        {
            CloseTopUi();
        }

        currentQuantityUI = itemQuantityCheckUI;
        currentQuantityUI.SetActive(true);

        // 기존에 있는 컴포넌트를 제거하고 새로운 컴포넌트를 추가
        var quantityCheck = currentQuantityUI.GetComponent<T>();
        if (quantityCheck != null)
        {
            Destroy(quantityCheck);
        }

        quantityCheck = currentQuantityUI.AddComponent<T>();
        if (quantityCheck != null)
        {
            (quantityCheck as IQuantityCheck)?.Initialize(item, onConfirm, title);
        }

        uiStack.Push(currentQuantityUI);
    }

    public void OpenQuantityCheckUI(ItemKind item, Action onConfirm)
    {
        OpenQuantityUI<ItemQuantityCheck>(item, onConfirm, "구매 수량");
    }

    public void OpenSellQuantityCheckUI(ItemKind item, Action onConfirm)
    {
        OpenQuantityUI<SellQuantityCheck>(item, onConfirm, "판매 수량");
    }


    public void ShowNPCUI(NpcType type)
    {
        GameObject npcUI = null;

        switch (type)
        {
            case NpcType.Shop:
                {
                    npcUI = shop;
                    break;
                }
            case NpcType.Forge:
                {
                    npcUI = forge;
                    break;
                }
        }
        if (npcUI != null)
        {
            npcUI.SetActive(true);
            uiStack.Push(npcUI);
        }
    }
    public void CloseNPCUI(NpcType type)
    {
        switch (type)
        {
            case NpcType.Shop:
                {
                    if (shop.gameObject.activeSelf)
                    {
                        shop.SetActive(false);
                        uiStack.Pop();
                    }
                    break;
                }
            case NpcType.Forge:
                {
                    if (forge.gameObject.activeSelf)
                    {
                        forge.SetActive(false);
                        uiStack.Pop();
                    }
                    break;
                }
        }
    }

    public void ShowOkbuttonUI(OkBoxType msgtype)
    {
        GameObject uiInstance = PopupObjectPool.Instance.GetFromPool("OkButtonUI");

        OnlyOkButtonUI Box = uiInstance.GetComponent<OnlyOkButtonUI>();
        switch (msgtype)
        {
            case OkBoxType.NoEmptySlot:
                Box.msg.text = "인벤토리에 여유 공간이 없습니다";
                break;
            case OkBoxType.NotEnoughGold:
                Box.msg.text = "소지금이 충분하지 않습니다";
                break;
            case OkBoxType.NotEnoughMaterial:
                Box.msg.text = "재료가 부족합니다.";
                break;
            case OkBoxType.NomoreUpgrade:
                Box.msg.text = "최고 강화 등급입니다.";
                break;
                    
        }

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
